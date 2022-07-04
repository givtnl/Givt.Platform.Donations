using Givt.Donations.Domain.Entities;
using Givt.Donations.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Givt.Donations.Persistence.Configurations;

public class PayInConfiguration : EntityBaseConfiguration<PayIn>
{
    public override void Configure(EntityTypeBuilder<PayIn> builder)
    {
        base.Configure(builder);

        builder
            .Property(e => e.Currency)
            .HasMaxLength(3);

        builder
            .HasMany(e => e.Donations)
            .WithOne(d => d.Payin)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
