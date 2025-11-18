
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

            var builder = WebApplication.CreateBuilder(args);

            var myAllowSpecificOrigins = "_myAllowSpecificOrigins";
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: myAllowSpecificOrigins,
                                  policy =>
                                  {
                                      policy.WithOrigins("https://localhost:49825")
                                            .AllowAnyHeader()
                                            .AllowAnyMethod();
                                  });
            });
            builder.Services.AddHttpClient(); // Para SpotifyProfileService
            builder.Services.AddScoped<ISpotifyProfileService, SpotifyProfileService>();


            var databaseConfig = builder.Configuration.GetSection("ConnectionStrings").Get<DatabaseConfig>();
            Console.WriteLine($"la conexion es: {databaseConfig.SupabaseConnection}, se logr");

            // ⭐ AGREGAR ESTAS LÍNEAS PARA DEBUG
            var spotifyClientId = builder.Configuration["Spotify:ClientId"];
            var spotifyClientSecret = builder.Configuration["Spotify:ClientSecret"];
            Console.WriteLine($"Spotify ClientId: {spotifyClientId ?? "NULL"}");
            Console.WriteLine($"Spotify ClientSecret: {(string.IsNullOrEmpty(spotifyClientSecret) ? "NULL" : "***EXISTE***")}");

            builder.Services.AddDbContext<SoundTrackContext>(options => options.UseNpgsql(databaseConfig.SupabaseConnection)); //mando a llamar el contexto para usar ORM, y le paso la configuracion para la base de 
            builder.Services.AddControllers();

            builder.Services.AddScoped<ISoundTrackRepository, SoundTrackRepository>();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors(myAllowSpecificOrigins);

            app.UseAuthorization();

            app.MapControllers();

            app.MapFallbackToFile("/index.html");

            app.Run();

        }

    }

}