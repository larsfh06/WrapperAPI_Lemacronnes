using WrapperAPI.Models.GiteModels;

namespace WrapperAPI.Interfaces.IGiteRepository
{
    public interface IGiteRepository
    {
        IEnumerable<BoekingResponseDTO> GetAllReserveringen();

        BoekingResponseDTO GetReserveringenById(int id);
        BoekingResponseDTO CreateReservering(BoekingRequestDTO reservering);

        void UpdateReservering(BoekingResponseDTO reservering);

        void DeleteReservering(int id);

    }
}
