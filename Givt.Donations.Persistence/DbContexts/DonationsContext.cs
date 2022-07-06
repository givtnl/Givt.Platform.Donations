using AutoMapper;
using Givt.Donations.Domain.Entities;
using Givt.Platform.EF.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Givt.Donations.Persistence.DbContexts;

public class DonationsContext : CommonContext
{
    public DonationsContext(DbContextOptions options, IMapper mapper)
        : base(options, mapper)
    {
    }

    public DbSet<Donation> Donations { get; set; }
    public DbSet<DonationHistory> DonationHistory { get; set; }
    public DbSet<PayIn> PayIns { get; set; }
    public DbSet<PayOut> PayOuts { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }

}
