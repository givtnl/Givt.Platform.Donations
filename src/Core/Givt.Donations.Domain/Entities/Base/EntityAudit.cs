using Givt.Donations.Domain.Interfaces;

namespace Givt.Donations.Domain.Entities.Base;

public abstract class EntityAudit : EntityBase
{
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
}