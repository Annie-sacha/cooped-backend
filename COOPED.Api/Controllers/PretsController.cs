using COOPED.Application.DTOs.Pret;
using COOPED.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace COOPED.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]   // exige un token JWT valide
public class PretsController : ControllerBase
{
    private readonly IPretService _pretService;
    private readonly IPenaliteService _penaliteService;

    public PretsController(IPretService pretService, IPenaliteService penaliteService)
    {
        _pretService = pretService;
        _penaliteService = penaliteService;
    }

    [HttpPost]
    public async Task<IActionResult> Creer([FromBody] CreerPretRequest request)
    {
        try
        {
            var resultat = await _pretService.CreerPretAsync(
                request.ClientId, request.MontantMise, request.Type);
            return Ok(resultat);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}/verifier-penalite")]
    public async Task<IActionResult> VerifierPenalite(int id)
    {
        try
        {
            var resultat = await _penaliteService.VerifierEtAppliquerAsync(id);
            return Ok(resultat);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}