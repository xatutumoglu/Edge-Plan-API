using EdgePlan.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EdgePlan.API.Controllers;

[Tags("Session")]
[ApiController]
[Route("api/[controller]")]
public class SessionController : ControllerBase
{
    private readonly SessionService _sessionService;
    
    public SessionController(SessionService sessionService)
    {
        _sessionService = sessionService;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(UserRegisterRequestModel model, CancellationToken cancellationToken = default)
    {
        await _sessionService.RegisterAsync(model, cancellationToken);

        return Created();
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(UserLoginRequestModel model, CancellationToken cancellationToken = default)
    {
        var result = await _sessionService.LoginAsync(model, cancellationToken);
        
        return Ok(result);
    }
    
    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout([FromServices] SessionService sessionService)
    {
        var token = HttpContext.Request.Headers["Authorization"]
            .ToString().Replace("Bearer ", "");

        if (!string.IsNullOrEmpty(token))
            sessionService.BlacklistToken(token, TimeSpan.FromMinutes(30));

        return NoContent();
    }
}