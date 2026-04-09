using FibiEmlakDanismanlik.Application.Features.Results.BlogResults;
using FibiEmlakDanismanlik.Application.Interfaces.BlogInterfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using FibiEmlakDanismanlik.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            var blog = await _context.Blogs
                .Include(b => b.Author)
                .Include(b => b.BlogCategory)
                .Include(b => b.BlogTags)
                    .ThenInclude(bt => bt.Tag)
                .FirstOrDefaultAsync(b => b.BlogId == blogId);

            if (blog == null)
            {
                return null;
            }

            return new GetBlogDetailWithAuthorResult
            {
                BlogId = blog.BlogId,
                BlogTitle = blog.BlogTitle,
                BlogDescription = blog.BlogDescription,
                BlogImgUrl = blog.BlogImgUrl,
                CreatedDate = blog.CreatedDate,
                TopNews = blog.TopNews,

                AuthorId = blog.AuthorId,
                AuthorImgUrl = blog.Author.AuthorImgUrl,
                AuthorName = blog.Author.AuthorName,

                BlogCategoryId = blog.BlogCategoryId,
                BlogCategoryName = blog.BlogCategory.CategoryName,

                Tags = blog.BlogTags.Select(x => new BlogTagItemResult
                {
                    Id = x.TagId,
                    Name = x.Tag.Name
                }).ToList()
            };
        }

        public Task<List<GetBlogDetailWithAuthorResult?>> GetBlogListWithAuthor(int? blogCategoryId = null)
        {
            var query = _context.Blogs
                .Include(x => x.Author)
                .Include(x => x.BlogCategory)
                .AsQueryable();

            if (blogCategoryId.HasValue)
            {
                query = query.Where(x => x.BlogCategoryId == blogCategoryId.Value);
            }

            return query
                .OrderByDescending(x => x.CreatedDate)
                .Select(x => new GetBlogDetailWithAuthorResult
                {
                    BlogId = x.BlogId,
                    BlogTitle = x.BlogTitle,
                    BlogDescription = x.BlogDescription,
                    BlogImgUrl = x.BlogImgUrl,
                    CreatedDate = x.CreatedDate,
                    TopNews = x.TopNews,

                    AuthorId = x.AuthorId,
                    AuthorImgUrl = x.Author.AuthorImgUrl,
                    AuthorName = x.Author.AuthorName,

                    BlogCategoryId = x.BlogCategoryId,
                    BlogCategoryName = x.BlogCategory.CategoryName
                })
                .ToListAsync()!;
        }

        public Task<List<GetBlogDetailWithAuthorResult?>> GetLast3BlogWithAuthor()
        {
            return _context.Blogs
                .Include(x => x.Author)
                .Include(x => x.BlogCategory)
                .OrderByDescending(x => x.CreatedDate)
                .Take(3)
                .Select(x => new GetBlogDetailWithAuthorResult
                {
                    BlogId = x.BlogId,
                    BlogTitle = x.BlogTitle,
                    BlogDescription = x.BlogDescription,
                    BlogImgUrl = x.BlogImgUrl,
                    CreatedDate = x.CreatedDate,
                    TopNews = x.TopNews,

                    AuthorId = x.AuthorId,
                    AuthorImgUrl = x.Author.AuthorImgUrl,
                    AuthorName = x.Author.AuthorName,

                    BlogCategoryId = x.BlogCategoryId,
                    BlogCategoryName = x.BlogCategory.CategoryName
                })
                .ToListAsync()!;
        }

        public Task<List<BlogCategoryCountResult>> GetTopBlogCategories(int take = 5)
        {
            if (take <= 0)
                take = 5;

            return _context.Blogs
                .Include(x => x.BlogCategory)
                .GroupBy(x => new { x.BlogCategoryId, x.BlogCategory.CategoryName })
                .Select(g => new BlogCategoryCountResult
                {
                    BlogCategoryId = g.Key.BlogCategoryId,
                    CategoryName = g.Key.CategoryName,
                    BlogCount = g.Count()
                })
                .OrderByDescending(x => x.BlogCount)
                .ThenBy(x => x.CategoryName)
                .Take(take)
                .ToListAsync();
        }

        public async Task<List<BlogSuggestionResult>> SearchBlogSuggestion(string q, int take = 6)
        {
            if (string.IsNullOrWhiteSpace(q))
                return new List<BlogSuggestionResult>();

            q = q.Trim();

            if (q.Length < 2)
                return new List<BlogSuggestionResult>();

            if (take <= 0) take = 6;
            if (take > 10) take = 10;

            var list = await _context.Blogs
                .OrderByDescending(b => b.CreatedDate)
                .Where(b =>
                    EF.Functions.Like(b.BlogTitle, $"%{q}%") ||
                    EF.Functions.Like(b.BlogDescription, $"%{q}%"))
                .Select(b => new BlogSuggestionResult
                {
                    BlogTitle = b.BlogTitle,
                    BlogImgUrl = b.BlogImgUrl,
                    BlogId = b.BlogId,
                    BlogDescPrew = b.BlogDescription
                })
                .Take(take)
                .ToListAsync();

            foreach (var item in list)
            {
                var plain = StripHtml(item.BlogDescPrew ?? "");
                item.BlogDescPrew = TruncateWithDots(plain, 50);
            }

            return list;
        }

        private static string TruncateWithDots(string text, int maxLen)
        {
            if (string.IsNullOrEmpty(text)) return "";
            if (text.Length <= maxLen) return text;
            return text.Substring(0, maxLen) + "...";
        }

        private static string StripHtml(string input)
        {
            return Regex.Replace(input, "<.*?>", string.Empty);
        }
    }
    }
