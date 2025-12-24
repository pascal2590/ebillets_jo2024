using System;
using System.Collections.Generic;

namespace ebillets_jo2024_API.Models.DTO
{
    public class PanierDto
    {
        public int IdPanier { get; set; }

        // Date de création du panier
        public DateTime DateCreation { get; set; }

        // Liste des offres dans le panier
        public List<PanierOffreDtoRetour> PaniersOffres { get; set; } = new List<PanierOffreDtoRetour>();
    }
}
