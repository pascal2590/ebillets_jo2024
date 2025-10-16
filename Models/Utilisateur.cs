using ebillets_jo2024.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ebillets_jo2024_API.Models
{
    [Table("utilisateur")]
    public class Utilisateur
    {
        [Key]
        [Column("idUtilisateur")]
        public int IdUtilisateur { get; set; }

        [Required, StringLength(50)]
        [Column("nom")]
        public string Nom { get; set; }

        [Required, StringLength(50)]
        [Column("prenom")]
        public string Prenom { get; set; }

        [Required, EmailAddress, StringLength(100)]
        [Column("email")]
        public string Email { get; set; }

        [Required, StringLength(255)]
        [Column("motDePasseHash")]
        public string MotDePasseHash { get; set; }

        [Required, StringLength(64)]
        [Column("cleUtilisateur")]
        public string CleUtilisateur { get; set; }

        [Column("role")]
        public string Role { get; set; } = "Client";

        [Column("dateCreation")]
        public DateTime DateCreation { get; set; } = DateTime.Now;

        public ICollection<Panier> Paniers { get; set; }
        public ICollection<Reservation> Reservations { get; set; }
    }
}
