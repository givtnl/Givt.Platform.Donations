using Givt.Donations.Domain.Interfaces;
using Givt.Platform.EF.Entities;
using Givt.Platform.EF.Interfaces;

namespace Givt.Donations.Domain.Entities;

public class Donation : EntityAudit, IDonation, IEntity, IAuditBasic, ILoggedEntity
{
    public Guid MediumId { get; set; }
    public Guid DonorId { get; set; }
    public Guid PaymentProviderLinkId { get; set; }
    public PaymentProviderLink PaymentProviderLink { get; set; }
    public string Currency { get; set; }
    public int Amount { get; set; }
    public DateTime DonationDateTime { get; set; }
    public string TransactionReference { get; set; }
    public Guid PayinId { get; set; }
    public PayIn Payin { get; set; }
    public string Last4 { get; set; }
    public string Fingerprint { get; set; }

    public Type HistoryEntityType => typeof(DonationHistory);
}
