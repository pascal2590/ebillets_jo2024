using ebillets_jo2024_API.Data;
using ebillets_jo2024_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ebillets_jo2024_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PanierController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PanierController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Panier>>> GetPaniers()
        {
            return await _context.Paniers
                .Include(p => p.Utilisateur)
                .Include(p => p.PaniersOffres)
                    .ThenInclude(po => po.Offre)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Panier>> GetPanier(int id)
        {
            var panier = await _context.Paniers
                .Include(p => p.Utilisateur)
                .Include(p => p.PaniersOffres)
                    .ThenInclude(po => po.Offre)
                .FirstOrDefaultAsync(p => p.IdPanier == id);

            if (panier == null)
                return NotFound();

            return panier;
        }

        /// <summary>
        /// 🔹 Ajoute une offre dans le panier d’un utilisateur.
        /// Si le panier n’existe pas, il est créé automatiquement.
        /// </summary>
        [HttpPost("ajouter")]
        public async Task<IActionResult> AjouterAuPanier([FromBody] JsonElement data)
        {
            if (data.ValueKind != JsonValueKind.Object)
                return BadRequest("Format JSON invalide.");

            int idUtilisateur = data.GetProperty("idUtilisateur").GetInt32();
            int idOffre = data.GetProperty("idOffre").GetInt32();
            int quantite = data.TryGetProperty("quantite", out var q) ? q.GetInt32() : 1;

            // 🔹 Étape 1 : récupérer ou créer le panier de l’utilisateur
            var panier = await _context.Paniers
                .FirstOrDefaultAsync(p => p.IdUtilisateur == idUtilisateur);

            if (panier == null)
            {
                panier = new Panier
                {
                    IdUtilisateur = idUtilisateur,
                    DateCreation = DateTime.Now
                };

                _context.Paniers.Add(panier);
                await _context.SaveChangesAsync();
            }

            // 🔹 Étape 2 : vérifier si l’offre existe déjà dans le panier
            var panierOffre = await _context.PaniersOffres
                .FirstOrDefaultAsync(po => po.IdPanier == panier.IdPanier && po.IdOffre == idOffre);

            if (panierOffre != null)
            {
                // Si l’offre existe déjà, on incrémente la quantité
                panierOffre.Quantite += quantite;
            }
            else
            {
                // Sinon, on l’ajoute
                panierOffre = new PanierOffre
                {
                    IdPanier = panier.IdPanier,
                    IdOffre = idOffre,
                    Quantite = quantite
                };
                _context.PaniersOffres.Add(panierOffre);
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "✅ Offre ajoutée au panier avec succès",
                panier.IdPanier
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePanier(int id)
        {
            var panier = await _context.Paniers.FindAsync(id);
            if (panier == null)
                return NotFound();

            _context.Paniers.Remove(panier);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("utilisateur/{idUtilisateur}")]
        public async Task<IActionResult> GetPanierParUtilisateur(int idUtilisateur)
        {
            var panier = await _context.Paniers
                .Include(p => p.PaniersOffres)
                    .ThenInclude(po => po.Offre)
                .FirstOrDefaultAsync(p => p.IdUtilisateur == idUtilisateur);

            if (panier == null)
                return NotFound("Aucun panier trouvé pour cet utilisateur.");

            return Ok(panier);
        }

        [HttpDelete("supprimer/{idUtilisateur}/{idOffre}")]
        public async Task<IActionResult> SupprimerDuPanier(int idUtilisateur, int idOffre)
        {
            // 🔹 Récupérer le panier de l'utilisateur
            var panier = await _context.Paniers
                .FirstOrDefaultAsync(p => p.IdUtilisateur == idUtilisateur);

            if (panier == null)
                return NotFound("Aucun panier trouvé pour cet utilisateur.");

            // 🔹 Trouver l'offre à supprimer
            var panierOffre = await _context.PaniersOffres
                .FirstOrDefaultAsync(po => po.IdPanier == panier.IdPanier && po.IdOffre == idOffre);

            if (panierOffre == null)
                return NotFound("Offre non trouvée dans le panier.");

            _context.PaniersOffres.Remove(panierOffre);
            await _context.SaveChangesAsync();

            return Ok(new { message = "✅ Offre supprimée du panier." });
        }

        [HttpDelete("utilisateur/{idUtilisateur}/offre/{idOffre}")]
        public async Task<ActionResult> SupprimerOffre(int idUtilisateur, int idOffre)
        {
            // Chercher le panier de l'utilisateur
            var panier = await _context.Paniers
                .Include(p => p.PaniersOffres)
                .FirstOrDefaultAsync(p => p.IdUtilisateur == idUtilisateur);

            if (panier == null)
                return NotFound("Panier introuvable.");

            // Chercher l'offre dans le panier
            var item = panier.PaniersOffres.FirstOrDefault(po => po.IdOffre == idOffre);
            if (item == null)
                return NotFound("Offre introuvable dans le panier.");

            _context.PaniersOffres.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }




    }
}
