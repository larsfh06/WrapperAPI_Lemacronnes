using WrapperAPI.Models.CampingModels;
using WrapperAPI.Models.GiteModels;
using WrapperAPI.Models.RestaurantModels;
using WrapperAPI.NewFolder;

namespace WrapperAPI.Models
{
    public class FlexiItemDTO
    {
        public string AccommodatieType { get; set; } // "Hotel", "Gite", "Restaurant", "Camping"
        public int GastID { get; set; }
        public int EenheidID { get; set; } // Verwijst naar KamerID, GiteID, TafelID of AccommodatieID
        public DateTime StartDatum { get; set; }
        public DateTime EindDatum { get; set; }

        // Opsplitsing voor Camping, totaal voor de rest
        public int AantalVolwassenen { get; set; }
        public int AantalJongeKinderen { get; set; } 
        public int AantalOudereKinderen { get; set; } 

        public int Platform { get; set; }
        public string? Opmerking { get; set; }
    }
    public class FlexiCombiDTO
    {
        // De JSON bevat nu één lijst met alle soorten boekingen
        public List<FlexiItemDTO> Boekingen { get; set; } = new();
    }
}
