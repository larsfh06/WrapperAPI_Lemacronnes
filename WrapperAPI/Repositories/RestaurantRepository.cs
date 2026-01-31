using System.Text;
using System.Text.Json;
using BookingOrchestrationApi.DTOs.Orchestration;
using BookingOrchestrationApi.DTOs.Restaurant;
using BookingOrchestrationApi.Interfaces.Repositories;
using BookingOrchestrationApi.Models.Orchestration;

namespace BookingOrchestrationApi.Repositories;

public class RestaurantRepository : IRestaurantRepository
{
    private readonly HttpClient _httpClient;
    private readonly HttpClient _campingHttpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public RestaurantRepository(HttpClient httpClient, IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClient;
        _campingHttpClient = httpClientFactory.CreateClient("CampingApi");
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<ServiceReservationResult> CreateReservationAsync(BookingItem booking)
    {
        int boekingId = await FindMatchingCampingBookingAsync(booking.GuestId, booking.StartDate);

        var request = new RestaurantReservationRequest
        {
            boekingID = boekingId,
            tafelID = booking.UnitId,
            datumTijd = booking.StartDate,
            aantalVolwassenen = booking.AdultCount,
            aantalJongeKinderen = booking.YoungChildCount,
            aantalOudereKinderen = booking.OlderChildCount
        };

        var json = JsonSerializer.Serialize(request, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("api/Reserveringen", content);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return ServiceReservationResult.CreateFailure($"Restaurant API error: {response.StatusCode} - {responseBody}");
        }

        var trimmedResponse = responseBody.Trim();

        if (trimmedResponse.Contains("Deze tafel is niet beschikbaar", StringComparison.OrdinalIgnoreCase))
        {
            return ServiceReservationResult.CreateFailure("Deze tafel is niet beschikbaar in de gekozen periode.");
        }

        if (trimmedResponse.Equals("true", StringComparison.OrdinalIgnoreCase))
        {
            return ServiceReservationResult.CreateSuccess(0);
        }

        if (trimmedResponse.StartsWith("{"))
        {
            try
            {
                var reservationResponse = JsonSerializer.Deserialize<RestaurantReservationResponse>(trimmedResponse, _jsonOptions);
                if (reservationResponse != null)
                {
                    return ServiceReservationResult.CreateSuccess(reservationResponse.reserveringID);
                }
            }
            catch (JsonException)
            {
                // Fall through
            }
        }

        return ServiceReservationResult.CreateFailure($"Unexpected response: {trimmedResponse}");
    }

    public async Task DeleteReservationAsync(int reservationId)
    {
        var response = await _httpClient.DeleteAsync($"api/Reserveringen/{reservationId}");
        response.EnsureSuccessStatusCode();
    }

    private async Task<int> FindMatchingCampingBookingAsync(int userId, DateTime reservationDateTime)
    {
        try
        {
            var url = $"api/Boeking/0/{userId}/0?BetalingID=0&IncludeGebruiker=false&IncludeAccommodatie=false&IncludeBetalingen=false";
            var response = await _campingHttpClient.GetAsync(url);
            
            if (!response.IsSuccessStatusCode) return 0;

            var json = await response.Content.ReadAsStringAsync();
            var bookings = JsonSerializer.Deserialize<List<dynamic>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // Find booking where reservation date falls within check-in and check-out
            // For now, return 0 if not found (restaurant can work without camping booking)
            // or whatever applies here... not sure?
            return 0;
        }
        catch
        {
            return 0;
        }
    }
}
