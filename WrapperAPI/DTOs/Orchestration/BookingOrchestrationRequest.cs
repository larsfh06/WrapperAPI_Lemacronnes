namespace BookingOrchestrationApi.DTOs.Orchestration;

public class BookingOrchestrationRequest
{
    public List<BookingItem> Bookings { get; set; } = new();
}

public class BookingItem
{
    public string AccommodationType { get; set; } = string.Empty;
    public int GuestId { get; set; }
    public int UnitId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int AdultCount { get; set; }
    public int YoungChildCount { get; set; }
    public int OlderChildCount { get; set; }
    public int Platform { get; set; }
    public string? Remarks { get; set; }
}
