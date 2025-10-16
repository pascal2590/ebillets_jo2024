using ebillets_jo2024.Data;
using ebillets_jo2024.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using BCrypt.Net;
using Org.BouncyCastle.Crypto.Generators;

namespace ebillets_jo2024.Controllers
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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            if (model == null || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
                return BadRequest("Email et mot de passe sont requis.");

            var utilisateur = await _context.Utilisateurs.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (utilisateur == null)
                return Unauthorized("Utilisateur non trouvé.");

            bool passwordValid = BCrypt.Net.BCrypt.Verify(model.Password, utilisateur.MotDePasseHash);

            if (!passwordValid)
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

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
