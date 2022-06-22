using Givt.Donations.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Givt.Donations.Persistence.Configurations;

public class DonationHistoryConfiguration : IEntityTypeConfiguration<DonationHistory>
{
    public void Configure(EntityTypeBuilder<DonationHistory> builder)
    {
        builder
            .HasKey(e => new { e.Id, e.Modified });

        builder
            .Property(e => e.Currency)
            .HasMaxLength(3);

        builder
            .Property(e => e.TransactionReference)
            .HasMaxLength(50); // Stripe seems to use 27 characters

        builder.Ignore(e => e.Payin);

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
