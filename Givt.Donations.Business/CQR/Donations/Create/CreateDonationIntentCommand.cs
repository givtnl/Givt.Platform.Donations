using Givt.Platform.Payments.Enums;
using MediatR;
//using Givt.Donations.Integrations.Interfaces.Models;
//using Givt.Donations.Business.Models;
//using Givt.Donations.Persistance.Entities;

namespace Givt.Donations.Business.QRS.Donations.Create;

public class CreateDonationIntentCommand : IRequest<CreateDonationIntentCommandResponse>
{    
    public string Currency { get; set; }
    public decimal Amount { get; set; }
    public decimal ApplicationFeePercentage { get; set; }
    public decimal ApplicationFeeFixedAmount { get; set; }
    public string Description { get; set; }
    public string MediumId { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string Language { get; set; }
    public int TimezoneOffset { get; set; }
    internal string PaymentProviderAccountReference { get; set; }
    internal Guid CampaignId { get; set; }
}