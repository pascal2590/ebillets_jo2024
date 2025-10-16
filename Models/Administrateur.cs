using ebillets_jo2024_API.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ebillets_jo2024.Models
{
    [Table("administrateur")]
    public class Administrateur
    {
        [Key]
        [Column("idAdmin")]
        public int IdAdmin { get; set; }

        [Required]
        [Column("idUtilisateur")]
        public int IdUtilisateur { get; set; }

        [Column("roleDetail")]
        public string RoleDetail { get; set; } = "Administrateur";

        [Column("dateAjout")]
        public DateTime DateAjout { get; set; } = DateTime.Now;

        [ForeignKey("IdUtilisateur")]
        public Utilisateur Utilisateur { get; set; }
    }
}
