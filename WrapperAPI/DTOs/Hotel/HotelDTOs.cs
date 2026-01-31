namespace BookingOrchestrationApi.DTOs.Hotel;

public class HotelReservationRequest
{
    public string gastNaam { get; set; } = string.Empty;
    public string gastEmail { get; set; } = string.Empty;
    public string gastTel { get; set; } = string.Empty;
    public string gastStraat { get; set; } = string.Empty;
    public string gastHuisnr { get; set; } = string.Empty;
    public string gastPostcode { get; set; } = string.Empty;
    public string gastPlaats { get; set; } = string.Empty;
    public string gastLand { get; set; } = string.Empty;
    public int eenheidID { get; set; }
    public DateTime startDatum { get; set; }
    public DateTime eindDatum { get; set; }
    public int aantalPersonen { get; set; }
    public int platformID { get; set; }
}

public class HotelReservationResponse
{
    public int reserveringID { get; set; }
    public string? foutMelding { get; set; }
}

public class HotelGuest
{
    public int gastID { get; set; }
    public string naam { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public string? tel { get; set; }
    public string straat { get; set; } = string.Empty;
    public string huisnr { get; set; } = string.Empty;
    public string postcode { get; set; } = string.Empty;
    public string plaats { get; set; } = string.Empty;
    public string land { get; set; } = string.Empty;
}
