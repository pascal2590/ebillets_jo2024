using ebillets_jo2024_API.Data;
using ebillets_jo2024_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace ebillets_jo2024_API.Controllers
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
            try
            {
                var reservation = await _context.Reservations
                    .Include(r => r.Utilisateur)
                    .Include(r => r.Offre)
                    .FirstOrDefaultAsync(r => r.IdReservation == idReservation);

                if (reservation == null)
                    return NotFound("Réservation introuvable.");

                // ✅ Création du paiement
                var paiement = new Paiement
                {
                    IdReservation = idReservation,
                    Montant = reservation.Offre?.Prix ?? 0,
                    ModePaiement = "Mock",
                    Statut = "Réussi"
                };

                reservation.Statut = "Payée";

                _context.Paiements.Add(paiement);

                // ✅ Génération des clés du billet
                string cleBillet = Guid.NewGuid().ToString("N"); // clé aléatoire
                string cleUtilisateur = reservation.Utilisateur?.CleUtilisateur ?? string.Empty;
                string cleFinale = cleUtilisateur + cleBillet;

                // ✅ Création du billet
                var billet = new Billet
                {
                    IdReservation = reservation.IdReservation,
                    IdOffre = reservation.IdOffre,
                    IdUtilisateur = reservation.IdUtilisateur,
                    DateEmission = DateTime.Now,
                    CleBillet = cleBillet,
                    CleFinale = cleFinale,
                    QrCode = cleFinale, // provisoire
                    Statut = "Valide",
                    TitulaireNom = reservation.Utilisateur?.Nom ?? "Utilisateur"
                };

                _context.Billets.Add(billet);

                await _context.SaveChangesAsync();

                // ✅ Réponse API
                return Ok(new
                {
                    message = "Paiement effectué avec succès.",
                    idReservation = reservation.IdReservation,
                    montant = paiement.Montant,
                    statut = paiement.Statut,
                    billet = new
                    {
                        billet.IdBillet,
                        billet.CleBillet,
                        billet.CleFinale,
                        billet.QrCode,
                        billet.Statut,
                        billet.DateEmission
                    }
                });

            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur dans PaiementController : {ex.Message}");
                return StatusCode(500, new { message = "Erreur lors du paiement", erreur = ex.Message });
            }
        }
    }
}
