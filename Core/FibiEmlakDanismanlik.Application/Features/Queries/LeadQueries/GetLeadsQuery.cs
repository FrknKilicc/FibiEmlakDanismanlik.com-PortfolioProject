using System;
using FibiEmlakDanismanlik.Application.Features.Results.LeadResults;
using FibiEmlakDanismanlik.Application.Interfaces.LeadInterfaces;
using FibiEmlakDanismanlik.Domain.Enums;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Queries.LeadQueries
{
    /// <summary>
    /// Yönetim paneli müşteri taleplerini filtreleme ve sayfalamalı listeleme sorgusu.
    /// </summary>
    public class GetLeadsQuery : LeadFilter, IRequest<GetLeadsResult>
    {
    }
}
