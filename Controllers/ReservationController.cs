using ebillets_jo2024_API.Data;
using ebillets_jo2024_API.Models;
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
    public ReservationController(ApplicationDbContext context)
    {
        _context = context;
    }

    // ============================================
    // 🔹 POST : /api/Reservation
    // ============================================
    [HttpPost]
    public async Task<ActionResult<Reservation>> PostReservation(Reservation reservation)
    {
        var user = await _context.Utilisateurs.FindAsync(reservation.IdUtilisateur);
        var offre = await _context.Offres.FindAsync(reservation.IdOffre);

        if (user == null || offre == null)
            return BadRequest("Utilisateur ou offre introuvable.");

        // Vérifier ou générer la clé utilisateur
        if (string.IsNullOrEmpty(user.CleUtilisateur))
        {
            user.CleUtilisateur = GenerateKey();
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        // Générer la clé de réservation
        reservation.CleReservation = GenerateKey();
        reservation.Statut = "En attente";

        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        // Génération des billets
        int nbBillets = offre.NbPersonnes * (reservation.Quantite <= 0 ? 1 : reservation.Quantite);
        var billets = new List<Billet>();

        for (int i = 0; i < nbBillets; i++)
        {
            var cleBillet = GenerateKey();
            var cleFinale = ComputeSha256Hash(user.CleUtilisateur + reservation.CleReservation + cleBillet);

            billets.Add(new Billet
            {
                IdReservation = reservation.IdReservation,
                IdOffre = reservation.IdOffre,
                CleBillet = cleBillet,
                CleFinale = cleFinale,
                QrCode = "QR_" + cleFinale.Substring(0, 20),
                Statut = "Valide"
            });
        }

        _context.Billets.AddRange(billets);
        await _context.SaveChangesAsync();

        var result = await _context.Reservations
            .Include(r => r.Billets)
            .FirstOrDefaultAsync(r => r.IdReservation == reservation.IdReservation);

        return CreatedAtAction(nameof(PostReservation), new { id = reservation.IdReservation }, result);
    }

    // ============================================
    // 🔹 POST : /api/Reservation/commander
    // ============================================
    [HttpPost("commander")]
    public async Task<ActionResult> Commander([FromBody] CommandeRequest request)
    {
        var user = await _context.Utilisateurs.FindAsync(request.IdUtilisateur);
        if (user == null)
            return BadRequest("Utilisateur introuvable.");

        // Vérifier ou générer la clé utilisateur
        if (string.IsNullOrEmpty(user.CleUtilisateur))
        {
            user.CleUtilisateur = GenerateKey();
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        var reservationsCreees = new List<object>();

        foreach (var item in request.Panier)
        {
            var offre = await _context.Offres.FindAsync(item.IdOffre);
            if (offre == null)
                continue;

            var reservation = new Reservation
            {
                IdUtilisateur = request.IdUtilisateur,
                IdOffre = item.IdOffre,
                Quantite = item.Quantite,
                CleReservation = GenerateKey(),
                Statut = "En attente"
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            int nbBillets = offre.NbPersonnes * (item.Quantite <= 0 ? 1 : item.Quantite);
            var billets = new List<Billet>();



            for (int i = 0; i < nbBillets; i++)
            {
                var cleBillet = GenerateKey();

                if (string.IsNullOrEmpty(user.CleUtilisateur))
                   throw new Exception("CleUtilisateur est vide");
                if (string.IsNullOrEmpty(reservation.CleReservation))
                   throw new Exception("CleReservation est vide");
                if (string.IsNullOrEmpty(cleBillet))
                   throw new Exception("CleBillet est vide");               

                Console.WriteLine($"DEBUG CleUtilisateur={user.CleUtilisateur}, CleReservation={reservation.CleReservation}, CleBillet={cleBillet}"); // Ligne de debug

                var cleFinale = ComputeSha256Hash(user.CleUtilisateur + reservation.CleReservation + cleBillet);

                if (string.IsNullOrEmpty(cleFinale))
                    throw new Exception("Erreur génération cleFinale");

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

            reservationsCreees.Add(new
            {
                reservation.IdReservation,
                reservation.CleReservation,
                Billets = billets.Select(b => new { b.IdBillet, b.CleFinale, b.QrCode })
            });
        }

        return Ok(new
        {
            message = "Commandes créées avec succès",
            reservations = reservationsCreees
        });
    }

    // ============================================
    // 🔹 Modèles internes
    // ============================================
    public class CommandeRequest
    {
        public int IdUtilisateur { get; set; }
        public List<PanierItem> Panier { get; set; }
    }

    public class PanierItem
    {
        public int IdOffre { get; set; }
        public int Quantite { get; set; }
    }

    // ============================================
    // 🔹 Méthodes utilitaires
    // ============================================
    private string GenerateKey()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[32];
        rng.GetBytes(bytes);
        return BitConverter.ToString(bytes).Replace("-", "").ToLower();
    }

    private string ComputeSha256Hash(string rawData)
    {
        if (string.IsNullOrEmpty(rawData))
            throw new ArgumentException("Les données à hasher ne peuvent pas être nulles ou vides.");

        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(rawData);
        var hash = sha256.ComputeHash(bytes);
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }

    // ============================================
    // 🔹 GET : /api/Reservation/utilisateur/{id}
    // Récupère toutes les réservations d’un utilisateur avec ses billets et offres associées
    // ============================================
    [HttpGet("utilisateur/{idUtilisateur}")]
    public async Task<ActionResult> GetReservationsParUtilisateur(int idUtilisateur)
    {
        var utilisateur = await _context.Utilisateurs.FindAsync(idUtilisateur);
        if (utilisateur == null)
            return NotFound("Utilisateur introuvable.");

        var reservations = await _context.Reservations
            .Include(r => r.Billets)
            .Include(r => r.Offre)
            .Where(r => r.IdUtilisateur == idUtilisateur)
            .Select(r => new
            {
                r.IdReservation,
                r.CleReservation,
                r.Statut,
                Offre = new
                {
                    r.Offre.IdOffre,
                    r.Offre.NomOffre,
                    r.Offre.Prix,
                    r.Offre.NbPersonnes,
                    r.Offre.DateCreation
                },
                Billets = r.Billets.Select(b => new
                {
                    b.IdBillet,
                    b.CleFinale,
                    b.QrCode,
                    b.Statut
                })
            })
            .ToListAsync();

        return Ok(new
        {
            Utilisateur = new
            {
                utilisateur.IdUtilisateur,
                utilisateur.Nom,
                utilisateur.Prenom,
                utilisateur.CleUtilisateur
            },
            Reservations = reservations
        });
    }

}
