using WrapperAPI.Models.CampingModels;

namespace WrapperAPI.Interfaces.ICampingRepositories
{
    public interface IGebruikerRepository
    {
        Gebruiker GetGebruikerById(int id);
    }

    
}
