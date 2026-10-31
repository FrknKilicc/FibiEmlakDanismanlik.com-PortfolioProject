using System;
using System.Collections.Generic;
using FibiEmlakDanismanlik.Application.Features.Handlers.ContentImageHandlers;
using FibiEmlakDanismanlik.Application.Interfaces.ListingImageInterfaces;
using Xunit;

namespace FibiEmlakDanismanlik.Unit.Tests
{
    public class OrphanContentImageTests
    {
        [Fact]
        public void IsOrphan_WhenUnreferencedAndOlderThanCutoff_ReturnsTrue()
        {
            // Arrange
            var cutoffUtc = DateTime.UtcNow.AddHours(-24);
            var file = new StoredFileInfo("content/blog/2026/10/abc-1600.webp", 1024, cutoffUtc.AddHours(-1));
            var references = Array.Empty<string>();

            // Act
            var result = OrphanContentImageHelper.IsOrphan(file, references, cutoffUtc);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsOrphan_WhenReferencedAsFullUrl_ReturnsFalse()
        {
            // Arrange
            var cutoffUtc = DateTime.UtcNow.AddHours(-24);
            var file = new StoredFileInfo("content/blog/2026/10/abc-1600.webp", 1024, cutoffUtc.AddHours(-1));
            var references = new[] { "https://site/uploads/content/blog/2026/10/abc-1600.webp" };

            // Act
            var result = OrphanContentImageHelper.IsOrphan(file, references, cutoffUtc);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsOrphan_WhenFileNameAppearsInsideBlogHtmlText_ReturnsFalse()
        {
            // Arrange
            var cutoffUtc = DateTime.UtcNow.AddHours(-24);
            var file = new StoredFileInfo("content/blog/2026/10/abc-1600.webp", 1024, cutoffUtc.AddHours(-1));
            var references = new[] { "<p><img src=\"/uploads/content/blog/2026/10/abc-1600.webp\"></p>" };

            // Act
            var result = OrphanContentImageHelper.IsOrphan(file, references, cutoffUtc);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsOrphan_WhenReferenceHasDifferentCasing_ReturnsFalse()
        {
            // Arrange
            var cutoffUtc = DateTime.UtcNow.AddHours(-24);
            var file = new StoredFileInfo("content/blog/2026/10/abc-1600.webp", 1024, cutoffUtc.AddHours(-1));
            var references = new[] { "ABC-1600.WEBP" };

            // Act
            var result = OrphanContentImageHelper.IsOrphan(file, references, cutoffUtc);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsOrphan_WhenFileIsNewerThanCutoffUtc_ReturnsFalse()
        {
            // Arrange
            var cutoffUtc = DateTime.UtcNow.AddHours(-24);
            var file = new StoredFileInfo("content/blog/2026/10/abc-1600.webp", 1024, cutoffUtc.AddHours(1));
            var references = Array.Empty<string>();

            // Act
            var result = OrphanContentImageHelper.IsOrphan(file, references, cutoffUtc);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsOrphan_WhenPathIsOutsideContentFolder_ReturnsFalse()
        {
            // Arrange
            var cutoffUtc = DateTime.UtcNow.AddHours(-24);
            var file = new StoredFileInfo("listings/2026/10/x-800.webp", 1024, cutoffUtc.AddHours(-1));
            var references = Array.Empty<string>();

            // Act
            var result = OrphanContentImageHelper.IsOrphan(file, references, cutoffUtc);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsOrphan_WhenPathContainsDotDotTraversal_ReturnsFalse()
        {
            // Arrange
            var cutoffUtc = DateTime.UtcNow.AddHours(-24);
            var file = new StoredFileInfo("content/../listings/x.webp", 1024, cutoffUtc.AddHours(-1));
            var references = Array.Empty<string>();

            // Act
            var result = OrphanContentImageHelper.IsOrphan(file, references, cutoffUtc);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsOrphan_WhenPathUsesBackslashesAndUnreferenced_ReturnsTrue()
        {
            // Arrange
            var cutoffUtc = DateTime.UtcNow.AddHours(-24);
            var file = new StoredFileInfo(@"content\blog\x-1600.webp", 1024, cutoffUtc.AddHours(-1));
            var references = Array.Empty<string>();

            // Act
            var result = OrphanContentImageHelper.IsOrphan(file, references, cutoffUtc);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsOrphan_WhenRelativePathIsEmpty_ReturnsFalse()
        {
            // Arrange
            var cutoffUtc = DateTime.UtcNow.AddHours(-24);
            var file = new StoredFileInfo(string.Empty, 1024, cutoffUtc.AddHours(-1));
            var references = Array.Empty<string>();

            // Act
            var result = OrphanContentImageHelper.IsOrphan(file, references, cutoffUtc);

            // Assert
            Assert.False(result);
        }
    }
}
