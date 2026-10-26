using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Interfaces.ListingAlertInterfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using FibiEmlakDanismanlik.Domain.Enums;
using FibiEmlakDanismanlik.Persistence.Repositories.ListingAlertRepositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FibiEmlakDanismanlik.Persistence.Tests;

[Collection("LocalDb")]
public class ListingAlertPendingCountTests
{
    private readonly LocalDbFixture _fixture;

    public ListingAlertPendingCountTests(LocalDbFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task PendingCounts_Match_GetPendingMatchesAsync_Equivalent()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var baseDate = new DateTime(2026, 3, 1, 10, 0, 0);

        int agentId;
        int cityAId, cityBId;
        int distA1Id, distA2Id, distB1Id;
        string cityAName, cityBName;
        string distA1Name, distA2Name, distB1Name;
        int categoryId;

        int sale1Id, sale2Id, sale3Id, salePassiveId;
        int rental1Id, rental2Id, rental3Id;

        int alert1Id, alert2Id, alert3Id, alert4Id, alert5Id, alert6Id;

        using (var seedContext = _fixture.CreateContext())
        {
            // 1. Danışman
            var agent = new Agent
            {
                AgentName = $"Danışman {suffix}",
                Mail = $"agent_{suffix}@test.com",
                AgentPhoneNumber = "5551234567",
                Password = "Password123!",
                AgentTitle = "Danışman",
                AgentDescription = "Test Danışman",
                AgentImgUrl = "https://example.com/agent.jpg"
            };
            seedContext.Agents.Add(agent);
            await seedContext.SaveChangesAsync();
            agentId = agent.AgentId;

            // 2. Şehir ve İlçeler
            cityAName = $"ŞehirA_{suffix}";
            cityBName = $"ŞehirB_{suffix}";
            var cityA = new City { Name = cityAName };
            var cityB = new City { Name = cityBName };
            seedContext.Cities.AddRange(cityA, cityB);
            await seedContext.SaveChangesAsync();
            cityAId = cityA.CityId;
            cityBId = cityB.CityId;

            distA1Name = $"İlçeA1_{suffix}";
            distA2Name = $"İlçeA2_{suffix}";
            distB1Name = $"İlçeB1_{suffix}";
            var distA1 = new District { Name = distA1Name, CityId = cityAId };
            var distA2 = new District { Name = distA2Name, CityId = cityAId };
            var distB1 = new District { Name = distB1Name, CityId = cityBId };
            seedContext.Districts.AddRange(distA1, distA2, distB1);
            await seedContext.SaveChangesAsync();
            distA1Id = distA1.DistrictId;
            distA2Id = distA2.DistrictId;
            distB1Id = distB1.DistrictId;

            // 3. Konut Kategorisi ve İlan Türleri (ListingTypeId 2 ve 8)
            var category = new HousingCategory { HousingCategoryName = $"KonutKat_{suffix}" };
            seedContext.HousingCategories.Add(category);
            await seedContext.SaveChangesAsync();
            categoryId = category.HousingCategoryId;

            await seedContext.Database.ExecuteSqlRawAsync(@"
                SET IDENTITY_INSERT dbo.listingTypes ON;
                IF NOT EXISTS (SELECT 1 FROM dbo.listingTypes WHERE ListingTypeId = 2)
                    INSERT INTO dbo.listingTypes (ListingTypeId, Name, UsageType) VALUES (2, 'Satılık Konut', 1);
                IF NOT EXISTS (SELECT 1 FROM dbo.listingTypes WHERE ListingTypeId = 8)
                    INSERT INTO dbo.listingTypes (ListingTypeId, Name, UsageType) VALUES (8, 'Kiralık Konut', 1);
                SET IDENTITY_INSERT dbo.listingTypes OFF;
            ");

            // 4. İlanlar
            // Satılık Konutlar (ListingTypeId = 2)
            var sale1 = CreateSaleListing(agentId, categoryId, cityAId, cityAName, distA1Id, distA1Name, 3_000_000m, "2+1", baseDate.AddDays(5), PropertyStatuses.Aktif);
            var sale2 = CreateSaleListing(agentId, categoryId, cityAId, cityAName, distA2Id, distA2Name, 5_000_000m, "3+1", baseDate.AddDays(10), PropertyStatuses.Aktif);
            var sale3 = CreateSaleListing(agentId, categoryId, cityBId, cityBName, distB1Id, distB1Name, 2_500_000m, "2+1", baseDate.AddDays(15), PropertyStatuses.Aktif);
            var salePassive = CreateSaleListing(agentId, categoryId, cityAId, cityAName, distA1Id, distA1Name, 3_200_000m, "2+1", baseDate.AddDays(5), PropertyStatuses.Pasif);

            seedContext.forSaleHousingPropertyListings.AddRange(sale1, sale2, sale3, salePassive);

            // Kiralık Konutlar (ListingTypeId = 8)
            var rental1 = CreateRentalListing(agentId, categoryId, cityAId, cityAName, distA1Id, distA1Name, 20_000m, "2+1", baseDate.AddDays(6), PropertyStatuses.Aktif);
            var rental2 = CreateRentalListing(agentId, categoryId, cityAId, cityAName, distA1Id, distA1Name, 30_000m, "3+1", baseDate.AddDays(8), PropertyStatuses.Aktif);
            var rental3 = CreateRentalListing(agentId, categoryId, cityAId, cityAName, distA2Id, distA2Name, 45_000m, "6+1", baseDate.AddDays(14), PropertyStatuses.Aktif);

            seedContext.rentalHousingListings.AddRange(rental1, rental2, rental3);
            await seedContext.SaveChangesAsync();

            sale1Id = sale1.ForSaleHousingListId;
            sale2Id = sale2.ForSaleHousingListId;
            sale3Id = sale3.ForSaleHousingListId;
            salePassiveId = salePassive.ForSaleHousingListId;

            rental1Id = rental1.RentalHousingListId;
            rental2Id = rental2.RentalHousingListId;
            rental3Id = rental3.RentalHousingListId;

            // 5. Haber Bekleyen Talepler (ListingAlerts)
            // Talep 1: Satılık Konut, Şehir A, İlçe A1, 2M-4M, "2+1", Tarih: baseDate -> sale1 eşleşir (salePassive pasif olduğundan eşleşmez)
            var alert1 = CreateAlert(2, cityAId, distA1Id, 2_000_000m, 4_000_000m, "2+1", baseDate, ListingAlertStatus.Active, agentId);

            // Talep 2: Satılık Konut, Şehir A, İlçe yok (tüm il), 2.5M-6M, "2+1,3+1", Tarih: baseDate -> sale1 ve sale2 eşleşir
            var alert2 = CreateAlert(2, cityAId, null, 2_500_000m, 6_000_000m, "2+1,3+1", baseDate, ListingAlertStatus.Active, agentId);

            // Talep 3: Kiralık Konut, Şehir A, İlçe A1, 15k-35k, "2+1,3+1", Tarih: baseDate -> rental1 ve rental2 kriterlere uyar, ancak rental1 için eşleşme kaydı eklenecek
            var alert3 = CreateAlert(8, cityAId, distA1Id, 15_000m, 35_000m, "2+1,3+1", baseDate, ListingAlertStatus.Active, agentId);

            // Talep 4: Satılık Konut, Pasif/Closed -> Bekleyen sayısı 0 olmalı
            var alert4 = CreateAlert(2, cityAId, distA1Id, 2_000_000m, 4_000_000m, "2+1", baseDate, ListingAlertStatus.Closed, agentId);

            // Talep 5: Kiralık Konut, Şehir A, İlçe yok, 40k-60k, "5+1 ve üzeri", Tarih: baseDate -> rental3 (6+1) eşleşir
            var alert5 = CreateAlert(8, cityAId, null, 40_000m, 60_000m, "5+1 ve üzeri", baseDate, ListingAlertStatus.Active, agentId);

            // Talep 6: Satılık Konut, Şehir A, İlçe A1, Tarih: baseDate + 30 gün -> İlanlardan sonra açılmış, eşleşme 0
            var alert6 = CreateAlert(2, cityAId, distA1Id, 2_000_000m, 4_000_000m, "2+1", baseDate.AddDays(30), ListingAlertStatus.Active, agentId);

            seedContext.ListingAlerts.AddRange(alert1, alert2, alert3, alert4, alert5, alert6);
            await seedContext.SaveChangesAsync();

            alert1Id = alert1.ListingAlertId;
            alert2Id = alert2.ListingAlertId;
            alert3Id = alert3.ListingAlertId;
            alert4Id = alert4.ListingAlertId;
            alert5Id = alert5.ListingAlertId;
            alert6Id = alert6.ListingAlertId;

            // 6. Eşleşme kaydı: alert3 için rental1 ilanı daha önce haber verilmiş/işlem görmüş
            var matchRecord = new ListingAlertMatch
            {
                ListingAlertId = alert3Id,
                ListingTypeId = 8,
                ListingId = rental1Id,
                Status = ListingAlertMatchStatus.Notified,
                AgentId = agentId,
                CreatedAtUtc = DateTime.UtcNow
            };
            seedContext.ListingAlertMatches.Add(matchRecord);
            await seedContext.SaveChangesAsync();
        }

        // Test Sorguları ve Doğrulama
        using (var queryContext = _fixture.CreateContext())
        {
            var repository = new ListingAlertRepository(queryContext);
            var scope = new ListingAlertScope { SeeAll = false, AgentId = agentId };

            var alertIds = new[] { alert1Id, alert2Id, alert3Id, alert4Id, alert5Id, alert6Id };

            // A. Her talep için zemin doğrusu (GetPendingMatchesAsync):
            var expectedCounts = new Dictionary<int, int>();
            foreach (var id in alertIds)
            {
                var matches = await repository.GetPendingMatchesAsync(id);
                expectedCounts[id] = matches.Count;
            }

            // Kriter senaryolarının beklenen eşleşme sayıları
            Assert.Equal(1, expectedCounts[alert1Id]); // Yalnızca sale1
            Assert.Equal(2, expectedCounts[alert2Id]); // sale1 ve sale2
            Assert.Equal(1, expectedCounts[alert3Id]); // rental1 elendiği için yalnızca rental2
            Assert.Equal(0, expectedCounts[alert4Id]); // Closed olduğu için 0
            Assert.Equal(1, expectedCounts[alert5Id]); // 5+1 ve üzeri ile 6+1 (rental3)
            Assert.Equal(0, expectedCounts[alert6Id]); // İlanlardan sonraki tarihli, 0

            // B. SearchAsync (OnlyWithPending = false) testi:
            var (totalAll, rowsAll) = await repository.SearchAsync(
                new ListingAlertFilter { OnlyWithPending = false, Page = 1, PageSize = 50 },
                scope);

            Assert.Equal(6, totalAll);
            Assert.Equal(6, rowsAll.Count);

            foreach (var row in rowsAll)
            {
                var expected = expectedCounts[row.ListingAlertId];
                Assert.Equal(expected, row.PendingMatchCount);
            }

            // C. SearchAsync (OnlyWithPending = true) testi:
            var (totalPending, rowsPending) = await repository.SearchAsync(
                new ListingAlertFilter { OnlyWithPending = true, Page = 1, PageSize = 50 },
                scope);

            // Bekleyen eşleşmesi olan aktif talepler: alert1, alert2, alert3, alert5 (4 adet)
            Assert.Equal(4, totalPending);
            Assert.Equal(4, rowsPending.Count);

            foreach (var row in rowsPending)
            {
                Assert.True(row.PendingMatchCount > 0);
                var expected = expectedCounts[row.ListingAlertId];
                Assert.Equal(expected, row.PendingMatchCount);
            }

            var returnedPendingIds = rowsPending.Select(r => r.ListingAlertId).ToHashSet();
            Assert.Contains(alert1Id, returnedPendingIds);
            Assert.Contains(alert2Id, returnedPendingIds);
            Assert.Contains(alert3Id, returnedPendingIds);
            Assert.Contains(alert5Id, returnedPendingIds);
            Assert.DoesNotContain(alert4Id, returnedPendingIds);
            Assert.DoesNotContain(alert6Id, returnedPendingIds);

            // D. GetSummaryAsync testi:
            var summary = await repository.GetSummaryAsync(scope);

            Assert.Equal(5, summary.Active);            // alert1, alert2, alert3, alert5, alert6
            Assert.Equal(1, summary.Closed);            // alert4
            Assert.Equal(0, summary.Unassigned);        // tümü agent'a atandı
            Assert.Equal(4, summary.ActiveWithPending); // alert1, alert2, alert3, alert5

            // E. SearchAsync (SeeAll = true) kapsamındaki satırlar için doğrulama:
            var (totalSeeAll, rowsSeeAll) = await repository.SearchAsync(
                new ListingAlertFilter { OnlyWithPending = false, PageSize = 100 },
                new ListingAlertScope { SeeAll = true });

            foreach (var id in alertIds)
            {
                var row = rowsSeeAll.FirstOrDefault(r => r.ListingAlertId == id);
                if (row != null)
                {
                    Assert.Equal(expectedCounts[id], row.PendingMatchCount);
                }
            }
        }
    }

    private static ForSaleHousingListing CreateSaleListing(
        int agentId,
        int categoryId,
        int cityId,
        string cityName,
        int districtId,
        string districtName,
        decimal price,
        string numberOfRoom,
        DateTime createdDate,
        string propertyStatus)
    {
        return new ForSaleHousingListing
        {
            PropertyName = $"Satılık Konut {Guid.NewGuid():N}",
            PropertyDescription = "Açıklama",
            PropertyStatus = propertyStatus,
            CreatedDate = createdDate,
            Price = price,
            City = cityName,
            District = districtName,
            CityId = cityId,
            DistrictId = districtId,
            Neighborhood = "Merkez Mah.",
            IsElevator = true,
            GrossArea = 120,
            NetArea = 100,
            BuildingAge = "0",
            TotalNumberOfFloor = 5,
            NumberOfFloors = 2,
            NumberOfRoom = numberOfRoom,
            Heating = "Kombi",
            UsageStatus = "Boş",
            TitleDeedStatus = "Kat Mülkiyeti",
            HomeLoan = true,
            HousingCategoryId = categoryId,
            AgentId = agentId,
            ListingTypeId = 2,
            PropImgUrl1 = "https://example.com/img1.jpg"
        };
    }

    private static RentalHousingListing CreateRentalListing(
        int agentId,
        int categoryId,
        int cityId,
        string cityName,
        int districtId,
        string districtName,
        decimal rent,
        string numberOfRoom,
        DateTime createdDate,
        string propertyStatus)
    {
        return new RentalHousingListing
        {
            PropertyName = $"Kiralık Konut {Guid.NewGuid():N}",
            PropertyDescription = "Açıklama",
            PropertyStatus = propertyStatus,
            CreatedDate = createdDate,
            Rent = rent,
            Deposit = rent,
            City = cityName,
            District = districtName,
            CityId = cityId,
            DistrictId = districtId,
            Neighborhood = "Merkez Mah.",
            IsElevator = true,
            GrossArea = 110,
            NetArea = 95,
            BuildingAge = "2",
            TotalNumberOfFloor = 4,
            NumberOfFloors = 1,
            NumberOfRoom = numberOfRoom,
            Heating = "Kombi",
            HousingCategoryId = categoryId,
            AgentId = agentId,
            ListingTypeId = 8,
            PropImgUrl1 = "https://example.com/img2.jpg"
        };
    }

    private static ListingAlert CreateAlert(
        int listingTypeId,
        int cityId,
        int? districtId,
        decimal? minPrice,
        decimal? maxPrice,
        string? roomCounts,
        DateTime createdDate,
        ListingAlertStatus status,
        int? assignedAgentId)
    {
        return new ListingAlert
        {
            FullName = $"Müşteri {Guid.NewGuid():N}",
            PhoneNumber = "555" + Random.Shared.Next(1000000, 9999999),
            Email = "test@musteri.com",
            ListingTypeId = listingTypeId,
            CityId = cityId,
            DistrictId = districtId,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            RoomCounts = roomCounts,
            Status = status,
            AssignedAgentId = assignedAgentId,
            KvkkApprovedAt = DateTime.UtcNow,
            CreatedDate = createdDate
        };
    }
}
