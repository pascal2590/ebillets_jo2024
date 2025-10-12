using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ebillets_jo2024.Models
{
    public class ScanBillet
    {
        [Key]
        public int IdScan { get; set; }

        [Required]
        public int IdReservation { get; set; }

        [Required]
        public int IdEmploye { get; set; }

        public DateTime DateScan { get; set; } = DateTime.Now;

        public string Resultat { get; set; } = "Valide";

        [ForeignKey("IdReservation")]
        public Reservation Reservation { get; set; }

        [ForeignKey("IdEmploye")]
        public Utilisateur Employe { get; set; }
    }
}
