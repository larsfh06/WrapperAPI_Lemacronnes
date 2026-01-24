namespace WrapperAPI.Models.RestaurantModels
{
    public class Rekening
    {
        public int rekeningID { get; set; }
        public string status { get; set; }
        public string betaalMethode { get; set; }
        public double reedsBetaald { get; set; }
        public double totaalBedrag { get; set; }

    }
}
