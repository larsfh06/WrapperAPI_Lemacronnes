namespace WrapperAPI.Models.GiteModels
{
    public class ReserveringDTO
    {
        public int reserveringID { get; set; }
        public int gastID { get; set; }
        public int eenheidID { get; set; }
        public int platformID { get; set; }
        public DateTime startdatum { get; set; }
        public DateTime einddatum { get; set; }
        public string status { get; set; }
        public GastDTO gast { get; set; }
        public EenheidDTO eenheid { get; set; }
        public PlatformDTO platform { get; set; }
    }
}
