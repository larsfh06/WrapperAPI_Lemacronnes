namespace BookingOrchestrationApi.DTOs.Restaurant;

public class RestaurantReservationRequest
{
    public int boekingID { get; set; }
    public int tafelID { get; set; }
    public DateTime datumTijd { get; set; }
    public int aantalVolwassenen { get; set; }
    public int aantalJongeKinderen { get; set; }
    public int aantalOudereKinderen { get; set; }
}

public class RestaurantReservationResponse
{
    public int reserveringID { get; set; }
    public DateTime datumTijd { get; set; }
    public int aantalPersonen { get; set; }
    public int tafelID { get; set; }
}
