using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ebillets_jo2024.Models;
using System.Threading.Tasks;
using ebillets_jo2024_API.Data;

namespace ebillets_jo2024_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaiementController(ApplicationDbContext context) : ControllerBase
    {
        private readonly ApplicationDbContext _context = context;

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
