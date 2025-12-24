using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ebillets_jo2024_API.Models
{
    [Table("panier")]
    public class Panier
    {
        [Key]
        [Column("idPanier")]
        public int IdPanier { get; set; }

        [Required]
        [Column("idUtilisateur")]
        public int IdUtilisateur { get; set; }

        [Column("dateCreation")]
        public DateTime DateCreation { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(IdUtilisateur))]
        public Utilisateur Utilisateur { get; set; }

        // Toujours initialiser les collections
        public ICollection<PanierOffre> PaniersOffres { get; set; } = new List<PanierOffre>();
    }
}
