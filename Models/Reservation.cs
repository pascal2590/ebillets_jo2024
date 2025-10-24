using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ebillets_jo2024_API.Models
{
    [Table("reservation")]
    public class Reservation
    {
        [Key]
        [Column("idReservation")]
        public int IdReservation { get; set; }

        [Required]
        [Column("idUtilisateur")]
        public int IdUtilisateur { get; set; }

        [Required]
        [Column("idOffre")]
        public int IdOffre { get; set; }

        [Column("quantite")]
        public int Quantite { get; set; } = 1; // Pour ajout au schéma au cas où une offre permettrait plusieurs entrées

        //[Required]
        [StringLength(128)]
        [Column("cleReservation")]
        public string CleReservation { get; set; }

        [Column("cleFinale")]
        public string CleFinale { get; set; }

        [Column("qrcode")]
        public string QrCode { get; set; }

        [Required]
        [StringLength(50)]
        [Column("statut")]
        public string Statut { get; set; } = "En attente";

        [Column("dateReservation")]
        public DateTime DateReservation { get; set; } = DateTime.Now;

        [ForeignKey("IdUtilisateur")]
        public Utilisateur Utilisateur { get; set; }

        [ForeignKey("IdOffre")]
        public Offre Offre { get; set; }

        // Relation
        public ICollection<Billet> Billets { get; set; }

        public Paiement Paiement { get; set; }
    }
}
