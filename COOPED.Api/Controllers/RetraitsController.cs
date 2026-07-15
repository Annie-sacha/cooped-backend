using COOPED.Application.DTOs.Retrait;
using COOPED.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace COOPED.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]  // exige un token JWT valide
public class RetraitsController : ControllerBase
{
    private readonly IRetraitService _retraitService;

    public RetraitsController(IRetraitService retraitService)
    {
        _retraitService = retraitService;
    }

    [HttpPost]
    public async Task<IActionResult> Creer([FromBody] CreerRetraitRequest request)
    {
        var retrait = await _retraitService.CreerAsync(request);
        return Ok(retrait);
    }

    [HttpGet("client/{clientId}")]
    public async Task<IActionResult> ObtenirParClient(int clientId)
    {
        return Ok(await _retraitService.ObtenirParClientAsync(clientId));
    }
}