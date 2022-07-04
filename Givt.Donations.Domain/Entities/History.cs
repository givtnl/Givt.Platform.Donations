using Givt.Donations.Domain.Enums;
using Givt.Donations.Domain.Interfaces;

namespace Givt.Donations.Domain.Entities
{
    public abstract class History : IHistory
    {
        public Guid Id { get; set; }
        public DateTime Modified { get; set; }
        public LogReason Reason { get; set; }
    }
}