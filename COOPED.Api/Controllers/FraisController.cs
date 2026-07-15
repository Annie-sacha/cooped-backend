using COOPED.Application.DTOs.Frais;
using COOPED.Application.Interfaces;
using COOPED.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace COOPED.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]  // exige un token JWT valide
public class FraisController : ControllerBase
{
    private readonly IGenericRepository<Frais> _fraisRepository;

    public FraisController(IGenericRepository<Frais> fraisRepository)
    {
        _fraisRepository = fraisRepository;
    }

    [HttpGet("client/{clientId}")]
    public async Task<IActionResult> ObtenirParClient(int clientId)
    {
        var frais = await _fraisRepository.GetAllAsync();
        var resultat = frais.Where(f => f.ClientId == clientId).Select(f => new FraisDto
        {
            Id = f.Id,
            Date = f.Date,
            Montant = f.Montant,
            Type = f.Type,
            ClientId = f.ClientId,
            PretId = f.PretId
        }).ToList();

        return Ok(resultat);
    }
}