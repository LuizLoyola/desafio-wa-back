using DesafioWaBack.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DesafioWaBack.Domain.Mappings;

public class DeliveryTeamMap : IEntityTypeConfiguration<DeliveryTeam>
{
    public void Configure(EntityTypeBuilder<DeliveryTeam> builder)
    {
        builder.HasKey(dt => dt.Id);
        builder.Property(dt => dt.Id).ValueGeneratedOnAdd();
        builder.Property(dt => dt.Name).IsRequired();
        builder.Property(dt => dt.Description).IsRequired();
        builder.Property(dt => dt.LicensePlate).IsRequired();
    }
}