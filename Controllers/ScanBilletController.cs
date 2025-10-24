//using ebillets_jo2024.Models;
using ebillets_jo2024_API.Data;
using ebillets_jo2024_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class ScanBilletController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public ScanBilletController(ApplicationDbContext context) { _context = context; }

    // POST: api/ScanBillet/{cleFinale}?idEmploye=1
    [HttpPost("{cleFinale}")]
    public async Task<IActionResult> ScannerBillet(string cleFinale, [FromQuery] int idEmploye = 0)
    {
        var billet = await _context.Billets.FirstOrDefaultAsync(b => b.CleFinale == cleFinale);
        if (billet == null) return NotFound("Billet invalide.");
        if (billet.Statut == "Utilisé") return BadRequest("Billet déjà utilisé.");

        billet.Statut = "Utilisé";
        _context.Billets.Update(billet);

        var scan = new ScanBillet
        {
            IdBillet = billet.IdBillet,
            IdEmploye = idEmploye > 0 ? idEmploye : 1,
            Resultat = "Valide"
        };
        _context.ScansBillets.Add(scan);
        await _context.SaveChangesAsync();

        return Ok(new { billet.IdBillet, billet.QrCode, message = "Billet validé" });
    }
}
