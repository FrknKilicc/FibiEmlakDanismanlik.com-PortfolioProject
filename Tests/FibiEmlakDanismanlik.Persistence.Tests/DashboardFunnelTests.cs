using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Domain.Entities;
using FibiEmlakDanismanlik.Domain.Enums;
using FibiEmlakDanismanlik.Persistence.Repositories.DashboardRepositories;
using Xunit;

namespace FibiEmlakDanismanlik.Persistence.Tests;

[Collection("LocalDb")]
public class DashboardFunnelTests
{
    private readonly LocalDbFixture _fixture;

    public DashboardFunnelTests(LocalDbFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetLeadStatsAsync_CountsFunnelCumulatively()
    {
        var from = new DateTime(2001, 1, 1);
        var toExclusive = new DateTime(2001, 2, 1);
        var createdDate = new DateTime(2001, 1, 15);

        var contactA = new CustomerContact
        {
            CustomerContactName = "Müşteri A",
            CustomerContactPhone = "05551112233",
            CustomerContactMessage = "Talep A mesajı",
            LeadStatus = LeadStatus.Won,
            CreatedDate = createdDate
        };

        var contactB = new CustomerContact
        {
            CustomerContactName = "Müşteri B",
            CustomerContactPhone = "05552223344",
            CustomerContactMessage = "Talep B mesajı",
            LeadStatus = LeadStatus.Lost,
            CreatedDate = createdDate
        };

        var contactC = new CustomerContact
        {
            CustomerContactName = "Müşteri C",
            CustomerContactPhone = "05553334455",
            CustomerContactMessage = "Talep C mesajı",
            LeadStatus = LeadStatus.New,
            CreatedDate = createdDate
        };

        using (var seedContext = _fixture.CreateContext())
        {
            seedContext.customerContacts.AddRange(contactA, contactB, contactC);
            await seedContext.SaveChangesAsync();

            var activities = new List<LeadActivity>
            {
                // A: LeadStatus = Won; geçmiş: tek LeadActivity (Type = StatusChange, OldStatus = New, NewStatus = Won)
                new LeadActivity
                {
                    Source = LeadSource.Contact,
                    LeadId = contactA.CustomerContactId,
                    Type = LeadActivityType.StatusChange,
                    OldStatus = LeadStatus.New,
                    NewStatus = LeadStatus.Won,
                    CreatedAtUtc = new DateTime(2001, 1, 16, 0, 0, 0, DateTimeKind.Utc)
                },
                // B: LeadStatus = Lost; geçmiş: StatusChange NewStatus = Contacted, sonra Appointment, sonra Lost
                new LeadActivity
                {
                    Source = LeadSource.Contact,
                    LeadId = contactB.CustomerContactId,
                    Type = LeadActivityType.StatusChange,
                    OldStatus = LeadStatus.New,
                    NewStatus = LeadStatus.Contacted,
                    CreatedAtUtc = new DateTime(2001, 1, 16, 0, 0, 0, DateTimeKind.Utc)
                },
                new LeadActivity
                {
                    Source = LeadSource.Contact,
                    LeadId = contactB.CustomerContactId,
                    Type = LeadActivityType.StatusChange,
                    OldStatus = LeadStatus.Contacted,
                    NewStatus = LeadStatus.Appointment,
                    CreatedAtUtc = new DateTime(2001, 1, 17, 0, 0, 0, DateTimeKind.Utc)
                },
                new LeadActivity
                {
                    Source = LeadSource.Contact,
                    LeadId = contactB.CustomerContactId,
                    Type = LeadActivityType.StatusChange,
                    OldStatus = LeadStatus.Appointment,
                    NewStatus = LeadStatus.Lost,
                    CreatedAtUtc = new DateTime(2001, 1, 18, 0, 0, 0, DateTimeKind.Utc)
                }
            };

            seedContext.LeadActivities.AddRange(activities);
            await seedContext.SaveChangesAsync();
        }

        using (var queryContext = _fixture.CreateContext())
        {
            var repository = new DashboardRepository(queryContext);
            var stats = await repository.GetLeadStatsAsync(from, toExclusive, seeAll: true, agentId: null);

            Assert.NotNull(stats);
            Assert.Equal(3, stats.CreatedInRange);

            Assert.Equal(3, stats.ByStatusInRange.GetValueOrDefault(nameof(LeadStatus.New)));
            Assert.Equal(2, stats.ByStatusInRange.GetValueOrDefault(nameof(LeadStatus.Contacted)));
            Assert.Equal(2, stats.ByStatusInRange.GetValueOrDefault(nameof(LeadStatus.Appointment)));
            Assert.Equal(1, stats.ByStatusInRange.GetValueOrDefault(nameof(LeadStatus.Offer)));
            Assert.Equal(1, stats.ByStatusInRange.GetValueOrDefault(nameof(LeadStatus.Deposit)));
            Assert.Equal(1, stats.ByStatusInRange.GetValueOrDefault(nameof(LeadStatus.Won)));
            Assert.Equal(1, stats.ByStatusInRange.GetValueOrDefault(nameof(LeadStatus.Lost)));
        }
    }
}
