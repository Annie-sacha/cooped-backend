using COOPED.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace COOPED.Infrastructure.Data.Configurations;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{

    // table client
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.NomCli).IsRequired().HasMaxLength(100);
        builder.Property(c => c.PrenomCli).IsRequired().HasMaxLength(100);

        builder.HasOne(c => c.Promoteur) // un client possède un seul promoteur 
            .WithMany(p => p.Clients)     // un promoteur peut avoir plusieurs clients
            .HasForeignKey(c => c.PromoteurId)
            .OnDelete(DeleteBehavior.Restrict);   // on peut pas supprimer un promoteur
    }
}