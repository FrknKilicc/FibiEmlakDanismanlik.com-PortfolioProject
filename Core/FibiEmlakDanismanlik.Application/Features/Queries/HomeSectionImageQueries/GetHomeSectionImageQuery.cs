using FibiEmlakDanismanlik.Application.Features.Results.HomeSectionImageResults;
using MediatR;
using System.Collections.Generic;

namespace FibiEmlakDanismanlik.Application.Features.Queries.HomeSectionImageQueries
{
    public class GetHomeSectionImageQuery : IRequest<List<GetHomeSectionImageResult>>
    {
        public string? SectionKey { get; set; }

        public GetHomeSectionImageQuery(string? sectionKey = null)
        {
            SectionKey = sectionKey;
        }
    }
}
