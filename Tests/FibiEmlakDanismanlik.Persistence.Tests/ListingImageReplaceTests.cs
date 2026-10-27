using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Domain.Entities;
using FibiEmlakDanismanlik.Domain.Enums;
using FibiEmlakDanismanlik.Persistence.Repositories.ListingImageRepositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FibiEmlakDanismanlik.Persistence.Tests;

[Collection("LocalDb")]
public class ListingImageReplaceTests
{
    private readonly LocalDbFixture _fixture;
    private static int _seed = 9_000_000 + Random.Shared.Next(1, 500_000);

    public ListingImageReplaceTests(LocalDbFixture fixture)
    {
        _fixture = fixture;
    }

    private static int NextListingId() => Interlocked.Increment(ref _seed);

    [Fact]
    public async Task Replace_KeepsMatchingRows_RemovesOthers_AndOrders()
    {
        const int listingTypeId = 2; // satılık konut
        var listingId = NextListingId();

        int originalAId;
        var urlA = $"http://example.com/test_{listingId}_a.jpg";
        var urlB = $"http://example.com/test_{listingId}_b.jpg";
        var urlC = $"http://example.com/test_{listingId}_c.jpg";
        var urlD = $"http://example.com/test_{listingId}_d.jpg";
        var urlPlan = $"http://example.com/test_{listingId}_plan.jpg";

        // 1. Seed: 3 fotoğraf (A, B, C) ve 1 kat planı (P)
        using (var context = _fixture.CreateContext())
        {
            var photoA = new ListingImage
            {
                ListingTypeId = listingTypeId,
                ListingId = listingId,
                Url = urlA,
                StoragePath = "x/a.webp",
                SortOrder = 1,
                IsCover = true,
                Kind = ListingImageKind.Photo
            };
            var photoB = new ListingImage
            {
                ListingTypeId = listingTypeId,
                ListingId = listingId,
                Url = urlB,
                SortOrder = 2,
                IsCover = false,
                Kind = ListingImageKind.Photo
            };
            var photoC = new ListingImage
            {
                ListingTypeId = listingTypeId,
                ListingId = listingId,
                Url = urlC,
                SortOrder = 3,
                IsCover = false,
                Kind = ListingImageKind.Photo
            };
            var planP = new ListingImage
            {
                ListingTypeId = listingTypeId,
                ListingId = listingId,
                Url = urlPlan,
                SortOrder = 10,
                IsCover = false,
                Kind = ListingImageKind.FloorPlan
            };

            context.ListingImages.AddRange(photoA, photoB, photoC, planP);
            await context.SaveChangesAsync();

            originalAId = photoA.ListingImageId;
        }

        // 2. ReplacePhotoUrlsAsync çağır: C.Url, null, "  ", D yeni adres, A.Url, C.Url (tekrar)
        using (var context = _fixture.CreateContext())
        {
            var repo = new ListingImageRepository(context);
            await repo.ReplacePhotoUrlsAsync(listingTypeId, listingId, new[]
            {
                urlC,
                null,
                "  ",
                urlD,
                urlA,
                urlC
            });
        }

        // 3. Yeni context ile doğrulama
        using (var context = _fixture.CreateContext())
        {
            var allImages = await context.ListingImages
                .Where(x => x.ListingTypeId == listingTypeId && x.ListingId == listingId)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();

            var photos = allImages.Where(x => x.Kind == ListingImageKind.Photo).ToList();
            Assert.Equal(3, photos.Count);

            // C: SortOrder 1, IsCover true
            Assert.Equal(urlC, photos[0].Url);
            Assert.Equal(1, photos[0].SortOrder);
            Assert.True(photos[0].IsCover);

            // D: SortOrder 2, IsCover false (yeni adres)
            Assert.Equal(urlD, photos[1].Url);
            Assert.Equal(2, photos[1].SortOrder);
            Assert.False(photos[1].IsCover);

            // A: SortOrder 3, IsCover false (ListingImageId ve StoragePath korunmuş)
            Assert.Equal(urlA, photos[2].Url);
            Assert.Equal(3, photos[2].SortOrder);
            Assert.False(photos[2].IsCover);
            Assert.Equal(originalAId, photos[2].ListingImageId);
            Assert.Equal("x/a.webp", photos[2].StoragePath);

            // B silinmiş olmalı
            Assert.DoesNotContain(allImages, x => x.Url == urlB);

            // Plan P aynen duruyor olmalı (SortOrder 10)
            var plan = Assert.Single(allImages, x => x.Kind == ListingImageKind.FloorPlan);
            Assert.Equal(urlPlan, plan.Url);
            Assert.Equal(10, plan.SortOrder);
        }
    }

