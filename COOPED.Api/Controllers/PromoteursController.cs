using COOPED.Application.DTOs.Promoteur;
using COOPED.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace COOPED.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]   // niveau classe : il faut être connecté (admin OU promoteur) pour tout le reste
public class PromoteursController : ControllerBase
{
    private readonly IPromoteurService _promoteurService;
    private readonly IClientService _clientService;

    public PromoteursController(IPromoteurService promoteurService, IClientService clientService)
    {
        _promoteurService = promoteurService;
        _clientService = clientService;
    }

    [Authorize(Roles = "Administrateur")]   // seul l'admin peut créer un promoteur
    [HttpPost]
    public async Task<IActionResult> Creer([FromBody] CreatePromoteurDto dto)
    {
        var promoteur = await _promoteurService.CreerAsync(dto);
        return CreatedAtAction(nameof(ObtenirParId), new { id = promoteur.Id }, promoteur);
    }

    [HttpGet]
    public async Task<IActionResult> ObtenirTous()
    {
        return Ok(await _promoteurService.ObtenirTousAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenirParId(int id)
    {
        var promoteur = await _promoteurService.ObtenirParIdAsync(id);
        return promoteur is null ? NotFound() : Ok(promoteur);
    }

    [HttpGet("{id}/clients")]
    public async Task<IActionResult> ObtenirClients(int id)
    {
        return Ok(await _clientService.ObtenirParPromoteurAsync(id));
    }

    [Authorize(Roles = "Administrateur")]   // ← seul l'admin peut modifier
    [HttpPut("{id}")]
    public async Task<IActionResult> Modifier(int id, [FromBody] UpdatePromoteurDto dto)
    {
        var succes = await _promoteurService.ModifierAsync(id, dto);
        return succes ? NoContent() : NotFound();
    }

    [Authorize(Roles = "Administrateur")]   // ← seul l'admin peut supprimer
    [HttpDelete("{id}")]
    public async Task<IActionResult> Supprimer(int id)
    {
        var succes = await _promoteurService.SupprimerAsync(id);
        return succes ? NoContent() : NotFound();
    }
}