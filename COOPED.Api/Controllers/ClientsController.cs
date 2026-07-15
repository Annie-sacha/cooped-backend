using COOPED.Application.DTOs.Client;
using COOPED.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace COOPED.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]  // exige un token JWT valide
public class ClientsController : ControllerBase
{
    private readonly IClientService _clientService;
    private readonly ISuiviService _suiviService;

    public ClientsController(IClientService clientService, ISuiviService suiviService)
    {
        _clientService = clientService;
        _suiviService = suiviService;
    }

    [Authorize(Roles = "Administrateur")]
    [HttpPost]
    public async Task<IActionResult> Creer([FromBody] CreateClientDto dto)
    {
        var client = await _clientService.CreerAsync(dto);
        return CreatedAtAction(nameof(ObtenirParId), new { id = client.Id }, client);
    }

    [HttpGet]
    public async Task<IActionResult> ObtenirTous()
    {
        return Ok(await _clientService.ObtenirTousAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenirParId(int id)
    {
        var client = await _clientService.ObtenirParIdAsync(id);
        return client is null ? NotFound() : Ok(client);
    }

    [HttpGet("promoteur/{promoteurId}")]
    public async Task<IActionResult> ObtenirParPromoteur(int promoteurId)
    {
        return Ok(await _clientService.ObtenirParPromoteurAsync(promoteurId));
    }

    [HttpGet("{id}/suivi")]
    public async Task<IActionResult> ObtenirSuivi(int id)
    {
        return Ok(await _suiviService.GenererSuiviAsync(id));
    }

    [Authorize(Roles = "Administrateur")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Modifier(int id, [FromBody] UpdateClientDto dto)
    {
        var succes = await _clientService.ModifierAsync(id, dto);
        return succes ? NoContent() : NotFound();
    }

    [Authorize(Roles = "Administrateur")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Supprimer(int id)
    {
        var succes = await _clientService.SupprimerAsync(id);
        return succes ? NoContent() : NotFound();
    }
}