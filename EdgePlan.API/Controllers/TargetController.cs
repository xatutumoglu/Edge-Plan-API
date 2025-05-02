using System.Security.Claims;
using EdgePlan.API.Services;
using EdgePlan.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EdgePlan.API.Controllers;

[Tags("Target")]
[ApiController]
[Route("api/[controller]")]
public class TargetController : ControllerBase
{
    private readonly TargetService _targetService;
    
    public TargetController(TargetService targetService)
    {
        _targetService = targetService;
    }
    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateAsync(TargetCreateModel model, CancellationToken cancellationToken = default)
    {
        var sessionId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _targetService.CreateAsync(model, sessionId,cancellationToken);
        
        return Created(string.Empty, result);   
    }
    
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _targetService.GetAllAsync(userId, cancellationToken);
        return Ok(result);  
    }
    
    [Authorize]
    [HttpGet("target/{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _targetService.GetByIdAsync(userId, id, cancellationToken);
        return Ok(result);      
    }
    
    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(TargetUpdateModel model, [FromQuery] Guid id, CancellationToken cancellationToken)
    {
        var sessionId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _targetService.UpdateAsync(model, id, sessionId, cancellationToken);
        
        return Ok(result);
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sessionId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _targetService.DeleteAsync(sessionId, id, cancellationToken);
        return Ok();
    }
}