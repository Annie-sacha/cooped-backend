using COOPED.Application.DTOs.Tontine;
using COOPED.Application.Interfaces;
using COOPED.Domain.Entities;

namespace COOPED.Infrastructure.Services;

public class TontineService : ITontineService
{
    private readonly ITontineRepository _tontineRepository;
    private readonly IGenericRepository<Cotisation> _cotisationRepository;

    public TontineService(
        ITontineRepository tontineRepository,
        IGenericRepository<Cotisation> cotisationRepository)
    {
        _tontineRepository = tontineRepository;
        _cotisationRepository = cotisationRepository;
    }

    public async Task<TontineDto> CreerTontineNormaleAsync(int clientId, decimal mise, int nbreMise = 31)
    {
        var tontine = new Tontine
        {
            ClientId = clientId,
            Mise = mise,
            NbreMise = nbreMise,
            DateCreation = DateOnly.FromDateTime(DateTime.Now),
            Type = TypeTontine.Normale
        };

        await _tontineRepository.AddAsync(tontine);
        await _tontineRepository.SaveChangesAsync();

        return new TontineDto
        {
            Numero = tontine.Numero,
            Mise = tontine.Mise,
            NbreMise = tontine.NbreMise,
            DateCreation = tontine.DateCreation
        };
    }

    public async Task<CotisationResultDto> AjouterCotisationAsync(int tontineId, decimal montant, int nbreMise = 1)
    {
        var tontine = await _tontineRepository.GetWithCotisationsAsync(tontineId);
        if (tontine is null)
            throw new InvalidOperationException("Tontine introuvable.");

        if (tontine.DateFin is not null)
            throw new InvalidOperationException("Cette tontine est déjà clôturée.");

        // Cases déjà remplies = somme des NbreMise des cotisations existantes
        int casesRemplies = tontine.Cotisations.Sum(c => c.NbreMise);
        int nouvellePosition = casesRemplies + nbreMise;

        if (nouvellePosition > tontine.NbreMise)
            throw new InvalidOperationException(
                $"Cette cotisation dépasse le nombre de cases restantes ({tontine.NbreMise - casesRemplies}).");

        var cotisation = new Cotisation
        {
            ClientId = tontine.ClientId,
            TontineId = tontine.Numero,
            Date = DateOnly.FromDateTime(DateTime.Now),
            Montant = montant,
            NbreMise = nbreMise,
            Position = nouvellePosition
        };

        await _cotisationRepository.AddAsync(cotisation);

        bool cloturee = nouvellePosition == tontine.NbreMise;
        if (cloturee)
        {
            tontine.DateFin = DateOnly.FromDateTime(DateTime.Now);
            _tontineRepository.Update(tontine);
        }

        await _cotisationRepository.SaveChangesAsync();

        return new CotisationResultDto
        {
            CotisationId = cotisation.Id,
            Position = nouvellePosition,
            TontineCloturee = cloturee,
            CasesRestantes = tontine.NbreMise - nouvellePosition
        };
    }

    public async Task<CarnetDto> ObtenirCarnetAsync(int tontineId)
    {
        var tontine = await _tontineRepository.GetWithCotisationsAsync(tontineId);
        if (tontine is null)
            throw new InvalidOperationException("Tontine introuvable.");

        // Initialiser toutes les cases comme vides
        var cases = Enumerable.Range(1, tontine.NbreMise)
            .Select(i => new CaseCarnetDto { Position = i, Remplie = false })
            .ToList();

        // Remplir les cases couvertes par chaque cotisation (accès direct par indice, comme dans le carnet console)
        foreach (var cotisation in tontine.Cotisations.OrderBy(c => c.Position))
        {
            int positionDebut = cotisation.Position - cotisation.NbreMise + 1;

            for (int pos = positionDebut; pos <= cotisation.Position; pos++)
            {
                if (pos >= 1 && pos <= tontine.NbreMise)
                {
                    var caseCible = cases[pos - 1];   // position 1 → index 0
                    caseCible.Date = cotisation.Date;
                    caseCible.Remplie = true;
                }
            }
        }

        return new CarnetDto
        {
            TontineId = tontine.Numero,
            Mise = tontine.Mise,
            NbreMise = tontine.NbreMise,
            Cloturee = tontine.DateFin is not null,
            Cases = cases
        };
    }


}