using WrapperAPI.Models.GiteModels;

namespace WrapperAPI.Interfaces.IHotelRepositories
{
    public interface IHotelRepository
    {
        IEnumerable<BoekingResponseDTO> GetAllReserveringen();

        BoekingResponseDTO GetReserveringenById(int id);
        BoekingResponseDTO CreateReservering(BoekingRequestDTO reservering);

        void UpdateReservering(BoekingResponseDTO reservering);

        void DeleteReservering(int id);

    }
}
