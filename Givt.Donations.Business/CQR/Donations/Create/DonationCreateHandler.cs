using Givt.Donations.Business.Infrastructure.Services;
using Givt.Donations.Domain.Entities;
using Givt.Donations.Persistence.DbContexts;
using Givt.Platform.Payments.Interfaces;
using MediatR;
using Serilog.Sinks.Http.Logger;

namespace Givt.Donations.Business.QRS.Donations.Create;

public class DonationCreateHandler : IRequestHandler<DonationCreateCommand, DonationCreateResponse>
{
    private readonly ILog _log;
    private readonly CoreService _coreService;
    private readonly IPaymentServiceBroker _paymentServiceBroker;
    private readonly DonationsContext _context;
    public DonationCreateHandler(
        ILog log,
        CoreService CoreService,
        IPaymentServiceBroker PaymentServiceBroker,
        DonationsContext Context)
    {
        _log = log;
        _coreService = CoreService;
        _paymentServiceBroker = PaymentServiceBroker;
        _context = Context;
    }

    public async Task<DonationCreateResponse> Handle(DonationCreateCommand request, CancellationToken cancellationToken)
    {
       
        // get info about campaign (country, fees)
        var campaign = await _coreService.GetCampaignAsync(request.MediumId, request.Languages, cancellationToken);
        /*
            var description = new StringBuilder();
            // on payments through Stripe, only the first 22 chars are shown. Make sure most important info comes first.
            description.Append(medium.Organisation.Name);
            var campaignName = medium.GetLocalisedText(nameof(MediumTexts.Title), request.Language)?.Trim();
            if (!String.IsNullOrWhiteSpace(campaignName))
                description.Append(" - ").Append(campaignName);

            request.Description = description.ToString();

         */
        // get a suitable PaymentServiceProvider
        var paymentService = _paymentServiceBroker.GetInboundPaymentService(
            request.PaymentMethod, request.Amount, request.Currency, request.DonorCountry, campaign.RecipientCountry);
        var pspLink = campaign.PaymentServiceProviders
            .Where(psp => psp.PaymentProvider == paymentService.PaymentProvider)
            .FirstOrDefault();
        if (pspLink == null)
            throw new Exception("No suitable payment service provider linked for this campaign");

        var feePercentage = pspLink.FeePercentage ?? campaign.FeePercentage;
        var feeFixedAmount = pspLink.FeeFixedAmount?? campaign.FeeFixedAmount;
        // initiate a payment at the provider
        var result = await paymentService.CreatePaymentInboundAsync(
                request.PaymentMethod,
                request.Currency,
                request.Amount,
                request.Amount * feePercentage + feeFixedAmount,
                request.Description,
                pspLink.Reference);
        // store details in database     
        Guid paymentProviderLinkId = Guid.NewGuid(); // TODO: implement, get from Givt.Platform.Core on request.MediumId, get proper PSP using broker
        var donation = new Donation
        {
            Amount = Convert.ToInt32(request.Amount * 100),
            TransactionReference = result.TransactionReference,
            DonationDateTime = DateTime.UtcNow,
            PaymentProviderLinkId = paymentProviderLinkId,
            Currency = request.Currency
        };
        await _context.Donations.AddAsync(donation, cancellationToken);
        var writeCount = await _context.SaveChangesAsync(cancellationToken);
        _log.Debug("Donation with reference '{0}' recorded, {1} records written",
            new object[] { result.TransactionReference, writeCount });

        // return information about the payment for further client processing
        return new DonationCreateResponse
        {
            TransactionReference = donation.TransactionReference,
            PaymentToken = result.ClientToken
        };
    }
}