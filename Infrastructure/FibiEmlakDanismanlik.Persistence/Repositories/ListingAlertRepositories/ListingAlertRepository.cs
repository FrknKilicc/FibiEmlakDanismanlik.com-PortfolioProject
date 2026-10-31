using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Interfaces.ListingAlertInterfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using FibiEmlakDanismanlik.Domain.Enums;
using FibiEmlakDanismanlik.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FibiEmlakDanismanlik.Persistence.Repositories.ListingAlertRepositories
{
    /// <summary>
    /// Benzer ilan bildirim talepleri (haber bekleyenler) veri erişim, sorgulama ve eşleşme kurallarını yöneten repository.
    /// </summary>
    public class ListingAlertRepository : IListingAlertRepository
    {
        private readonly FibiEmlakDanismanlikContext _context;

        public ListingAlertRepository(FibiEmlakDanismanlikContext context)
        {
            _context = context;
        }

        public async Task<bool> CityExistsAsync(int cityId)
        {
            return await _context.Cities.AnyAsync(c => c.CityId == cityId);
        }

        public async Task<bool> DistrictBelongsToCityAsync(int districtId, int cityId)
        {
            return await _context.Districts.AnyAsync(d => d.DistrictId == districtId && d.CityId == cityId);
        }

        public async Task<int> CountActiveByPhoneAsync(string phone)
        {
            return await _context.ListingAlerts.CountAsync(x => x.PhoneNumber == phone && x.Status == ListingAlertStatus.Active);
        }

        public async Task AddAsync(ListingAlert alert)
        {
            await _context.ListingAlerts.AddAsync(alert);
            await _context.SaveChangesAsync();
        }

        public async Task<(int Total, List<ListingAlertRow> Rows)> SearchAsync(ListingAlertFilter f, ListingAlertScope s)
        {
            var query = _context.ListingAlerts.AsNoTracking().AsQueryable();

            // Kapsam denetimi: Owner/Manager tümünü görür, Consultant yalnızca kendisine atananları
            if (!s.SeeAll)
            {
                var agentId = s.AgentId ?? -1;
                query = query.Where(x => x.AssignedAgentId == agentId);
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

            // Arama filtresi (ad, telefon, e-posta, not)
            if (!string.IsNullOrWhiteSpace(f.Search))
            {
                var term = f.Search.Trim();
                query = query.Where(x =>
                    x.FullName.Contains(term) ||
                    x.PhoneNumber.Contains(term) ||
                    (x.Email != null && x.Email.Contains(term)) ||
                    (x.Note != null && x.Note.Contains(term)));
            }

            var page = f.Page < 1 ? 1 : f.Page;
            var pageSize = f.PageSize < 1 ? AdminListingConstants.DefaultPageSize : f.PageSize;
            if (pageSize > AdminListingConstants.MaxPageSize)
            {
                pageSize = AdminListingConstants.MaxPageSize;
            }

            if (f.OnlyWithPending)
            {
                // OnlyWithPending true ise: Kapsamdaki aktif kayıtlar çekilip bekleyen eşleşme sayısı hesaplanır,
                // 0 olanlar elenir, ardından sonuçlar bellekte sayfalanır (küçük ölçekli ajans verisi için yeterli).
                var activeCandidates = await query
                    .Where(x => x.Status == ListingAlertStatus.Active)
                    .OrderByDescending(x => x.CreatedDate)
                    .ThenByDescending(x => x.ListingAlertId)
                    .ToListAsync();

                var counts = await GetPendingCountsAsync(activeCandidates);

                var alertsWithMatches = activeCandidates
                    .Where(x => counts.TryGetValue(x.ListingAlertId, out var count) && count > 0)
                    .Select(x => (Alert: x, PendingCount: counts[x.ListingAlertId]))
                    .ToList();

                var total = alertsWithMatches.Count;
                if (total == 0)
                {
                    return (0, new List<ListingAlertRow>());
                }

                var pagedWithMatches = alertsWithMatches
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var pagedAlerts = pagedWithMatches.Select(x => x.Alert).ToList();
                var pendingMap = pagedWithMatches.ToDictionary(x => x.Alert.ListingAlertId, x => x.PendingCount);

                var rows = await ProjectToRowsAsync(pagedAlerts, pendingMap);
                return (total, rows);
            }
            else
            {
                var total = await query.CountAsync();
                if (total == 0)
                {
                    return (0, new List<ListingAlertRow>());
                }

                // Sıralama: Aktif kayıtlar (bekleyen eşleşmesi olabilecekler) öne, ardından en yeniye göre
                var pagedAlerts = await query
                    .OrderBy(x => x.Status == ListingAlertStatus.Active ? 0 : 1)
                    .ThenByDescending(x => x.CreatedDate)
                    .ThenByDescending(x => x.ListingAlertId)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var rows = await ProjectToRowsAsync(pagedAlerts, null);

                // Sayfa içi sıralama: Bekleyen eşleşmesi olanlar önce, sonra en yeni
                rows = rows
                    .OrderByDescending(x => x.PendingMatchCount > 0)
                    .ThenByDescending(x => x.CreatedDate)
                    .ThenByDescending(x => x.ListingAlertId)
                    .ToList();

                return (total, rows);
            }
        }

        public async Task<ListingAlertSummary> GetSummaryAsync(ListingAlertScope s)
        {
            var query = _context.ListingAlerts.AsNoTracking().AsQueryable();

            if (!s.SeeAll)
            {
                var agentId = s.AgentId ?? -1;
                query = query.Where(x => x.AssignedAgentId == agentId);
            }

            var active = await query.CountAsync(x => x.Status == ListingAlertStatus.Active);
            var closed = await query.CountAsync(x => x.Status == ListingAlertStatus.Closed);
            var unassigned = await query.CountAsync(x => x.Status == ListingAlertStatus.Active && x.AssignedAgentId == null);

            // Kapsamdaki aktif kayıtlar için bekleyen eşleşmesi olanları hesapla
            var activeAlerts = await query.Where(x => x.Status == ListingAlertStatus.Active).ToListAsync();
            var counts = await GetPendingCountsAsync(activeAlerts);
            var activeWithPending = counts.Count(kv => kv.Value > 0);

            return new ListingAlertSummary
            {
                Active = active,
                ActiveWithPending = activeWithPending,
                Closed = closed,
                Unassigned = unassigned
            };
        }

        public async Task<ListingAlertDetail?> GetDetailAsync(int id)
        {
            var alert = await _context.ListingAlerts.AsNoTracking().FirstOrDefaultAsync(x => x.ListingAlertId == id);
            if (alert == null)
            {
                return null;
            }

            var rows = await ProjectToRowsAsync(new List<ListingAlert> { alert }, null);
            var row = rows.First();

            var pendingMatches = await GetPendingMatchesForAlertAsync(alert);

            var matches = await _context.ListingAlertMatches.AsNoTracking()
                .Where(m => m.ListingAlertId == id)
                .OrderByDescending(m => m.CreatedAtUtc)
                .ToListAsync();

            var matchRefs = matches.Select(m => (m.ListingTypeId, m.ListingId)).Distinct().ToList();
            var matchTitleMap = await GetListingTitlesAsync(matchRefs);

            var matchAgentIds = matches
                .Where(m => m.AgentId.HasValue)
                .Select(m => m.AgentId!.Value)
                .Distinct()
                .ToList();

            var matchAgentMap = matchAgentIds.Count > 0
                ? await _context.Agents.AsNoTracking()
                    .Where(a => matchAgentIds.Contains(a.AgentId))
                    .Select(a => new { a.AgentId, a.AgentName })
                    .ToDictionaryAsync(a => a.AgentId, a => a.AgentName)
                : new Dictionary<int, string>();

            var history = matches.Select(m =>
            {
                matchTitleMap.TryGetValue((m.ListingTypeId, m.ListingId), out var lTitle);
                string? agentName = null;
                if (m.AgentId.HasValue)
                {
                    matchAgentMap.TryGetValue(m.AgentId.Value, out agentName);
                }

                return new ListingAlertMatchItem
                {
                    ListingAlertMatchId = m.ListingAlertMatchId,
                    ListingTypeId = m.ListingTypeId,
                    ListingId = m.ListingId,
                    ListingTitle = lTitle,
                    Status = m.Status,
                    AgentName = agentName,
                    CreatedAtUtc = m.CreatedAtUtc
                };
            }).ToList();

            return new ListingAlertDetail
            {
                ListingAlertId = row.ListingAlertId,
                FullName = row.FullName,
                PhoneNumber = row.PhoneNumber,
                Email = row.Email,
                ListingTypeId = row.ListingTypeId,
                TypeDisplay = row.TypeDisplay,
                CityId = row.CityId,
                CityName = row.CityName,
                DistrictId = row.DistrictId,
                DistrictName = row.DistrictName,
                MinPrice = row.MinPrice,
                MaxPrice = row.MaxPrice,
                RoomCounts = row.RoomCounts,
                NotePreview = row.NotePreview,
                Status = row.Status,
                AssignedAgentId = row.AssignedAgentId,
                AssignedAgentName = row.AssignedAgentName,
                CreatedDate = row.CreatedDate,
                PendingMatchCount = pendingMatches.Count,
                NotifiedCount = history.Count(h => h.Status == ListingAlertMatchStatus.Notified),
                SourceListingTypeId = row.SourceListingTypeId,
                SourceListingId = row.SourceListingId,
                SourceListingTitle = row.SourceListingTitle,
                Note = alert.Note,
                KvkkApprovedAt = alert.KvkkApprovedAt,
                ClosedAtUtc = alert.ClosedAtUtc,
                PendingMatches = pendingMatches,
                History = history
            };
        }

        public async Task<int?> GetAssignedAgentIdAsync(int id)
        {
            return await _context.ListingAlerts.AsNoTracking()
                .Where(x => x.ListingAlertId == id)
                .Select(x => x.AssignedAgentId)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.ListingAlerts.AsNoTracking().AnyAsync(x => x.ListingAlertId == id);
        }

        public async Task<List<ListingAlertListingItem>> GetPendingMatchesAsync(int id)
        {
            var alert = await _context.ListingAlerts.AsNoTracking().FirstOrDefaultAsync(x => x.ListingAlertId == id);
            if (alert == null || alert.Status != ListingAlertStatus.Active)
            {
                return new List<ListingAlertListingItem>();
            }

            return await GetPendingMatchesForAlertAsync(alert);
        }

        public async Task<bool> SetStatusAsync(int id, ListingAlertStatus status)
        {
            var alert = await _context.ListingAlerts.FirstOrDefaultAsync(x => x.ListingAlertId == id);
            if (alert == null)
            {
                return false;
            }

            alert.Status = status;
            if (status == ListingAlertStatus.Closed)
            {
                alert.ClosedAtUtc = DateTime.UtcNow;
            }
            else if (status == ListingAlertStatus.Active)
            {
                alert.ClosedAtUtc = null;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AssignAsync(int id, int? agentId)
        {
            var alert = await _context.ListingAlerts.FirstOrDefaultAsync(x => x.ListingAlertId == id);
            if (alert == null)
            {
                return false;
            }

            alert.AssignedAgentId = agentId;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsActiveAgentAsync(int agentId)
        {
            return await _context.Agents.AsNoTracking().AnyAsync(a => a.AgentId == agentId && a.IsActive);
        }

        public async Task UpsertMatchAsync(int alertId, int listingTypeId, int listingId, ListingAlertMatchStatus status, int? agentId)
        {
            var match = await _context.ListingAlertMatches
                .FirstOrDefaultAsync(m => m.ListingAlertId == alertId && m.ListingTypeId == listingTypeId && m.ListingId == listingId);

            if (match != null)
            {
                match.Status = status;
                match.AgentId = agentId;
                match.CreatedAtUtc = DateTime.UtcNow;
            }
            else
            {
                match = new ListingAlertMatch
                {
                    ListingAlertId = alertId,
                    ListingTypeId = listingTypeId,
                    ListingId = listingId,
                    Status = status,
                    AgentId = agentId,
                    CreatedAtUtc = DateTime.UtcNow
                };
                await _context.ListingAlertMatches.AddAsync(match);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<ListingAlertForListing>> GetAlertsForListingAsync(int listingTypeId, int listingId, ListingAlertScope s)
        {
            var info = await GetListingCriteriaInfoAsync(listingTypeId, listingId);
            if (info == null || !info.CityId.HasValue)
            {
                return new List<ListingAlertForListing>();
            }

            var query = _context.ListingAlerts.AsNoTracking()
                .Where(x => x.Status == ListingAlertStatus.Active &&
                            x.ListingTypeId == listingTypeId &&
                            x.CityId == info.CityId.Value);

            if (!s.SeeAll)
            {
                var agentId = s.AgentId ?? -1;
                query = query.Where(x => x.AssignedAgentId == agentId);
            }

            // İlçe kuralı: alert.DistrictId doluysa DistrictId eşit; boşsa tüm il uyar
            query = query.Where(x => x.DistrictId == null || (info.DistrictId.HasValue && x.DistrictId == info.DistrictId.Value));

            // Fiyat aralığı kuralı
            query = query.Where(x => (x.MinPrice == null || info.Price >= x.MinPrice.Value) &&
                                     (x.MaxPrice == null || info.Price <= x.MaxPrice.Value));

            var candidates = await query.ToListAsync();

            // Oda kuralı: Yalnızca konut türünde (2 ve 8) ve alert.RoomCounts doluysa bellekte uygulanır
            if (listingTypeId == 2 || listingTypeId == 8)
            {
                candidates = candidates.Where(x => IsRoomMatch(x.RoomCounts, info.NumberOfRoom)).ToList();
            }

            if (candidates.Count == 0)
            {
                return new List<ListingAlertForListing>();
            }

            var alertIds = candidates.Select(x => x.ListingAlertId).ToList();

            // Eşleşme veya bildirim durumu
            var matches = await _context.ListingAlertMatches.AsNoTracking()
                .Where(m => alertIds.Contains(m.ListingAlertId) && m.ListingTypeId == listingTypeId && m.ListingId == listingId)
                .ToDictionaryAsync(m => m.ListingAlertId, m => m.Status);

            // Danışman adları
            var assignedAgentIds = candidates
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

            return candidates.Select(c =>
            {
                matches.TryGetValue(c.ListingAlertId, out var matchStatus);
                string? agentName = null;
                if (c.AssignedAgentId.HasValue)
                {
                    agentMap.TryGetValue(c.AssignedAgentId.Value, out agentName);
                }

                return new ListingAlertForListing
                {
                    ListingAlertId = c.ListingAlertId,
                    FullName = c.FullName,
                    PhoneNumber = c.PhoneNumber,
                    AssignedAgentName = agentName,
                    CreatedDate = c.CreatedDate,
                    MatchStatus = matches.ContainsKey(c.ListingAlertId) ? matchStatus : null
                };
            }).OrderByDescending(x => x.CreatedDate).ToList();
        }

        public async Task<int> CountAlertsForListingOutsideScopeAsync(int listingTypeId, int listingId, ListingAlertScope s)
        {
            if (s.SeeAll)
            {
                return 0;
            }

            var info = await GetListingCriteriaInfoAsync(listingTypeId, listingId);
            if (info == null || !info.CityId.HasValue)
            {
                return 0;
            }

            var agentId = s.AgentId ?? -1;
            var query = _context.ListingAlerts.AsNoTracking()
                .Where(x => x.Status == ListingAlertStatus.Active &&
                            x.ListingTypeId == listingTypeId &&
                            x.CityId == info.CityId.Value &&
                            x.AssignedAgentId != agentId);

            query = query.Where(x => x.DistrictId == null || (info.DistrictId.HasValue && x.DistrictId == info.DistrictId.Value));
            query = query.Where(x => (x.MinPrice == null || info.Price >= x.MinPrice.Value) &&
                                     (x.MaxPrice == null || info.Price <= x.MaxPrice.Value));

            var candidates = await query.ToListAsync();

            if (listingTypeId == 2 || listingTypeId == 8)
            {
                candidates = candidates.Where(x => IsRoomMatch(x.RoomCounts, info.NumberOfRoom)).ToList();
            }

            return candidates.Count;
        }

        #region Eşleşme ve Projeksiyon Yardımcı Metotları

        /// <summary>
        /// Verilen haber talepleri için bekleyen eşleşme sayılarını toplu (batch) sorgularla hesaplar.
        /// Toplam sorgu sayısı: en fazla (farklı ilan türü sayısı + 1).
        /// </summary>
        private async Task<Dictionary<int, int>> GetPendingCountsAsync(IReadOnlyCollection<ListingAlert> alerts)
        {
            var result = new Dictionary<int, int>();
            if (alerts == null || alerts.Count == 0)
            {
                return result;
            }

            foreach (var alert in alerts)
            {
                result[alert.ListingAlertId] = 0;
            }

            var activeAlerts = alerts.Where(a => a.Status == ListingAlertStatus.Active).ToList();
            if (activeAlerts.Count == 0)
            {
                return result;
            }

            var alertIds = activeAlerts.Select(a => a.ListingAlertId).Distinct().ToList();
            var matches = await _context.ListingAlertMatches.AsNoTracking()
                .Where(m => alertIds.Contains(m.ListingAlertId))
                .Select(m => new { m.ListingAlertId, m.ListingTypeId, m.ListingId })
                .ToListAsync();

            var matchSet = new HashSet<(int AlertId, int ListingTypeId, int ListingId)>(
                matches.Select(m => (m.ListingAlertId, m.ListingTypeId, m.ListingId))
            );

            var alertsByType = activeAlerts.GroupBy(a => a.ListingTypeId);

            foreach (var group in alertsByType)
            {
                var listingTypeId = group.Key;
                var cityIds = group.Select(a => a.CityId).Distinct().ToList();
                var minCreatedDate = group.Min(a => a.CreatedDate);

                List<CandidateListing> candidates;

                switch (listingTypeId)
                {
                    case 1: // Satılık Arsa
                        candidates = await _context.forSaleLandListings.AsNoTracking()
                            .Where(l => l.PropertyStatus == PropertyStatuses.Aktif &&
                                        l.CreatedDate >= minCreatedDate &&
                                        l.CityId.HasValue && cityIds.Contains(l.CityId.Value))
                            .Select(l => new CandidateListing
                            {
                                ListingId = l.ForSaleLandListingId,
                                CityId = l.CityId,
                                DistrictId = l.DistrictId,
                                Price = l.Price,
                                CreatedDate = l.CreatedDate,
                                NumberOfRoom = null
                            })
                            .ToListAsync();
                        break;

                    case 2: // Satılık Konut
                        candidates = await _context.forSaleHousingPropertyListings.AsNoTracking()
                            .Where(l => l.PropertyStatus == PropertyStatuses.Aktif &&
                                        l.CreatedDate >= minCreatedDate &&
                                        l.CityId.HasValue && cityIds.Contains(l.CityId.Value))
                            .Select(l => new CandidateListing
                            {
                                ListingId = l.ForSaleHousingListId,
                                CityId = l.CityId,
                                DistrictId = l.DistrictId,
                                Price = l.Price,
                                CreatedDate = l.CreatedDate,
                                NumberOfRoom = l.NumberOfRoom
                            })
                            .ToListAsync();
                        break;

                    case 3: // Satılık İşyeri
                        candidates = await _context.forSaleCommercialPropertyListings.AsNoTracking()
                            .Where(l => l.PropertyStatus == PropertyStatuses.Aktif &&
                                        l.CreatedDate >= minCreatedDate &&
                                        l.CityId.HasValue && cityIds.Contains(l.CityId.Value))
                            .Select(l => new CandidateListing
                            {
                                ListingId = l.ForSaleCommercialListingId,
                                CityId = l.CityId,
                                DistrictId = l.DistrictId,
                                Price = l.Price,
                                CreatedDate = l.CreatedDate,
                                NumberOfRoom = null
                            })
                            .ToListAsync();
                        break;

                    case 7: // Kiralık Arsa
                        candidates = await _context.rentalLandListings.AsNoTracking()
                            .Where(l => l.PropertyStatus == PropertyStatuses.Aktif &&
                                        l.CreatedDate >= minCreatedDate &&
                                        l.CityId.HasValue && cityIds.Contains(l.CityId.Value))
                            .Select(l => new CandidateListing
                            {
                                ListingId = l.RentalLandListingId,
                                CityId = l.CityId,
                                DistrictId = l.DistrictId,
                                Price = l.Rent,
                                CreatedDate = l.CreatedDate,
                                NumberOfRoom = null
                            })
                            .ToListAsync();
                        break;

                    case 8: // Kiralık Konut
                        candidates = await _context.rentalHousingListings.AsNoTracking()
                            .Where(l => l.PropertyStatus == PropertyStatuses.Aktif &&
                                        l.CreatedDate >= minCreatedDate &&
                                        l.CityId.HasValue && cityIds.Contains(l.CityId.Value))
                            .Select(l => new CandidateListing
                            {
                                ListingId = l.RentalHousingListId,
                                CityId = l.CityId,
                                DistrictId = l.DistrictId,
                                Price = l.Rent,
                                CreatedDate = l.CreatedDate,
                                NumberOfRoom = l.NumberOfRoom
                            })
                            .ToListAsync();
                        break;

                    case 9: // Kiralık İşyeri
                        candidates = await _context.rentalCommercialPropertyListings.AsNoTracking()
                            .Where(l => l.PropertyStatus == PropertyStatuses.Aktif &&
                                        l.CreatedDate >= minCreatedDate &&
                                        l.CityId.HasValue && cityIds.Contains(l.CityId.Value))
                            .Select(l => new CandidateListing
                            {
                                ListingId = l.RentalCommercialListId,
                                CityId = l.CityId,
                                DistrictId = l.DistrictId,
                                Price = l.Rent,
                                CreatedDate = l.CreatedDate,
                                NumberOfRoom = null
                            })
                            .ToListAsync();
                        break;

                    default:
                        candidates = new List<CandidateListing>();
                        break;
                }

                foreach (var alert in group)
                {
                    var count = 0;
                    foreach (var c in candidates)
                    {
                        if (c.CityId != alert.CityId)
                            continue;

                        if (c.CreatedDate < alert.CreatedDate)
                            continue;

                        if (alert.DistrictId.HasValue && c.DistrictId != alert.DistrictId.Value)
                            continue;

                        if (alert.MinPrice.HasValue && c.Price < alert.MinPrice.Value)
                            continue;

                        if (alert.MaxPrice.HasValue && c.Price > alert.MaxPrice.Value)
                            continue;

                        if (matchSet.Contains((alert.ListingAlertId, alert.ListingTypeId, c.ListingId)))
                            continue;

                        if ((listingTypeId == 2 || listingTypeId == 8) &&
                            !string.IsNullOrWhiteSpace(alert.RoomCounts) &&
                            !IsRoomMatch(alert.RoomCounts, c.NumberOfRoom))
                        {
                            continue;
                        }

                        count++;
                    }

                    result[alert.ListingAlertId] = count;
                }
            }

            return result;
        }

        /// <summary>
        /// Aktif bir haber talebi için eşleşme kriterlerine uyan ve henüz işlem görmemiş ilanları çeker.
        /// </summary>
        private async Task<List<ListingAlertListingItem>> GetPendingMatchesForAlertAsync(ListingAlert alert)
        {
            if (alert.Status != ListingAlertStatus.Active)
            {
                return new List<ListingAlertListingItem>();
            }

            // Bu talep ve bu ilan türü için daha önce kaydedilmiş bildirim veya eleme kayıtları
            var matchedListingIds = await _context.ListingAlertMatches.AsNoTracking()
                .Where(m => m.ListingAlertId == alert.ListingAlertId && m.ListingTypeId == alert.ListingTypeId)
                .Select(m => m.ListingId)
                .ToListAsync();

            var minPrice = alert.MinPrice;
            var maxPrice = alert.MaxPrice;
            var districtId = alert.DistrictId;
            var cityId = alert.CityId;
            var createdDate = alert.CreatedDate;

            switch (alert.ListingTypeId)
            {
                case 1: // Satılık Arsa
                {
                    var q = _context.forSaleLandListings.AsNoTracking()
                        .Where(l => l.PropertyStatus == PropertyStatuses.Aktif &&
                                    l.CityId == cityId &&
                                    l.CreatedDate >= createdDate);

                    if (districtId.HasValue)
                        q = q.Where(l => l.DistrictId == districtId.Value);

                    if (minPrice.HasValue)
                        q = q.Where(l => l.Price >= minPrice.Value);

                    if (maxPrice.HasValue)
                        q = q.Where(l => l.Price <= maxPrice.Value);

                    if (matchedListingIds.Count > 0)
                        q = q.Where(l => !matchedListingIds.Contains(l.ForSaleLandListingId));

                    return await q.Select(l => new ListingAlertListingItem
                    {
                        ListingTypeId = 1,
                        ListingId = l.ForSaleLandListingId,
                        Title = l.PropertyName,
                        Price = l.Price,
                        City = l.City,
                        District = l.District,
                        NumberOfRoom = null,
                        CreatedDate = l.CreatedDate,
                        AgentId = l.AgentId,
                        AgentName = l.Agent != null ? l.Agent.AgentName : null
                    }).ToListAsync();
                }

                case 2: // Satılık Konut
                {
                    var q = _context.forSaleHousingPropertyListings.AsNoTracking()
                        .Where(l => l.PropertyStatus == PropertyStatuses.Aktif &&
                                    l.CityId == cityId &&
                                    l.CreatedDate >= createdDate);

                    if (districtId.HasValue)
                        q = q.Where(l => l.DistrictId == districtId.Value);

                    if (minPrice.HasValue)
                        q = q.Where(l => l.Price >= minPrice.Value);

                    if (maxPrice.HasValue)
                        q = q.Where(l => l.Price <= maxPrice.Value);

                    if (matchedListingIds.Count > 0)
                        q = q.Where(l => !matchedListingIds.Contains(l.ForSaleHousingListId));

                    var items = await q.Select(l => new ListingAlertListingItem
                    {
                        ListingTypeId = 2,
                        ListingId = l.ForSaleHousingListId,
                        Title = l.PropertyName,
                        Price = l.Price,
                        City = l.City,
                        District = l.District,
                        NumberOfRoom = l.NumberOfRoom,
                        CreatedDate = l.CreatedDate,
                        AgentId = l.AgentId,
                        AgentName = l.Agent != null ? l.Agent.AgentName : null
                    }).ToListAsync();

                    if (!string.IsNullOrWhiteSpace(alert.RoomCounts))
                    {
                        items = items.Where(x => IsRoomMatch(alert.RoomCounts, x.NumberOfRoom)).ToList();
                    }

                    return items;
                }

                case 3: // Satılık İşyeri
                {
                    var q = _context.forSaleCommercialPropertyListings.AsNoTracking()
                        .Where(l => l.PropertyStatus == PropertyStatuses.Aktif &&
                                    l.CityId == cityId &&
                                    l.CreatedDate >= createdDate);

                    if (districtId.HasValue)
                        q = q.Where(l => l.DistrictId == districtId.Value);

                    if (minPrice.HasValue)
                        q = q.Where(l => l.Price >= minPrice.Value);

                    if (maxPrice.HasValue)
                        q = q.Where(l => l.Price <= maxPrice.Value);

                    if (matchedListingIds.Count > 0)
                        q = q.Where(l => !matchedListingIds.Contains(l.ForSaleCommercialListingId));

                    return await q.Select(l => new ListingAlertListingItem
                    {
                        ListingTypeId = 3,
                        ListingId = l.ForSaleCommercialListingId,
                        Title = l.PropertyName,
                        Price = l.Price,
                        City = l.City,
                        District = l.District,
                        NumberOfRoom = null,
                        CreatedDate = l.CreatedDate,
                        AgentId = l.AgentId,
                        AgentName = l.Agent != null ? l.Agent.AgentName : null
                    }).ToListAsync();
                }

                case 7: // Kiralık Arsa
                {
                    var q = _context.rentalLandListings.AsNoTracking()
                        .Where(l => l.PropertyStatus == PropertyStatuses.Aktif &&
                                    l.CityId == cityId &&
                                    l.CreatedDate >= createdDate);

                    if (districtId.HasValue)
                        q = q.Where(l => l.DistrictId == districtId.Value);

                    if (minPrice.HasValue)
                        q = q.Where(l => l.Rent >= minPrice.Value);

                    if (maxPrice.HasValue)
                        q = q.Where(l => l.Rent <= maxPrice.Value);

                    if (matchedListingIds.Count > 0)
                        q = q.Where(l => !matchedListingIds.Contains(l.RentalLandListingId));

                    return await q.Select(l => new ListingAlertListingItem
                    {
                        ListingTypeId = 7,
                        ListingId = l.RentalLandListingId,
                        Title = l.PropertyName,
                        Price = l.Rent,
                        City = l.City,
                        District = l.District,
                        NumberOfRoom = null,
                        CreatedDate = l.CreatedDate,
                        AgentId = l.AgentId,
                        AgentName = l.Agent != null ? l.Agent.AgentName : null
                    }).ToListAsync();
                }

                case 8: // Kiralık Konut
                {
                    var q = _context.rentalHousingListings.AsNoTracking()
                        .Where(l => l.PropertyStatus == PropertyStatuses.Aktif &&
                                    l.CityId == cityId &&
                                    l.CreatedDate >= createdDate);

                    if (districtId.HasValue)
                        q = q.Where(l => l.DistrictId == districtId.Value);

                    if (minPrice.HasValue)
                        q = q.Where(l => l.Rent >= minPrice.Value);

                    if (maxPrice.HasValue)
                        q = q.Where(l => l.Rent <= maxPrice.Value);

                    if (matchedListingIds.Count > 0)
                        q = q.Where(l => !matchedListingIds.Contains(l.RentalHousingListId));

                    var items = await q.Select(l => new ListingAlertListingItem
                    {
                        ListingTypeId = 8,
                        ListingId = l.RentalHousingListId,
                        Title = l.PropertyName,
                        Price = l.Rent,
                        City = l.City,
                        District = l.District,
                        NumberOfRoom = l.NumberOfRoom,
                        CreatedDate = l.CreatedDate,
                        AgentId = l.AgentId,
                        AgentName = l.Agent != null ? l.Agent.AgentName : null
                    }).ToListAsync();

                    if (!string.IsNullOrWhiteSpace(alert.RoomCounts))
                    {
                        items = items.Where(x => IsRoomMatch(alert.RoomCounts, x.NumberOfRoom)).ToList();
                    }

                    return items;
                }

                case 9: // Kiralık İşyeri
                {
                    var q = _context.rentalCommercialPropertyListings.AsNoTracking()
                        .Where(l => l.PropertyStatus == PropertyStatuses.Aktif &&
                                    l.CityId == cityId &&
                                    l.CreatedDate >= createdDate);

                    if (districtId.HasValue)
                        q = q.Where(l => l.DistrictId == districtId.Value);

                    if (minPrice.HasValue)
                        q = q.Where(l => l.Rent >= minPrice.Value);

                    if (maxPrice.HasValue)
                        q = q.Where(l => l.Rent <= maxPrice.Value);

                    if (matchedListingIds.Count > 0)
                        q = q.Where(l => !matchedListingIds.Contains(l.RentalCommercialListId));

                    return await q.Select(l => new ListingAlertListingItem
                    {
                        ListingTypeId = 9,
                        ListingId = l.RentalCommercialListId,
                        Title = l.PropertyName,
                        Price = l.Rent,
                        City = l.City,
                        District = l.District,
                        NumberOfRoom = null,
                        CreatedDate = l.CreatedDate,
                        AgentId = l.AgentId,
                        AgentName = l.Agent != null ? l.Agent.AgentName : null
                    }).ToListAsync();
                }

                default:
                    return new List<ListingAlertListingItem>();
            }
        }

        /// <summary>
        /// Konut oda sayısı tercihlerini ilanın oda sayısıyla eşleştirir.
        /// Listede "5+1 ve üzeri" varsa ilk sayısı 5 veya daha büyük olan oda değerleri de uyar ("6+1", "5+2" vb.).
        /// </summary>
        private static bool IsRoomMatch(string? roomCounts, string? listingRooms)
        {
            if (string.IsNullOrWhiteSpace(roomCounts))
            {
                return true;
            }

            if (string.IsNullOrWhiteSpace(listingRooms))
            {
                return false;
            }

            var desiredRooms = roomCounts.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var trimmedListing = listingRooms.Trim();

            // Tam eşleşme (örn: "2+1" == "2+1")
            if (desiredRooms.Any(r => string.Equals(r, trimmedListing, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            // "5+1 ve üzeri" seçeneği varsa ilk sayısı 5 veya daha büyük olan oda değerlerini de eşleştir
            var hasFivePlus = desiredRooms.Any(r => string.Equals(r, "5+1 ve üzeri", StringComparison.OrdinalIgnoreCase));
            if (hasFivePlus)
            {
                var plusIndex = trimmedListing.IndexOf('+');
                var roomPrefix = plusIndex > 0 ? trimmedListing.Substring(0, plusIndex).Trim() : trimmedListing;
                if (int.TryParse(roomPrefix, out var roomCount) && roomCount >= 5)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// İlan türü ve kimliğine göre eşleşme kriteri kontrolü için gerekli temel bilgileri çeker.
        /// </summary>
        private async Task<ListingCriteriaInfo?> GetListingCriteriaInfoAsync(int listingTypeId, int listingId)
        {
            switch (listingTypeId)
            {
                case 1:
                    return await _context.forSaleLandListings.AsNoTracking()
                        .Where(x => x.ForSaleLandListingId == listingId)
                        .Select(x => new ListingCriteriaInfo
                        {
                            CityId = x.CityId,
                            DistrictId = x.DistrictId,
                            Price = x.Price,
                            NumberOfRoom = null
                        })
                        .FirstOrDefaultAsync();

                case 2:
                    return await _context.forSaleHousingPropertyListings.AsNoTracking()
                        .Where(x => x.ForSaleHousingListId == listingId)
                        .Select(x => new ListingCriteriaInfo
                        {
                            CityId = x.CityId,
                            DistrictId = x.DistrictId,
                            Price = x.Price,
                            NumberOfRoom = x.NumberOfRoom
                        })
                        .FirstOrDefaultAsync();

                case 3:
                    return await _context.forSaleCommercialPropertyListings.AsNoTracking()
                        .Where(x => x.ForSaleCommercialListingId == listingId)
                        .Select(x => new ListingCriteriaInfo
                        {
                            CityId = x.CityId,
                            DistrictId = x.DistrictId,
                            Price = x.Price,
                            NumberOfRoom = null
                        })
                        .FirstOrDefaultAsync();

                case 7:
                    return await _context.rentalLandListings.AsNoTracking()
                        .Where(x => x.RentalLandListingId == listingId)
                        .Select(x => new ListingCriteriaInfo
                        {
                            CityId = x.CityId,
                            DistrictId = x.DistrictId,
                            Price = x.Rent,
                            NumberOfRoom = null
                        })
                        .FirstOrDefaultAsync();

                case 8:
                    return await _context.rentalHousingListings.AsNoTracking()
                        .Where(x => x.RentalHousingListId == listingId)
                        .Select(x => new ListingCriteriaInfo
                        {
                            CityId = x.CityId,
                            DistrictId = x.DistrictId,
                            Price = x.Rent,
                            NumberOfRoom = x.NumberOfRoom
                        })
                        .FirstOrDefaultAsync();

                case 9:
                    return await _context.rentalCommercialPropertyListings.AsNoTracking()
                        .Where(x => x.RentalCommercialListId == listingId)
                        .Select(x => new ListingCriteriaInfo
                        {
                            CityId = x.CityId,
                            DistrictId = x.DistrictId,
                            Price = x.Rent,
                            NumberOfRoom = null
                        })
                        .FirstOrDefaultAsync();

                default:
                    return null;
            }
        }

        /// <summary>
        /// İlan kayıt listesini yönetim paneli liste satırlarına dönüştürür.
        /// </summary>
        private async Task<List<ListingAlertRow>> ProjectToRowsAsync(List<ListingAlert> alerts, Dictionary<int, int>? knownPendingCounts)
        {
            if (alerts.Count == 0)
            {
                return new List<ListingAlertRow>();
            }

            var cityIds = alerts.Select(x => x.CityId).Distinct().ToList();
            var cityMap = await _context.Cities.AsNoTracking()
                .Where(c => cityIds.Contains(c.CityId))
                .Select(c => new { c.CityId, c.Name })
                .ToDictionaryAsync(c => c.CityId, c => c.Name);

            var districtIds = alerts.Where(x => x.DistrictId.HasValue).Select(x => x.DistrictId!.Value).Distinct().ToList();
            var districtMap = districtIds.Count > 0
                ? await _context.Districts.AsNoTracking()
                    .Where(d => districtIds.Contains(d.DistrictId))
                    .Select(d => new { d.DistrictId, d.Name })
                    .ToDictionaryAsync(d => d.DistrictId, d => d.Name)
                : new Dictionary<int, string>();

            var assignedAgentIds = alerts.Where(x => x.AssignedAgentId.HasValue).Select(x => x.AssignedAgentId!.Value).Distinct().ToList();
            var agentMap = assignedAgentIds.Count > 0
                ? await _context.Agents.AsNoTracking()
                    .Where(a => assignedAgentIds.Contains(a.AgentId))
                    .Select(a => new { a.AgentId, a.AgentName })
                    .ToDictionaryAsync(a => a.AgentId, a => a.AgentName)
                : new Dictionary<int, string>();

            var sourceRefs = alerts
                .Where(x => x.SourceListingTypeId.HasValue && x.SourceListingId.HasValue)
                .Select(x => (TypeId: x.SourceListingTypeId!.Value, Id: x.SourceListingId!.Value))
                .Distinct()
                .ToList();

            var titleMap = await GetListingTitlesAsync(sourceRefs);

            var alertIds = alerts.Select(x => x.ListingAlertId).ToList();

            var notifiedCounts = await _context.ListingAlertMatches.AsNoTracking()
                .Where(m => alertIds.Contains(m.ListingAlertId) && m.Status == ListingAlertMatchStatus.Notified)
                .GroupBy(m => m.ListingAlertId)
                .Select(g => new { AlertId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.AlertId, g => g.Count);

            var pendingCounts = new Dictionary<int, int>();
            var alertsToFetch = new List<ListingAlert>();

            foreach (var alert in alerts)
            {
                if (knownPendingCounts != null && knownPendingCounts.TryGetValue(alert.ListingAlertId, out var count))
                {
                    pendingCounts[alert.ListingAlertId] = count;
                }
                else if (alert.Status == ListingAlertStatus.Active)
                {
                    alertsToFetch.Add(alert);
                }
                else
                {
                    pendingCounts[alert.ListingAlertId] = 0;
                }
            }

            if (alertsToFetch.Count > 0)
            {
                var fetchedCounts = await GetPendingCountsAsync(alertsToFetch);
                foreach (var kvp in fetchedCounts)
                {
                    pendingCounts[kvp.Key] = kvp.Value;
                }
            }

            return alerts.Select(e =>
            {
                cityMap.TryGetValue(e.CityId, out var cityName);
                string? districtName = null;
                if (e.DistrictId.HasValue)
                {
                    districtMap.TryGetValue(e.DistrictId.Value, out districtName);
                }

                string? agentName = null;
                if (e.AssignedAgentId.HasValue)
                {
                    agentMap.TryGetValue(e.AssignedAgentId.Value, out agentName);
                }

                string? sourceTitle = null;
                if (e.SourceListingTypeId.HasValue && e.SourceListingId.HasValue)
                {
                    titleMap.TryGetValue((e.SourceListingTypeId.Value, e.SourceListingId.Value), out sourceTitle);
                }

                notifiedCounts.TryGetValue(e.ListingAlertId, out var notifCount);
                pendingCounts.TryGetValue(e.ListingAlertId, out var pendCount);

                var isRent = AdminListingConstants.IsRent(e.ListingTypeId);
                var typeDisplay = $"{(isRent ? "Kiralık" : "Satılık")} {AdminListingConstants.CategoryDisplay(e.ListingTypeId)}";

                return new ListingAlertRow
                {
                    ListingAlertId = e.ListingAlertId,
                    FullName = e.FullName,
                    PhoneNumber = e.PhoneNumber,
                    Email = e.Email,
                    ListingTypeId = e.ListingTypeId,
                    TypeDisplay = typeDisplay,
                    CityId = e.CityId,
                    CityName = cityName ?? string.Empty,
                    DistrictId = e.DistrictId,
                    DistrictName = districtName,
                    MinPrice = e.MinPrice,
                    MaxPrice = e.MaxPrice,
                    RoomCounts = e.RoomCounts,
                    NotePreview = GetNotePreview(e.Note),
                    Status = e.Status,
                    AssignedAgentId = e.AssignedAgentId,
                    AssignedAgentName = agentName,
                    CreatedDate = e.CreatedDate,
                    PendingMatchCount = pendCount,
                    NotifiedCount = notifCount,
                    SourceListingTypeId = e.SourceListingTypeId,
                    SourceListingId = e.SourceListingId,
                    SourceListingTitle = sourceTitle
                };
            }).ToList();
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

        private static string? GetNotePreview(string? note)
        {
            if (string.IsNullOrWhiteSpace(note)) return null;
            var trimmed = note.Trim();
            return trimmed.Length <= 140 ? trimmed : trimmed.Substring(0, 140);
        }

        private sealed class ListingCriteriaInfo
        {
            public int? CityId { get; set; }
            public int? DistrictId { get; set; }
            public decimal Price { get; set; }
            public string? NumberOfRoom { get; set; }
        }

        private sealed class CandidateListing
        {
            public int ListingId { get; set; }
            public int? CityId { get; set; }
            public int? DistrictId { get; set; }
            public decimal Price { get; set; }
            public DateTime CreatedDate { get; set; }
            public string? NumberOfRoom { get; set; }
        }

        #endregion
    }
}
