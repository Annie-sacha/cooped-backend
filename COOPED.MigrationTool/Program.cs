using ClosedXML.Excel;
using COOPED.Domain.Entities;
using COOPED.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using COOPED.Domain.Enums;

var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json")
    .Build();

var connectionString = config.GetConnectionString("CoopedDb");
var optionsBuilder = new DbContextOptionsBuilder<CoopedDbContext>();
optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

using var context = new CoopedDbContext(optionsBuilder.Options);

Console.Write("Chemin du fichier Excel : ");
string chemin = Console.ReadLine() ?? "";

Console.Write("Nom du promoteur (doit déjà exister en base, créé via l'API) : ");
string nomPromoteur = Console.ReadLine() ?? "";

var promoteur = context.Promoteurs.FirstOrDefault(p => p.Nom.ToUpper() == nomPromoteur.ToUpper());
if (promoteur is null)
{
    Console.WriteLine("Promoteur introuvable. Crée-le d'abord via POST /api/promoteurs.");
    return;
}

Console.Write("\nImporter aussi un fichier de décaissements (retraits/prêts) ? (o/n) : ");
if ((Console.ReadLine() ?? "").Trim().ToLower() == "o")
{
    Console.Write("Chemin du fichier de décaissements : ");
    string cheminDecaissements = Console.ReadLine() ?? "";
    ImporterDecaissements(context, cheminDecaissements);
}

var enregistrements = LireEnregistrements(chemin);
Console.WriteLine($"{enregistrements.Count} lignes lues depuis Excel.\n");

int clientsCrees = 0, tontinesCreees = 0, cotisationsCreees = 0, ignorees = 0;

var groupesClients = enregistrements.GroupBy(e => new { Nom = e.Nom?.Trim().ToUpper(), Prenoms = e.Prenoms?.Trim().ToUpper() });

foreach (var groupeClient in groupesClients)
{
    string nom = groupeClient.Key.Nom ?? "";
    string prenoms = groupeClient.Key.Prenoms ?? "";
    if (string.IsNullOrWhiteSpace(nom)) { ignorees += groupeClient.Count(); continue; }

    var client = context.Clients.FirstOrDefault(c =>
        c.NomCli.ToUpper() == nom && c.PrenomCli.ToUpper() == prenoms);

    if (client is null)
    {
        client = new Client
        {
            NomCli = nom,
            PrenomCli = prenoms,
            PromoteurId = promoteur.Id,
            DateCarnet = DateOnly.FromDateTime(DateTime.Now)
        };
        context.Clients.Add(client);
        context.SaveChanges();
        clientsCrees++;
    }

    // Regrouper par mise = une tontine distincte, comme dans ton carnet original
    var groupesTontines = groupeClient.GroupBy(e => e.Mise);

    foreach (var groupeTontine in groupesTontines)
    {
        decimal mise = groupeTontine.Key ?? 0;
        if (mise <= 0) { ignorees += groupeTontine.Count(); continue; }

        var lignes = groupeTontine.OrderBy(e => e.Date).ToList();

        int nbreMiseTotal = lignes
            .Where(e => e.Position != null)
            .Select(e => e.Position!.Value)
            .DefaultIfEmpty(31)
            .Max();

        var tontine = new Tontine
        {
            ClientId = client.Id,
            Mise = mise,
            NbreMise = nbreMiseTotal,
            DateCreation = lignes.First(l => l.Date != null).Date ?? DateOnly.FromDateTime(DateTime.Now),
            Type = TypeTontine.Normale
        };
        context.Tontines.Add(tontine);
        context.SaveChanges();
        tontinesCreees++;

        foreach (var enr in lignes)
        {
            if (enr.Date is null || enr.Position is null || enr.Nombre is null) { ignorees++; continue; }

            context.Cotisations.Add(new Cotisation
            {
                ClientId = client.Id,
                TontineId = tontine.Numero,
                Date = enr.Date.Value,
                Montant = mise * enr.Nombre.Value,
                NbreMise = enr.Nombre.Value,
                Position = enr.Position.Value
            });
            cotisationsCreees++;
        }
        context.SaveChanges();
    }
}

Console.WriteLine("\nMigration terminée :");
Console.WriteLine($"   {clientsCrees} client(s) créé(s)");
Console.WriteLine($"   {tontinesCreees} tontine(s) créée(s)");
Console.WriteLine($"   {cotisationsCreees} cotisation(s) créée(s)");
Console.WriteLine($"   {ignorees} ligne(s) ignorée(s) (données incomplètes)");

// ── Lecture Excel (reprise de ton code original) ──

