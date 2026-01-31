using BookingOrchestrationApi.DTOs.Orchestration;

namespace BookingOrchestrationApi.Interfaces.Services;

public interface IBookingOrchestrationService
{
    Task<BookingOrchestrationResponse> ProcessBookingsAsync(BookingOrchestrationRequest request);
}
