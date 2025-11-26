using BCrypt.Net;
using ebillets_jo2024_API.Data;
using ebillets_jo2024_API.Models;
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
        public AuthController(ApplicationDbContext context) => _context = context;

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest model)
        {
            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
                return BadRequest("Email et mot de passe requis.");

            if (await _context.Utilisateurs.AnyAsync(u => u.Email == model.Email))
                return Conflict("Compte déjà existant.");

            string hash = BCrypt.Net.BCrypt.HashPassword(model.Password);
            string cleUtilisateur = Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper();

            var user = new Utilisateur
            {
                Nom = model.Nom,
                Prenom = model.Prenom,
                Email = model.Email,
                MotDePasseHash = hash,
                CleUtilisateur = cleUtilisateur,
                Role = model.Role == "Employe" ? RoleUtilisateur.Employe : RoleUtilisateur.Client
            };

            _context.Utilisateurs.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { utilisateur = new { user.IdUtilisateur, user.Nom, user.Prenom, user.Email, user.Role } });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            var user = await _context.Utilisateurs.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.MotDePasseHash))
                return Unauthorized("Email ou mot de passe incorrect.");

            return Ok(new { utilisateur = new { user.IdUtilisateur, user.Nom, user.Prenom, user.Email, user.Role } });
        }
    }

    public class RegisterRequest
    {
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } = "Client";
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
