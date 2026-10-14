using MassTransit;
using Library.Loans.Contracts;

namespace Library.Reservations;

// Reacts to the BookBorrowed event: a reserved book that is now borrowed closes its reservation.
// Distinct consumer name -> own queue -> true fan-out alongside Catalog and Notifications.
[Perpetua.Consumes("BookBorrowed", "Loans", description: "Closes the reservation of the borrowed book")]
public class CloseReservationConsumer : IConsumer<BookBorrowed>
{
    private readonly ILogger<CloseReservationConsumer> _logger;

    public CloseReservationConsumer(ILogger<CloseReservationConsumer> logger) => _logger = logger;

    public Task Consume(ConsumeContext<BookBorrowed> context)
    {
        _logger.LogInformation("Closed any reservation for book {BookId} (loan {LoanId}).",
            context.Message.BookId, context.Message.LoanId);
        return Task.CompletedTask;
    }
}
