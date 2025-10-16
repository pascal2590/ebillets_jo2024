using ebillets_jo2024_API.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ebillets_jo2024.Models
{
    [Table("scan_billet")]
    public class ScanBillet
    {
        [Key]
        [Column("idScan")]
        public int IdScan { get; set; }

        [Required]
        [Column("idBillet")]
        public int IdBillet { get; set; }

        [Required]
        [Column("idEmploye")]
        public int IdEmploye { get; set; }

        [Column("dateScan")]
        public DateTime DateScan { get; set; } = DateTime.Now;

        [Column("resultat")]
        public string Resultat { get; set; } = "Valide";

        [ForeignKey("IdBillet")]
        public Billet Billet { get; set; }

        [ForeignKey("IdEmploye")]
        public Utilisateur Employe { get; set; }
    }
}
