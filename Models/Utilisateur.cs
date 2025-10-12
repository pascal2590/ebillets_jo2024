using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ebillets_jo2024.Models
{
    public class Utilisateur
    {
        [Key]
        public int IdUtilisateur { get; set; }

        [Required, StringLength(50)]
        public string Nom { get; set; }

        [Required, StringLength(50)]
        public string Prenom { get; set; }

        [Required, EmailAddress, StringLength(100)]
        public string Email { get; set; }

        [Required, StringLength(255)]
        public string MotDePasseHash { get; set; }

        [Required, StringLength(64)]
        public string CleUtilisateur { get; set; }

        [Required]
        public string Role { get; set; } = "Client";

        public DateTime DateCreation { get; set; } = DateTime.Now;

        // Relations
        public ICollection<Panier> Paniers { get; set; }
        public ICollection<Reservation> Reservations { get; set; }
    }
}
