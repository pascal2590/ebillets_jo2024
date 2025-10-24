using ebillets_jo2024_API.Data;
using ebillets_jo2024_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ebillets_jo2024.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UtilisateurController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UtilisateurController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============================================
        // 🔹 Vérification de connexion à la base
        // ============================================
        [HttpGet("test-connexion")]
        public IActionResult TestConnexion()
        {
            try
            {
                var isConnected = _context.Database.CanConnect();
                return Ok(new { connected = isConnected });
            }
            catch (Exception ex)
            {
                return BadRequest(new { connected = false, error = ex.Message });
            }
        }

        // ============================================
        // 🔹 GET : api/Utilisateur
        // ============================================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Utilisateur>>> GetUtilisateurs()
        {
            return await _context.Utilisateurs.ToListAsync();
        }

        // ============================================
        // 🔹 GET : api/Utilisateur/{id}
        // ============================================
        [HttpGet("{id}")]
        public async Task<ActionResult<Utilisateur>> GetUtilisateur(int id)
        {
            var utilisateur = await _context.Utilisateurs.FindAsync(id);

            if (utilisateur == null)
                return NotFound();

            return utilisateur;
        }

        // ============================================
        // 🔹 POST : api/Utilisateur (Inscription)
        // ============================================
        [HttpPost]
        public async Task<ActionResult<Utilisateur>> PostUtilisateur(Utilisateur utilisateur)
        {
            if (string.IsNullOrWhiteSpace(utilisateur.MotDePasseHash))
                return BadRequest("Le mot de passe est obligatoire.");

            // ✅ Génère une clé utilisateur unique si absente
            if (string.IsNullOrEmpty(utilisateur.CleUtilisateur))
                utilisateur.CleUtilisateur = GenerateKey();

            // ✅ Hachage sécurisé du mot de passe
            utilisateur.MotDePasseHash = HashPassword(utilisateur.MotDePasseHash);

            _context.Utilisateurs.Add(utilisateur);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUtilisateur), new { id = utilisateur.IdUtilisateur }, utilisateur);
        }

        // ============================================
        // 🔹 PUT : api/Utilisateur/{id}
        // ============================================
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUtilisateur(int id, Utilisateur utilisateur)
        {
            if (id != utilisateur.IdUtilisateur)
                return BadRequest();

            var existingUser = await _context.Utilisateurs.FindAsync(id);
            if (existingUser == null)
                return NotFound();

            // Mise à jour sécurisée
            existingUser.Nom = utilisateur.Nom;
            existingUser.Prenom = utilisateur.Prenom;
            existingUser.Email = utilisateur.Email;

            // Si le mot de passe est modifié, le rehacher
            if (!string.IsNullOrEmpty(utilisateur.MotDePasseHash) &&
                utilisateur.MotDePasseHash != existingUser.MotDePasseHash)
            {
                existingUser.MotDePasseHash = HashPassword(utilisateur.MotDePasseHash);
            }

            // Si la clé utilisateur est vide, on la régénère
            if (string.IsNullOrEmpty(existingUser.CleUtilisateur))
                existingUser.CleUtilisateur = GenerateKey();

            _context.Entry(existingUser).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ============================================
        // 🔹 DELETE : api/Utilisateur/{id}
        // ============================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUtilisateur(int id)
        {
            var utilisateur = await _context.Utilisateurs.FindAsync(id);
            if (utilisateur == null)
                return NotFound();

            _context.Utilisateurs.Remove(utilisateur);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ============================================
        // 🔹 Méthodes utilitaires privées
        // ============================================
        private string GenerateKey()
        {
            using var rng = RandomNumberGenerator.Create();
            var data = new byte[32];
            rng.GetBytes(data);
            return BitConverter.ToString(data).Replace("-", "").ToLower();
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}
