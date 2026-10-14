using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Library.Loans.Contracts;
using Library.Members.Contracts;
using Library.Loans.Clients;

namespace Library.Loans.Controllers;

[ApiController]
[Route("loans")]
[Perpetua.Event("BookBorrowed")]
public class LoansController : ControllerBase
{
    private readonly MembersClient _members;
    private readonly IPublishEndpoint _publish;
    private readonly ILogger<LoansController> _logger;

    public LoansController(MembersClient members, IPublishEndpoint publish, ILogger<LoansController> logger)
    {
        _members = members;
        _publish = publish;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Borrow(BorrowRequest request, CancellationToken ct)
    {
        // Synchronous HTTP call: may this member borrow? (limit / outstanding fines)
        var rights = await _members.CheckRightsAsync(new BorrowingRightsRequest(request.MemberId), ct);
        if (rights is null || !rights.Allowed)
        {
            return Problem($"Member is not allowed to borrow: {rights?.Reason ?? "unknown"}.");
        }

        var loanId = Guid.NewGuid();
        await _publish.Publish(new BookBorrowed(loanId, request.MemberId, request.BookId, DateTime.UtcNow), ct);
        _logger.LogInformation("Loan {LoanId} started, BookBorrowed published.", loanId);

        return Ok(new BorrowResponse(loanId, "borrowed"));
    }
}
