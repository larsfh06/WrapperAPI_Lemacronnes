namespace BookingOrchestrationApi.DTOs.Camping;

public class CampingBookingRequest
{
    public int GebruikerID { get; set; }
    public int AccommodatieID { get; set; }
    public DateTime checkInDatum { get; set; }
    public DateTime checkOutDatum { get; set; }
    public byte AantalVolwassenen { get; set; }
    public byte AantalJongeKinderen { get; set; }
    public byte AantalOudereKinderen { get; set; }
    public string Opmerking { get; set; } = string.Empty;
    public bool Cancelled { get; set; } = false;
    public DateTime Datum { get; set; } = DateTime.Now;
}

public class CampingBookingResponse
{
    public int BoekingID { get; set; }
    public int GebruikerID { get; set; }
    public int AccommodatieID { get; set; }
    public DateTime checkInDatum { get; set; }
    public DateTime checkOutDatum { get; set; }
    public byte AantalVolwassenen { get; set; }
    public byte AantalJongeKinderen { get; set; }
    public byte AantalOudereKinderen { get; set; }
    public string? Opmerking { get; set; }
    public bool Cancelled { get; set; }
    public DateTime Datum { get; set; }
}

public class CampingUser
{
    public int GebruikerID { get; set; }
    public string Naam { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telefoon { get; set; }
}
