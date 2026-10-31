using FibiEmlakDanismanlik.Application.Features.Results.ListingAlertResults;
using FibiEmlakDanismanlik.Application.Interfaces.ListingAlertInterfaces;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Queries.ListingAlertQueries
{
    /// <summary>
    /// Yönetim paneli haber bekleyenleri filtreleme ve sayfalamalı listeleme sorgusu.
    /// </summary>
    public class GetListingAlertsQuery : ListingAlertFilter, IRequest<GetListingAlertsResult>
    {
    }
}
