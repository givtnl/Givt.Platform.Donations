using Givt.Platform.EF.Entities;
using Givt.Platform.EF.Interfaces;

namespace Givt.Donations.Domain.Entities;

/// <summary>
/// A transaction collecting a sum of donations from the donor
/// </summary>
public class PayIn : EntityBase, IEntity
{
    public DateTime EndDate { get; set; }
    public DateTime ExecutedDate { get; set; }
    public DateTime? PaymentProviderExecutionDate { get; set; }
    public string Currency { get; set; }
    public ICollection<Donation> Donations { get; set; }

    public Guid PayInMethodId { get; set; }    
        
    public int TotalPaid { get; set; }
}