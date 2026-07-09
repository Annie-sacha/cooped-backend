using COOPED.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace COOPED.Infrastructure.Data.Configurations;

public class TontineConfiguration : IEntityTypeConfiguration<Tontine>
{
    public void Configure(EntityTypeBuilder<Tontine> builder)
    {
        builder.HasKey(t => t.Numero);

        builder.HasOne(t => t.Client)   //une tontine appartient à un seul client
            .WithMany(c => c.Tontines)    // un client peut avoir plusieurs tontines
            .HasForeignKey(t => t.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.Cotisations)
            .WithOne(c => c.Tontine)         // une tontine possède plusieurs cotisations
            .HasForeignKey(c => c.TontineId)
            .OnDelete(DeleteBehavior.Cascade);   // la suppression d'une tontine entraine celle de ses cotisations

        // Une tontine liée à un prêt ou un achat (relations optionnelles)

        builder.HasOne(t => t.Pret) 
            .WithOne(p => p.Tontine)
            .HasForeignKey<Pret>(p => p.TontineId)
            .OnDelete(DeleteBehavior.SetNull);    // pret existe mais pas la tontine 

        builder.HasOne(t => t.Achat)
            .WithOne(a => a.Tontine)
            .HasForeignKey<Achat>(a => a.TontineId)  // un client est supprimé avec ses tontines
            .OnDelete(DeleteBehavior.SetNull);   //La suppression d'une tontine ne supprime pas l'achat.
    }
}