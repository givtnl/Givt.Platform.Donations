using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Givt.Donations.API.Models.Reports;

public class SendDonationReportRequest
{
    [Description("Language/Region for texts. Defaults to AcceptLanguage, otherwise 'en'")]
    public string Language { get; set; }

    [Required()]
    [Description("Email address to send the report to")]
    public string Email { get; set; }

}
