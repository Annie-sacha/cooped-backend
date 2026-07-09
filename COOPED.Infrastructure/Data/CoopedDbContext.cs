using COOPED.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace COOPED.Infrastructure.Data;

public class CoopedDbContext : DbContext
{
    public CoopedDbContext(DbContextOptions<CoopedDbContext> options)
        : base(options) { }

    public DbSet<Administrateur> Administrateurs => Set<Administrateur>();
    public DbSet<Promoteur> Promoteurs => Set<Promoteur>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Tontine> Tontines => Set<Tontine>();
    public DbSet<Cotisation> Cotisations => Set<Cotisation>();
    public DbSet<Retrait> Retraits => Set<Retrait>();
    public DbSet<Pret> Prets => Set<Pret>();
    public DbSet<Frais> FraisList => Set<Frais>();
    public DbSet<Achat> Achats => Set<Achat>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Héritage Transaction : table unique avec colonne discriminante (TPH)
        modelBuilder.Entity<Transaction>()
            .HasDiscriminator<string>("TypeTransaction")
            .HasValue<Cotisation>("Cotisation")
            .HasValue<Retrait>("Retrait")
            .HasValue<Pret>("Pret")
            .HasValue<Frais>("Frais")
            .HasValue<Achat>("Achat");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CoopedDbContext).Assembly);
    }
}