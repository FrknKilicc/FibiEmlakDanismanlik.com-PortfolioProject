using System;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Domain.Entities;
using FibiEmlakDanismanlik.Persistence.Repositories.BlogRepositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FibiEmlakDanismanlik.Persistence.Tests;

[Collection("LocalDb")]
public class SoftDeleteTests
{
    private readonly LocalDbFixture _fixture;

    public SoftDeleteTests(LocalDbFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Remove_SetsDeletedAtUtc_AndHidesFromQueries()
    {
        int blogId;

        // 1. Seed: yazar + blog ekle
        using (var context = _fixture.CreateContext())
        {
            var author = new Author
            {
                AuthorName = "Test Yazar 1",
                AuthorImgUrl = "http://example.com/author1.jpg"
            };
            context.Authors.Add(author);
            await context.SaveChangesAsync();

            var blog = new Blog
            {
                BlogTitle = "Test Blog 1",
                BlogDescription = "Açıklama 1",
                BlogImgUrl = "http://example.com/blog1.jpg",
                CreatedDate = DateTime.UtcNow,
                TopNews = false,
                AuthorId = author.AuthorId
            };
            context.Blogs.Add(blog);
            await context.SaveChangesAsync();

            blogId = blog.BlogId;
        }

        // 2. Yeni context'te blogu bul, Remove, SaveChangesAsync
        using (var context = _fixture.CreateContext())
        {
            var blog = await context.Blogs.FirstOrDefaultAsync(b => b.BlogId == blogId);
            Assert.NotNull(blog);

            context.Blogs.Remove(blog);
            await context.SaveChangesAsync();
        }

        // 3. Yeni context'te doğrulama: global filter aktifken null, IgnoreQueryFilters() ile DeletedAtUtc dolu
        using (var context = _fixture.CreateContext())
        {
            var activeBlog = await context.Blogs.FirstOrDefaultAsync(b => b.BlogId == blogId);
            Assert.Null(activeBlog);

            var softDeletedBlog = await context.Blogs.IgnoreQueryFilters().FirstOrDefaultAsync(b => b.BlogId == blogId);
            Assert.NotNull(softDeletedBlog);
            Assert.NotNull(softDeletedBlog.DeletedAtUtc);
        }
    }

    [Fact]
    public async Task AllowHardDelete_RemovesRowPhysically()
    {
        int blogId;

        // 1. Seed: yazar + blog ekle
        using (var context = _fixture.CreateContext())
        {
            var author = new Author
            {
                AuthorName = "Test Yazar 2",
                AuthorImgUrl = "http://example.com/author2.jpg"
            };
            context.Authors.Add(author);
            await context.SaveChangesAsync();

            var blog = new Blog
            {
                BlogTitle = "Test Blog 2",
                BlogDescription = "Açıklama 2",
                BlogImgUrl = "http://example.com/blog2.jpg",
                CreatedDate = DateTime.UtcNow,
                TopNews = false,
                AuthorId = author.AuthorId
            };
            context.Blogs.Add(blog);
            await context.SaveChangesAsync();

            blogId = blog.BlogId;
        }

        // 2. Yeni context'te AllowHardDelete = true ile Remove, SaveChangesAsync
        using (var context = _fixture.CreateContext())
        {
            var blog = await context.Blogs.FirstOrDefaultAsync(b => b.BlogId == blogId);
            Assert.NotNull(blog);

            context.AllowHardDelete = true;
            context.Blogs.Remove(blog);
            await context.SaveChangesAsync();
        }

        // 3. Yeni context'te IgnoreQueryFilters() ile de fiziksel olarak bulunmamalı
        using (var context = _fixture.CreateContext())
        {
            var deletedBlog = await context.Blogs.IgnoreQueryFilters().FirstOrDefaultAsync(b => b.BlogId == blogId);
            Assert.Null(deletedBlog);
        }
    }

    [Fact]
    public async Task DetachDeletedBlogsFromCategory_AllowsCategoryRemoval()
    {
        int categoryId;
        int blogId;

        // 1. Seed: kategori + yazar + kategorili blog ekle
        using (var context = _fixture.CreateContext())
        {
            var category = new BlogCategory
            {
                CategoryName = $"Kategori_{Guid.NewGuid():N}"
            };
            context.BlogCategories.Add(category);

            var author = new Author
            {
                AuthorName = "Test Yazar 3",
                AuthorImgUrl = "http://example.com/author3.jpg"
            };
            context.Authors.Add(author);
            await context.SaveChangesAsync();

            var blog = new Blog
            {
                BlogTitle = "Test Blog 3",
                BlogDescription = "Açıklama 3",
                BlogImgUrl = "http://example.com/blog3.jpg",
                CreatedDate = DateTime.UtcNow,
                TopNews = false,
                AuthorId = author.AuthorId,
                BlogCategoryId = category.BlogCategoryId
            };
            context.Blogs.Add(blog);
            await context.SaveChangesAsync();

            categoryId = category.BlogCategoryId;
            blogId = blog.BlogId;
        }

        // 2. Blogu soft delete et
        using (var context = _fixture.CreateContext())
        {
            var blog = await context.Blogs.FirstOrDefaultAsync(b => b.BlogId == blogId);
            Assert.NotNull(blog);

            context.Blogs.Remove(blog);
            await context.SaveChangesAsync();
        }

        // 3. DetachDeletedBlogsFromCategoryAsync çağır ve kategoriyi Remove et (FK kısıtına takılmamalı)
        using (var context = _fixture.CreateContext())
        {
            var repo = new BlogRepository(context);
            await repo.DetachDeletedBlogsFromCategoryAsync(categoryId);

            var category = await context.BlogCategories.FirstOrDefaultAsync(c => c.BlogCategoryId == categoryId);
            Assert.NotNull(category);

            context.BlogCategories.Remove(category);
            await context.SaveChangesAsync();
        }

        // 4. Doğrulama: kategori silindi ve soft delete edilmiş blogun BlogCategoryId'si null oldu
        using (var context = _fixture.CreateContext())
        {
            var deletedCategory = await context.BlogCategories.FirstOrDefaultAsync(c => c.BlogCategoryId == categoryId);
            Assert.Null(deletedCategory);

            var softDeletedBlog = await context.Blogs.IgnoreQueryFilters().FirstOrDefaultAsync(b => b.BlogId == blogId);
            Assert.NotNull(softDeletedBlog);
            Assert.Null(softDeletedBlog.BlogCategoryId);
        }
    }
}
