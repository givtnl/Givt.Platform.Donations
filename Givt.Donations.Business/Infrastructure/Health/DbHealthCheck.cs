using Givt.Donations.Persistence.DbContexts;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Givt.Donations.Business.Infrastructure.Health;

public class DbHealthCheck : IHealthCheck
{
    private readonly DonationsContext _context;

    public DbHealthCheck(DonationsContext context)
    {
        _context = context;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // connectible:
            //_context.Database.GetService<IRelationalDatabaseCreator>().Exists();
            // structures exist:
            //_context.Database.GetService<IRelationalDatabaseCreator>().HasTables();
            // >= EF Core 3.1:
            var result = await _context.Database.CanConnectAsync(cancellationToken);
            if (result)
                return HealthCheckResult.Healthy();
            else
                return HealthCheckResult.Unhealthy(); // server can be connected but database is not found or credentials invalid
        }
        catch (Exception ex)
        {
            // server cannot be connected
            return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
        }
    }

}