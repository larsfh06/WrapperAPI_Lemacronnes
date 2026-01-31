using System.Text;
using System.Text.Json;
using BookingOrchestrationApi.DTOs.Hotel;
using BookingOrchestrationApi.DTOs.Orchestration;
using BookingOrchestrationApi.Interfaces.Repositories;
using BookingOrchestrationApi.Models.Orchestration;

namespace BookingOrchestrationApi.Repositories;

public class HotelRepository : IHotelRepository
{
    private readonly HttpClient _httpClient;
    private readonly string _adminKey;
    private readonly string _userKey;
    private readonly JsonSerializerOptions _jsonOptions;

    public HotelRepository(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _adminKey = configuration["ExternalApis:Hotel:AdminApiKey"] ?? string.Empty;
        _userKey = configuration["ExternalApis:Hotel:UserApiKey"] ?? string.Empty;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<ServiceReservationResult> CreateReservationAsync(BookingItem booking)
    {
        var guest = await FetchGuestAsync(booking.GuestId);
        if (guest == null)
        {
            return ServiceReservationResult.CreateFailure($"Hotel guest {booking.GuestId} not found");
        }

        var request = new HotelReservationRequest
        {
            gastNaam = guest.naam,
            gastEmail = guest.email,
            gastTel = guest.tel ?? string.Empty,
            gastStraat = guest.straat,
            gastHuisnr = guest.huisnr,
            gastPostcode = guest.postcode,
            gastPlaats = guest.plaats,
            gastLand = guest.land,
            eenheidID = booking.UnitId,
            startDatum = booking.StartDate,
            eindDatum = booking.EndDate ?? booking.StartDate.AddDays(1),
            aantalPersonen = booking.AdultCount + booking.YoungChildCount + booking.OlderChildCount,
            platformID = booking.Platform
        };

        SetApiKey(_userKey);

        var json = JsonSerializer.Serialize(request, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("api/Reserveringen/boeken", content);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return ServiceReservationResult.CreateFailure($"Hotel API error: {response.StatusCode} - {responseBody}");
        }

        try
        {
            var reservationResponse = JsonSerializer.Deserialize<HotelReservationResponse>(responseBody, _jsonOptions);
            if (reservationResponse != null)
            {
                if (!string.IsNullOrEmpty(reservationResponse.foutMelding))
                {
                    return ServiceReservationResult.CreateFailure(reservationResponse.foutMelding);
                }
                return ServiceReservationResult.CreateSuccess(reservationResponse.reserveringID);
            }
        }
        catch (JsonException)
        {
            return ServiceReservationResult.CreateFailure($"Failed to parse Hotel API response: {responseBody}");
        }

        return ServiceReservationResult.CreateFailure("Unexpected Hotel API response");
    }

    public async Task DeleteReservationAsync(int reservationId)
    {
        SetApiKey(_adminKey);
        var response = await _httpClient.DeleteAsync($"api/Reserveringen/{reservationId}");
        response.EnsureSuccessStatusCode();
    }

    private async Task<HotelGuest?> FetchGuestAsync(int guestId)
    {
        try
        {
            SetApiKey(_adminKey);
            var response = await _httpClient.GetAsync("api/Gasten");
            
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var guests = JsonSerializer.Deserialize<List<HotelGuest>>(json, _jsonOptions);
            
            return guests?.FirstOrDefault(g => g.gastID == guestId);
        }
        catch
        {
            return null;
        }
    }

    private void SetApiKey(string key)
    {
        if (string.IsNullOrEmpty(key)) return;

        _httpClient.DefaultRequestHeaders.Remove("X-Api-Key");
        _httpClient.DefaultRequestHeaders.Add("X-Api-Key", key);
    }
}
