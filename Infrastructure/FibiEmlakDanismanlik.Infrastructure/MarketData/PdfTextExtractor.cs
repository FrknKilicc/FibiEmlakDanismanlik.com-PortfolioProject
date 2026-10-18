using System;
using System.Collections.Generic;
using System.Linq;
using UglyToad.PdfPig;

namespace FibiEmlakDanismanlik.Infrastructure.MarketData
{
    /// <summary>PDF belgelerinden satır bazında metin çıkarma yardımcısı.</summary>
    internal static class PdfTextExtractor
    {
        public static string ExtractLines(byte[] pdfBytes)
        {
            if (pdfBytes == null || pdfBytes.Length == 0)
                return string.Empty;

            using var document = PdfDocument.Open(pdfBytes);
            var allLines = new List<string>();
            var isFirstPage = true;

            foreach (var page in document.GetPages())
            {
                if (!isFirstPage)
                {
                    allLines.Add("=====");
                }
                isFirstPage = false;

                var pageLines = page.GetWords()
                    .GroupBy(word => Math.Round(word.BoundingBox.Bottom))
                    .OrderByDescending(group => group.Key)
                    .Select(group => string.Join(" ", group.OrderBy(word => word.BoundingBox.Left).Select(word => word.Text)));

                allLines.AddRange(pageLines);
            }

            return string.Join("\n", allLines);
        }
    }
}
