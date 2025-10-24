using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ebillets_jo2024_API.Models
{
    [Table("paiement")]
    public class Paiement
    {
        [Key]
        public int IdPaiement { get; set; }

        [Required]
        public int IdReservation { get; set; }

        [Required]
        public decimal Montant { get; set; }

        [Required]
        public string ModePaiement { get; set; } = "Mock";

        public DateTime DatePaiement { get; set; } = DateTime.Now;

        public string Statut { get; set; } = "Réussi";

        [ForeignKey("IdReservation")]
        public Reservation Reservation { get; set; }
    }
}
