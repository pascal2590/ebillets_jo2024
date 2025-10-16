using BCrypt.Net;
//using ebillets_jo2024.Data;
using ebillets_jo2024.Models;
using ebillets_jo2024_API.Data;
using ebillets_jo2024_API.Models;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace ebillets_jo2024_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // === INSCRIPTION ===
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest model)
        {
            if (model == null ||
                string.IsNullOrWhiteSpace(model.Email) ||
                string.IsNullOrWhiteSpace(model.Password) ||
                string.IsNullOrWhiteSpace(model.Nom) ||
                string.IsNullOrWhiteSpace(model.Prenom))
            {
                return BadRequest("Tous les champs sont obligatoires.");
            }

            // Vérifie si l'utilisateur existe déjà
            var existingUser = await _context.Utilisateurs.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (existingUser != null)
            {
                return Conflict("Un compte existe déjà avec cet email.");
            }

            // Hash du mot de passe
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

            // Génère une clé utilisateur unique (exemple simple)
            string cleUtilisateur = Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper();

            var utilisateur = new Utilisateur
            {
                Nom = model.Nom,
                Prenom = model.Prenom,
                Email = model.Email,
                MotDePasseHash = passwordHash,
                CleUtilisateur = cleUtilisateur,
                Role = "Utilisateur"
            };

            _context.Utilisateurs.Add(utilisateur);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Compte créé avec succès !",
                utilisateur = new
                {
                    utilisateur.IdUtilisateur,
                    utilisateur.Nom,
                    utilisateur.Prenom,
                    utilisateur.Email
                }
            });
        }

        // ===== CONNEXION =====
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
                return BadRequest("Email et mot de passe sont requis.");

            var utilisateur = await _context.Utilisateurs.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (utilisateur == null)
                return Unauthorized("Utilisateur inconnu.");

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(model.Password, utilisateur.MotDePasseHash);
            if (!isPasswordValid)
                return Unauthorized("Mot de passe incorrect.");

            return Ok(new
            {
                message = "Connexion réussie",
                utilisateur = new
                {
                    utilisateur.IdUtilisateur,
                    utilisateur.Nom,
                    utilisateur.Prenom,
                    utilisateur.Email,
                    utilisateur.Role
                }
            });
        }
    }

    // Classe utilisée pour recevoir la requête d'inscription
    public class RegisterRequest
    {
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    // Classe utilisée pour recevoir la requête de connexion
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
