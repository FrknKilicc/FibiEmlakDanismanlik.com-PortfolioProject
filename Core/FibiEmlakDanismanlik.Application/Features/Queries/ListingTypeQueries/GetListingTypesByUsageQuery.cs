using FibiEmlakDanismanlik.Application.Features.Results.ListingTypeResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Queries.ListingTypeQueries
{
    public record GetListingTypesByUsageQuery(int UsageType) : IRequest<List<ListingTypeResult>>;
}

