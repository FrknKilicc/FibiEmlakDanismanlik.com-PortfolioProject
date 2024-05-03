using FibiEmlakDanismanlik.Application.Features.Queries.BlogQueries;
using FibiEmlakDanismanlik.Application.Features.Results.BlogResults;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.BlogHandlers
{
    public class GetBlogQueryHandler : IRequestHandler<GetBlogQuery, List<GetBlogResult>>
    {
        private readonly IRepository<Blog> _repository;

        public GetBlogQueryHandler(IRepository<Blog> repository)
        {
            _repository = repository;
        }
        

        public async Task<List<GetBlogResult>> Handle(GetBlogQuery request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetAllAsync();
            return value .Select(x=>new GetBlogResult
            {
                BlogId = x.BlogId,
                BlogTitle = x.BlogTitle,
                BlogDescription = x.BlogDescription,
                BlogImgUrl = x.BlogImgUrl,
                AuthorId = x.AuthorId,
                CreatedDate = x.CreatedDate,
                TopNews = x.TopNews 
                
            }) .ToList();
   
        }
    }
}
