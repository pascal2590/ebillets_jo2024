using ebillets_jo2024_API.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

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
            // Crée un admin si inexistant
            if (!_context.Utilisateurs.Any(u => u.Email == "admin@example.fr"))
            {
                var adminPassword = _configuration["AdminPassword"];
                if (string.IsNullOrEmpty(adminPassword))
                    throw new Exception("AdminPassword manquant dans la configuration.");

                var admin = new Utilisateur
                {
                    Nom = "Admin",
                    Prenom = "Admin",
                    Email = "admin@example.fr",
                    MotDePasseHash = BCrypt.Net.BCrypt.HashPassword(adminPassword),
                    CleUtilisateur = "ADMINKEY123",
                    Role = RoleUtilisateur.Administrateur
                };

                _context.Utilisateurs.Add(admin);
                _context.SaveChanges();
            }
        }
    }
}
