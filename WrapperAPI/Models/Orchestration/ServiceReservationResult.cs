namespace BookingOrchestrationApi.Models.Orchestration;

public class ServiceReservationResult
{
    public bool Success { get; set; }
    public int ExternalId { get; set; }
    public string? ErrorMessage { get; set; }

    public static ServiceReservationResult CreateSuccess(int externalId)
    {
        return new ServiceReservationResult
        {
            Success = true,
            ExternalId = externalId,
            ErrorMessage = null
        };
    }

    public static ServiceReservationResult CreateFailure(string errorMessage)
    {
        return new ServiceReservationResult
        {
            Success = false,
            ExternalId = -1,
            ErrorMessage = errorMessage
        };
    }
}
