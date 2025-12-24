using ebillets_jo2024_API.Data;
using ebillets_jo2024_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class ScanBilletController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ScanBilletController(ApplicationDbContext context)
    {
        _context = context;
    }

    // POST: api/ScanBillet/{cleFinale}
    [HttpPost("{cleFinale}")]
    public async Task<IActionResult> ScannerBillet(string cleFinale)
    {
        // Récupération ID employé via token (ou fallback pour test)
        int idEmploye;
        var userIdClaim = User.FindFirst("sub");
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out idEmploye))
        {
            idEmploye = 23; // mode test
        }

        // Recherche du billet avec l'offre liée et l'utilisateur
        var billet = await _context.Billets
            .Include(b => b.Offre) // Inclut l'offre
            .FirstOrDefaultAsync(b => b.CleFinale == cleFinale);

        if (billet == null)
            return NotFound(new { message = "Billet invalide." });

        // Récupération des infos de l'utilisateur
        var utilisateur = await _context.Utilisateurs
            .FirstOrDefaultAsync(u => u.IdUtilisateur == billet.IdUtilisateur);

        string clientNom = utilisateur?.Nom ?? "Inconnu";
        string clientPrenom = utilisateur?.Prenom ?? "Inconnu";

        // Compter les scans déjà effectués
        int scansCount = await _context.ScansBillets
            .CountAsync(s => s.IdBillet == billet.IdBillet);

        // Nombre maximum de scans autorisés
        int maxScans = billet.Offre.NbPersonnes;

        if (scansCount >= maxScans)
        {
            return BadRequest(new { message = "Billet déjà scanné (limite atteinte)." });
        }

        // Enregistrement du scan
        var scan = new ScanBillet
        {
            IdBillet = billet.IdBillet,
            IdEmploye = idEmploye,
            DateScan = DateTime.Now,
            ResultatScan = "Valide",
            LieuScan = "Entrée principale"
        };

        _context.ScansBillets.Add(scan);

        // Si tous les scans faits → marquer le billet comme utilisé
        if (scansCount + 1 >= maxScans)
            billet.Statut = "Utilisé";

        await _context.SaveChangesAsync();

        // Retourne les infos nécessaires côté Angular
        return Ok(new
        {
            message = $"Scan validé ({scansCount + 1}/{maxScans})",
            nomOffre = billet.Offre.NomOffre,
            clientNom,
            clientPrenom
        });
    }
}
