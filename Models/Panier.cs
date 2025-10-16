using ebillets_jo2024_API.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ebillets_jo2024.Models
{
    [Table("panier")]
    public class Panier
    {
        [Key]
        public int IdPanier { get; set; }

        [Required]
        public int IdUtilisateur { get; set; }

        public DateTime DateCreation { get; set; } = DateTime.Now;

        [ForeignKey("IdUtilisateur")]
        public Utilisateur Utilisateur { get; set; }

        // Relation
        public ICollection<PanierOffre> PaniersOffres { get; set; }
    }
}
