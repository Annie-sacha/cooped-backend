using COOPED.Application.DTOs.Tontine;
using COOPED.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace COOPED.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]    // exige un token JWT valide
public class TontinesController : ControllerBase
{
    private readonly ITontineService _tontineService;

    public TontinesController(ITontineService tontineService)
    {
        _tontineService = tontineService;
    }

    [HttpPost]
    public async Task<IActionResult> Creer([FromBody] CreerTontineRequest request)
    {
        var tontine = await _tontineService.CreerTontineNormaleAsync(
            request.ClientId, request.Mise, request.NbreMise);
        return CreatedAtAction(nameof(ObtenirCarnet), new { id = tontine.Numero }, tontine);
    }

    [HttpPost("{id}/cotisations")]
    public async Task<IActionResult> AjouterCotisation(int id, [FromBody] AjouterCotisationRequest request)
    {
        try
        {
            var resultat = await _tontineService.AjouterCotisationAsync(id, request.Montant, request.NbreMise);
            return Ok(resultat);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}/carnet")]
    public async Task<IActionResult> ObtenirCarnet(int id)
    {
        try
        {
            var carnet = await _tontineService.ObtenirCarnetAsync(id);
            return Ok(carnet);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}