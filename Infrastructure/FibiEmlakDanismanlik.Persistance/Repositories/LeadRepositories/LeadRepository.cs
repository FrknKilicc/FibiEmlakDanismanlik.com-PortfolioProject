using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Interfaces.LeadInterfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using FibiEmlakDanismanlik.Domain.Enums;
using FibiEmlakDanismanlik.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FibiEmlakDanismanlik.Persistence.Repositories.LeadRepositories
{
    /// <summary>
    /// Müşteri iletişim ve FİBİ analiz taleplerini birleştiren, yetki kapsamını uygulayan ve hareket geçmişini yöneten repository implementasyonu.
    /// </summary>
    public class LeadRepository : ILeadRepository
    {
        private readonly FibiEmlakDanismanlikContext _context;

        public LeadRepository(FibiEmlakDanismanlikContext context)
        {
            _context = context;
        }

        public async Task<(int Total, List<LeadRow> Rows)> SearchAsync(LeadFilter f, LeadScope s)
        {
            var query = BuildUnifiedQuery(f, s);

            var total = await query.CountAsync();
            if (total == 0)
            {
                return (0, new List<LeadRow>());
            }

            // Sıralama: Önce Yeni durumdakiler, ardından en yeni tarihe göre
            query = query
                .OrderBy(x => x.Status == LeadStatus.New ? 0 : 1)
                .ThenByDescending(x => x.CreatedDate)
                .ThenByDescending(x => x.LeadId);

            var page = f.Page < 1 ? 1 : f.Page;
            var pageSize = f.PageSize < 1 ? AdminListingConstants.DefaultPageSize : f.PageSize;
            if (pageSize > AdminListingConstants.MaxPageSize)
            {
                pageSize = AdminListingConstants.MaxPageSize;
            }

            var paged = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            if (paged.Count == 0)
            {
                return (total, new List<LeadRow>());
            }

            // Sayfadaki danışman adları
            var assignedAgentIds = paged
                .Where(x => x.AssignedAgentId.HasValue)
                .Select(x => x.AssignedAgentId!.Value)
                .Distinct()
                .ToList();

            var agentMap = assignedAgentIds.Count > 0
                ? await _context.Agents.AsNoTracking()
                    .Where(a => assignedAgentIds.Contains(a.AgentId))
                    .Select(a => new { a.AgentId, a.AgentName })
                    .ToDictionaryAsync(a => a.AgentId, a => a.AgentName)
                : new Dictionary<int, string>();

            // Sayfadaki bağlı ilan başlıkları
            var listingRefs = paged
                .Where(x => x.ListingTypeId.HasValue && x.ListingId.HasValue)
                .Select(x => (TypeId: x.ListingTypeId!.Value, Id: x.ListingId!.Value))
                .Distinct()
                .ToList();

            var listingTitleMap = await GetListingTitlesAsync(listingRefs);

            // Sayfadaki taleplerin son hareket tarihleri
            var contactIds = paged.Where(x => x.Source == LeadSource.Contact).Select(x => x.LeadId).ToList();
            var fibiIds = paged.Where(x => x.Source == LeadSource.FibiService).Select(x => x.LeadId).ToList();

            var lastActivityDict = new Dictionary<(LeadSource Source, int LeadId), DateTime>();
            if (contactIds.Count > 0 || fibiIds.Count > 0)
            {
                var activities = await _context.Set<LeadActivity>().AsNoTracking()
                    .Where(a => (a.Source == LeadSource.Contact && contactIds.Contains(a.LeadId)) ||
                                (a.Source == LeadSource.FibiService && fibiIds.Contains(a.LeadId)))
                    .GroupBy(a => new { a.Source, a.LeadId })
                    .Select(g => new { g.Key.Source, g.Key.LeadId, MaxDate = g.Max(x => x.CreatedAtUtc) })
                    .ToListAsync();

                foreach (var act in activities)
                {
                    lastActivityDict[(act.Source, act.LeadId)] = act.MaxDate;
                }
            }

            var rows = paged.Select(e =>
            {
                string? listingTitle = null;
                if (e.ListingTypeId.HasValue && e.ListingId.HasValue)
                {
                    listingTitleMap.TryGetValue((e.ListingTypeId.Value, e.ListingId.Value), out listingTitle);
                }

                string? agentName = null;
                if (e.AssignedAgentId.HasValue)
                {
                    agentMap.TryGetValue(e.AssignedAgentId.Value, out agentName);
                }

                lastActivityDict.TryGetValue((e.Source, e.LeadId), out var lastAct);

                return new LeadRow
                {
                    Source = e.Source,
                    LeadId = e.LeadId,
                    Key = e.Source == LeadSource.Contact ? $"C-{e.LeadId}" : $"F-{e.LeadId}",
                    FullName = e.FullName,
                    Phone = e.Phone,
                    Email = e.Email,
                    MessagePreview = GetMessagePreview(e.Message),
                    Status = e.Status,
                    AssignedAgentId = e.AssignedAgentId,
                    AssignedAgentName = agentName,
                    CreatedDate = e.CreatedDate,
                    AppointmentDateTime = e.AppointmentDateTime,
                    ListingTypeId = e.ListingTypeId,
                    ListingId = e.ListingId,
                    ListingTitle = listingTitle,
                    ServiceModule = FormatServiceModule(e.ServiceModule),
                    NeighborhoodName = e.NeighborhoodName,
                    LastActivityAtUtc = lastAct != default ? lastAct : null
                };
            }).ToList();

            return (total, rows);
        }

        public async Task<LeadSummary> GetSummaryAsync(LeadScope s)
        {
            var query = BuildUnifiedQuery(new LeadFilter(), s);

            var total = await query.CountAsync();
            var unassigned = await query.CountAsync(x => x.AssignedAgentId == null);

            var nowUtc = DateTime.UtcNow;
            var sevenDaysAgo = nowUtc.AddDays(-7);
            var sevenDaysAhead = nowUtc.AddDays(7);

            var newLast7Days = await query.CountAsync(x => x.Status == LeadStatus.New && x.CreatedDate >= sevenDaysAgo);

            var appointmentsUpcoming = await query.CountAsync(x =>
                x.AppointmentDateTime.HasValue &&
                x.AppointmentDateTime.Value >= nowUtc.Date &&
                x.AppointmentDateTime.Value <= sevenDaysAhead);

            var statusGroups = await query
                .GroupBy(x => x.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            var byStatus = Enum.GetValues<LeadStatus>()
                .ToDictionary(st => st.ToString(), _ => 0, StringComparer.OrdinalIgnoreCase);

            foreach (var sg in statusGroups)
            {
                byStatus[sg.Status.ToString()] = sg.Count;
            }

            return new LeadSummary
            {
                Total = total,
                ByStatus = byStatus,
                Unassigned = unassigned,
                NewLast7Days = newLast7Days,
                AppointmentsUpcoming = appointmentsUpcoming
            };
        }

        public async Task<LeadDetail?> GetDetailAsync(LeadSource source, int leadId)
        {
            LeadDetail? detail = null;

            if (source == LeadSource.Contact)
            {
                var c = await _context.Set<CustomerContact>().AsNoTracking()
                    .FirstOrDefaultAsync(x => x.CustomerContactId == leadId);

                if (c == null) return null;

                detail = new LeadDetail
                {
                    Source = LeadSource.Contact,
                    LeadId = c.CustomerContactId,
                    Key = $"C-{c.CustomerContactId}",
                    FullName = c.CustomerContactName ?? string.Empty,
                    Phone = c.CustomerContactPhone ?? string.Empty,
                    Email = c.CustomerContactMail,
                    Message = c.CustomerContactMessage,
                    CustomerNote = c.CustomerContactMessage,
                    MessagePreview = GetMessagePreview(c.CustomerContactMessage),
                    Status = c.LeadStatus,
                    AssignedAgentId = c.AssignedAgentId,
                    CreatedDate = c.CreatedDate,
                    AppointmentDateTime = c.AppointmentDateTime,
                    ListingTypeId = (int?)c.PropertyListingTypeEnum,
                    ListingId = c.PropertyListingId,
                    CalculationSummaryJson = null,
                    ServiceModule = null,
                    NeighborhoodName = null,
                    KvkkApprovedAt = c.KvkkApprovedAt
                };
            }
            else if (source == LeadSource.FibiService)
            {
                var f = await _context.Set<FibiServiceLead>().AsNoTracking()
                    .Include(x => x.Neighborhood)
                    .FirstOrDefaultAsync(x => x.FibiServiceLeadId == leadId);

                if (f == null) return null;

                detail = new LeadDetail
                {
                    Source = LeadSource.FibiService,
                    LeadId = f.FibiServiceLeadId,
                    Key = $"F-{f.FibiServiceLeadId}",
                    FullName = f.FullName ?? string.Empty,
                    Phone = f.PhoneNumber ?? string.Empty,
                    Email = f.Email,
                    Message = f.CustomerNote,
                    CustomerNote = f.CustomerNote,
                    MessagePreview = GetMessagePreview(f.CustomerNote),
                    Status = f.LeadStatus,
                    AssignedAgentId = f.AssignedAgentId,
                    CreatedDate = f.CreatedDate,
                    AppointmentDateTime = null,
                    ListingTypeId = null,
                    ListingId = null,
                    CalculationSummaryJson = f.CalculationSummaryJson,
                    ServiceModule = FormatServiceModule(f.SourceModule),
                    NeighborhoodName = f.Neighborhood?.Name,
                    KvkkApprovedAt = f.KvkkApprovedAt
                };
            }
            else
            {
                return null;
            }

            // Atanmış danışman adı
            if (detail.AssignedAgentId.HasValue)
            {
                detail.AssignedAgentName = await _context.Agents.AsNoTracking()
                    .Where(a => a.AgentId == detail.AssignedAgentId.Value)
                    .Select(a => a.AgentName)
                    .FirstOrDefaultAsync();
            }

            // Bağlı ilan başlığı
            if (detail.ListingTypeId.HasValue && detail.ListingId.HasValue)
            {
                var titles = await GetListingTitlesAsync(new List<(int, int)> { (detail.ListingTypeId.Value, detail.ListingId.Value) });
                if (titles.TryGetValue((detail.ListingTypeId.Value, detail.ListingId.Value), out var title))
                {
                    detail.ListingTitle = title;
                }
            }

            // Hareket geçmişi
            var activities = await _context.Set<LeadActivity>().AsNoTracking()
                .Where(a => a.Source == source && a.LeadId == leadId)
                .OrderByDescending(a => a.CreatedAtUtc)
                .ToListAsync();

            var activityAgentIds = activities
                .Where(a => a.AgentId.HasValue)
                .Select(a => a.AgentId!.Value)
                .Concat(activities.Where(a => a.AssignedAgentId.HasValue).Select(a => a.AssignedAgentId!.Value))
                .Distinct()
                .ToList();

            var agentNameMap = activityAgentIds.Count > 0
                ? await _context.Agents.AsNoTracking()
                    .Where(a => activityAgentIds.Contains(a.AgentId))
                    .Select(a => new { a.AgentId, a.AgentName })
                    .ToDictionaryAsync(a => a.AgentId, a => a.AgentName)
                : new Dictionary<int, string>();

            detail.Activities = activities.Select(a => new LeadActivityItem
            {
                ActivityId = a.LeadActivityId,
                Type = a.Type,
                Text = a.Text,
                OldStatus = a.OldStatus,
                NewStatus = a.NewStatus,
                AgentId = a.AgentId,
                AgentName = a.AgentId.HasValue && agentNameMap.TryGetValue(a.AgentId.Value, out var aName) ? aName : null,
                AssignedAgentName = a.AssignedAgentId.HasValue && agentNameMap.TryGetValue(a.AssignedAgentId.Value, out var asName) ? asName : null,
                CreatedAtUtc = a.CreatedAtUtc
            }).ToList();

            detail.LastActivityAtUtc = detail.Activities.FirstOrDefault()?.CreatedAtUtc;

            return detail;
        }

        public async Task<(int? AssignedAgentId, int? ListingAgentId)?> GetOwnershipAsync(LeadSource source, int leadId)
        {
            if (source == LeadSource.Contact)
            {
                var contact = await _context.Set<CustomerContact>().AsNoTracking()
                    .Where(x => x.CustomerContactId == leadId)
                    .Select(x => new
                    {
                        x.AssignedAgentId,
                        ListingTypeId = (int?)x.PropertyListingTypeEnum,
                        ListingId = x.PropertyListingId
                    })
                    .FirstOrDefaultAsync();

                if (contact == null) return null;

                int? listingAgentId = null;
                if (contact.ListingTypeId.HasValue && contact.ListingId.HasValue)
                {
                    listingAgentId = await GetListingAgentIdAsync(contact.ListingTypeId.Value, contact.ListingId.Value);
                }

                return (contact.AssignedAgentId, listingAgentId);
            }
            else if (source == LeadSource.FibiService)
            {
                var lead = await _context.Set<FibiServiceLead>().AsNoTracking()
                    .Where(x => x.FibiServiceLeadId == leadId)
                    .Select(x => new { x.AssignedAgentId })
                    .FirstOrDefaultAsync();

                if (lead == null) return null;

                return (lead.AssignedAgentId, null);
            }

            return null;
        }

        public async Task<bool> UpdateStatusAsync(LeadSource source, int leadId, LeadStatus status)
        {
            if (source == LeadSource.Contact)
            {
                var contact = await _context.Set<CustomerContact>()
                    .FirstOrDefaultAsync(x => x.CustomerContactId == leadId);
                if (contact == null) return false;

                contact.LeadStatus = status;
                contact.StatusUpdatedAtUtc = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return true;
            }
            else if (source == LeadSource.FibiService)
            {
                var lead = await _context.Set<FibiServiceLead>()
                    .FirstOrDefaultAsync(x => x.FibiServiceLeadId == leadId);
                if (lead == null) return false;

                lead.LeadStatus = status;
                lead.StatusUpdatedAtUtc = DateTime.UtcNow;
                if (status != LeadStatus.New)
                {
                    lead.IsContacted = true;
                }
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> AssignAsync(LeadSource source, int leadId, int? agentId)
        {
            if (source == LeadSource.Contact)
            {
                var contact = await _context.Set<CustomerContact>()
                    .FirstOrDefaultAsync(x => x.CustomerContactId == leadId);
                if (contact == null) return false;

                contact.AssignedAgentId = agentId;
                await _context.SaveChangesAsync();
                return true;
            }
            else if (source == LeadSource.FibiService)
            {
                var lead = await _context.Set<FibiServiceLead>()
                    .FirstOrDefaultAsync(x => x.FibiServiceLeadId == leadId);
                if (lead == null) return false;

                lead.AssignedAgentId = agentId;
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task AddActivityAsync(LeadActivity a)
        {
            _context.Set<LeadActivity>().Add(a);
            await _context.SaveChangesAsync();
        }

        public async Task<List<LeadRow>> GetByListingAsync(int listingTypeId, int listingId)
        {
            var contacts = await _context.Set<CustomerContact>().AsNoTracking()
                .Where(x => (int?)x.PropertyListingTypeEnum == listingTypeId && x.PropertyListingId == listingId)
                .OrderBy(x => x.LeadStatus == LeadStatus.New ? 0 : 1)
                .ThenByDescending(x => x.CreatedDate)
                .ToListAsync();

            if (contacts.Count == 0)
            {
                return new List<LeadRow>();
            }

            var assignedAgentIds = contacts
                .Where(x => x.AssignedAgentId.HasValue)
                .Select(x => x.AssignedAgentId!.Value)
                .Distinct()
                .ToList();

            var agentMap = assignedAgentIds.Count > 0
                ? await _context.Agents.AsNoTracking()
                    .Where(a => assignedAgentIds.Contains(a.AgentId))
                    .Select(a => new { a.AgentId, a.AgentName })
                    .ToDictionaryAsync(a => a.AgentId, a => a.AgentName)
                : new Dictionary<int, string>();

            var listingTitleMap = await GetListingTitlesAsync(new List<(int, int)> { (listingTypeId, listingId) });
            listingTitleMap.TryGetValue((listingTypeId, listingId), out var listingTitle);

            var contactIds = contacts.Select(c => c.CustomerContactId).ToList();
            var lastActivityDict = new Dictionary<int, DateTime>();
            if (contactIds.Count > 0)
            {
                var activities = await _context.Set<LeadActivity>().AsNoTracking()
                    .Where(a => a.Source == LeadSource.Contact && contactIds.Contains(a.LeadId))
                    .GroupBy(a => a.LeadId)
                    .Select(g => new { LeadId = g.Key, MaxDate = g.Max(x => x.CreatedAtUtc) })
                    .ToListAsync();

                foreach (var act in activities)
                {
                    lastActivityDict[act.LeadId] = act.MaxDate;
                }
            }

            return contacts.Select(c =>
            {
                string? agentName = null;
                if (c.AssignedAgentId.HasValue)
                {
                    agentMap.TryGetValue(c.AssignedAgentId.Value, out agentName);
                }

                lastActivityDict.TryGetValue(c.CustomerContactId, out var lastAct);

                return new LeadRow
                {
                    Source = LeadSource.Contact,
                    LeadId = c.CustomerContactId,
                    Key = $"C-{c.CustomerContactId}",
                    FullName = c.CustomerContactName ?? string.Empty,
                    Phone = c.CustomerContactPhone ?? string.Empty,
                    Email = c.CustomerContactMail,
                    MessagePreview = GetMessagePreview(c.CustomerContactMessage),
                    Status = c.LeadStatus,
                    AssignedAgentId = c.AssignedAgentId,
                    AssignedAgentName = agentName,
                    CreatedDate = c.CreatedDate,
                    AppointmentDateTime = c.AppointmentDateTime,
                    ListingTypeId = listingTypeId,
                    ListingId = listingId,
                    ListingTitle = listingTitle,
                    ServiceModule = null,
                    NeighborhoodName = null,
                    LastActivityAtUtc = lastAct != default ? lastAct : null
                };
            }).ToList();
        }

        public async Task<bool> IsActiveAgentAsync(int agentId)
        {
            return await _context.Agents.AsNoTracking().AnyAsync(a => a.AgentId == agentId && a.IsActive);
        }

        #region Yardımcı Metotlar ve Birleşik Sorgu

        private IQueryable<LeadUnifiedEntry> BuildUnifiedQuery(LeadFilter f, LeadScope s)
        {
            IQueryable<LeadUnifiedEntry>? contactProjected = null;
            IQueryable<LeadUnifiedEntry>? fibiProjected = null;

            var includeContact = !f.Source.HasValue || f.Source.Value == LeadSource.Contact;
            var includeFibi = (!f.Source.HasValue || f.Source.Value == LeadSource.FibiService) && !f.ListingTypeId.HasValue && !f.ListingId.HasValue;

            if (includeContact)
            {
                var contactQuery = _context.Set<CustomerContact>().AsNoTracking();

                if (!s.SeeAll)
                {
                    var agentId = s.AgentId ?? -1;
                    contactQuery = contactQuery.Where(x =>
                        x.AssignedAgentId == agentId ||
                        (x.PropertyListingId != null && x.PropertyListingTypeEnum != null && (
                            ((int)x.PropertyListingTypeEnum == 1 && _context.forSaleLandListings.Any(l => l.ForSaleLandListingId == x.PropertyListingId && l.AgentId == agentId)) ||
                            ((int)x.PropertyListingTypeEnum == 2 && _context.forSaleHousingPropertyListings.Any(l => l.ForSaleHousingListId == x.PropertyListingId && l.AgentId == agentId)) ||
                            ((int)x.PropertyListingTypeEnum == 3 && _context.forSaleCommercialPropertyListings.Any(l => l.ForSaleCommercialListingId == x.PropertyListingId && l.AgentId == agentId)) ||
                            ((int)x.PropertyListingTypeEnum == 7 && _context.rentalLandListings.Any(l => l.RentalLandListingId == x.PropertyListingId && l.AgentId == agentId)) ||
                            ((int)x.PropertyListingTypeEnum == 8 && _context.rentalHousingListings.Any(l => l.RentalHousingListId == x.PropertyListingId && l.AgentId == agentId)) ||
                            ((int)x.PropertyListingTypeEnum == 9 && _context.rentalCommercialPropertyListings.Any(l => l.RentalCommercialListId == x.PropertyListingId && l.AgentId == agentId))
                        ))
                    );
                }

                contactProjected = contactQuery.Select(c => new LeadUnifiedEntry
                {
                    Source = LeadSource.Contact,
                    LeadId = c.CustomerContactId,
                    FullName = c.CustomerContactName ?? string.Empty,
                    Phone = c.CustomerContactPhone ?? string.Empty,
                    Email = c.CustomerContactMail,
                    Message = c.CustomerContactMessage,
                    Status = c.LeadStatus,
                    AssignedAgentId = c.AssignedAgentId,
                    CreatedDate = c.CreatedDate,
                    AppointmentDateTime = c.AppointmentDateTime,
                    ListingTypeId = (int?)c.PropertyListingTypeEnum,
                    ListingId = c.PropertyListingId,
                    ServiceModule = null,
                    NeighborhoodId = null,
                    NeighborhoodName = null
                });
            }

            if (includeFibi)
            {
                var fibiQuery = _context.Set<FibiServiceLead>().AsNoTracking();

                if (!s.SeeAll)
                {
                    var agentId = s.AgentId ?? -1;
                    fibiQuery = fibiQuery.Where(x => x.AssignedAgentId == agentId);
                }

                fibiProjected = fibiQuery.Select(f => new LeadUnifiedEntry
                {
                    Source = LeadSource.FibiService,
                    LeadId = f.FibiServiceLeadId,
                    FullName = f.FullName ?? string.Empty,
                    Phone = f.PhoneNumber ?? string.Empty,
                    Email = f.Email,
                    Message = f.CustomerNote,
                    Status = f.LeadStatus,
                    AssignedAgentId = f.AssignedAgentId,
                    CreatedDate = f.CreatedDate,
                    AppointmentDateTime = null,
                    ListingTypeId = null,
                    ListingId = null,
                    ServiceModule = (FibiServiceModule?)f.SourceModule,
                    NeighborhoodId = f.NeighborhoodId,
                    NeighborhoodName = f.Neighborhood != null ? f.Neighborhood.Name : null
                });
            }

            IQueryable<LeadUnifiedEntry> query;
            if (contactProjected != null && fibiProjected != null)
            {
                query = contactProjected.Concat(fibiProjected);
            }
            else if (contactProjected != null)
            {
                query = contactProjected;
            }
            else if (fibiProjected != null)
            {
                query = fibiProjected;
            }
            else
            {
                query = Enumerable.Empty<LeadUnifiedEntry>().AsQueryable();
            }

            // Durum filtresi
            if (f.Status.HasValue)
            {
                query = query.Where(x => x.Status == f.Status.Value);
            }

            // Danışman filtresi
            if (f.AgentId.HasValue)
            {
                query = query.Where(x => x.AssignedAgentId == f.AgentId.Value);
            }

            // Atanmamış filtresi
            if (f.Unassigned.HasValue)
            {
                query = f.Unassigned.Value
                    ? query.Where(x => x.AssignedAgentId == null)
                    : query.Where(x => x.AssignedAgentId != null);
            }

            // İlan türü ve kimliği filtresi
            if (f.ListingTypeId.HasValue)
            {
                query = query.Where(x => x.ListingTypeId == f.ListingTypeId.Value);
            }
            if (f.ListingId.HasValue)
            {
                query = query.Where(x => x.ListingId == f.ListingId.Value);
            }

            // Tarih aralığı filtresi
            if (f.FromUtc.HasValue)
            {
                query = query.Where(x => x.CreatedDate >= f.FromUtc.Value);
            }
            if (f.ToUtc.HasValue)
            {
                query = query.Where(x => x.CreatedDate <= f.ToUtc.Value);
            }

            // Arama filtresi (ad, telefon, e-posta, mesaj)
            if (!string.IsNullOrWhiteSpace(f.Search))
            {
                var term = f.Search.Trim();
                query = query.Where(x =>
                    x.FullName.Contains(term) ||
                    x.Phone.Contains(term) ||
                    (x.Email != null && x.Email.Contains(term)) ||
                    (x.Message != null && x.Message.Contains(term)));
            }

            return query;
        }

        private async Task<int?> GetListingAgentIdAsync(int listingTypeId, int listingId)
        {
            return listingTypeId switch
            {
                1 => await _context.forSaleLandListings.AsNoTracking()
                    .Where(x => x.ForSaleLandListingId == listingId)
                    .Select(x => (int?)x.AgentId)
                    .FirstOrDefaultAsync(),

                2 => await _context.forSaleHousingPropertyListings.AsNoTracking()
                    .Where(x => x.ForSaleHousingListId == listingId)
                    .Select(x => (int?)x.AgentId)
                    .FirstOrDefaultAsync(),

                3 => await _context.forSaleCommercialPropertyListings.AsNoTracking()
                    .Where(x => x.ForSaleCommercialListingId == listingId)
                    .Select(x => (int?)x.AgentId)
                    .FirstOrDefaultAsync(),

                7 => await _context.rentalLandListings.AsNoTracking()
                    .Where(x => x.RentalLandListingId == listingId)
                    .Select(x => (int?)x.AgentId)
                    .FirstOrDefaultAsync(),

                8 => await _context.rentalHousingListings.AsNoTracking()
                    .Where(x => x.RentalHousingListId == listingId)
                    .Select(x => (int?)x.AgentId)
                    .FirstOrDefaultAsync(),

                9 => await _context.rentalCommercialPropertyListings.AsNoTracking()
                    .Where(x => x.RentalCommercialListId == listingId)
                    .Select(x => (int?)x.AgentId)
                    .FirstOrDefaultAsync(),

                _ => null
            };
        }

        private async Task<Dictionary<(int TypeId, int Id), string>> GetListingTitlesAsync(List<(int TypeId, int Id)> refs)
        {
            var dict = new Dictionary<(int TypeId, int Id), string>();
            if (refs.Count == 0) return dict;

            var landSaleIds = refs.Where(r => r.TypeId == 1).Select(r => r.Id).ToList();
            if (landSaleIds.Count > 0)
            {
                var items = await _context.forSaleLandListings.AsNoTracking()
                    .Where(l => landSaleIds.Contains(l.ForSaleLandListingId))
                    .Select(l => new { l.ForSaleLandListingId, l.PropertyName })
                    .ToListAsync();
                foreach (var it in items) dict[(1, it.ForSaleLandListingId)] = it.PropertyName;
            }

            var houseSaleIds = refs.Where(r => r.TypeId == 2).Select(r => r.Id).ToList();
            if (houseSaleIds.Count > 0)
            {
                var items = await _context.forSaleHousingPropertyListings.AsNoTracking()
                    .Where(l => houseSaleIds.Contains(l.ForSaleHousingListId))
                    .Select(l => new { l.ForSaleHousingListId, l.PropertyName })
                    .ToListAsync();
                foreach (var it in items) dict[(2, it.ForSaleHousingListId)] = it.PropertyName;
            }

            var commSaleIds = refs.Where(r => r.TypeId == 3).Select(r => r.Id).ToList();
            if (commSaleIds.Count > 0)
            {
                var items = await _context.forSaleCommercialPropertyListings.AsNoTracking()
                    .Where(l => commSaleIds.Contains(l.ForSaleCommercialListingId))
                    .Select(l => new { l.ForSaleCommercialListingId, l.PropertyName })
                    .ToListAsync();
                foreach (var it in items) dict[(3, it.ForSaleCommercialListingId)] = it.PropertyName;
            }

            var landRentIds = refs.Where(r => r.TypeId == 7).Select(r => r.Id).ToList();
            if (landRentIds.Count > 0)
            {
                var items = await _context.rentalLandListings.AsNoTracking()
                    .Where(l => landRentIds.Contains(l.RentalLandListingId))
                    .Select(l => new { l.RentalLandListingId, l.PropertyName })
                    .ToListAsync();
                foreach (var it in items) dict[(7, it.RentalLandListingId)] = it.PropertyName;
            }

            var houseRentIds = refs.Where(r => r.TypeId == 8).Select(r => r.Id).ToList();
            if (houseRentIds.Count > 0)
            {
                var items = await _context.rentalHousingListings.AsNoTracking()
                    .Where(l => houseRentIds.Contains(l.RentalHousingListId))
                    .Select(l => new { l.RentalHousingListId, l.PropertyName })
                    .ToListAsync();
                foreach (var it in items) dict[(8, it.RentalHousingListId)] = it.PropertyName;
            }

            var commRentIds = refs.Where(r => r.TypeId == 9).Select(r => r.Id).ToList();
            if (commRentIds.Count > 0)
            {
                var items = await _context.rentalCommercialPropertyListings.AsNoTracking()
                    .Where(l => commRentIds.Contains(l.RentalCommercialListId))
                    .Select(l => new { l.RentalCommercialListId, l.PropertyName })
                    .ToListAsync();
                foreach (var it in items) dict[(9, it.RentalCommercialListId)] = it.PropertyName;
            }

            return dict;
        }

        private static string? FormatServiceModule(FibiServiceModule? module) => module switch
        {
            FibiServiceModule.RiskAnalysis => "Risk Analizi",
            FibiServiceModule.NeighborhoodAnalysis => "Mahalle Analizi",
            FibiServiceModule.FinanceSimulation => "Finans",
            FibiServiceModule.ExpertValuation => "Ekspertiz",
            FibiServiceModule.InvestmentFinder => "Yatırım",
            _ => null
        };

        private static string? GetMessagePreview(string? message)
        {
            if (string.IsNullOrWhiteSpace(message)) return null;
            var trimmed = message.Trim();
            return trimmed.Length <= 140 ? trimmed : trimmed.Substring(0, 140);
        }

        private sealed class LeadUnifiedEntry
        {
            public LeadSource Source { get; set; }
            public int LeadId { get; set; }
            public string FullName { get; set; } = string.Empty;
            public string Phone { get; set; } = string.Empty;
            public string? Email { get; set; }
            public string? Message { get; set; }
            public LeadStatus Status { get; set; }
            public int? AssignedAgentId { get; set; }
            public DateTime CreatedDate { get; set; }
            public DateTime? AppointmentDateTime { get; set; }
            public int? ListingTypeId { get; set; }
            public int? ListingId { get; set; }
            public FibiServiceModule? ServiceModule { get; set; }
            public int? NeighborhoodId { get; set; }
            public string? NeighborhoodName { get; set; }
        }

        #endregion
    }
}
