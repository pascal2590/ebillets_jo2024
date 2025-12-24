using ebillets_jo2024_API.Data;
using ebillets_jo2024_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ebillets_jo2024_API.Models.DTO;

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

        /// Ajoute une offre dans le panier        
        [HttpPost("ajouter")]
        public async Task<IActionResult> AjouterAuPanier([FromBody] PanierOffreDto panierOffreDto)
        {
            Console.WriteLine($"DEBUG: Requête reçue pour utilisateur {panierOffreDto.IdUtilisateur} et offre {panierOffreDto.IdOffre}");

            var utilisateur = await _context.Utilisateurs.FindAsync(panierOffreDto.IdUtilisateur);
            if (utilisateur == null)
            {
                Console.WriteLine("DEBUG: Utilisateur non trouvé");
                return BadRequest($"Utilisateur {panierOffreDto.IdUtilisateur} n'existe pas.");
            }

            var offre = await _context.Offres.FindAsync(panierOffreDto.IdOffre);
            if (offre == null)
            {
                Console.WriteLine("DEBUG: Offre non trouvée");
                return BadRequest($"Offre {panierOffreDto.IdOffre} n'existe pas.");
            }

            var panier = await _context.Paniers
                .Include(p => p.PaniersOffres)
                .FirstOrDefaultAsync(p => p.IdUtilisateur == panierOffreDto.IdUtilisateur);

            if (panier == null)
            {
                panier = new Panier
                {
                    IdUtilisateur = panierOffreDto.IdUtilisateur,
                    DateCreation = DateTime.UtcNow
                };
                _context.Paniers.Add(panier);
                await _context.SaveChangesAsync();
                Console.WriteLine("DEBUG: Nouveau panier créé");
            }

            // Vérifier si l'offre existe déjà dans le panier
            var existe = panier.PaniersOffres.Any(po => po.IdOffre == panierOffreDto.IdOffre);
            if (existe)
            {
                Console.WriteLine("DEBUG: Offre déjà dans le panier");
                return Conflict(new { message = "Cette offre est déjà dans le panier" });
            }

            var panierOffre = new PanierOffre
            {
                IdPanier = panier.IdPanier,
                IdOffre = panierOffreDto.IdOffre,
                Quantite = panierOffreDto.Quantite
            };

            _context.PaniersOffres.Add(panierOffre);
            await _context.SaveChangesAsync();

            Console.WriteLine("DEBUG: Offre ajoutée avec succès");
            return Ok(new { message = "Offre ajoutée au panier" });
        }

        [HttpGet("utilisateur/{idUtilisateur}")]
        public IActionResult GetPanierUtilisateur(int idUtilisateur)
        {
            var panier = _context.Paniers
                .Include(p => p.PaniersOffres)
                .ThenInclude(po => po.Offre)
                .FirstOrDefault(p => p.IdUtilisateur == idUtilisateur);

            if (panier == null)
                return Ok(new List<PanierOffreDtoRetour>());

            var result = panier.PaniersOffres.Select(po => new PanierOffreDtoRetour
            {
                IdOffre = po.IdOffre,
                NomOffre = po.Offre.NomOffre,
                Prix = po.Offre.Prix,
                Quantite = po.Quantite
            }).ToList();

            return Ok(result);
        }


        [HttpDelete("utilisateur/{idUtilisateur}/offre/{idOffre}")]
        public async Task<IActionResult> SupprimerOffre(int idUtilisateur, int idOffre)
        {
            var panier = await _context.Paniers
                .Include(p => p.PaniersOffres)
                .FirstOrDefaultAsync(p => p.IdUtilisateur == idUtilisateur);

            if (panier == null)
                return NotFound("Panier introuvable.");

            var item = panier.PaniersOffres.FirstOrDefault(po => po.IdOffre == idOffre);
            if (item == null)
                return NotFound("Offre introuvable dans le panier.");

            _context.PaniersOffres.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("utilisateur/{idUtilisateur}/vider")]
        public async Task<IActionResult> ViderPanier(int idUtilisateur)
        {
            var panier = await _context.Paniers
                .Include(p => p.PaniersOffres)
                .FirstOrDefaultAsync(p => p.IdUtilisateur == idUtilisateur);

            if (panier == null)
                return NotFound("Panier introuvable.");

            _context.PaniersOffres.RemoveRange(panier.PaniersOffres);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Panier vidé avec succès" });
        }
    }
}
