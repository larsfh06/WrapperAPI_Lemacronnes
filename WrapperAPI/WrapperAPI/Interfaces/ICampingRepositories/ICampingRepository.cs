using WrapperAPI.Models.CampingModels;

namespace WrapperAPI.Interfaces.ICampingRepositories
{
    public interface ICampingRepository
    {
        Camping GetCampingById(int id);
    }
}
