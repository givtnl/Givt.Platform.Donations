using Givt.Donations.Domain.Entities;

namespace Givt.Donations.Domain.Interfaces;

public interface IDonation 
{
    public Guid MediumId { get; set; }    
    public Guid DonorId { get; set; }
    public Guid PaymentProviderLinkId { get; set; }
    public string Currency { get; set; }
    public int Amount { get; set; }
    public DateTime DonationDateTime { get; set; }
    public string TransactionReference { get; set; }
    public Guid PayinId { get; set; }    
    public PayIn Payin { get; set; }
    public string Last4 { get; set; }
    public string Fingerprint { get; set; }    
}