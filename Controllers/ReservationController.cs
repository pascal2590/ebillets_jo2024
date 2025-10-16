using ebillets_jo2024.Models;
using ebillets_jo2024_API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class ReservationController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public ReservationController(ApplicationDbContext context) { _context = context; }

    [HttpPost]
    public async Task<ActionResult<Reservation>> PostReservation(Reservation reservation)
    {
        var user = await _context.Utilisateurs.FindAsync(reservation.IdUtilisateur);
        var offre = await _context.Offres.FindAsync(reservation.IdOffre);
        if (user == null || offre == null)
            return BadRequest("Utilisateur ou offre introuvable.");

        // Générer cleReservation
        reservation.CleReservation = GenerateKey();
        reservation.Statut = "En attente";

        // Sauvegarder réservation
        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        // Créer billets : offre.nbPersonnes * reservation.quantite (par unité)
        int nbBillets = offre.NbPersonnes * (reservation.Quantite <= 0 ? 1 : reservation.Quantite);
        var billets = new List<Billet>();
        for (int i = 0; i < nbBillets; i++)
        {
            var cleBillet = GenerateKey();
            var cleFinale = ComputeSha256Hash(user.CleUtilisateur + reservation.CleReservation + cleBillet);
            var billet = new Billet
            {
                IdReservation = reservation.IdReservation,
                IdOffre = reservation.IdOffre,
                CleBillet = cleBillet,
                CleFinale = cleFinale,
                QrCode = "QR_" + cleFinale.Substring(0, 20),
                Statut = "Valide"
            };
            billets.Add(billet);
        }

        _context.Billets.AddRange(billets);
        await _context.SaveChangesAsync();

        var result = await _context.Reservations
            .Include(r => r.Billets)
            .FirstOrDefaultAsync(r => r.IdReservation == reservation.IdReservation);

        return CreatedAtAction(nameof(PostReservation), new { id = reservation.IdReservation }, result);
    }

    private string GenerateKey()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[32];
        rng.GetBytes(bytes);
        return BitConverter.ToString(bytes).Replace("-", "").ToLower();
    }

    private string ComputeSha256Hash(string rawData)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(rawData);
        var hash = sha256.ComputeHash(bytes);
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }
}
