using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SoundTrack.Server.Data;
using SoundTrack.Server.Models;
using SoundTrack.Server.Services;
using AspNet.Security.OAuth.Spotify;
using Microsoft.AspNetCore.DataProtection;

namespace SoundTrack.Server
{
	public class Program
	{
		public static void Main(string[] args)
		{
			AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
			var builder = WebApplication.CreateBuilder(args);

			// En Vercel (contenedor) el puerto llega en la variable PORT.
			// En local no existe y se usan los puertos de launchSettings.json
			var port = Environment.GetEnvironmentVariable("PORT");
			if (!string.IsNullOrEmpty(port))
			{
				builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
			}

			
			var myAllowSpecificOrigins = "_myAllowSpecificOrigins";
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: myAllowSpecificOrigins,
                    policy =>
                    {
                        policy
                            .SetIsOriginAllowed(origin => true) // Permitir cualquier origen (React en cualquier puerto)
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    });
            });

            //Servicios
            builder.Services.AddHttpClient();
			builder.Services.AddScoped<ISpotifyProfileService, SpotifyProfileService>();
			builder.Services.AddScoped<ISpotifyTokenService, SpotifyTokenService>();
			builder.Services.AddScoped<ISoundTrackRepository, SoundTrackRepository>();

			// Base de datos
			var databaseConfig = builder.Configuration.GetSection("ConnectionStrings").Get<DatabaseConfig>();
			builder.Services.AddDbContext<SoundTrackContext>(options =>
				options.UseNpgsql(databaseConfig!.SupabaseConnection,
					o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));

			// Llaves de Data Protection (cifran cookies de sesion y del login con Spotify).
			// En Vercel el contenedor se apaga y puede haber varias instancias: si las llaves
			// vivieran en memoria/disco, cada arranque cerraria la sesion de todos. Se guardan en la BD.
			builder.Services.AddDataProtection()
				.PersistKeysToDbContext<SoundTrackContext>()
				.SetApplicationName("SoundTrack");

			//Identity
			builder.Services.AddIdentity<User, IdentityRole>(options =>
			{
				options.Password.RequireDigit = true;
				options.Password.RequireLowercase = true;
				options.Password.RequireUppercase = true;
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequiredLength = 6;
				options.User.RequireUniqueEmail = true;
			})
			.AddEntityFrameworkStores<SoundTrackContext>()
			.AddDefaultTokenProviders();

			// Cookies
			builder.Services.ConfigureApplicationCookie(options =>
			{
				options.Cookie.Name = "SoundTrackAuth";
				options.Cookie.HttpOnly = true;
				options.Cookie.SameSite = SameSiteMode.Lax; //Cambio de None a Lax para debug
				options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
				options.ExpireTimeSpan = TimeSpan.FromDays(30);
				options.SlidingExpiration = true;

				options.Events.OnRedirectToLogin = context =>
				{
					context.Response.StatusCode = 401;
					return Task.CompletedTask;
				};
				options.Events.OnRedirectToAccessDenied = context =>
				{
					context.Response.StatusCode = 403;
					return Task.CompletedTask;
				};
			});

			// Cookies OAuth 2 spotify
			builder.Services.ConfigureExternalCookie(options =>
			{
				options.Cookie.SameSite = SameSiteMode.Lax; 
				options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
				options.Cookie.HttpOnly = true;
			});

			//OAuth2 spotify
			builder.Services.AddAuthentication().AddSpotify(options =>
			{
				options.ClientId = builder.Configuration["Spotify:ClientId"]!;
				options.ClientSecret = builder.Configuration["Spotify:ClientSecret"]!;
				options.CallbackPath = "/signin-spotify";
				options.SaveTokens = true;

				options.Scope.Add("user-read-private");
				options.Scope.Add("user-read-email");
				options.Scope.Add("user-top-read");

				// El redirect_uri ya no se arma a mano: sale de PublicUrl (ver middleware abajo)


				options.Events.OnCreatingTicket = async context =>
				{
					var accessToken = context.AccessToken;
					var refreshToken = context.RefreshToken;
					var email = context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
					var userName = email;

					// Desde feb 2026 Spotify ya no manda el email en GET /me.
					// Se usa el id de Spotify (no cambia) para generar un email interno unico,
					// porque Identity tiene RequireUniqueEmail = true y el resto del backend busca por email
					var spotifyId = context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
					if (string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(spotifyId))
					{
						email = $"{spotifyId}@spotify.soundtrack.local";
						userName = $"spotify_{spotifyId}";
					}

					if (email != null)
					{
						var services = context.HttpContext.RequestServices;
						var userManager = services.GetRequiredService<UserManager<User>>();
						var signInManager = services.GetRequiredService<SignInManager<User>>();

						var user = await userManager.FindByEmailAsync(email);

						if (user == null)
						{
							user = new User
							{
								UserName = userName,
								Email = email,
								EmailConfirmed = true,
								CreateDate = DateTime.UtcNow
							};
							var createResult = await userManager.CreateAsync(user);
							if (!createResult.Succeeded)
							{
								// Sin esto el error se perdia y despues tronaba UpdateAsync
								Console.WriteLine("Error creando usuario de Spotify: " + string.Join(", ", createResult.Errors.Select(e => e.Description)));
								return;
							}
						}

						// Guardar tokens
						user.SpotifyAccessToken = accessToken;
						user.SpotifyRefreshToken = refreshToken;
						await userManager.UpdateAsync(user);

						// Login del usuario
						await signInManager.SignInAsync(user, isPersistent: true);
					}
				};
			});

			builder.Services.AddControllers();
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			var app = builder.Build();

			// URL publica de la app (local: https://127.0.0.1:7232, Vercel: https://<proyecto>.vercel.app).
			// Detras del proxy de Vercel la peticion llega por http y con otro host; esto hace que
			// el redirect_uri de Spotify, las cookies Secure y UseHttpsRedirection usen la URL real.
			var publicUrl = app.Configuration["PublicUrl"];
			// Si no se configuro PublicUrl, en Vercel se usa el dominio de produccion que Vercel inyecta solo
			var vercelDomain = Environment.GetEnvironmentVariable("VERCEL_PROJECT_PRODUCTION_URL");
			if (string.IsNullOrEmpty(publicUrl) && !string.IsNullOrEmpty(vercelDomain))
			{
				publicUrl = $"https://{vercelDomain}";
			}
			if (!string.IsNullOrEmpty(publicUrl))
			{
				var publicUri = new Uri(publicUrl);
				app.Use((context, next) =>
				{
					context.Request.Scheme = publicUri.Scheme;
					context.Request.Host = new HostString(publicUri.Authority);
					return next();
				});
			}

			app.UseDefaultFiles();
			app.UseStaticFiles();

			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();
			app.UseCors(myAllowSpecificOrigins);
			app.UseAuthentication();
			app.UseAuthorization();

			app.MapControllers();
			app.MapFallbackToFile("/index.html");

			app.Run();
		}
	}
}