namespace BookingOrchestrationApi.Models.Orchestration;

public class BookingState
{
    public List<int> CampingBookingIds { get; set; } = new();
    public List<int> RestaurantReservationIds { get; set; } = new();
    public List<int> HotelReservationIds { get; set; } = new();
    public List<int> GiteReservationIds { get; set; } = new();

    public void AddBooking(BookingStep step, int externalId)
    {
        switch (step)
        {
            case BookingStep.Camping:
                CampingBookingIds.Add(externalId);
                break;
            case BookingStep.Restaurant:
                RestaurantReservationIds.Add(externalId);
                break;
            case BookingStep.Hotel:
                HotelReservationIds.Add(externalId);
                break;
            case BookingStep.Gite:
                GiteReservationIds.Add(externalId);
                break;
        }
    }

    public bool HasAnyBookings()
    {
        return CampingBookingIds.Any() || 
               RestaurantReservationIds.Any() || 
               HotelReservationIds.Any() || 
               GiteReservationIds.Any();
    }
}
