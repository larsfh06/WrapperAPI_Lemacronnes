using WrapperAPI.Models.GiteModels;

namespace WrapperAPI.Interfaces.IHotelRepositories
{
    public interface IHotelGastRepository
    {
        GastDTO GetHotelGastById(int id);
    }

}
