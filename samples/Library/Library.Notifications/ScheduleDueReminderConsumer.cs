using MassTransit;
using Library.Loans.Contracts;

namespace Library.Notifications;

// Reacts to the BookBorrowed event: schedule a due-date reminder for the member.
// Distinct consumer name -> own queue -> true fan-out alongside Catalog and Reservations.
[Perpetua.Consumes("BookBorrowed", "Loans", description: "Schedules a due-date reminder")]
public class ScheduleDueReminderConsumer : IConsumer<BookBorrowed>
{
    private readonly ILogger<ScheduleDueReminderConsumer> _logger;

    public ScheduleDueReminderConsumer(ILogger<ScheduleDueReminderConsumer> logger) => _logger = logger;

    public Task Consume(ConsumeContext<BookBorrowed> context)
    {
        _logger.LogInformation("Scheduled due-date reminder for member {MemberId} (loan {LoanId}).",
            context.Message.MemberId, context.Message.LoanId);
        return Task.CompletedTask;
    }
}
