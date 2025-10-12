using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ebillets_jo2024.Models
{
    [Table("panier_offre")]
    public class PanierOffre
    {
        [Key, Column(Order = 0)]
        public int IdPanier { get; set; }

        [Key, Column(Order = 1)]
        public int IdOffre { get; set; }

        [Required]
        public int Quantite { get; set; } = 1;

        [ForeignKey("IdPanier")]
        public Panier Panier { get; set; }

        [ForeignKey("IdOffre")]
        public Offre Offre { get; set; }
    }
}
