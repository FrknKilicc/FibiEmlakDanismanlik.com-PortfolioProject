using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Interfaces.ListingDealInterfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using FibiEmlakDanismanlik.Domain.Enums;
using FibiEmlakDanismanlik.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FibiEmlakDanismanlik.Persistence.Repositories.ListingDealRepositories
{
    /// <summary>
    /// Satış ve kiralama işlem kayıtları (kapora, komisyon) ve hak ediş (prim) repository implementasyonu.
    /// </summary>
    public class ListingDealRepository : IListingDealRepository
    {
        private readonly FibiEmlakDanismanlikContext _context;

        public ListingDealRepository(FibiEmlakDanismanlikContext context)
        {
            _context = context;
        }

        public async Task<(int Total, List<ListingDealRow> Rows)> SearchAsync(ListingDealFilter f)
        {
            var query = ApplyFilter(f, includeStatus: true).AsNoTracking();

            var total = await query.CountAsync();
            if (total == 0)
            {
                return (0, new List<ListingDealRow>());
            }

            var page = f.Page < 1 ? 1 : f.Page;
            var pageSize = f.PageSize < 1 ? ListingDealConstants.DefaultPageSize : f.PageSize;
            if (pageSize > ListingDealConstants.MaxPageSize)
            {
                pageSize = ListingDealConstants.MaxPageSize;
            }

            var rows = await query
                .OrderByDescending(d => d.CompletedDate ?? d.CancelledDate ?? d.DepositDate ?? d.CreatedAtUtc.Date)
                .ThenByDescending(d => d.ListingDealId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(d => new ListingDealRow
                {
                    ListingDealId = d.ListingDealId,
                    ListingTypeId = d.ListingTypeId,
                    ListingId = d.ListingId,
                    ListingTitle = null,
                    DealType = d.DealType,
                    Status = d.Status,
                    AgreedPrice = d.AgreedPrice,
                    DepositAmount = d.DepositAmount,
                    TotalCommission = (d.BuyerCommissionAmount ?? 0m) + (d.SellerCommissionAmount ?? 0m),
                    TotalShares = d.Shares.Sum(s => (decimal?)s.Amount) ?? 0m,
                    CompanyShare = ((d.BuyerCommissionAmount ?? 0m) + (d.SellerCommissionAmount ?? 0m)) - (d.Shares.Sum(s => (decimal?)s.Amount) ?? 0m),
                    CustomerName = d.CustomerName,
                    DealDate = d.CompletedDate ?? d.CancelledDate ?? d.DepositDate ?? d.CreatedAtUtc.Date,
                    ShareAgentNames = _context.DealShares
                        .Where(s => s.ListingDealId == d.ListingDealId)
                        .Join(_context.Agents, s => s.AgentId, a => a.AgentId, (s, a) => a.AgentName)
                        .Distinct()
                        .ToList()
                })
                .ToListAsync();

            if (rows.Count == 0)
            {
                return (total, rows);
            }

            var listingRefs = rows
                .Select(r => (TypeId: r.ListingTypeId, Id: r.ListingId))
                .Distinct()
                .ToList();

            var titleMap = await GetListingTitlesAsync(listingRefs);

            foreach (var r in rows)
            {
                if (titleMap.TryGetValue((r.ListingTypeId, r.ListingId), out var title))
                {
                    r.ListingTitle = title;
                }
            }

            return (total, rows);
        }

        public async Task<ListingDealSummary> GetSummaryAsync(ListingDealFilter f)
        {
            var baseQuery = ApplyFilter(f, includeStatus: false).AsNoTracking();

            var dealStats = await baseQuery
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    DepositReceivedCount = g.Count(d => d.Status == DealStatus.DepositReceived),
                    CompletedCount = g.Count(d => d.Status == DealStatus.Completed),
                    CancelledCount = g.Count(d => d.Status == DealStatus.Cancelled),
                    CompletedCommissionTotal = g.Sum(d => d.Status == DealStatus.Completed
                        ? ((d.BuyerCommissionAmount ?? 0m) + (d.SellerCommissionAmount ?? 0m))
                        : 0m),
                    KeptDepositTotal = g.Sum(d => (d.Status == DealStatus.Cancelled && d.DepositOutcome == DepositOutcome.Kept)
                        ? (d.DepositAmount ?? 0m)
                        : 0m)
                })
                .FirstOrDefaultAsync();

            var depositReceivedCount = dealStats?.DepositReceivedCount ?? 0;
            var completedCount = dealStats?.CompletedCount ?? 0;
            var cancelledCount = dealStats?.CancelledCount ?? 0;
            var completedCommissionTotal = dealStats?.CompletedCommissionTotal ?? 0m;
            var keptDepositTotal = dealStats?.KeptDepositTotal ?? 0m;

            var completedSharesTotal = await baseQuery
                .Where(d => d.Status == DealStatus.Completed)
                .SelectMany(d => d.Shares)
                .SumAsync(s => (decimal?)s.Amount) ?? 0m;

            var pendingSharesTotal = await baseQuery
                .Where(d => d.Status != DealStatus.Cancelled)
                .SelectMany(d => d.Shares)
                .Where(s => s.PaymentStatus == DealSharePaymentStatus.Pending)
                .SumAsync(s => (decimal?)s.Amount) ?? 0m;

            var companyShareTotal = completedCommissionTotal - completedSharesTotal;

            return new ListingDealSummary
            {
                DepositReceivedCount = depositReceivedCount,
                CompletedCount = completedCount,
                CancelledCount = cancelledCount,
                CompletedCommissionTotal = completedCommissionTotal,
                CompletedSharesTotal = completedSharesTotal,
                CompanyShareTotal = companyShareTotal,
                PendingSharesTotal = pendingSharesTotal,
                KeptDepositTotal = keptDepositTotal
            };
        }

        public async Task<ListingDealDetail?> GetDetailAsync(int id)
        {
            var deal = await _context.ListingDeals.AsNoTracking()
                .Include(d => d.Shares)
                .FirstOrDefaultAsync(d => d.ListingDealId == id);

            if (deal == null)
            {
                return null;
            }

            var agentIds = deal.Shares.Select(s => s.AgentId).ToList();
            if (deal.CreatedByAgentId.HasValue)
            {
                agentIds.Add(deal.CreatedByAgentId.Value);
            }
            agentIds = agentIds.Distinct().ToList();

            var agentNameMap = agentIds.Count > 0
                ? await _context.Agents.AsNoTracking()
                    .Where(a => agentIds.Contains(a.AgentId))
                    .Select(a => new { a.AgentId, a.AgentName })
                    .ToDictionaryAsync(a => a.AgentId, a => a.AgentName)
                : new Dictionary<int, string>();

            var (title, status) = await GetListingTitleAndStatusAsync(deal.ListingTypeId, deal.ListingId);

            string? createdByAgentName = null;
            if (deal.CreatedByAgentId.HasValue && agentNameMap.TryGetValue(deal.CreatedByAgentId.Value, out var cbName))
            {
                createdByAgentName = cbName;
            }

            var shares = deal.Shares
                .OrderBy(s => s.DealShareId)
                .Select(s => new DealShareItem
                {
                    DealShareId = s.DealShareId,
                    AgentId = s.AgentId,
                    AgentName = agentNameMap.TryGetValue(s.AgentId, out var aName) ? aName : string.Empty,
                    RoleAtDeal = s.RoleAtDeal,
                    SharePercent = s.SharePercent,
                    Amount = s.Amount,
                    PaymentStatus = s.PaymentStatus,
                    PaidDate = s.PaidDate,
                    Note = s.Note
                })
                .ToList();

            var totalCommission = (deal.BuyerCommissionAmount ?? 0m) + (deal.SellerCommissionAmount ?? 0m);
            var totalShares = shares.Sum(s => s.Amount);
            var companyShare = totalCommission - totalShares;

            return new ListingDealDetail
            {
                ListingDealId = deal.ListingDealId,
                ListingTypeId = deal.ListingTypeId,
                ListingId = deal.ListingId,
                DealType = deal.DealType,
                Status = deal.Status,
                AgreedPrice = deal.AgreedPrice,
                DepositAmount = deal.DepositAmount,
                DepositDate = deal.DepositDate,
                BuyerCommissionRate = deal.BuyerCommissionRate,
                SellerCommissionRate = deal.SellerCommissionRate,
                BuyerCommissionAmount = deal.BuyerCommissionAmount,
                SellerCommissionAmount = deal.SellerCommissionAmount,
                DepositOutcome = deal.DepositOutcome,
                DepositRefundAmount = deal.DepositRefundAmount,
                DepositRefundDate = deal.DepositRefundDate,
                CompletedDate = deal.CompletedDate,
                CancelledDate = deal.CancelledDate,
                CustomerName = deal.CustomerName,
                CustomerPhone = deal.CustomerPhone,
                Notes = deal.Notes,
                CreatedByAgentId = deal.CreatedByAgentId,
                CreatedAtUtc = deal.CreatedAtUtc,
                UpdatedAtUtc = deal.UpdatedAtUtc,
                ListingTitle = title,
                ListingStatus = status,
                CreatedByAgentName = createdByAgentName,
                TotalCommission = totalCommission,
                TotalShares = totalShares,
                CompanyShare = companyShare,
                Shares = shares
            };
        }

        public async Task<List<ListingDealRow>> GetForListingAsync(int listingTypeId, int listingId)
        {
            var query = _context.ListingDeals.AsNoTracking()
                .Where(d => d.ListingTypeId == listingTypeId && d.ListingId == listingId)
                .OrderByDescending(d => d.CompletedDate ?? d.CancelledDate ?? d.DepositDate ?? d.CreatedAtUtc.Date)
                .ThenByDescending(d => d.ListingDealId);

            var rows = await query
                .Select(d => new ListingDealRow
                {
                    ListingDealId = d.ListingDealId,
                    ListingTypeId = d.ListingTypeId,
                    ListingId = d.ListingId,
                    ListingTitle = null,
                    DealType = d.DealType,
                    Status = d.Status,
                    AgreedPrice = d.AgreedPrice,
                    DepositAmount = d.DepositAmount,
                    TotalCommission = (d.BuyerCommissionAmount ?? 0m) + (d.SellerCommissionAmount ?? 0m),
                    TotalShares = d.Shares.Sum(s => (decimal?)s.Amount) ?? 0m,
                    CompanyShare = ((d.BuyerCommissionAmount ?? 0m) + (d.SellerCommissionAmount ?? 0m)) - (d.Shares.Sum(s => (decimal?)s.Amount) ?? 0m),
                    CustomerName = d.CustomerName,
                    DealDate = d.CompletedDate ?? d.CancelledDate ?? d.DepositDate ?? d.CreatedAtUtc.Date,
                    ShareAgentNames = _context.DealShares
                        .Where(s => s.ListingDealId == d.ListingDealId)
                        .Join(_context.Agents, s => s.AgentId, a => a.AgentId, (s, a) => a.AgentName)
                        .Distinct()
                        .ToList()
                })
                .ToListAsync();

            if (rows.Count == 0)
            {
                return rows;
            }

            var titleMap = await GetListingTitlesAsync(new List<(int, int)> { (listingTypeId, listingId) });
            if (titleMap.TryGetValue((listingTypeId, listingId), out var title))
            {
                foreach (var r in rows)
                {
                    r.ListingTitle = title;
                }
            }

            return rows;
        }

        public async Task<ListingDeal?> GetTrackedAsync(int id)
        {
            return await _context.ListingDeals
                .Include(d => d.Shares)
                .FirstOrDefaultAsync(d => d.ListingDealId == id);
        }

        public async Task<DealShare?> GetTrackedShareAsync(int shareId)
        {
            return await _context.DealShares
                .Include(s => s.ListingDeal)
                    .ThenInclude(d => d.Shares)
                .FirstOrDefaultAsync(s => s.DealShareId == shareId);
        }

        public async Task<bool> HasOpenDealAsync(int listingTypeId, int listingId)
        {
            return await _context.ListingDeals.AsNoTracking()
                .AnyAsync(d => d.ListingTypeId == listingTypeId &&
                               d.ListingId == listingId &&
                               d.Status == DealStatus.DepositReceived);
        }

        public async Task<bool> AnyForListingAsync(int listingTypeId, int listingId)
        {
            return await _context.ListingDeals.AsNoTracking()
                .AnyAsync(d => d.ListingTypeId == listingTypeId && d.ListingId == listingId);
        }

        public async Task<AgentRole?> GetActiveAgentRoleAsync(int agentId)
        {
            return await _context.Agents.AsNoTracking()
                .Where(a => a.AgentId == agentId && a.IsActive)
                .Select(a => (AgentRole?)a.Role)
                .FirstOrDefaultAsync();
        }

        public async Task AddAsync(ListingDeal deal)
        {
            await _context.ListingDeals.AddAsync(deal);
            await _context.SaveChangesAsync();
        }

        public void RemoveShare(DealShare share)
        {
            _context.DealShares.Remove(share);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        #region Yardımcı Metotlar

        private IQueryable<ListingDeal> ApplyFilter(ListingDealFilter f, bool includeStatus)
        {
            var query = _context.ListingDeals.AsQueryable();

            if (includeStatus && f.Status.HasValue)
            {
                query = query.Where(d => d.Status == f.Status.Value);
            }

            if (f.DealType.HasValue)
            {
                query = query.Where(d => d.DealType == f.DealType.Value);
            }

            if (f.AgentId.HasValue)
            {
                query = query.Where(d => d.Shares.Any(s => s.AgentId == f.AgentId.Value));
            }

            if (f.From.HasValue)
            {
                var fromDate = f.From.Value.Date;
                query = query.Where(d => (d.CompletedDate ?? d.CancelledDate ?? d.DepositDate ?? d.CreatedAtUtc.Date) >= fromDate);
            }

            if (f.To.HasValue)
            {
                var toExclusive = f.To.Value.Date.AddDays(1);
                query = query.Where(d => (d.CompletedDate ?? d.CancelledDate ?? d.DepositDate ?? d.CreatedAtUtc.Date) < toExclusive);
            }

            if (!string.IsNullOrWhiteSpace(f.Search))
            {
                var term = f.Search.Trim();
                if (int.TryParse(term, out var searchId))
                {
                    query = query.Where(d => d.ListingId == searchId ||
                                             d.CustomerName.Contains(term) ||
                                             (d.CustomerPhone != null && d.CustomerPhone.Contains(term)));
                }
                else
                {
                    query = query.Where(d => d.CustomerName.Contains(term) ||
                                             (d.CustomerPhone != null && d.CustomerPhone.Contains(term)));
                }
            }

            return query;
        }

        private async Task<(string? Title, string? Status)> GetListingTitleAndStatusAsync(int listingTypeId, int listingId)
        {
            switch (listingTypeId)
            {
                case 1:
                {
                    var item = await _context.forSaleLandListings.AsNoTracking()
                        .Where(x => x.ForSaleLandListingId == listingId)
                        .Select(x => new { x.PropertyName, x.PropertyStatus })
                        .FirstOrDefaultAsync();
                    return item != null ? (item.PropertyName, item.PropertyStatus) : (null, null);
                }
                case 2:
                {
                    var item = await _context.forSaleHousingPropertyListings.AsNoTracking()
                        .Where(x => x.ForSaleHousingListId == listingId)
                        .Select(x => new { x.PropertyName, x.PropertyStatus })
                        .FirstOrDefaultAsync();
                    return item != null ? (item.PropertyName, item.PropertyStatus) : (null, null);
                }
                case 3:
                {
                    var item = await _context.forSaleCommercialPropertyListings.AsNoTracking()
                        .Where(x => x.ForSaleCommercialListingId == listingId)
                        .Select(x => new { x.PropertyName, x.PropertyStatus })
                        .FirstOrDefaultAsync();
                    return item != null ? (item.PropertyName, item.PropertyStatus) : (null, null);
                }
                case 7:
                {
                    var item = await _context.rentalLandListings.AsNoTracking()
                        .Where(x => x.RentalLandListingId == listingId)
                        .Select(x => new { x.PropertyName, x.PropertyStatus })
                        .FirstOrDefaultAsync();
                    return item != null ? (item.PropertyName, item.PropertyStatus) : (null, null);
                }
                case 8:
                {
                    var item = await _context.rentalHousingListings.AsNoTracking()
                        .Where(x => x.RentalHousingListId == listingId)
                        .Select(x => new { x.PropertyName, x.PropertyStatus })
                        .FirstOrDefaultAsync();
                    return item != null ? (item.PropertyName, item.PropertyStatus) : (null, null);
                }
                case 9:
                {
                    var item = await _context.rentalCommercialPropertyListings.AsNoTracking()
                        .Where(x => x.RentalCommercialListId == listingId)
                        .Select(x => new { x.PropertyName, x.PropertyStatus })
                        .FirstOrDefaultAsync();
                    return item != null ? (item.PropertyName, item.PropertyStatus) : (null, null);
                }
                default:
                    return (null, null);
            }
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

        #endregion
    }
}
