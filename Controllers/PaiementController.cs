using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ebillets_jo2024.Data;
using ebillets_jo2024.Models;
using System.Threading.Tasks;

namespace ebillets_jo2024.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaiementController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PaiementController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("{idReservation}")]
        public async Task<IActionResult> PayerReservation(int idReservation)
        {
            var reservation = await _context.Reservations.FindAsync(idReservation);
            if (reservation == null)
                return NotFound("Réservation introuvable.");

            var paiement = new Paiement
            {
                IdReservation = idReservation,
                Montant = 100.0m, // Simulation
                ModePaiement = "Mock",
                Statut = "Réussi"
            };

            reservation.Statut = "Payée";
            _context.Paiements.Add(paiement);
            await _context.SaveChangesAsync();

            return Ok(paiement);
        }
    }
}
