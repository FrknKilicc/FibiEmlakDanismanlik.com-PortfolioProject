using System.Text;

namespace FibiEmlakDanismanlik.Infrastructure.MarketData
{
    /// <summary>Türkçe karakter kodlama yardımcısı.</summary>
    internal static class TurkishEncoding
    {
        public static Encoding Windows1254 { get; }

        static TurkishEncoding()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Windows1254 = Encoding.GetEncoding(1254);
        }
    }
}
