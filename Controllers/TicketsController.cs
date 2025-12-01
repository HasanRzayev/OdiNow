using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OdiNow.Security;
using OdiNow.Services.Interfaces;

namespace OdiNow.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;
    private readonly IUserContextAccessor _userContextAccessor;

    public TicketsController(ITicketService ticketService, IUserContextAccessor userContextAccessor)
    {
        _ticketService = ticketService;
        _userContextAccessor = userContextAccessor;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
    {
        var userId = _userContextAccessor.GetUserId();
        var summary = await _ticketService.GetSummaryAsync(userId, cancellationToken);
        return Ok(summary);
    }
}





