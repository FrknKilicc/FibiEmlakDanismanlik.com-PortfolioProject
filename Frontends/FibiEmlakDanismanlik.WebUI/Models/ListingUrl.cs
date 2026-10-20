using System.Text;
using System.Text.RegularExpressions;

namespace FibiEmlakDanismanlik.WebUI.Models
{
    public static class ListingUrl
    {
        /// <summary>
        /// Metni SEO dostu slug biçimine dönüştürür.
        /// Türkçe karakterleri sadeleştirir, kesme işaretlerini siler ve alfasayısal olmayan karakterleri tire yapar.
        /// </summary>
        public static string Slugify(string? text, int maxLength = 80)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            // Kesme işaretleri tamamen silinir (tire olmaz)
            var cleaned = text.Replace("'", "").Replace("’", "");

            // Türkçe karakterleri ASCII karşılıklarına dönüştür
            var sb = new StringBuilder(cleaned.Length);
            foreach (var ch in cleaned)
            {
                switch (ch)
                {
                    case 'ç':
                    case 'Ç':
                        sb.Append('c');
                        break;
                    case 'ğ':
                    case 'Ğ':
                        sb.Append('g');
                        break;
                    case 'ı':
                    case 'I':
                    case 'İ':
                    case 'i':
                        sb.Append('i');
                        break;
                    case 'ö':
                    case 'Ö':
                        sb.Append('o');
                        break;
                    case 'ş':
                    case 'Ş':
                        sb.Append('s');
                        break;
                    case 'ü':
                    case 'Ü':
                        sb.Append('u');
                        break;
                    default:
                        sb.Append(ch);
                        break;
                }
            }

            var lower = sb.ToString().ToLowerInvariant();

            // a-z ve 0-9 dışındaki her karakter dizisi tek bir '-' olur; baştaki/sondaki '-' kırpılır
            var slug = Regex.Replace(lower, @"[^a-z0-9]+", "-").Trim('-');

            // maxLength aşılırsa son tireden kes, yoksa düz kes ve sondaki tireyi kırp
            if (slug.Length > maxLength)
            {
                var truncated = slug[..maxLength];
                var lastDash = truncated.LastIndexOf('-');
                if (lastDash > 0)
                {
                    truncated = truncated[..lastDash];
                }
                slug = truncated.TrimEnd('-');
            }

            return slug;
        }

        /// <summary>
        /// İlan başlığının sonundaki " - Satılık", " - Kiralık" gibi ekleri temizler.
        /// </summary>
        public static string NameForSlug(string? propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                return string.Empty;

            var name = propertyName.Trim();
            string[] suffixes = { " - Satılık", " - Kiralık", " - Satilik", " - Kiralik" };
            bool removed;
            do
            {
                removed = false;
                foreach (var suffix in suffixes)
                {
                    if (name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
                    {
                        name = name[..^suffix.Length].Trim();
                        removed = true;
                        break;
                    }
                }
            } while (removed);

            return name;
        }

        /// <summary>
        /// İlan türü id veya adına göre kategori slug'ını ("arsa", "konut", "isyeri" veya "ilan") döndürür.
        /// </summary>
        public static string CategorySlug(int? listingTypeId, string? listingTypeName)
        {
            if (listingTypeId == 1 || listingTypeId == 7)
                return "arsa";
            if (listingTypeId == 2 || listingTypeId == 8)
                return "konut";
            if (listingTypeId == 3 || listingTypeId == 9)
                return "isyeri";

            var slugFromName = Slugify(listingTypeName);
            if (slugFromName == "arsa" || slugFromName == "konut" || slugFromName == "isyeri")
                return slugFromName;

            return "ilan";
        }

        /// <summary>
        /// Kategori slug'ı ve kiralık durumuna göre veritabanındaki ListingTypeId eşlemesini döndürür.
        /// </summary>
        public static int? ListingTypeIdFromCategory(bool isRent, string? category)
        {
            if (string.IsNullOrWhiteSpace(category))
                return null;

            var cat = category.Trim();
            if (isRent)
            {
                if (string.Equals(cat, "arsa", StringComparison.OrdinalIgnoreCase)) return 7;
                if (string.Equals(cat, "konut", StringComparison.OrdinalIgnoreCase)) return 8;
                if (string.Equals(cat, "isyeri", StringComparison.OrdinalIgnoreCase)) return 9;
            }
            else
            {
                if (string.Equals(cat, "arsa", StringComparison.OrdinalIgnoreCase)) return 1;
                if (string.Equals(cat, "konut", StringComparison.OrdinalIgnoreCase)) return 2;
                if (string.Equals(cat, "isyeri", StringComparison.OrdinalIgnoreCase)) return 3;
            }

            return null;
        }

        /// <summary>
        /// İlanın canonical detay yolu adresini üretir.
        /// </summary>
        public static string DetailPath(bool isRent, int listingId, int? listingTypeId, string? listingTypeName, string? propertyName, string? district)
        {
            var prefix = isRent ? "kiralik" : "satilik";
            var effectiveTypeId = (listingTypeId.HasValue && listingTypeId.Value > 0) ? listingTypeId : null;
            var category = CategorySlug(effectiveTypeId, listingTypeName);

            var rawSlugText = $"{district ?? string.Empty} {NameForSlug(propertyName)}".Trim();
            var slug = Slugify(rawSlugText);

            var lastPart = string.IsNullOrEmpty(slug) ? listingId.ToString() : $"{slug}-{listingId}";
            return $"/{prefix}/{category}/{lastPart}";
        }

        /// <summary>
        /// Slug ve id içeren dizgeden son tireden sonraki id değerini ayrıştırır.
        /// </summary>
        public static bool TryParseId(string? slugAndId, out int id)
        {
            id = 0;
            if (string.IsNullOrWhiteSpace(slugAndId))
                return false;

            var trimmed = slugAndId.Trim();
            var lastDash = trimmed.LastIndexOf('-');
            var idPart = lastDash >= 0 ? trimmed[(lastDash + 1)..] : trimmed;

            if (int.TryParse(idPart, out var parsedId) && parsedId > 0)
            {
                id = parsedId;
                return true;
            }

            return false;
        }
    }
}
