using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ebillets_jo2024.Models
{
    public class Reservation
    {
        [Key]
        public int IdReservation { get; set; }

        [Required]
        public int IdUtilisateur { get; set; }

        [Required]
        public int IdOffre { get; set; }

        [Required, StringLength(64)]
        public string CleReservation { get; set; }

        [Required, StringLength(64)]
        public string CleFinale { get; set; }

        public string QrCode { get; set; }

        public string Statut { get; set; } = "En attente";

        public DateTime DateReservation { get; set; } = DateTime.Now;

        [ForeignKey("IdUtilisateur")]
        public Utilisateur Utilisateur { get; set; }

        [ForeignKey("IdOffre")]
        public Offre Offre { get; set; }

        public Paiement Paiement { get; set; }
    }
}
