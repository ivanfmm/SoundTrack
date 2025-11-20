using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SoundTrack.Server.Data;
using SoundTrack.Server.Models;
using SoundTrack.Server.Services;
using AspNet.Security.OAuth.Spotify;

namespace SoundTrack.Server
{
	public class Program
	{
		public static void Main(string[] args)
		{
			AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
			var builder = WebApplication.CreateBuilder(args);

			
			var myAllowSpecificOrigins = "_myAllowSpecificOrigins";
			builder.Services.AddCors(options =>
			{
				options.AddPolicy(name: myAllowSpecificOrigins,
					policy =>
					{
						policy.WithOrigins(
						
							"https://127.0.0.1:49825")
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


				options.Events.OnRedirectToAuthorizationEndpoint = context =>
				{
					context.Request.Scheme = "https";
					context.Request.Host = new HostString("127.0.0.1", 7232);

					var redirectUri = context.RedirectUri
						.Replace("http://", "https://")
						.Replace("localhost", "127.0.0.1");

					context.Response.Redirect(redirectUri);
					return Task.CompletedTask;
				};


				options.Events.OnCreatingTicket = async context =>
				{
					context.Request.Scheme = "https";
					context.Request.Host = new HostString("127.0.0.1", 7232);

					var accessToken = context.AccessToken;
					var refreshToken = context.RefreshToken;
					var email = context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

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
								UserName = email,
								Email = email,
								EmailConfirmed = true,
								CreateDate = DateTime.UtcNow
							};
							await userManager.CreateAsync(user);
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