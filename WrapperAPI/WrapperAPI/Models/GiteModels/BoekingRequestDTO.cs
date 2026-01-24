 namespace WrapperAPI.Models.GiteModels
{
    public class BoekingRequestDTO
    {
        public string gastNaam { get; set; }
        public string gastEmail { get; set; }
        public string gastTel {  get; set; }
        public string gastStraat { get; set; }
        public string gastHuisnr { get; set; }
        public string gastPostcode { get; set; }
        public string gastPlaats { get; set; }
        public string gastLand {  get; set; }
        public int eenheidID { get; set; }
        public int platformID { get; set; }
        public DateTime startDatum { get; set; }
        public DateTime eindDatum { get; set; }
        public int aantalPersonen {  get; set; }

    }
}
