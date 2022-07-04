using Givt.Platform.Payments;
using MediatR;

namespace Givt.Donations.Business.QRS.Donations.Create;

public record CreateDonationIntentCommandHandler(ISinglePaymentService SinglePaymentService) : IRequestHandler<CreateDonationIntentCommand, CreateDonationIntentCommandResponse>
{
    public async Task<CreateDonationIntentCommandResponse> Handle(CreateDonationIntentCommand request, CancellationToken cancellationToken)
    {
        var result = await SinglePaymentService.CreatePaymentIntent(
                request.Currency,
                request.Amount,
                request.Amount * (request.ApplicationFeePercentage / 100M) + request.ApplicationFeeFixedAmount,
                request.Description,
                request.PaymentProviderAccountReference,
                request.PaymentMethod);

        return new CreateDonationIntentCommandResponse
        {
            PaymentIntentSecret = result.ClientToken,
            TransactionReference = result.TransactionReference
        };
    }
}