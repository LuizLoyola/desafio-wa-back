using DesafioWaBack.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DesafioWaBack.Domain.Mappings;

public class OrderItemMap : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(op => op.Id);
        builder.Property(op => op.Id).ValueGeneratedOnAdd();
        builder.HasOne(op => op.Order).WithMany(o => o.Products).IsRequired();
        builder.HasOne(op => op.Product).WithMany().IsRequired();
        builder.Property(op => op.Quantity).IsRequired();
    }
}