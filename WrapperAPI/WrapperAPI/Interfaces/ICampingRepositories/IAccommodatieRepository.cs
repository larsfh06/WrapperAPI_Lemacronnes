using WrapperAPI.Models.CampingModels;

namespace WrapperAPI.Interfaces.ICampingRepositories
{
    public interface IAccommodatieRepository
    {
        IEnumerable<Accommodatie> GetAllAccommodaties();
        Accommodatie GetAccommodatieById(int id);
    }
}
