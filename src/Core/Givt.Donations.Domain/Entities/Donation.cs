using Givt.Donations.Domain.Entities.Base;
using Givt.Donations.Domain.Interfaces;

namespace Givt.Donations.Domain.Entities;

public class Donation : EntityAudit, IDonation, IEntity, IAuditBasic, ILoggedEntity
{
    public Guid MediumId { get; set; }    
    public Guid DonorId { get; set; }
    public Guid RecipientId { get; set; }
    public Guid CampaignId { get; set; }
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
