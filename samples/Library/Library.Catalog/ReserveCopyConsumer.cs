using MassTransit;
using Library.Catalog.Contracts;

namespace Library.Catalog;

// Handles the ReserveCopy command (point-to-point). The endpoint name "reserve-copy"
// matches the address Reservations sends to (queue:reserve-copy).
public class ReserveCopyConsumer : IConsumer<ReserveCopy>
{
    private readonly ILogger<ReserveCopyConsumer> _logger;

    public ReserveCopyConsumer(ILogger<ReserveCopyConsumer> logger) => _logger = logger;

    public Task Consume(ConsumeContext<ReserveCopy> context)
    {
        _logger.LogInformation("Held a copy of title {TitleId} for member {MemberId} (reservation {ReservationId}).",
            context.Message.TitleId, context.Message.MemberId, context.Message.ReservationId);
        return Task.CompletedTask;
    }
}
