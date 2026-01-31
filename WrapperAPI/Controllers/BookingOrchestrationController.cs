using BookingOrchestrationApi.DTOs.Orchestration;
using BookingOrchestrationApi.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingOrchestrationApi.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingOrchestrationController : ControllerBase
{
    private readonly IBookingOrchestrationService _orchestrationService;

    public BookingOrchestrationController(IBookingOrchestrationService orchestrationService)
    {
        _orchestrationService = orchestrationService;
    }

    [HttpPost("orchestrate")]
    public async Task<ActionResult<BookingOrchestrationResponse>> OrchestrateBookings(
        [FromBody] BookingOrchestrationRequest request)
    {
        if (request == null || !request.Bookings.Any())
        {
            return BadRequest(new BookingOrchestrationResponse
            {
                Success = false,
                Message = "No bookings provided",
                Error = new BookingError
                {
                    Type = "validation",
                    Details = "Request must contain at least one booking"
                }
            });
        }

        var response = await _orchestrationService.ProcessBookingsAsync(request);

        if (!response.Success)
        {
            return Ok(response);
        }

        return Ok(response);
    }
}
