using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Givt.Donations.Business.Infrastructure.Options
{
    public class InfrastructureOptions
    {
        public const string SectionName = "Infrastructure";

        public string CoreServiceUrl { get; set; } = "https://core.api.givtapp.net";
    }
}
