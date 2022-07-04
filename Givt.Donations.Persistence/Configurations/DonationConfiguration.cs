using Givt.Donations.Domain.Entities;
using Givt.Donations.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Givt.Donations.Persistence.Configurations;

public class DonationConfiguration : EntityBaseConfiguration<Donation>
{
    public override void Configure(EntityTypeBuilder<Donation> builder)
    {
        base.Configure(builder);

        builder
            .Property(e => e.Currency)
            .HasMaxLength(3);

        builder
            .Property(e => e.TransactionReference)
            .HasMaxLength(50); // Stripe seems to use 27 characters

        builder
            .Property(e => e.Last4)
            .HasMaxLength(20);
        builder
            .Property(e => e.Fingerprint)
            .HasMaxLength(20);

        builder
            .HasIndex(e => e.TransactionReference);
    }
}
