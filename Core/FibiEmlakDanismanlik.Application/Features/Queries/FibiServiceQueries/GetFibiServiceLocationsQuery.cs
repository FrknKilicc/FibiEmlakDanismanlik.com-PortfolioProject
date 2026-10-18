using FibiEmlakDanismanlik.Application.Features.Results.FibiServiceResults;
using MediatR;
using System.Collections.Generic;

namespace FibiEmlakDanismanlik.Application.Features.Queries.FibiServiceQueries
{
    /// <summary>Emsal verisi bulunan mahalleleri hiyerarşik olarak getiren sorgu.</summary>
    public record GetFibiServiceLocationsQuery() : IRequest<List<GetFibiServiceLocationResult>>;
}
