namespace BookingOrchestrationApi.DTOs.Orchestration;

public class BookingOrchestrationResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public BookingResults? Results { get; set; }
    public BookingError? Error { get; set; }
}

public class BookingResults
{
    public List<int> CampingBookingIds { get; set; } = new();
    public List<int> RestaurantReservationIds { get; set; } = new();
    public List<int> HotelReservationIds { get; set; } = new();
    public List<int> GiteReservationIds { get; set; } = new();
}

public class BookingError
{
    public string Type { get; set; } = string.Empty;
    public int? FailedUnitId { get; set; }
    public string? FailedDate { get; set; }
    public string Details { get; set; } = string.Empty;
}
