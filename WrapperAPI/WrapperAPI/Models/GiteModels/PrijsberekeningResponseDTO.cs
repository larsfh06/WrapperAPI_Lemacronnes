namespace WrapperAPI.Models.GiteModels
{
    public class PrijsberekeningResponseDTO
    {
        public double totaalPrijs {  get; set; }
        public int aantalNachten { get; set; }
        public int aantalPersonen { get; set; }
        public double prijsPerNacht { get; set; }
    }
}
