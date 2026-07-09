using COOPED.Application.DTOs.Client;
using COOPED.Application.Interfaces;
using COOPED.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace COOPED.Application.Services;

public class SuiviService : ISuiviService
{

    private readonly DbContext _context;
    // Remplace DbContext par CoopedDbContext une fois Infrastructure référencé,
    // ou injecte via interface de repository si tu préfères ne pas dépendre d'EF ici.

    public SuiviService(DbContext context)
    {
        _context = context;
    }

    public async Task<List<LigneSuiviDto>> GenererSuiviAsync(int clientId)
    {
        // 1. Cotisations des tontines NORMALES uniquement (entrées)
        var cotisations = await _context.Set<Cotisation>()
            .Include(c => c.Tontine)
            .Where(c => c.ClientId == clientId
                        && c.Tontine != null
                        && c.Tontine.Type == TypeTontine.Normale)
            .Select(c => new LigneSuiviDto
            {
                Date = c.Date,
                Designation = $"Cotisation tontine n°{c.TontineId} (Position {c.Position})",
                Entree = c.Montant,
                Sortie = 0
            })
            .ToListAsync();

        // 2. Retraits (sorties)
        var retraits = await _context.Set<Retrait>()
            .Where(r => r.ClientId == clientId)
            .Select(r => new LigneSuiviDto
            {
                Date = r.Date,
                Designation = $"Retrait" + (r.Motif != null ? $" ({r.Motif})" : ""),
                Entree = 0,
                Sortie = r.MontantTotal
            })
            .ToListAsync();

        // 3. Achats (sorties) — seul le résultat final, pas les cotisations de la tontine achat
        var achats = await _context.Set<Achat>()
            .Where(a => a.ClientId == clientId)
            .Select(a => new LigneSuiviDto
            {
                Date = a.Date,
                Designation = $"Achat : {a.Article}",
                Entree = 0,
                Sortie = a.Montant
            })
            .ToListAsync();

        // 4. Fusion + tri chronologique
        var lignes = cotisations
            .Concat(retraits)
            .Concat(achats)
            .OrderBy(l => l.Date)
            .ToList();

        // 5. Calcul du solde cumulé
        decimal solde = 0;
        foreach (var ligne in lignes)
        {
            solde += ligne.Entree - ligne.Sortie;
            ligne.Solde = solde;
        }

        return lignes;
    }
}