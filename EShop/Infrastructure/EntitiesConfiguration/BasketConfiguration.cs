using Domain;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntitiesConfiguration;

public class BasketConfiguration : IEntityTypeConfiguration<Basket>
{
    public void Configure(EntityTypeBuilder<Basket> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasMany(x => x.Products)
            .WithOne()
            .HasForeignKey(x => x.BasketId)
            .IsRequired();

        builder.Property(x => x.Total);
    }
}