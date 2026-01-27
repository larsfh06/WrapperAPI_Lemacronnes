using System.Text.Json.Serialization;

namespace WrapperAPI.Models.RestaurantModels
{
    public class SendingReservering
    {
        [JsonPropertyName("boekingID")]
        public int boekingID { get; set; }

        [JsonPropertyName("datumTijd")]
        public DateTime datumTijd { get; set; }

        // De velden die de Azure API echt wil hebben:
        [JsonPropertyName("aantalVolwassenen")]
        public int aantalVolwassenen { get; set; }

        [JsonPropertyName("aantalJongeKinderen")]
        public int aantalJongeKinderen { get; set; }

        [JsonPropertyName("aantalOudereKinderen")]
        public int aantalOudereKinderen { get; set; }

        [JsonPropertyName("tafelID")]
        public int tafelID { get; set; }



    }
}
