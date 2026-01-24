namespace WrapperAPI.Models.RestaurantModels
{
    public class Bestelling
    {
        public int rekeningID { get; set; }
        public ICollection<Item> items { get; set; }
    }
}
