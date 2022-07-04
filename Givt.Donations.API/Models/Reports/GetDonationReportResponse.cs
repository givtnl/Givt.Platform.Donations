//using Givt.Donations.Integrations.Interfaces;

namespace Givt.Donations.API.Models.Reports;

public class GetDonationReportResponse //: IFileData
{
    public byte[] Content { get; set; }
    public string Filename { get; set; }
    public string MimeType { get; set; }
}
