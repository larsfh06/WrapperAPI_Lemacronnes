using WrapperAPI.Models.GiteModels;

namespace WrapperAPI.Interfaces.IGiteRepositories
{
    public interface IGiteGastRepository
    {
        GastDTO GetGiteGastById(int id);
    }

}
