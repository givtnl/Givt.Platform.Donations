using Givt.Platform.EF.Entities;
using Givt.Platform.Payments.Enums;

namespace Givt.Donations.Domain.Entities;

public class PaymentProviderLink : EntityBase
{
    public Guid OwnerId { get; set; } // Campaign or Recipient
    public PaymentProvider PaymentProvider { get; set; }
    public string PaymentProviderReference { get; set; }
}