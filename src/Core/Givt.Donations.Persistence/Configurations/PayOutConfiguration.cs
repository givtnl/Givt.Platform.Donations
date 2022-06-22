using Givt.Donations.Domain.Entities;
using Givt.Donations.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Givt.Donations.Persistence.Configurations;

public class PayOutConfiguration : EntityBaseConfiguration<PayOut>
{
    public override void Configure(EntityTypeBuilder<PayOut> builder)
    {
        base.Configure(builder);

        builder
            .Property(e => e.Currency)
            .HasMaxLength(3);

        builder
            .Property(e => e.PaymentProviderId)
            .HasMaxLength(100);
    }
}
