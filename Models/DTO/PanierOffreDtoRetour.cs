namespace ebillets_jo2024_API.Models.DTO
{
    /// DTO pour retourner les informations d'une offre dans le panier côté front        
    public class PanierOffreDtoRetour
    {
        public int IdOffre { get; set; }
        public string NomOffre { get; set; }
        public decimal Prix { get; set; }
        public int Quantite { get; set; }
    }
}
