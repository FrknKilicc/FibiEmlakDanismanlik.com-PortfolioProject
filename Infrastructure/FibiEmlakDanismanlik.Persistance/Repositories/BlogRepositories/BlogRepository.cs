using FibiEmlakDanismanlik.Application.Features.Results.BlogResults;
using FibiEmlakDanismanlik.Application.Interfaces.BlogInterfaces;
using FibiEmlakDanismanlik.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Persistence.Repositories.BlogRepositories
{
    public class BlogRepository : IBlogRepository
    {
        private readonly FibiEmlakDanismanlikContext _context;

        public BlogRepository(FibiEmlakDanismanlikContext context)
        {
            _context = context;
        }

        public async Task<GetBlogDetailWithAuthorResult?> GetBlogDetailWithAuthorById(int blogId)
        {
            var blog = await _context.Blogs.Include(b => b.Author).FirstOrDefaultAsync(b => b.BlogId == blogId);

            if (blog == null)
            {
                return null;

            }

            return new GetBlogDetailWithAuthorResult
            {
                BlogId = blogId,
                BlogTitle = blog.BlogTitle,
                BlogDescription = blog.BlogDescription,
                BlogImgUrl = blog.BlogImgUrl,
                CreatedDate = blog.CreatedDate,
                TopNews = blog.TopNews,

                AuthorId = blog.AuthorId,
                AuthorImgUrl = blog.Author.AuthorImgUrl,
                AuthorName = blog.Author.AuthorName,

            };

        }

        public Task<List<GetBlogDetailWithAuthorResult?>> GetBlogListWithAuthor()
        {
            var value = _context.Blogs.Include(_b => _b.Author).Select(y=> new GetBlogDetailWithAuthorResult
            {
                BlogId = y.BlogId,
                BlogTitle = y.BlogTitle,
                BlogDescription = y.BlogDescription,
                BlogImgUrl = y.BlogImgUrl,
                CreatedDate = y.CreatedDate,
                TopNews = y.TopNews,
                AuthorId = y.AuthorId,
                AuthorImgUrl= y.Author.AuthorImgUrl,
                AuthorName= y.Author.AuthorName,
            }).ToListAsync();
            return value;
        }

        public Task<List<GetBlogDetailWithAuthorResult>> GetLast3BlogWithAuthor()
        {
            var value = _context.Blogs.Include(_b => _b.Author).OrderByDescending(x=>x.CreatedDate).Take(3).Select(x=> new GetBlogDetailWithAuthorResult
            {
                BlogId = x.BlogId,
                BlogTitle=x.BlogTitle,
                BlogDescription= x.BlogDescription,
                BlogImgUrl = x.BlogImgUrl,
                CreatedDate = x.CreatedDate,
                TopNews = x.TopNews,
                AuthorId = x.AuthorId,
                AuthorImgUrl= x.Author.AuthorImgUrl,
                AuthorName = x.Author.AuthorName,
    
            }).ToListAsync();

            return value;
        }
    }
}
