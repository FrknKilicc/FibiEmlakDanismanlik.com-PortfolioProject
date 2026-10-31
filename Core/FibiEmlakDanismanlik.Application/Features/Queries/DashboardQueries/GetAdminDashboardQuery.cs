using System;
using FibiEmlakDanismanlik.Application.Features.Results.DashboardResults;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Queries.DashboardQueries
{
    /// <summary>
    /// Yönetim paneli kontrol paneli (dashboard) verilerini getiren sorgu.
    /// </summary>
    public class GetAdminDashboardQuery : IRequest<GetAdminDashboardResult>
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}
