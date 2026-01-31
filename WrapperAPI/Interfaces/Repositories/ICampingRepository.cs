using BookingOrchestrationApi.DTOs.Orchestration;
using BookingOrchestrationApi.Models.Orchestration;

namespace BookingOrchestrationApi.Interfaces.Repositories;

public interface ICampingRepository
{
    Task<ServiceReservationResult> CreateBookingAsync(BookingItem booking);
    Task DeleteBookingAsync(int bookingId);
}
