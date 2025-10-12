using ebillets_jo2024.Data;
using ebillets_jo2024.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ebillets_jo2024.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReservationController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetReservations()
        {
            var reservations = await _context.Reservations
                .Include(r => r.Utilisateur)
                .Include(r => r.Offre)
                .ToListAsync();
            return Ok(reservations);
        }

        [HttpPost]
        public async Task<ActionResult<Reservation>> PostReservation(Reservation reservation)
        {
            reservation.CleReservation = GenerateKey();
            var user = await _context.Utilisateurs.FindAsync(reservation.IdUtilisateur);

            if (user == null)
                return BadRequest("Utilisateur introuvable.");

            reservation.CleFinale = user.CleUtilisateur + reservation.CleReservation;
            reservation.QrCode = "QR_" + reservation.CleFinale.Substring(0, 10); // Simulation du QRCode

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(PostReservation), new { id = reservation.IdReservation }, reservation);
        }

        private string GenerateKey()
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[32];
            rng.GetBytes(bytes);
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}
