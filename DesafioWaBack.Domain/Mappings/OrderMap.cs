using DesafioWaBack.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DesafioWaBack.Domain.Mappings;

public class OrderMap : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Number);
        builder.Property(o => o.Number).ValueGeneratedOnAdd();
        builder.Property(o => o.CreationDate).IsRequired();
        builder.Property(o => o.DeliveryDate);
        builder.Property(o => o.Address).IsRequired();
        builder.HasOne(o => o.DeliveryTeam).WithMany();
    }
}