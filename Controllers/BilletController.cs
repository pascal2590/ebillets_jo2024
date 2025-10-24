//using ebillets_jo2024.Data;
//using ebillets_jo2024.Models;
using ebillets_jo2024_API.Data;
using ebillets_jo2024_API.Models;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ebillets_jo2024.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BilletController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BilletController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult CreerBillets([FromBody] List<Billet> billets)
        {
            if (billets == null || billets.Count == 0)
                return BadRequest("Aucun billet à créer.");

            // Simulation d’un paiement
            var paiementOk = true;
            if (!paiementOk)
                return BadRequest("Échec du paiement (simulation).");

            foreach (var billet in billets)
            {
                var utilisateur = _context.Utilisateurs.FirstOrDefault(u => u.IdUtilisateur == billet.IdUtilisateur);
                if (utilisateur == null)
                    return BadRequest($"Utilisateur {billet.IdUtilisateur} introuvable.");

                // 1. Génération d’une clé aléatoire pour ce billet
                billet.CleSecrete = GenererCleAleatoire();

                // 2. Clé finale = concaténation des deux clés
                billet.CleFinale = utilisateur.CleUtilisateur + billet.CleSecrete;

                // 3. Génération du QR code
                billet.QrCode = GenererQrCodeBase64(billet.CleFinale);

                billet.DateEmission = DateTime.Now;

                _context.Billets.Add(billet);
            }

            _context.SaveChanges();
            return Ok(new { message = "Billets créés avec succès (mock paiement + QR codes générés)" });
        }

        // Génération d’une clé aléatoire de 32 caractères
        private string GenererCleAleatoire()
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[16];
            rng.GetBytes(bytes);
            return BitConverter.ToString(bytes).Replace("-", "");
        }

        // Génération du QR code encodé en base64
        private string GenererQrCodeBase64(string data)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            var qrBytes = qrCode.GetGraphic(20);
            return "data:image/png;base64," + Convert.ToBase64String(qrBytes);
        }
    }
}
