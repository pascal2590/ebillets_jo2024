using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ebillets_jo2024_API.Models
{
    [Table("offre")]
    public class Offre
    {
        [Key]
        public int IdOffre { get; set; }

        [Required, StringLength(50)]
        public string NomOffre { get; set; }

        public string Description { get; set; }

        [Required]
        public int NbPersonnes { get; set; }

        [Required]
        public decimal Prix { get; set; }

        public DateTime DateCreation { get; set; } = DateTime.Now;

        // Relations
        public ICollection<PanierOffre> PaniersOffres { get; set; }
        public ICollection<Reservation> Reservations { get; set; }
    }
}
