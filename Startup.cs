using ebillets_jo2024_API.Data;
using ebillets_jo2024_API.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Linq;
// using BCrypt.Net;

namespace ebillets_jo2024_API
{
    public class Startup(IConfiguration configuration)
    {
        public IConfiguration Configuration { get; } = configuration;

        // Méthode appelée au démarrage pour enregistrer les services
        public void ConfigureServices(IServiceCollection services)
        {
            // === Connexion é la base MySQL ===
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(
                    Configuration.GetConnectionString("DefaultConnection"),
                    new MySqlServerVersion(new Version(8, 0, 30))
                )
            );

            // === Ajout des contréleurs ===
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                    options.JsonSerializerOptions.WriteIndented = true;
                });



            // === Configuration CORS ===
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularClient", builder =>
                {
                    builder.WithOrigins(
                        "http://localhost:4200",    // IP appli Angular sur PC
                        "http://127.0.0.1:4200",   // IP pour compatibilité
                        "http://192.168.1.196:4200" // IP locale pour mobile
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials(); // gérer les cookies/tokens
                });
            });

            // === Swagger pour les tests ===
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "eBillets JO2024 API", Version = "v1" });
            });
        }

        // Méthode appelée pour configurer le pipeline HTTP
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "eBillets JO2024 API v1");
                });
            }

            app.UseCors(builder =>
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());


            app.UseRouting();

            // === Active la bonne stratégie CORS ===
            app.UseCors("AllowAngularClient"); // app.UseCors("AllowAngularClient")
                                               // app.UseCors("AllowAngular");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            
            // Crée un compte administrateur par défaut s'il n'existe pas            
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // Vérifie si un admin existe déjà
                if (!context.Utilisateurs.Any(u => u.Email == "admin@example.fr"))
                {
                    var admin = new Utilisateur
                    {
                        Nom = "JO2024",
                        Prenom = "Admin",
                        Email = "admin@example.fr",
                        MotDePasseHash = BCrypt.Net.BCrypt.HashPassword("pascal"),
                        CleUtilisateur = "ADMINKEY123",
                        Role = RoleUtilisateur.Administrateur
                    };

                    context.Utilisateurs.Add(admin);
                    context.SaveChanges();

                    Console.WriteLine("? Compte administrateur créé : admin@example.fr / Admin123!");
                }
            }
        }
    }
}
