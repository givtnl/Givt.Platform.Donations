﻿using System.ComponentModel;

namespace Givt.Donations.API.Models.Reports;

public class GetDonationReportRequest
{    
    [Description("Language/Region for texts. Defaults to AcceptLanguage, otherwise 'en'")]
    public string Language { get; set; }

}
