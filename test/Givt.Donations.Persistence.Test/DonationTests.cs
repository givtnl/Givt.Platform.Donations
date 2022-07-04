using AutoMapper;
using Givt.Donations.Domain.Entities;
using Givt.Donations.Domain.Enums;
using Givt.Donations.Domain.Mappings;
using Givt.Donations.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Givt.Donations.Persistence.Test
{
    public class DonationTests
    {
        private readonly DbContextOptions<DonationsContext> dbContextOptions =
            new DbContextOptionsBuilder<DonationsContext>()
            .UseInMemoryDatabase(databaseName: "givt_donations_test")
            .Options;

        private readonly IMapper mapper =
            new MapperConfiguration(mc =>
            {
                mc.AddProfiles(new List<Profile>
                {
                    new DonationHistoryMappingProfile(),
                });
            })
            .CreateMapper();

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task DonationHistory_Maintained()
        {
            var now = DateTime.UtcNow;
            var donationId = Guid.Empty;

            // save donation
            using (var context = new DonationsContext(dbContextOptions, mapper))
            {
                var donation = new Donation
                {
                    //MediumId =
                    //DonorId =
                    //RecipientId =
                    //CampaignId =
                    Currency = "EUR",
                    Amount = 1000,
                    DonationDateTime = now
                    //TransactionReference =
                    //PayinId =
                    //Last4 =
                    //Fingerprint
                };
                context.Add(donation);
                await context.SaveChangesAsync();
                var timeNeededForSave = DateTime.UtcNow - now;
                Assert.Multiple(() =>
                {
                    Assert.That(donation.Id, Is.Not.EqualTo(Guid.Empty));
                    Assert.That(donation.Created, Is.EqualTo(DateTime.UtcNow).Within(timeNeededForSave));
                    Assert.That(donation.Modified, Is.EqualTo(DateTime.UtcNow).Within(timeNeededForSave));
                });
                donationId = donation.Id;
                var historyList = GetHistoryList(context, donationId);
                Assert.That(historyList, Has.Count.EqualTo(1));
                // test log entry
                var logged = historyList.Last();
                Assert.That(logged.Reason, Is.EqualTo(LogReason.Created));
                CheckPropertiesCopied(donation, logged);
            }

            // update donation
            using (var context = new DonationsContext(dbContextOptions, mapper))
            {
                var donation = context.Donations.Where(d => d.Id == donationId).First();
                donation.TransactionReference = "{642743C9-2AED-4D73-BF2F-11038A1320ED}";
                donation.Last4= "0123";
                donation.Fingerprint = "012345678901234567890123456789";
                await context.SaveChangesAsync();
                var historyList = GetHistoryList(context, donationId);
                Assert.That(historyList, Has.Count.EqualTo(2));
                var logged = historyList.Last();
                Assert.That(logged.Reason, Is.EqualTo(LogReason.Updated));
                CheckPropertiesCopied(donation, logged);
            }

            // delete donation
            using (var context = new DonationsContext(dbContextOptions, mapper))
            {
                var donation = context.Donations.Where(d => d.Id == donationId).First();
                context.Remove(donation);
                await context.SaveChangesAsync();
                var historyList = GetHistoryList(context, donationId);
                Assert.That(historyList, Has.Count.EqualTo(3));
                var logged = historyList.Last();
                Assert.That(logged.Reason, Is.EqualTo(LogReason.Deleted));
            }

            // clean up
            using (var context = new DonationsContext(dbContextOptions, mapper))
            {
                var historyList = GetHistoryList(context, donationId);
                context.RemoveRange(historyList);
                await context.SaveChangesAsync();
                historyList = GetHistoryList(context, donationId);
                Assert.That(historyList, Has.Count.EqualTo(0));
            }
        }

        private static void CheckPropertiesCopied(Donation donation, DonationHistory logged)
        {
            Assert.Multiple(() =>
            {
                Assert.That(donation.Id, Is.EqualTo(logged.Id));
                Assert.That(donation.MediumId, Is.EqualTo(logged.MediumId));
                Assert.That(donation.DonorId, Is.EqualTo(logged.DonorId));
                Assert.That(donation.RecipientId, Is.EqualTo(logged.RecipientId));
                Assert.That(donation.CampaignId, Is.EqualTo(logged.CampaignId));
                Assert.That(donation.Currency, Is.EqualTo(logged.Currency));
                Assert.That(donation.Amount, Is.EqualTo(logged.Amount));
                Assert.That(donation.DonationDateTime, Is.EqualTo(logged.DonationDateTime));
                Assert.That(donation.TransactionReference, Is.EqualTo(logged.TransactionReference));
                Assert.That(donation.PayinId, Is.EqualTo(logged.PayinId));
                Assert.That(donation.Last4, Is.EqualTo(logged.Last4));
                Assert.That(donation.Fingerprint, Is.EqualTo(logged.Fingerprint));
            });
        }

        private static List<DonationHistory> GetHistoryList(DonationsContext context, Guid donationId)
        {
            return context.DonationHistory
                .Where(d => d.Id == donationId)
                .OrderBy(d => d.Modified)
                .ToList();
        }
    }
}