    [Fact]
    public async Task Replace_WithEmptyList_RemovesAllPhotos_KeepsPlans()
    {
        const int listingTypeId = 2; // satılık konut
        var listingId = NextListingId();

        var url1 = $"http://example.com/test_{listingId}_1.jpg";
        var url2 = $"http://example.com/test_{listingId}_2.jpg";
        var urlPlan = $"http://example.com/test_{listingId}_plan.jpg";

        // 1. Seed: 2 fotoğraf + 1 plan
        using (var context = _fixture.CreateContext())
        {
            var photo1 = new ListingImage
            {
                ListingTypeId = listingTypeId,
                ListingId = listingId,
                Url = url1,
                SortOrder = 1,
                IsCover = true,
                Kind = ListingImageKind.Photo
            };
            var photo2 = new ListingImage
            {
                ListingTypeId = listingTypeId,
                ListingId = listingId,
                Url = url2,
                SortOrder = 2,
                IsCover = false,
                Kind = ListingImageKind.Photo
            };
            var plan = new ListingImage
            {
                ListingTypeId = listingTypeId,
                ListingId = listingId,
                Url = urlPlan,
                SortOrder = 5,
                IsCover = false,
                Kind = ListingImageKind.FloorPlan
            };

            context.ListingImages.AddRange(photo1, photo2, plan);
            await context.SaveChangesAsync();
        }

        // 2. Boş liste ile çağır
        using (var context = _fixture.CreateContext())
        {
            var repo = new ListingImageRepository(context);
            await repo.ReplacePhotoUrlsAsync(listingTypeId, listingId, Array.Empty<string>());
        }

        // 3. Yeni context ile doğrulama: fotoğraf kalmaz, plan kalır
        using (var context = _fixture.CreateContext())
        {
            var allImages = await context.ListingImages
                .Where(x => x.ListingTypeId == listingTypeId && x.ListingId == listingId)
                .ToListAsync();

            Assert.Empty(allImages.Where(x => x.Kind == ListingImageKind.Photo));

            var plan = Assert.Single(allImages);
            Assert.Equal(ListingImageKind.FloorPlan, plan.Kind);
            Assert.Equal(urlPlan, plan.Url);
            Assert.Equal(5, plan.SortOrder);
        }
    }

    [Fact]
    public async Task GetPhotoUrlMap_ReturnsLegacyOrder()
    {
        const int listingTypeId = 2; // satılık konut
        var listingId = NextListingId();

        var url1 = $"http://example.com/test_{listingId}_1.jpg";
        var url2 = $"http://example.com/test_{listingId}_2.jpg";
        var url3 = $"http://example.com/test_{listingId}_3.jpg";
        var urlPlan = $"http://example.com/test_{listingId}_plan.jpg";

        // 1. Seed: 3 fotoğraf:
        //    Photo 1: SortOrder 1, IsCover false
        //    Photo 2: SortOrder 2, IsCover true
        //    Photo 3: SortOrder 2, IsCover false (aynı sıra, daha büyük Id)
        //    ve bir plan
        using (var context = _fixture.CreateContext())
        {
            var photo1 = new ListingImage
            {
                ListingTypeId = listingTypeId,
                ListingId = listingId,
                Url = url1,
                SortOrder = 1,
                IsCover = false,
                Kind = ListingImageKind.Photo
            };
            var photo2 = new ListingImage
            {
                ListingTypeId = listingTypeId,
                ListingId = listingId,
                Url = url2,
                SortOrder = 2,
                IsCover = true,
                Kind = ListingImageKind.Photo
            };
            var photo3 = new ListingImage
            {
                ListingTypeId = listingTypeId,
                ListingId = listingId,
                Url = url3,
                SortOrder = 2,
                IsCover = false,
                Kind = ListingImageKind.Photo
            };
            var plan = new ListingImage
            {
                ListingTypeId = listingTypeId,
                ListingId = listingId,
                Url = urlPlan,
                SortOrder = 1,
                IsCover = false,
                Kind = ListingImageKind.FloorPlan
            };

            context.ListingImages.AddRange(photo1, photo2, photo3, plan);
            await context.SaveChangesAsync();
        }

        // 2. GetPhotoUrlMapAsync(new[] { (2, id), (2, id) }) çağır
        Dictionary<(int ListingTypeId, int ListingId), List<string>> map;
        using (var context = _fixture.CreateContext())
        {
            var repo = new ListingImageRepository(context);
            map = await repo.GetPhotoUrlMapAsync(new[]
            {
                (listingTypeId, listingId),
                (listingTypeId, listingId)
            });
        }

        // 3. Doğrulama: tek anahtar, sıra: kapak (photo2), sonra SortOrder 1 (photo1), sonra kalan (photo3); plan yok
        Assert.Single(map);
        Assert.True(map.TryGetValue((listingTypeId, listingId), out var urls));
        Assert.NotNull(urls);
        Assert.Equal(3, urls.Count);

        Assert.Equal(url2, urls[0]); // kapak önce
        Assert.Equal(url1, urls[1]); // SortOrder 1
        Assert.Equal(url3, urls[2]); // SortOrder 2, kalan (daha büyük Id)
        Assert.DoesNotContain(urlPlan, urls); // plan listede yok
    }
}
