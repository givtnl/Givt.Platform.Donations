using Givt.Donations.Persistence.DbContexts;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Givt.Donations.Business.QRS.Donations.Create;

public record CreateDonationIntentFetchAccountIdPreHandler(DonationsContext DbContext) : IRequestPreProcessor<CreateDonationIntentCommand>
{
    public async Task Process(CreateDonationIntentCommand request, CancellationToken cancellationToken)
    {
        //var medium = await DbContext.Mediums
        //    .Where(x => x.Medium == request.MediumId.ToString())
        //    .Include(x => x.Texts)
        //    .Include(x => x.Organisation)
        //    .FirstOrDefaultAsync(cancellationToken);

        //if (medium == null)
        //    throw new NotFoundException(nameof(MediumData), request.MediumId);

        //if (medium.Organisation.PaymentProviderAccountReference == null)
        //    throw new BadRequestException("Organisation has no account reference");

        //request.Medium = medium;

        //var description = new StringBuilder();
        //// on payments through Stripe, only the first 22 chars are shown. Make sure most important info comes first.
        //description.Append(medium.Organisation.Name);
        //var campaignName = medium.GetLocalisedText(nameof(MediumTexts.Title), request.Language)?.Trim();
        //if (!String.IsNullOrWhiteSpace(campaignName))        
        //    description.Append(" - ").Append(campaignName);

        //request.Description = description.ToString();
    }
}