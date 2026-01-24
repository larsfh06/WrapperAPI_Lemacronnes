namespace WrapperAPI.Models.GiteModels
{
    public class HotelBoekingInputDTO
    {
        public int GastID { get; set; } // Alleen het ID is genoeg
        public int EenheidID { get; set; }
        public DateTime StartDatum { get; set; }
        public DateTime EindDatum { get; set; }
        public int AantalPersonen { get; set; }
        public int Platform {  get; set; }

    }
}
