using BookingOrchestrationApi.DTOs.Orchestration;
using BookingOrchestrationApi.Interfaces.Repositories;
using BookingOrchestrationApi.Interfaces.Services;
using BookingOrchestrationApi.Models.Orchestration;

namespace BookingOrchestrationApi.Services;

public class BookingOrchestrationService : IBookingOrchestrationService
{
    private readonly ICampingRepository _campingRepository;
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly IGiteRepository _giteRepository;

    public BookingOrchestrationService(
        ICampingRepository campingRepository,
        IRestaurantRepository restaurantRepository,
        IHotelRepository hotelRepository,
        IGiteRepository giteRepository)
    {
        _campingRepository = campingRepository;
        _restaurantRepository = restaurantRepository;
        _hotelRepository = hotelRepository;
        _giteRepository = giteRepository;
    }

    public async Task<BookingOrchestrationResponse> ProcessBookingsAsync(BookingOrchestrationRequest request)
    {
        var state = new BookingState();

        try
        {
            foreach (var booking in request.Bookings)
            {
                var result = await ProcessSingleBookingAsync(booking);

                if (!result.Success)
                {
                    await RollbackAllBookingsAsync(state);
                    
                    return new BookingOrchestrationResponse
                    {
                        Success = false,
                        Message = "Booking failed, all reservations rolled back",
                        Error = new BookingError
                        {
                            Type = booking.AccommodationType.ToLower(),
                            FailedUnitId = booking.UnitId,
                            FailedDate = booking.StartDate.ToString("yyyy-MM-dd"),
                            Details = result.ErrorMessage ?? "Unknown error"
                        }
                    };
                }

                var step = MapAccommodationTypeToStep(booking.AccommodationType);
                state.AddBooking(step, result.ExternalId);
            }

            return new BookingOrchestrationResponse
            {
                Success = true,
                Message = "All bookings processed successfully",
                Results = new BookingResults
                {
                    CampingBookingIds = state.CampingBookingIds,
                    RestaurantReservationIds = state.RestaurantReservationIds,
                    HotelReservationIds = state.HotelReservationIds,
                    GiteReservationIds = state.GiteReservationIds
                }
            };
        }
        catch (Exception ex)
        {
            await RollbackAllBookingsAsync(state);
            
            return new BookingOrchestrationResponse
            {
                Success = false,
                Message = "error during booking process",
                Error = new BookingError
                {
                    Type = "system",
                    Details = ex.Message
                }
            };
        }
    }

    private async Task<ServiceReservationResult> ProcessSingleBookingAsync(BookingItem booking)
    {
        var type = booking.AccommodationType.ToLower();

        return type switch
        {
            "camping" => await _campingRepository.CreateBookingAsync(booking),
            "restaurant" => await _restaurantRepository.CreateReservationAsync(booking),
            "hotel" => await _hotelRepository.CreateReservationAsync(booking),
            "gite" => await _giteRepository.CreateReservationAsync(booking),
            _ => ServiceReservationResult.CreateFailure($"Unknown accommodation type: {booking.AccommodationType}")
        };
    }

    private async Task RollbackAllBookingsAsync(BookingState state)
    {
        var rollbackTasks = new List<Task>();

        foreach (var id in state.CampingBookingIds)
        {
            rollbackTasks.Add(SafeDeleteAsync(() => _campingRepository.DeleteBookingAsync(id)));
        }

        foreach (var id in state.RestaurantReservationIds)
        {
            rollbackTasks.Add(SafeDeleteAsync(() => _restaurantRepository.DeleteReservationAsync(id)));
        }

        foreach (var id in state.HotelReservationIds)
        {
            rollbackTasks.Add(SafeDeleteAsync(() => _hotelRepository.DeleteReservationAsync(id)));
        }

        foreach (var id in state.GiteReservationIds)
        {
            rollbackTasks.Add(SafeDeleteAsync(() => _giteRepository.DeleteReservationAsync(id)));
        }

        await Task.WhenAll(rollbackTasks);
    }

    private async Task SafeDeleteAsync(Func<Task> deleteAction)
    {
        try
        {
            await deleteAction();
        }
        catch
        {
            // Log, don't throw cause why should we :)
        }
    }

    private BookingStep MapAccommodationTypeToStep(string accommodationType)
    {
        return accommodationType.ToLower() switch
        {
            "camping" => BookingStep.Camping,
            "restaurant" => BookingStep.Restaurant,
            "hotel" => BookingStep.Hotel,
            "gite" => BookingStep.Gite,
            _ => throw new ArgumentException($"Unknown accommodation type: {accommodationType}")
        };
    }
}
