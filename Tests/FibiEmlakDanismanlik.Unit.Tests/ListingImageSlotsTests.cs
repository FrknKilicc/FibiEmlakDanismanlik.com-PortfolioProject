using FibiEmlakDanismanlik.Application.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace FibiEmlakDanismanlik.Unit.Tests
{
    public class ListingImageSlotsTests
    {
        private sealed class TestThirtySlotsDto
        {
            public string? PropImgUrl1 { get; set; }
            public string? PropImgUrl2 { get; set; }
            public string? PropImgUrl3 { get; set; }
            public string? PropImgUrl4 { get; set; }
            public string? PropImgUrl5 { get; set; }
            public string? PropImgUrl6 { get; set; }
            public string? PropImgUrl7 { get; set; }
            public string? PropImgUrl8 { get; set; }
            public string? PropImgUrl9 { get; set; }
            public string? PropImgUrl10 { get; set; }
            public string? PropImgUrl11 { get; set; }
            public string? PropImgUrl12 { get; set; }
            public string? PropImgUrl13 { get; set; }
            public string? PropImgUrl14 { get; set; }
            public string? PropImgUrl15 { get; set; }
            public string? PropImgUrl16 { get; set; }
            public string? PropImgUrl17 { get; set; }
            public string? PropImgUrl18 { get; set; }
            public string? PropImgUrl19 { get; set; }
            public string? PropImgUrl20 { get; set; }
            public string? PropImgUrl21 { get; set; }
            public string? PropImgUrl22 { get; set; }
            public string? PropImgUrl23 { get; set; }
            public string? PropImgUrl24 { get; set; }
            public string? PropImgUrl25 { get; set; }
            public string? PropImgUrl26 { get; set; }
            public string? PropImgUrl27 { get; set; }
            public string? PropImgUrl28 { get; set; }
            public string? PropImgUrl29 { get; set; }
            public string? PropImgUrl30 { get; set; }
        }

        private sealed class TestSingleSlotDto
        {
            public string? PropImgUrl1 { get; set; }
        }

        [Fact]
        public void Apply_ThreeUrls_FillsSlotsOneToThreeAndLeavesRestNull()
        {
            // 3 adres -> slot1..3 dolu, slot4..30 null
            var target = new TestThirtySlotsDto
            {
                PropImgUrl4 = "eski-deger",
                PropImgUrl30 = "eski-deger-30"
            };
            var urls = new[]
            {
                "https://example.com/photo1.jpg",
                "https://example.com/photo2.jpg",
                "https://example.com/photo3.jpg"
            };

            ListingImageSlots.Apply(target, urls);

            Assert.Equal("https://example.com/photo1.jpg", target.PropImgUrl1);
            Assert.Equal("https://example.com/photo2.jpg", target.PropImgUrl2);
            Assert.Equal("https://example.com/photo3.jpg", target.PropImgUrl3);

            Assert.Null(target.PropImgUrl4);
            Assert.Null(target.PropImgUrl5);
            Assert.Null(target.PropImgUrl6);
            Assert.Null(target.PropImgUrl7);
            Assert.Null(target.PropImgUrl8);
            Assert.Null(target.PropImgUrl9);
            Assert.Null(target.PropImgUrl10);
            Assert.Null(target.PropImgUrl11);
            Assert.Null(target.PropImgUrl12);
            Assert.Null(target.PropImgUrl13);
            Assert.Null(target.PropImgUrl14);
            Assert.Null(target.PropImgUrl15);
            Assert.Null(target.PropImgUrl16);
            Assert.Null(target.PropImgUrl17);
            Assert.Null(target.PropImgUrl18);
            Assert.Null(target.PropImgUrl19);
            Assert.Null(target.PropImgUrl20);
            Assert.Null(target.PropImgUrl21);
            Assert.Null(target.PropImgUrl22);
            Assert.Null(target.PropImgUrl23);
            Assert.Null(target.PropImgUrl24);
            Assert.Null(target.PropImgUrl25);
            Assert.Null(target.PropImgUrl26);
            Assert.Null(target.PropImgUrl27);
            Assert.Null(target.PropImgUrl28);
            Assert.Null(target.PropImgUrl29);
            Assert.Null(target.PropImgUrl30);
        }

        [Fact]
        public void Apply_NullList_SetsSlotOneToEmptyStringAndRestToNull()
        {
            // null liste -> slot1 "" ve diğerleri null
            var target = new TestThirtySlotsDto
            {
                PropImgUrl1 = "önceki",
                PropImgUrl2 = "önceki2"
            };

            ListingImageSlots.Apply(target, null);

            Assert.Equal(string.Empty, target.PropImgUrl1);
            Assert.Null(target.PropImgUrl2);
            Assert.Null(target.PropImgUrl3);
            Assert.Null(target.PropImgUrl4);
            Assert.Null(target.PropImgUrl30);
        }

        [Fact]
        public void Apply_EmptyList_SetsSlotOneToEmptyStringAndRestToNull()
        {
            // boş liste -> slot1 "" ve diğerleri null
            var target = new TestThirtySlotsDto
            {
                PropImgUrl1 = "önceki",
                PropImgUrl2 = "önceki2"
            };

            ListingImageSlots.Apply(target, Array.Empty<string>());

            Assert.Equal(string.Empty, target.PropImgUrl1);
            Assert.Null(target.PropImgUrl2);
            Assert.Null(target.PropImgUrl3);
            Assert.Null(target.PropImgUrl4);
            Assert.Null(target.PropImgUrl30);
        }

        [Fact]
        public void Apply_ThirtyFiveUrls_SetsOnlyThirtySlots()
        {
            // 35 adres -> yalnızca 30 slot
            var target = new TestThirtySlotsDto();
            var urls = Enumerable.Range(1, 35)
                .Select(i => $"https://example.com/img{i}.jpg")
                .ToList();

            ListingImageSlots.Apply(target, urls);

            Assert.Equal("https://example.com/img1.jpg", target.PropImgUrl1);
            Assert.Equal("https://example.com/img2.jpg", target.PropImgUrl2);
            Assert.Equal("https://example.com/img15.jpg", target.PropImgUrl15);
            Assert.Equal("https://example.com/img30.jpg", target.PropImgUrl30);
        }

        [Fact]
        public void Apply_SingleSlotDto_FillsSlotOneWithoutError()
        {
            // yalnızca PropImgUrl1 olan sınıfta hata vermeden slot1 dolar
            var target = new TestSingleSlotDto();
            var urls = new[]
            {
                "https://example.com/cover.jpg",
                "https://example.com/extra.jpg"
            };

            ListingImageSlots.Apply(target, urls);

            Assert.Equal("https://example.com/cover.jpg", target.PropImgUrl1);
        }

        [Fact]
        public void Apply_SingleSlotDto_EmptyUrls_SetsEmptyStringWithoutError()
        {
            // yalnızca PropImgUrl1 olan sınıfta boş listede slot1 "" olur
            var target = new TestSingleSlotDto { PropImgUrl1 = "önceki" };

            ListingImageSlots.Apply(target, Array.Empty<string>());

            Assert.Equal(string.Empty, target.PropImgUrl1);
        }

        [Fact]
        public void Apply_NullTarget_ThrowsArgumentNullException()
        {
            // null hedef -> ArgumentNullException
            Assert.Throws<ArgumentNullException>(() => ListingImageSlots.Apply(null!, new[] { "https://example.com/photo.jpg" }));
            Assert.Throws<ArgumentNullException>(() => ListingImageSlots.Apply(null!, null));
        }

        [Fact]
        public void ToList_NullUrls_ReturnsEmptyList()
        {
            var result = ListingImageSlots.ToList(null);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void ToList_ValidUrls_ReturnsCopiedList()
        {
            var urls = new[] { "url1", "url2" };

            var result = ListingImageSlots.ToList(urls);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("url1", result[0]);
            Assert.Equal("url2", result[1]);
        }
    }
}
