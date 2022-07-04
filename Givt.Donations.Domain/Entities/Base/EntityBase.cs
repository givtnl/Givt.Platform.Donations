using Givt.Donations.Domain.Interfaces;

namespace Givt.Donations.Domain.Entities.Base;

public abstract class EntityBase : IEntity
{
    public Guid Id { get; set; }
}