namespace Givt.Donations.Business.QRS.Donations.Create;

public class CreateDonationIntentCommandResponse
{
    public string PaymentIntentSecret { get; set; }
    public string TransactionReference { get; set; }
}