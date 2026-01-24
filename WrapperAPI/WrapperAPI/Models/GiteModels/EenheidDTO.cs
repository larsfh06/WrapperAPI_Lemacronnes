namespace WrapperAPI.Models.GiteModels
{
    public class EenheidDTO
    {
        public int eenheidID {  get; set; }
        public string naam {  get; set; }
        public int typeID { get; set; }
        public int maxCapaciteit { get; set; }
        public int parentEenheidID { get; set; }
        public AccommodatieTypeDTO type { get; set; }
        public ICollection<EenheidDTO> childEenheden { get; set; }
        public bool isBeschikbaar {  get; set; }
    }
}
