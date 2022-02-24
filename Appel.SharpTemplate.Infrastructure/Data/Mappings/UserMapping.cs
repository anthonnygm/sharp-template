using Appel.SharpTemplate.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Appel.SharpTemplate.Infrastructure.Data.Mappings;

public class UserMapping : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(b => b.Id);

        builder.HasIndex(b => b.Name);
        builder.HasIndex(b => b.Email).IsUnique();
        builder.HasIndex(b => b.CpfCnpj).IsUnique();

        builder
            .Property(b => b.Name)
            .IsRequired();

        builder
            .Property(b => b.Email)
            .IsRequired();

        builder
            .Property(b => b.CpfCnpj)
            .IsRequired();

        builder
            .Property(b => b.CellPhone)
            .IsRequired();

        builder
            .Property(b => b.Address)
            .IsRequired();

        builder
            .Property(b => b.AddressNumber)
            .IsRequired();

        builder
            .Property(b => b.Neighborhood)
            .IsRequired();

        builder
            .Property(b => b.City)
            .IsRequired();

        builder
            .Property(b => b.ZipCode)
            .IsRequired();

        builder
            .Property(b => b.FederativeUnit)
            .IsRequired()
            .HasMaxLength(2);
    }
}
