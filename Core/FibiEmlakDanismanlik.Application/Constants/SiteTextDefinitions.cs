using System.Collections.Generic;
using System.Linq;

namespace FibiEmlakDanismanlik.Application.Constants
{
    /// <summary>
    /// Panelden düzenlenebilen site metinlerinin tek kaynağı: anahtar, grup, etiket, varsayılan değer ve sınırlar.
    /// DB'de kaydı olmayan anahtar varsayılanla gösterilir; yeni metin eklemek için listeye satır eklemek yeterli.
    /// </summary>
    public static class SiteTextDefinitions
    {
        public const string KindText = "text";
        public const string KindMultiLine = "multiline";
        public const string KindNumber = "number";

        public sealed record Definition(
            string Key,
            string Group,
            string Label,
            string Default,
            string Kind = KindText,
            int MaxLength = 200,
            int Min = 0,
            int Max = 0,
            string? Help = null);

        public static readonly IReadOnlyList<Definition> All = new List<Definition>
        {
            new("home.banner.subtitle", "Ana Sayfa: Banner", "Banner alt yazısı", "Fibi Emlak Danışmanlık, Kazandıran Gayrimenkul Çözümleri",
                Help: "Tüm banner slaytlarında başlığın altında görünür. Başlıklar Ana Sayfa Banner ekranından düzenlenir."),

            new("home.latest.eyebrow", "Ana Sayfa: Son Eklenen İlanlar", "Üst başlık", "İlanlar", MaxLength: 60),
            new("home.latest.title", "Ana Sayfa: Son Eklenen İlanlar", "Başlık", "Son eklenenler", MaxLength: 100),
            new("home.latest.count", "Ana Sayfa: Son Eklenen İlanlar", "Gösterilecek ilan sayısı", "10", KindNumber, Min: 0, Max: 24,
                Help: "0 yazılırsa bölüm gizlenir."),

            new("home.deals.eyebrow", "Ana Sayfa: Vitrin İlanları", "Üst başlık", "Fırsat ilanlar", MaxLength: 60),
            new("home.deals.title", "Ana Sayfa: Vitrin İlanları", "Başlık", "Özel Teklifler", MaxLength: 100,
                Help: "Vitrine alınacak ilanlar ilan düzenleme ekranından, vitrin sınırı Ayarlar ekranından belirlenir."),

            new("home.chooseus.eyebrow", "Ana Sayfa: Neden Biz", "Üst başlık", "Neden Fibi Emlak", MaxLength: 60),
            new("home.chooseus.title", "Ana Sayfa: Neden Biz", "Başlık", "", MaxLength: 100, Help: "Boş bırakılabilir."),
            new("home.chooseus.card1.title", "Ana Sayfa: Neden Biz", "1. kart başlığı", "Güvenilirlik", MaxLength: 60),
            new("home.chooseus.card1.text", "Ana Sayfa: Neden Biz", "1. kart metni", "Fark yaratan hizmetimizle güvenilir bir emlak deneyimi sunuyoruz", KindMultiLine, 300),
            new("home.chooseus.card2.title", "Ana Sayfa: Neden Biz", "2. kart başlığı", "Deneyimli Danışmanlar", MaxLength: 60),
            new("home.chooseus.card2.text", "Ana Sayfa: Neden Biz", "2. kart metni", "Sektördeki en tecrübeli danışmanlarımız, size en uygun mülkü bulmanızda yardımcı olacak", KindMultiLine, 300),
            new("home.chooseus.card3.title", "Ana Sayfa: Neden Biz", "3. kart başlığı", "Kişiselleştirilmiş Hizmet", MaxLength: 60),
            new("home.chooseus.card3.text", "Ana Sayfa: Neden Biz", "3. kart metni", "İhtiyaçlarınıza uygun çözümler sunarak size özel bir emlak deneyimi yaşatıyoruz", KindMultiLine, 300),

            new("home.cta.title", "Ana Sayfa: Çağrı Bandı", "Çağrı metni", "Yeni bir mülk almak ya da bir mülkü satmak mı istiyorsunuz?", KindMultiLine, 200),
            new("home.cta.rent", "Ana Sayfa: Çağrı Bandı", "Kiralık düğmesi", "Kirala", MaxLength: 30),
            new("home.cta.buy", "Ana Sayfa: Çağrı Bandı", "Satılık düğmesi", "Satın Al", MaxLength: 30),

            new("home.places.eyebrow", "Ana Sayfa: Öne Çıkan Konumlar", "Üst başlık", "Öne Çıkan Konumlar", MaxLength: 60),

            new("home.agents.eyebrow", "Ana Sayfa: Danışmanlar", "Üst başlık", "Danışmanlarımız", MaxLength: 60),
            new("home.agents.title", "Ana Sayfa: Danışmanlar", "Başlık", "Uzman Danışmanlarımızla Tanışın", MaxLength: 100),

            new("home.partners.eyebrow", "Ana Sayfa: İş Ortakları", "Üst başlık", "Partnerlerimiz", MaxLength: 60),
            new("home.partners.title", "Ana Sayfa: İş Ortakları", "Başlık", "Uzun Vadeli İş Ortaklarımız", MaxLength: 100),

            new("detail.similar.title", "İlan Detayı", "Benzer ilanlar başlığı", "Benzer İlanlar", MaxLength: 60),
            new("detail.similar.count", "İlan Detayı", "Benzer ilan sayısı", "6", KindNumber, Min: 0, Max: 6,
                Help: "0 yazılırsa benzer ilanlar gizlenir. En fazla 6."),
        };

        private static readonly Dictionary<string, Definition> ByKey = All.ToDictionary(d => d.Key);

        public static Definition? Find(string key) => ByKey.TryGetValue(key, out var d) ? d : null;

        /// <summary>
        /// Varsayılanların üzerine kayıtlı değerleri yazar; tanımı olmayan kayıtlar atılır.
        /// </summary>
        public static Dictionary<string, string> Merge(IReadOnlyDictionary<string, string> stored)
        {
            var result = All.ToDictionary(d => d.Key, d => d.Default);
            foreach (var (key, value) in stored)
            {
                if (ByKey.ContainsKey(key))
                {
                    result[key] = value;
                }
            }
            return result;
        }
    }
}
