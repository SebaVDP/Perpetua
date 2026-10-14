using Microsoft.AspNetCore.Mvc;
using Library.Members.Contracts;

namespace Library.Members.Controllers;

[ApiController]
[Route("members")]
public class MembersController : ControllerBase
{
    private readonly ILogger<MembersController> _logger;

    public MembersController(ILogger<MembersController> logger) => _logger = logger;

    [HttpPost("check-rights")]
    public IActionResult CheckRights(BorrowingRightsRequest request)
    {
        // A real service would check the member's loan limit and outstanding fines here.
        _logger.LogInformation("Checked borrowing rights for member {MemberId}.", request.MemberId);
        return Ok(new BorrowingRightsResponse(true, "ok"));
    }
}
