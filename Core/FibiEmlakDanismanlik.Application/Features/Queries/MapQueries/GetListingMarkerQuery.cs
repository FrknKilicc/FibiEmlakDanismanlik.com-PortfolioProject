
using FibiEmlakDanismanlik.Application.Features.Results.MapResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Queries.MapQueries
{
    public class GetListingMarkerQuery:IRequest<ListingMarkerResult>
    {
        public int ListingId { get; set; }
        public int ListingTypeId { get; set; }
    }
}
