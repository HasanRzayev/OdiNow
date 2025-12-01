using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OdiNow.Security;
using OdiNow.Services.Interfaces;

namespace OdiNow.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CancellationRightsController : ControllerBase
{
    private readonly ICancellationRightsService _rightsService;
    private readonly IUserContextAccessor _userContextAccessor;

    public CancellationRightsController(ICancellationRightsService rightsService, IUserContextAccessor userContextAccessor)
    {
        _rightsService = rightsService;
        _userContextAccessor = userContextAccessor;
    }

    [HttpGet]
    public async Task<IActionResult> GetRights(CancellationToken cancellationToken)
    {
        var userId = _userContextAccessor.GetUserId();
        var rights = await _rightsService.GetRightsAsync(userId, cancellationToken);
        return Ok(rights);
    }

    [HttpPost("use/{orderId:guid}")]
    public async Task<IActionResult> UseRight(Guid orderId, CancellationToken cancellationToken)
    {
        var userId = _userContextAccessor.GetUserId();
        var used = await _rightsService.UseRightAsync(userId, orderId, cancellationToken);
        if (!used)
        {
            return BadRequest(new { error = "Ləğv haqqı yoxdur" });
        }
        return Ok(new { message = "Ləğv haqqı istifadə olundu" });
    }
}




