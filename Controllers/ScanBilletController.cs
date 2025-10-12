using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ebillets_jo2024.Data;
using ebillets_jo2024.Models;
using System.Threading.Tasks;

namespace ebillets_jo2024.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScanBilletController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ScanBilletController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("{cleFinale}")]
        public async Task<IActionResult> ScannerBillet(string cleFinale)
        {
            var reservation = await _context.Reservations.FirstOrDefaultAsync(r => r.CleFinale == cleFinale);
            if (reservation == null)
                return NotFound("Billet invalide.");

            var scan = new ScanBillet
            {
                IdReservation = reservation.IdReservation,
                IdEmploye = 1, // Exemple : employé connecté
                Resultat = "Valide"
            };

            _context.ScansBillets.Add(scan); // Remplacé ScanBillet en ScansBillets voir fichier "ApplicationDbContext.cs"
            await _context.SaveChangesAsync();

            return Ok(scan);
        }
    }
}