static List<Enregistrement> LireEnregistrements(string chemin)
{
    var liste = new List<Enregistrement>();
    using var workbook = new XLWorkbook(chemin);
    var feuille = workbook.Worksheet(1);

    foreach (var ligne in feuille.RowsUsed().Skip(1))
    {
        bool ligneVide =
            string.IsNullOrWhiteSpace(ligne.Cell(2).GetString()) &&
            string.IsNullOrWhiteSpace(ligne.Cell(5).GetString()) &&
            string.IsNullOrWhiteSpace(ligne.Cell(6).GetString()) &&
            string.IsNullOrWhiteSpace(ligne.Cell(7).GetString()) &&
            string.IsNullOrWhiteSpace(ligne.Cell(8).GetString()) &&
            string.IsNullOrWhiteSpace(ligne.Cell(10).GetString());

        if (ligneVide) continue;

        var e = new Enregistrement();

        try { e.Date = DateOnly.FromDateTime(ligne.Cell(2).GetDateTime()); }
        catch { e.Date = null; }

        e.Nom = ligne.Cell(5).GetString();
        e.Prenoms = ligne.Cell(6).GetString();

        e.Mise = decimal.TryParse(ligne.Cell(7).GetString(), out decimal mise) ? mise : null;
        e.Nombre = int.TryParse(ligne.Cell(8).GetString(), out int nombre) ? nombre : null;
        e.Position = int.TryParse(ligne.Cell(10).GetString(), out int position) ? position : null;

        liste.Add(e);
    }

    return liste;
}



static void ImporterDecaissements(CoopedDbContext context, string chemin)
{
    using var workbook = new XLWorkbook(chemin);
    var feuille = workbook.Worksheet(1);

    int retraitsCrees = 0, pretsCrees = 0, ignores = 0;

    foreach (var ligne in feuille.RowsUsed().Skip(1))
    {
        string nomComplet = ligne.Cell(3).GetString().Trim();   // "Nom et Prénoms"
        string objet = ligne.Cell(5).GetString().Trim();        // "Objets"

        if (string.IsNullOrWhiteSpace(nomComplet) || string.IsNullOrWhiteSpace(objet)
            || objet.Equals("Néant", StringComparison.OrdinalIgnoreCase))
        {
            ignores++;
            continue;
        }

        decimal montant = decimal.TryParse(ligne.Cell(4).GetString(), out decimal m) ? m : 0;
        if (montant <= 0) { ignores++; continue; }

        DateOnly? dateDemande = null;
        try { dateDemande = DateOnly.FromDateTime(ligne.Cell(6).GetDateTime()); } catch { }

        DateOnly dateExecution;
        try { dateExecution = DateOnly.FromDateTime(ligne.Cell(7).GetDateTime()); }
        catch { dateExecution = DateOnly.FromDateTime(DateTime.Now); }

        string observation = ligne.Cell(8).GetString().Trim();

        // "Nom et Prénoms" est une seule cellule ici (contrairement au 1er fichier) : on sépare
        var parts = nomComplet.Split(' ', 2);
        string nom = parts[0].Trim();
        string prenoms = parts.Length > 1 ? parts[1].Trim() : "";

        var client = context.Clients.FirstOrDefault(c =>
            c.NomCli.ToUpper() == nom.ToUpper() && c.PrenomCli.ToUpper() == prenoms.ToUpper());

        if (client is null)
        {
            Console.WriteLine($"Client introuvable : {nomComplet} — ligne ignorée.");
            ignores++;
            continue;
        }

        if (objet.Equals("Retrait", StringComparison.OrdinalIgnoreCase))
        {
            context.Retraits.Add(new Retrait
            {
                ClientId = client.Id,
                Date = dateExecution,
                Montant = montant,
                MontantTotal = montant,
                Motif = string.IsNullOrWhiteSpace(observation) ? null : observation,
                DateDemande = dateDemande,
                StatutValidation = StatutValidation.Valide
            });
            retraitsCrees++;
        }
        else if (objet.Equals("Mensuel", StringComparison.OrdinalIgnoreCase) ||
                 objet.Equals("Quinzaine", StringComparison.OrdinalIgnoreCase))
        {
            var type = objet.Equals("Mensuel", StringComparison.OrdinalIgnoreCase)
                ? TypePret.Mensuel : TypePret.Quinzaine;
            int duree = type == TypePret.Mensuel ? 30 : 15;

            context.Prets.Add(new Pret
            {
                ClientId = client.Id,
                Date = dateExecution,
                Montant = montant,                 // déjà le montant total prêté (mise x30 ou x60)
                Type = type,
                DureeRemboursement = duree,
                DateEcheance = dateExecution.AddDays(duree),
                Statut = StatutPret.EnCours,        // à corriger manuellement en SQL si tu sais lesquels sont soldés
                DateDemande = dateDemande,
                StatutValidation = StatutValidation.Valide,
                TontineId = null                    // historique simple, pas de tontine recréée
            });
            pretsCrees++;
        }
        else
        {
            ignores++;
        }
    }

    context.SaveChanges();

    Console.WriteLine("\nImport décaissements terminé :");
    Console.WriteLine($"   {retraitsCrees} retrait(s) créé(s)");
    Console.WriteLine($"   {pretsCrees} prêt(s) créé(s)");
    Console.WriteLine($"   {ignores} ligne(s) ignorée(s) (Néant, client introuvable, ou données invalides)");
}

struct Enregistrement
{
    public DateOnly? Date { get; set; }
    public string? Nom { get; set; }
    public string? Prenoms { get; set; }
    public decimal? Mise { get; set; }
    public int? Nombre { get; set; }
    public int? Position { get; set; }
}