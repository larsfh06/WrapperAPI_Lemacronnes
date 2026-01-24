using WrapperAPI.Models.RestaurantModels;

namespace WrapperAPI.Interfaces.IRestaurantRepositories
{
    public interface ITafelRepository
    {
        IEnumerable<Tafel> GetAllTafels();

        Tafel GetTafelsById(int id);
    }
}
