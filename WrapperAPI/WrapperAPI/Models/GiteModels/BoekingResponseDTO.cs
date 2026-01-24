namespace WrapperAPI.Models.GiteModels
{
    public class BoekingResponseDTO
    {
        public int reserveringID { get; set; }
        public string bevestiging { get; set; }
        public string eenheidNaam { get; set; }
        public DateTime startDatum { get; set; }
        public DateTime eindDatum { get; set; }
        public double totaalPrijs { get; set; }
        public bool succes { get; set; }
        public string foutMelding { get; set; }
    }
}
