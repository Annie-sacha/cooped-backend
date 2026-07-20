using ClosedXML.Excel;
using COOPED.Domain.Entities;
using COOPED.Domain.Enums;
using COOPED.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json")
    .Build();

var connectionString = config.GetConnectionString("CoopedDb");
var optionsBuilder = new DbContextOptionsBuilder<CoopedDbContext>();
optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

using var context = new CoopedDbContext(optionsBuilder.Options);

Console.Write("Dossier contenant les fichiers de collectes : ");
string dossier = Console.ReadLine() ?? "";

var fichiers = Directory.GetFiles(dossier, "*.xlsx");
Console.WriteLine($"{fichiers.Length} fichier(s) trouvé(s).\n");

var promoteursExistants = context.Promoteurs.ToList();
if (promoteursExistants.Count == 0)
{
    Console.WriteLine("❌ Aucun promoteur en base. Crée-les d'abord via POST /api/promoteurs.");
    return;
}

int clientsCrees = 0, tontinesCreees = 0, cotisationsCreees = 0, ignorees = 0;
var rapportPromoteurs = new List<string>();
var rapportClients = new List<string>();
var rapportTypesInconnus = new List<string>();
var fichiersIgnores = new List<string>();

foreach (var fichier in fichiers)
{
    string nomFichier = Path.GetFileNameWithoutExtension(fichier);

    try
    {
        var enregistrements = LireEnregistrements(fichier);

        if (enregistrements.Count == 0)
        {
            fichiersIgnores.Add($"{nomFichier} (aucune ligne exploitable)");
            continue;
        }

        var ligneAvecAgent = enregistrements.FirstOrDefault(e => !string.IsNullOrWhiteSpace(e.NomAgent));
        if (ligneAvecAgent.NomAgent is null)
        {
            fichiersIgnores.Add($"{nomFichier} (colonne \"Nom de l'agent\" entièrement vide)");
            continue;
        }

        string nomPromoteurFichier = ligneAvecAgent.NomAgent!.Trim();
        var promoteur = TrouverPlusProche(promoteursExistants, nomPromoteurFichier, p => p.Nom);
        if (promoteur is null)
        {
            fichiersIgnores.Add($"{nomFichier} (aucun promoteur correspondant à \"{nomPromoteurFichier}\")");
            continue;
        }
        if (!promoteur.Nom.Equals(nomPromoteurFichier, StringComparison.OrdinalIgnoreCase))
            rapportPromoteurs.Add($"\"{nomPromoteurFichier}\" (fichier {nomFichier}) → rapproché de \"{promoteur.Nom}\"");

        var clientsExistants = context.Clients.Where(c => c.PromoteurId == promoteur.Id).ToList();
        var groupesClients = enregistrements.GroupBy(e => new { Nom = e.Nom?.Trim(), Prenoms = e.Prenoms?.Trim() });

        foreach (var groupeClient in groupesClients)
        {
            string nom = groupeClient.Key.Nom ?? "";
            string prenoms = groupeClient.Key.Prenoms ?? "";
            if (string.IsNullOrWhiteSpace(nom)) { ignorees += groupeClient.Count(); continue; }

            string nomComplet = $"{nom} {prenoms}".Trim();
            var client = TrouverPlusProche(clientsExistants, nomComplet, c => $"{c.NomCli} {c.PrenomCli}".Trim());

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
                clientsExistants.Add(client);
                clientsCrees++;
            }
            else if (!$"{client.NomCli} {client.PrenomCli}".Equals(nomComplet, StringComparison.OrdinalIgnoreCase))
            {
                rapportClients.Add($"\"{nomComplet}\" → rapproché de \"{client.NomCli} {client.PrenomCli}\" (Id {client.Id})");
            }

            // Regroupement par mise ET par type (une tontine normale et un prêt peuvent coexister avec la même mise)
            var groupesTontines = groupeClient.GroupBy(e => new { e.Mise, Type = MapperType(e.TypeBrut, nomFichier, rapportTypesInconnus) });

            foreach (var groupeTontine in groupesTontines)
            {
                try
                {
                    decimal mise = groupeTontine.Key.Mise ?? 0;
                    if (mise <= 0) { ignorees += groupeTontine.Count(); continue; }

                    var lignes = groupeTontine.OrderBy(e => e.Date).ToList();

                    int cumul = 0;
                    var positions = new List<int>();
                    foreach (var l in lignes)
                    {
                        cumul += l.Nombre ?? 0;
                        positions.Add(cumul);
                    }

                    var premiereLigneAvecDate = lignes.FirstOrDefault(l => l.Date != null);

                    var tontine = new Tontine
                    {
                        ClientId = client.Id,
                        Mise = mise,
                        NbreMise = positions.DefaultIfEmpty(31).Max(),
                        DateCreation = premiereLigneAvecDate.Date ?? DateOnly.FromDateTime(DateTime.Now),
                        Type = groupeTontine.Key.Type
                    };
                    context.Tontines.Add(tontine);
                    context.SaveChanges();
                    tontinesCreees++;

                    for (int i = 0; i < lignes.Count; i++)
                    {
                        var enr = lignes[i];
                        if (enr.Date is null || enr.Nombre is null) { ignorees++; continue; }

                        context.Cotisations.Add(new Cotisation
                        {
                            ClientId = client.Id,
                            TontineId = tontine.Numero,
                            Date = enr.Date.Value,
                            Montant = enr.Montant ?? (mise * enr.Nombre.Value),
                            NbreMise = enr.Nombre.Value,
                            Position = positions[i]
                        });
                        cotisationsCreees++;
                    }
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    ignorees += groupeTontine.Count();
                    Console.WriteLine($"   ⚠️  Tontine ignorée (client {client.NomCli} {client.PrenomCli}, mise {groupeTontine.Key.Mise}) : {ex.Message}");
                }
            }
        }
    }
    catch (Exception ex)
    {
        fichiersIgnores.Add($"{nomFichier} (erreur inattendue : {ex.Message})");
    }
}

