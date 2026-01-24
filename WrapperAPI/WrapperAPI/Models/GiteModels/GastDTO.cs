namespace WrapperAPI.Models.GiteModels
{
    public class GastDTO
    {
        public int gastID {  get; set; }
        public string naam { get; set; }
        public string email { get; set; }
        public string? tel { get; set; }
        public string straat { get; set; }
        public string huisnr { get; set; }
        public string postcode { get; set; }
        public string plaats { get; set; }
        public string land { get; set; }
        public string iban { get; set; }
    }
}
