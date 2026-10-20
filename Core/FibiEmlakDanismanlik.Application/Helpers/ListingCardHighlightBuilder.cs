using System;
using System.Collections.Generic;
using System.Globalization;
using FibiEmlakDanismanlik.Application.ViewModels;

namespace FibiEmlakDanismanlik.Application.Helpers
{
    public static class ListingCardHighlightBuilder
    {
        private static readonly CultureInfo TrCulture = new("tr-TR");

        public static List<CardHighlightViewModel> Housing(
            string? numberOfRoom,
            double? netArea,
            double? grossArea,
            int? floor,
            int? bathrooms,
            string? buildingAge)
        {
            var highlights = new List<CardHighlightViewModel>(3);

            // 1. Oda
            AddIfValid(highlights, "fal fa-bed", numberOfRoom);

            // 2. m²
            double? m2 = (netArea.HasValue && netArea.Value > 0) ? netArea.Value :
                         (grossArea.HasValue && grossArea.Value > 0) ? grossArea.Value : null;
            if (m2.HasValue)
            {
                AddIfValid(highlights, "fal fa-ruler-combined", $"{m2.Value.ToString("N0", TrCulture)} m²");
            }

            // 3. Kat
            AddIfValid(highlights, "fal fa-layer-group", FloorLabel(floor));

            // 4. Banyo
            if (bathrooms.HasValue && bathrooms.Value > 0)
            {
                AddIfValid(highlights, "fal fa-bath", $"{bathrooms.Value} Banyo");
            }

            // 5. Bina yaşı
            AddIfValid(highlights, "fal fa-building", AgeLabel(buildingAge));

            return highlights;
        }

        public static List<CardHighlightViewModel> Land(
            double? area,
            string? zoningStatus,
            decimal? pricePerSquareMeter,
            string? titleDeedStatus)
        {
            var highlights = new List<CardHighlightViewModel>(3);

            // 1. Alan
            if (area.HasValue && area.Value > 0)
            {
                AddIfValid(highlights, "fal fa-vector-square", $"{area.Value.ToString("N0", TrCulture)} m²");
            }

            // 2. İmar durumu
            AddIfValid(highlights, "fal fa-map", zoningStatus);

            // 3. m² fiyatı
            if (pricePerSquareMeter.HasValue && pricePerSquareMeter.Value > 0)
            {
                AddIfValid(highlights, "fal fa-tag", $"{pricePerSquareMeter.Value.ToString("N0", TrCulture)} TL/m²");
            }

            // 4. Tapu durumu
            AddIfValid(highlights, "fal fa-file-contract", titleDeedStatus);

            return highlights;
        }

        public static List<CardHighlightViewModel> Commercial(
            double? netArea,
            double? grossArea,
            double? area,
            int? sections,
            int? floor,
            string? buildingAge)
        {
            var highlights = new List<CardHighlightViewModel>(3);

            // 1. m²
            double? m2 = (netArea.HasValue && netArea.Value > 0) ? netArea.Value :
                         (grossArea.HasValue && grossArea.Value > 0) ? grossArea.Value :
                         (area.HasValue && area.Value > 0) ? area.Value : null;
            if (m2.HasValue)
            {
                AddIfValid(highlights, "fal fa-ruler-combined", $"{m2.Value.ToString("N0", TrCulture)} m²");
            }

            // 2. Bölüm
            if (sections.HasValue && sections.Value > 0)
            {
                AddIfValid(highlights, "fal fa-th-large", $"{sections.Value} Bölüm");
            }

            // 3. Kat
            AddIfValid(highlights, "fal fa-layer-group", FloorLabel(floor));

            // 4. Bina yaşı
            AddIfValid(highlights, "fal fa-building", AgeLabel(buildingAge));

            return highlights;
        }

        private static void AddIfValid(List<CardHighlightViewModel> list, string icon, string? text)
        {
            if (list.Count >= 3)
                return;

            if (string.IsNullOrWhiteSpace(text))
                return;

            var trimmed = text.Trim();
            if (trimmed == "-")
                return;

            list.Add(new CardHighlightViewModel
            {
                Icon = icon,
                Text = trimmed
            });
        }

        private static string? FloorLabel(int? floor)
        {
            if (!floor.HasValue)
                return null;

            if (floor.Value > 0)
                return $"{floor.Value}. Kat";

            if (floor.Value == 0)
                return "Giriş Kat";

            return "Bodrum Kat";
        }

        private static string? AgeLabel(string? age)
        {
            if (string.IsNullOrWhiteSpace(age))
                return null;

            var trimmed = age.Trim();
            if (trimmed == "-")
                return null;

            if (trimmed == "0")
                return "Sıfır Bina";

            if (string.Equals(trimmed, "21 ve üzeri", StringComparison.OrdinalIgnoreCase))
                return "21+ Yaş";

            if (trimmed.EndsWith("yaş", StringComparison.OrdinalIgnoreCase))
                return trimmed;

            return $"{trimmed} Yaş";
        }
    }
}
