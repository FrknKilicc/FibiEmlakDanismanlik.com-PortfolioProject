using System;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Domain.Entities;
using FibiEmlakDanismanlik.Domain.Enums;
using FibiEmlakDanismanlik.Persistence.Repositories.ListingDealRepositories;
using Xunit;

namespace FibiEmlakDanismanlik.Persistence.Tests;

[Collection("LocalDb")]
public class ListingDealDateTests
{
    private readonly LocalDbFixture _fixture;

    public ListingDealDateTests(LocalDbFixture fixture)
    {
        _fixture = fixture;
    }

    [Theory]
    [InlineData(22, 30, 25)]
    [InlineData(20, 59, 24)]
    public async Task GetForListingAsync_CalculatesDealDateWithTurkeyTimeZone(int hour, int minute, int expectedDay)
    {
        var listingTypeId = 2;
        var listingId = hour * 100 + minute;

        var deal = new ListingDeal
        {
            ListingTypeId = listingTypeId,
            ListingId = listingId,
            CustomerName = "Test",
            AgreedPrice = 1,
            DealType = DealType.Sale,
            CreatedAtUtc = new DateTime(2026, 10, 24, hour, minute, 0, DateTimeKind.Utc),
            DepositDate = null,
            CompletedDate = null,
            CancelledDate = null
        };

        using (var seedContext = _fixture.CreateContext())
        {
            seedContext.ListingDeals.Add(deal);
            await seedContext.SaveChangesAsync();
        }

        using (var queryContext = _fixture.CreateContext())
        {
            var repository = new ListingDealRepository(queryContext);
            var rows = await repository.GetForListingAsync(listingTypeId, listingId);

            var row = Assert.Single(rows);
            Assert.Equal(new DateTime(2026, 10, expectedDay), row.DealDate);
        }
    }
}
