namespace WrapperAPI.Models.GiteModels
{
    public class TariefDTO
    {
        public int tariefID { get; set; }
        public int typeID { get; set; }
        public int categorieID { get; set; }
        public int platformID { get; set; }
        public double prijs { get; set; }
        public bool taxStatus { get; set; }
        public double taxTarief {  get; set; }
        public DateTime geldigVan {  get; set; }
        public DateTime geldigTot { get; set; }
        public TariefTypeDTO type { get; set; }
        public CategorieDTO categorie { get; set; }
        public PlatformDTO platform { get; set; }

    }
}
