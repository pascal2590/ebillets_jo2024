using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ebillets_jo2024_API.Models
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

        [Column("lieuScan")]
        public string LieuScan { get; set; } = "ScanBillet";

        [Column("resultatScan")]
        public string ResultatScan { get; set; } = "Valide";  // ← ici le nom correspond à la colonne MySQL

        // 🔹 Propriétés de navigation
        public Billet Billet { get; set; }

        // Suppose que tu as une entité Employe
        public Utilisateur Employe { get; set; }
    }
}
