using Givt.Donations.Domain.Enums;

namespace Givt.Donations.Domain.Interfaces;

public interface IHistory
{
    public Guid Id { get; set; }
    public DateTime Modified { get; set; }
    public LogReason Reason { get; set; }
}