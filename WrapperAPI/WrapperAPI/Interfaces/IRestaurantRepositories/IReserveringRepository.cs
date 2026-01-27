using WrapperAPI.Models.CampingModels;
using WrapperAPI.Models.RestaurantModels;

namespace WrapperAPI.Interfaces.IRestaurantRepositories
{
    public interface IReserveringRepository
    {
        IEnumerable<RecievingReservering> GetAllReserveringen();

        RecievingReservering GetReserveringenById(int id);  
        RecievingReservering CreateReservering(SendingReservering reservering, int gebruikerID);        

        void UpdateReservering(RecievingReservering reservering);

        void DeleteReservering(int id);

    }
}
