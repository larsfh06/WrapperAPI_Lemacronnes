using System.Text.Json.Serialization;

namespace WrapperAPI.Models.RestaurantModels
{
    public class RecievingReservering
    {
        [JsonPropertyName("reserveringID")]
        public int reserveringID { get; set; }
        [JsonPropertyName("datumTijd")]

        public DateTime datumTijd { get; set; }
        [JsonPropertyName("tafelID")]

        public int tafelID { get; set; }
        [JsonPropertyName("tafelnummer")]

        public int tafelnummer {  get; set; }
        [JsonPropertyName("aantalPersonen")]

        public int aantalPersonen { get; set; }
        [JsonPropertyName("isGeannuleerd")]

        public bool isGeannuleerd { get; set; }
        [JsonPropertyName("rekeningStatus")]

        public string rekeningStatus { get; set; } = string.Empty;
        [JsonPropertyName("tafel")]

        public Tafel? tafel { get; set; }

    }
}
