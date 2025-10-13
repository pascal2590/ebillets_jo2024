using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ebillets_jo2024.Models
{
    [Table("billet")]
    public class Billet
    {
        [Key]
        [Column("idBillet")]
        public int IdBillet { get; set; }

        [Required]
        [Column("idReservation")]
        public int IdReservation { get; set; }

        [Required]
        [Column("idOffre")]
        public int IdOffre { get; set; }

        [Required, StringLength(64)]
        [Column("cleBillet")]
        public string CleBillet { get; set; }

        [Required, StringLength(128)]
        [Column("cleFinale")]
        public string CleFinale { get; set; }

        [Column("qrcode")]
        public string QrCode { get; set; }

        [Column("statut")]
        public string Statut { get; set; } = "Valide";

        [Column("titulaireNom")]
        public string TitulaireNom { get; set; }

        [Column("dateEmission")]
        public DateTime DateEmission { get; set; } = DateTime.Now;

        [ForeignKey("IdReservation")]
        public Reservation Reservation { get; set; }

        [ForeignKey("IdOffre")]
        public Offre Offre { get; set; }

        public ICollection<ScanBillet> ScansBillets { get; set; }
    }
}
