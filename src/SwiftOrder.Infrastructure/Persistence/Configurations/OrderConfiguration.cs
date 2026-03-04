using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiftOrder.Domain.Entities;

namespace SwiftOrder.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OrderNumber)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.HasIndex(x => x.OrderNumber)
            .IsUnique()
            .HasFilter("[OrderNumber] IS NOT NULL");

        builder.Property(x => x.CustomerName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired();

        builder.HasIndex(x => x.Status);

        builder.Property(x => x.TotalAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.SubmittedAt)
            .IsRequired(false);

        builder.Property(x => x.ConfirmedAt)
            .IsRequired(false);

        // Mapping for backing field collection (_items)
        builder.HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);     
    }
}