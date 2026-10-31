using System;
using System.IO;
using System.Linq;
using System.Text;

namespace FibiEmlakDanismanlik.Infrastructure.MarketData
{
    /// <summary>Gömülü metin kaynaklarını okuma yardımcısı.</summary>
    internal static class EmbeddedResourceReader
    {
        public static string ReadText(string fileName, Encoding encoding)
        {
            var assembly = typeof(EmbeddedResourceReader).Assembly;
            var resourceName = assembly.GetManifestResourceNames()
                .SingleOrDefault(n => n.EndsWith(fileName, StringComparison.OrdinalIgnoreCase))
                ?? throw new InvalidOperationException($"Gömülü kaynak bulunamadı: {fileName}");

            using var stream = assembly.GetManifestResourceStream(resourceName)
                ?? throw new InvalidOperationException($"Gömülü kaynak açılamadı: {resourceName}");

            using var reader = new StreamReader(stream, encoding);
            return reader.ReadToEnd();
        }
    }
}
