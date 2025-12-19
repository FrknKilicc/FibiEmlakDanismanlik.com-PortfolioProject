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
    }
}
