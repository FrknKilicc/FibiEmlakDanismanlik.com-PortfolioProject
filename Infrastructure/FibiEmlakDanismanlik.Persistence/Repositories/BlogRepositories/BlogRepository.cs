using FibiEmlakDanismanlik.Application.Features.Results.BlogResults;
using FibiEmlakDanismanlik.Application.Interfaces.BlogInterfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using FibiEmlakDanismanlik.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
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
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedDate)
                .ThenByDescending(x => x.BlogId)
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
                    BlogCategoryName = x.BlogCategory != null ? x.BlogCategory.CategoryName : null,

                    Tags = x.BlogTags.Select(t => new BlogTagItemResult
                    {
                        Id = t.TagId,
                        Name = t.Tag.Name
                    }).ToList()
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

        public Task<Dictionary<int, int>> GetBlogCountsByAuthorAsync()
        {
            return _context.Blogs
                .GroupBy(b => b.AuthorId)
                .Select(g => new { AuthorId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.AuthorId, x => x.Count);
        }

        public Task<Dictionary<int, int>> GetBlogCountsByCategoryAsync()
        {
            return _context.Blogs
                .Where(b => b.BlogCategoryId.HasValue)
                .GroupBy(b => b.BlogCategoryId!.Value)
                .Select(g => new { CategoryId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.CategoryId, x => x.Count);
        }

        public Task<List<string>> GetTagNamesAsync(int blogId)
        {
            return _context.BlogTags
                .Where(bt => bt.BlogId == blogId)
                .Select(bt => bt.Tag.Name)
                .OrderBy(name => name)
                .ToListAsync();
        }

        public Task<List<string>> GetAllTagNamesAsync()
        {
            return _context.Tags
                .Select(t => t.Name)
                .OrderBy(name => name)
                .ToListAsync();
        }

        public async Task SetBlogTagsAsync(int blogId, IReadOnlyCollection<string> tagNames)
        {
            var trCulture = new CultureInfo("tr-TR");
            var trComparer = StringComparer.Create(trCulture, true);

            var targetNames = (tagNames ?? Array.Empty<string>())
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Distinct(trComparer)
                .ToList();

            var existingTags = targetNames.Count > 0
                ? await _context.Tags.Where(t => targetNames.Contains(t.Name)).ToListAsync()
                : new List<Tag>();

            var existingNames = new HashSet<string>(existingTags.Select(t => t.Name), trComparer);
            var newTags = targetNames
                .Where(name => !existingNames.Contains(name))
                .Select(name => new Tag { Name = name })
                .ToList();

            if (newTags.Count > 0)
            {
                await _context.Tags.AddRangeAsync(newTags);
                await _context.SaveChangesAsync();
            }

            var allTargetTags = existingTags.Concat(newTags).ToList();
            var targetTagIds = allTargetTags.Select(t => t.Id).ToHashSet();

            var currentBlogTags = await _context.BlogTags
                .Where(bt => bt.BlogId == blogId)
                .ToListAsync();

            var currentTagIds = currentBlogTags.Select(bt => bt.TagId).ToHashSet();

            var toRemove = currentBlogTags
                .Where(bt => !targetTagIds.Contains(bt.TagId))
                .ToList();

            var toAdd = targetTagIds
                .Where(tagId => !currentTagIds.Contains(tagId))
                .Select(tagId => new BlogTag { BlogId = blogId, TagId = tagId })
                .ToList();

            if (toRemove.Count > 0)
            {
                _context.BlogTags.RemoveRange(toRemove);
            }

            if (toAdd.Count > 0)
            {
                await _context.BlogTags.AddRangeAsync(toAdd);
            }

            if (toRemove.Count > 0 || toAdd.Count > 0)
            {
                await _context.SaveChangesAsync();
            }

            if (toRemove.Count > 0)
            {
                var removedTagIds = toRemove.Select(bt => bt.TagId).Distinct().ToList();
                var orphanTags = await _context.Tags
                    .Where(t => removedTagIds.Contains(t.Id) && !_context.BlogTags.Any(bt => bt.TagId == t.Id))
                    .ToListAsync();

                if (orphanTags.Count > 0)
                {
                    _context.Tags.RemoveRange(orphanTags);
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task DetachDeletedBlogsFromCategoryAsync(int blogCategoryId)
        {
            var deletedBlogs = await _context.Blogs
                .IgnoreQueryFilters()
                .Where(b => b.BlogCategoryId == blogCategoryId && b.DeletedAtUtc != null)
                .ToListAsync();

            if (deletedBlogs.Count > 0)
            {
                foreach (var blog in deletedBlogs)
                {
                    blog.BlogCategoryId = null;
                }

                await _context.SaveChangesAsync();
            }
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
