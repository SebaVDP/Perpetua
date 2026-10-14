using MassTransit;
using Library.Loans.Contracts;

namespace Library.Catalog;

// Reacts to the BookBorrowed event: mark the copy as on loan.
// Distinct consumer name -> own queue -> true fan-out alongside Notifications and Reservations.
[Perpetua.Consumes("BookBorrowed", "Loans", description: "Marks the copy as on loan")]
public class MarkCopyBorrowedConsumer : IConsumer<BookBorrowed>
{
    private readonly ILogger<MarkCopyBorrowedConsumer> _logger;

    public MarkCopyBorrowedConsumer(ILogger<MarkCopyBorrowedConsumer> logger) => _logger = logger;

    public Task Consume(ConsumeContext<BookBorrowed> context)
    {
        _logger.LogInformation("Marked copy of book {BookId} as on loan (loan {LoanId}).",
            context.Message.BookId, context.Message.LoanId);
        return Task.CompletedTask;
    }
}
