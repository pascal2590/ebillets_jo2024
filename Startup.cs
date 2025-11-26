using ebillets_jo2024_API.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace ebillets_jo2024_API
{
    public class Startup(IConfiguration configuration)
    {
        public IConfiguration Configuration { get; } = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(
                    Configuration.GetConnectionString("DefaultConnection"),
                    new MySqlServerVersion(new Version(8, 0, 30))
                )
            );

            services.AddControllers();

            // Note : l'origine doit contenir scheme + host + port exactement comme dans le navigateur
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
                //options.AddPolicy("AllowAngularClient", builder =>
                //{
                //builder.WithOrigins(
                //  "http://localhost:4200",            // Angular dev sur PC (http)
                // "https://localhost:4200",           // Angular dev si servi en https
                //   "http://127.0.0.1:4200",
                //  "http://192.168.1.196:4200",       // Angular sur PC, accessible depuis téléphone (http)
                //  "https://192.168.1.196:4200"       // si tu sers Angular en https
                //)
                //.AllowAnyHeader()
                //.AllowAnyMethod()
                //.AllowCredentials(); // si Angular utilise withCredentials
                //});
            });

            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRouting();

            // Appliquer la policy CORS ici (après UseRouting, avant Auth)
            //app.UseCors("AllowAngularClient");

            app.UseCors("AllowAll"); // une seule stratégie, avant auth

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            // DataSeeder
            using var scope = app.ApplicationServices.CreateScope();
            var seeder = new DataSeeder(scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(), Configuration);
            seeder.Seed();
        }
    }
}
