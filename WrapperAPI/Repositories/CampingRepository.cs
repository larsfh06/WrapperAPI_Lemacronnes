using System.Text;
using System.Text.Json;
using BookingOrchestrationApi.DTOs.Camping;
using BookingOrchestrationApi.DTOs.Orchestration;
using BookingOrchestrationApi.Interfaces.Repositories;
using BookingOrchestrationApi.Models.Orchestration;

namespace BookingOrchestrationApi.Repositories;

public class CampingRepository : ICampingRepository
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public CampingRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<ServiceReservationResult> CreateBookingAsync(BookingItem booking)
    {
        var request = new CampingBookingRequest
        {
            GebruikerID = booking.GuestId,
            AccommodatieID = booking.UnitId,
            checkInDatum = booking.StartDate,
            checkOutDatum = booking.EndDate ?? booking.StartDate.AddDays(1),
            AantalVolwassenen = (byte)booking.AdultCount,
            AantalJongeKinderen = (byte)booking.YoungChildCount,
            AantalOudereKinderen = (byte)booking.OlderChildCount,
            Opmerking = booking.Remarks ?? string.Empty,
            Cancelled = false,
            Datum = DateTime.Now
        };

        var json = JsonSerializer.Serialize(request, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("api/Boeking", content);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return ServiceReservationResult.CreateFailure($"Camping API error: {response.StatusCode} - {responseBody}");
        }

        var trimmedResponse = responseBody.Trim();

        if (trimmedResponse.Equals("false", StringComparison.OrdinalIgnoreCase))
        {
            return ServiceReservationResult.CreateFailure("Deze accommodatie is niet beschikbaar in de gekozen periode.");
        }

        if (trimmedResponse.Equals("true", StringComparison.OrdinalIgnoreCase))
        {
            var createdBooking = await FetchCreatedBookingAsync(booking.GuestId, booking.UnitId);
            if (createdBooking != null)
            {
                return ServiceReservationResult.CreateSuccess(createdBooking.BoekingID);
            }
            return ServiceReservationResult.CreateFailure("Booking created but ID could not be retrieved");
        }

        try
        {
            var bookingResponse = JsonSerializer.Deserialize<CampingBookingResponse>(trimmedResponse, _jsonOptions);
            if (bookingResponse != null)
            {
                return ServiceReservationResult.CreateSuccess(bookingResponse.BoekingID);
            }
        }
        catch (JsonException)
        {
            try
            {
                var list = JsonSerializer.Deserialize<List<CampingBookingResponse>>(trimmedResponse, _jsonOptions);
                var firstBooking = list?.FirstOrDefault();
                if (firstBooking != null)
                {
                    return ServiceReservationResult.CreateSuccess(firstBooking.BoekingID);
                }
            }
            catch
            {
                // Fall through to failure
            }
        }

        return ServiceReservationResult.CreateFailure($"Unexpected response: {trimmedResponse}");
    }

    public async Task DeleteBookingAsync(int bookingId)
    {
        var response = await _httpClient.DeleteAsync($"api/Boeking/{bookingId}");
        response.EnsureSuccessStatusCode();
    }

    private async Task<CampingBookingResponse?> FetchCreatedBookingAsync(int userId, int accommodationId)
    {
        try
        {
            var url = $"api/Boeking/0/{userId}/0?BetalingID=0&IncludeGebruiker=false&IncludeAccommodatie=false&IncludeBetalingen=false";
            var response = await _httpClient.GetAsync(url);
            
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var bookings = JsonSerializer.Deserialize<List<CampingBookingResponse>>(json, _jsonOptions);
            
            return bookings?
                .Where(b => b.GebruikerID == userId && b.AccommodatieID == accommodationId)
                .OrderByDescending(b => b.BoekingID)
                .FirstOrDefault();
        }
        catch
        {
            return null;
        }
    }
}
