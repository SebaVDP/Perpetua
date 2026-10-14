using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Library.Reservations.Contracts;
using Library.Catalog.Contracts;

namespace Library.Reservations.Controllers;

[ApiController]
[Route("reservations")]
[Perpetua.DependsOn("Catalog", description: "Sends the ReserveCopy command")]
public class ReservationsController : ControllerBase
{
    private readonly ISendEndpointProvider _sendProvider;
    private readonly ILogger<ReservationsController> _logger;

    public ReservationsController(ISendEndpointProvider sendProvider, ILogger<ReservationsController> logger)
    {
        _sendProvider = sendProvider;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Reserve(ReserveRequest request, CancellationToken ct)
    {
        var reservationId = Guid.NewGuid();

        // Async command: ask Catalog (the one owner) to hold a copy. Point-to-point (Send).
        var endpoint = await _sendProvider.GetSendEndpoint(new Uri("queue:reserve-copy"));
        await endpoint.Send(new ReserveCopy(reservationId, request.MemberId, request.TitleId), ct);
        _logger.LogInformation("Reservation {ReservationId} placed, ReserveCopy command sent.", reservationId);

        return Ok(new ReserveResponse(reservationId, "reserved"));
    }
}
