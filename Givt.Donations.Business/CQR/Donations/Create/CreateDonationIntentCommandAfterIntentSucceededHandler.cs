using Givt.Donations.Domain.Entities;
using Givt.Donations.Persistence.DbContexts;
using MediatR.Pipeline;
using Serilog.Sinks.Http.Logger;

namespace Givt.Donations.Business.QRS.Donations.Create;

public record CreateDonationIntentCommandAfterIntentSucceededHandler(DonationsContext DbContext, ILog logger) :
    IRequestPostProcessor<CreateDonationIntentCommand, CreateDonationIntentCommandResponse>
{
    public async Task Process(CreateDonationIntentCommand request, CreateDonationIntentCommandResponse response, CancellationToken cancellationToken)
    {
        Guid campaignId = Guid.NewGuid(); // TODO: implement, get from Givt.Platform.Core on request.MediumId
        var dataDonation = new Donation
        {
            Amount = Convert.ToInt32(request.Amount*100),
            TransactionReference = response.TransactionReference,
            DonationDateTime = DateTime.UtcNow,
            //TimezoneOffset = request.TimezoneOffset,
            CampaignId = campaignId,
            Currency = request.Currency
        };
        await DbContext.Donations.AddAsync(dataDonation, cancellationToken);
        var writeCount = await DbContext.SaveChangesAsync(cancellationToken);
        logger.Debug("Donation with reference '{0}' recorded, {1} records written",
            new object[] { response.TransactionReference, writeCount });
    }
}
