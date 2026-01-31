using BookingOrchestrationApi.DTOs.Orchestration;
using BookingOrchestrationApi.Models.Orchestration;

namespace BookingOrchestrationApi.Interfaces.Repositories;

public interface IGiteRepository
{
    Task<ServiceReservationResult> CreateReservationAsync(BookingItem booking);
    Task DeleteReservationAsync(int reservationId);
}