Console.WriteLine("\n✅ Migration terminée :");
Console.WriteLine($"   {clientsCrees} client(s) créé(s)");
Console.WriteLine($"   {tontinesCreees} tontine(s) créée(s)");
Console.WriteLine($"   {cotisationsCreees} cotisation(s) créée(s)");
Console.WriteLine($"   {ignorees} ligne(s) ignorée(s) (données incomplètes)");

if (fichiersIgnores.Count > 0)
{
    Console.WriteLine("\n⚠️  Fichiers ignorés :");
    fichiersIgnores.ForEach(f => Console.WriteLine($"   - {f}"));
}
if (rapportPromoteurs.Count > 0)
{
    Console.WriteLine("\n📋 Promoteurs rapprochés par similarité (à vérifier) :");
    rapportPromoteurs.ForEach(r => Console.WriteLine($"   - {r}"));
}
if (rapportClients.Count > 0)
{
    Console.WriteLine("\n📋 Clients rapprochés par similarité (à vérifier) :");
    rapportClients.ForEach(r => Console.WriteLine($"   - {r}"));
}
if (rapportTypesInconnus.Count > 0)
{
    Console.WriteLine("\n⚠️  Types de cotisation non reconnus (traités comme \"Ordinaire\" par défaut) :");
    rapportTypesInconnus.ForEach(r => Console.WriteLine($"   - {r}"));
}

// ── Fonctions utilitaires ──

static TypeTontine MapperType(string? typeBrut, string nomFichier, List<string> rapportTypesInconnus)
{
    string t = (typeBrut ?? "").Trim();

    if (string.IsNullOrWhiteSpace(t) || t.Equals("Ordinaire", StringComparison.OrdinalIgnoreCase))
        return TypeTontine.Normale;

    if (t.Equals("Quinzaine", StringComparison.OrdinalIgnoreCase) ||
        t.Equals("Mensuel", StringComparison.OrdinalIgnoreCase))
        return TypeTontine.Pret;

    rapportTypesInconnus.Add($"\"{t}\" (fichier {nomFichier})");
    return TypeTontine.Normale;
}

static T? TrouverPlusProche<T>(List<T> candidats, string nomRecherche, Func<T, string> selecteurNom) where T : class
{
    static string Normaliser(string s) => s.Trim().ToUpperInvariant();
    string cible = Normaliser(nomRecherche);

    var exact = candidats.FirstOrDefault(c => Normaliser(selecteurNom(c)) == cible);
    if (exact != null) return exact;

    T? meilleur = null;
    int meilleureDistance = int.MaxValue;
    foreach (var c in candidats)
    {
        int distance = Levenshtein(Normaliser(selecteurNom(c)), cible);
        if (distance < meilleureDistance)
        {
            meilleureDistance = distance;
            meilleur = c;
        }
    }

    int seuil = Math.Max(2, cible.Length / 5);
    return meilleureDistance <= seuil ? meilleur : null;
}

static int Levenshtein(string a, string b)
{
    var d = new int[a.Length + 1, b.Length + 1];
    for (int i = 0; i <= a.Length; i++) d[i, 0] = i;
    for (int j = 0; j <= b.Length; j++) d[0, j] = j;

    for (int i = 1; i <= a.Length; i++)
        for (int j = 1; j <= b.Length; j++)
        {
            int cout = a[i - 1] == b[j - 1] ? 0 : 1;
            d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cout);
        }

    return d[a.Length, b.Length];
}

static List<Enregistrement> LireEnregistrements(string chemin)
{
    var liste = new List<Enregistrement>();
    using var workbook = new XLWorkbook(chemin);
    var feuille = workbook.Worksheet(1);

    foreach (var ligne in feuille.RowsUsed().Skip(1))
    {
        bool ligneVide =
            string.IsNullOrWhiteSpace(ligne.Cell(1).GetString()) &&
            string.IsNullOrWhiteSpace(ligne.Cell(4).GetString()) &&
            string.IsNullOrWhiteSpace(ligne.Cell(6).GetString());

        if (ligneVide) continue;

        var e = new Enregistrement();

        e.NomAgent = ligne.Cell(1).GetString();

        try { e.Date = DateOnly.FromDateTime(ligne.Cell(2).GetDateTime()); }
        catch { e.Date = null; }

        e.TypeBrut = ligne.Cell(3).GetString();
        e.Nom = ligne.Cell(4).GetString();
        e.Prenoms = ligne.Cell(5).GetString();
        e.Mise = decimal.TryParse(ligne.Cell(6).GetString(), out decimal mise) ? mise : null;
        e.Nombre = int.TryParse(ligne.Cell(7).GetString(), out int nombre) ? nombre : null;
        e.Montant = decimal.TryParse(ligne.Cell(8).GetString(), out decimal montant) ? montant : null;

        liste.Add(e);
    }
    return liste;
}

struct Enregistrement
{
    public string? NomAgent { get; set; }
    public DateOnly? Date { get; set; }
    public string? TypeBrut { get; set; }
    public string? Nom { get; set; }
    public string? Prenoms { get; set; }
    public decimal? Mise { get; set; }
    public int? Nombre { get; set; }
    public decimal? Montant { get; set; }
}