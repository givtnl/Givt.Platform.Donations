using AutoMapper;
using Givt.Donations.Domain.Entities;

namespace Givt.Donations.Domain.Mappings;

public class DonationHistoryMappingProfile: Profile
{
    public DonationHistoryMappingProfile()
    {
        CreateMap<Donation, DonationHistory>();
    }
}