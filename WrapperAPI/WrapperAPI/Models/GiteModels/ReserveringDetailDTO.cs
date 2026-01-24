namespace WrapperAPI.Models.GiteModels
{
    public class ReserveringDetailDTO
    {
        public int detailID { get; set; }
        public int reserveringID {  get; set; }
        public int categorieID { get; set; }
        public int aantal {  get; set; }
        public double prijsOpMoment { get; set; }
        public CategorieDTO? categorie { get; set; }
    }
}
