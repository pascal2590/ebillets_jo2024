using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using Microsoft.Extensions.DependencyInjection;


namespace ebillets_jo2024_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            // Affichage de la version avant Run()
            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            var frenchDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm", CultureInfo.GetCultureInfo("fr-FR"));
            var buildVersion = DateTime.Now.ToString("yyyyMMddHHmm");
            logger.LogInformation($">>> VERSION API DU {frenchDate} - BUILD {buildVersion} <<<");

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("http://0.0.0.0:5000");
                });
    }
}
