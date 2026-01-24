using WrapperAPI.Models.CampingModels;

namespace WrapperAPI.Interfaces.ICampingRepositories
{
    public interface IBoekingRepository
    {
        IEnumerable<Boeking> GetAllBoekingen();

        Boeking GetBoekingById(int id);

        Boeking CreateBoeking(Boeking boeking);

        void UpdateBoeking(Boeking boeking);

        void DeleteBoeking(int id);

        IEnumerable<Boeking> ZoekBoekingen(DateTime? startDatum = null, DateTime? eindDatum = null, string? klantNaam = null);
    }
}