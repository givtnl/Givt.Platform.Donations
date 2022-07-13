using Givt.Platform.Common.Enums;
using MediatR;

namespace Givt.Donations.Business.QRS.Donations.Create;

public class DonationCreateCommand : IRequest<DonationCreateResponse>
{    
    public string Currency { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
    public string MediumId { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public IList<string> Languages { get; set; }
    public int TimezoneOffset { get; set; }
    public string DonorCountry { get; set; }
}