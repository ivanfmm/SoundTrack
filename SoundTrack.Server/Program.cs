using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SoundTrack.Server.Data;
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
							"https://localhost:49825",
							"https://127.0.0.1:49825")
						.AllowAnyHeader()
						.AllowAnyMethod()
						.AllowCredentials();
					});
			});

			builder.Services.AddHttpClient();
			builder.Services.AddScoped<ISpotifyProfileService, SpotifyProfileService>();
			builder.Services.AddScoped<ISpotifyTokenService, SpotifyTokenService>();

			var databaseConfig = builder.Configuration.GetSection("ConnectionStrings").Get<DatabaseConfig>();
			builder.Services.AddDbContext<SoundTrackContext>(options =>
				options.UseNpgsql(databaseConfig.SupabaseConnection));

			
			builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
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

			builder.Services.ConfigureApplicationCookie(options =>
			{
				options.Cookie.HttpOnly = true;
				options.ExpireTimeSpan = TimeSpan.FromDays(7);
				options.SlidingExpiration = true;
			});

			// Autenticacion con Spotify
			builder.Services.AddAuthentication().AddSpotify(options =>
			{
				options.ClientId = builder.Configuration["Spotify:ClientId"];
				options.ClientSecret = builder.Configuration["Spotify:ClientSecret"];
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
					var email = context.Principal.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
					var name = context.Principal.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

					if (email != null)
					{
						var services = context.HttpContext.RequestServices;
						var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
						var dbContext = services.GetRequiredService<SoundTrackContext>();

						// Buscar o crear usuario en AspNetUsers
						var identityUser = await userManager.FindByEmailAsync(email);

						if (identityUser == null)
						{
							identityUser = new IdentityUser
							{
								UserName = email,
								Email = email,
								EmailConfirmed = true
							};
							await userManager.CreateAsync(identityUser);
						}

						// Guardar tokens de Spotify en AspNetUserTokens
						await userManager.SetAuthenticationTokenAsync(
							identityUser,
							"Spotify",
							"access_token",
							accessToken
						);

						await userManager.SetAuthenticationTokenAsync(
							identityUser,
							"Spotify",
							"refresh_token",
							refreshToken
						);
					}
				};
			});

			builder.Services.AddControllers();
			builder.Services.AddScoped<ISoundTrackRepository, SoundTrackRepository>();
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