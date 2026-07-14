using COOPED.Application.DTOs.Achat;
using COOPED.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace COOPED.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AchatsController : ControllerBase
{
    private readonly IAchatService _achatService;

    public AchatsController(IAchatService achatService)
    {
        _achatService = achatService;
    }

    [HttpPost]
    public async Task<IActionResult> Creer([FromBody] CreerAchatRequest request)
    {
        var achat = await _achatService.CreerAsync(request);
        return Ok(achat);
    }

    [HttpGet("client/{clientId}")]
    public async Task<IActionResult> ObtenirParClient(int clientId)
    {
        return Ok(await _achatService.ObtenirParClientAsync(clientId));
    }
}