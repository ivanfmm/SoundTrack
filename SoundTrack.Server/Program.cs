using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using SoundTrack.Server.Data;
using SoundTrack.Server.Services;
using System.Configuration;



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
                                      policy.WithOrigins("https://localhost:49825")
                                            .AllowAnyHeader()
                                            .AllowAnyMethod()
                                            .AllowCredentials(); // ⭐ AGREGADO: Necesario para cookies de autenticación
                                  });
            });

            builder.Services.AddHttpClient();
            builder.Services.AddScoped<ISpotifyProfileService, SpotifyProfileService>();

            var databaseConfig = builder.Configuration.GetSection("ConnectionStrings").Get<DatabaseConfig>();
            Console.WriteLine($"la conexion es: {databaseConfig.SupabaseConnection}, se logr");

            var spotifyClientId = builder.Configuration["Spotify:ClientId"];
            var spotifyClientSecret = builder.Configuration["Spotify:ClientSecret"];
            Console.WriteLine($"Spotify ClientId: {spotifyClientId ?? "NULL"}");
            Console.WriteLine($"Spotify ClientSecret: {(string.IsNullOrEmpty(spotifyClientSecret) ? "NULL" : "***EXISTE***")}");

            builder.Services.AddDbContext<SoundTrackContext>(options => options.UseNpgsql(databaseConfig.SupabaseConnection));

            // ⭐ AGREGADO: Configuración de Identity
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

            // ⭐ AGREGADO: Configuración de cookies de autenticación
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
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

            // ⭐ AGREGADO: Authentication DEBE ir antes de Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapFallbackToFile("/index.html");

            app.Run();
        }
    }
}