using ebillets_jo2024_API.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Globalization;

namespace ebillets_jo2024_API.Data
{
    public class DataSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public DataSeeder(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public void Seed()
        {
            // Vérifie si l’admin existe déjà
            var adminEmail = "admin@jo2024.fr";
            var adminExiste = _context.Utilisateurs.Any(u => u.Email == adminEmail);

            if (!adminExiste)
            {
                var adminPassword = _configuration["AdminPassword"];
                if (string.IsNullOrEmpty(adminPassword))
                    throw new Exception("AdminPassword manquant dans la configuration.");

                var admin = new Utilisateur
                {
                    Nom = "JO2024",
                    Prenom = "Admin",
                    Email = adminEmail,
                    MotDePasseHash = BCrypt.Net.BCrypt.HashPassword(adminPassword),
                    CleUtilisateur = "ADMINKEY2024",
                    Role = RoleUtilisateur.Administrateur
                };

                _context.Utilisateurs.Add(admin);
                _context.SaveChanges();

                // Log uniquement si l’admin vient d’être créé
                var frenchDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm", CultureInfo.GetCultureInfo("fr-FR"));
                Console.WriteLine($">>> ADMIN CREE - VERSION API DU {frenchDate} <<<");
            }
            // Sinon, ne rien afficher
        }
    }
}
