using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OdiNow.Contracts.Requests.Tickets;
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

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableTickets(CancellationToken cancellationToken)
    {
        var userId = _userContextAccessor.GetUserId();
        var drops = await _ticketService.GetActiveDropsAsync(userId, cancellationToken);
        return Ok(drops);
    }

    [HttpPost("{dropId:guid}/claim")]
    public async Task<IActionResult> ClaimTicket(Guid dropId, CancellationToken cancellationToken)
    {
        var userId = _userContextAccessor.GetUserId();
        var result = await _ticketService.ClaimTicketAsync(userId, dropId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetHistory(CancellationToken cancellationToken)
    {
        var userId = _userContextAccessor.GetUserId();
        var history = await _ticketService.GetHistoryAsync(userId, cancellationToken);
        return Ok(history);
    }

    [HttpPost("{claimId:guid}/redeem")]
    public async Task<IActionResult> RedeemTicket(Guid claimId, [FromBody] RedeemTicketRequest request, CancellationToken cancellationToken)
    {
        var userId = _userContextAccessor.GetUserId();
        var success = await _ticketService.RedeemTicketAsync(userId, claimId, request, cancellationToken);
        return success ? NoContent() : BadRequest();
    }
}


