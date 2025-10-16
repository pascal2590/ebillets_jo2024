using ebillets_jo2024.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Microsoft.OpenApi.Models;

namespace ebillets_jo2024
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // Méthode appelée au démarrage pour enregistrer les services
        public void ConfigureServices(IServiceCollection services)
        {
            // === Connexion à la base MySQL ===
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(
                    Configuration.GetConnectionString("DefaultConnection"),
                    new MySqlServerVersion(new Version(8, 0, 30))
                )
            );

            // === Ajout des contrôleurs ===
            services.AddControllers();

            // === Configuration CORS ===
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularClient", builder =>
                {
                    builder.WithOrigins(
                        "http://localhost:4200",     // ton app Angular sur ton PC
                        "http://192.168.1.196:4200"  // ton app Angular sur ton téléphone (IP locale)
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                    //.AllowCredentials();
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

            app.UseRouting();

            // === Active la bonne stratégie CORS ===
            app.UseCors("AllowAngularClient");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
