namespace Givt.Donations.API.Models.Donations;

public class CreateDonationIntentRequest
{    
    public string Currency { get; set; }
    public decimal Amount { get; set; }
    public string Medium { get; set; }
    public string PaymentMethod { get; set; }
    public string Language { get; set; }
    public int TimezoneOffset { get; set; } 
}