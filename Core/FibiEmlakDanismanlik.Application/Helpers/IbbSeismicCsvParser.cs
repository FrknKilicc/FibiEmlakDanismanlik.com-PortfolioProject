using FibiEmlakDanismanlik.Application.Models.FibiServiceModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace FibiEmlakDanismanlik.Application.Helpers
{
    /// <summary>İBB deprem senaryosu ve bina sayısı CSV verilerini ayrıştırıcı.</summary>
    public static class IbbSeismicCsvParser
    {
        private static readonly string[] RequiredScenarioHeaders =
        {
            "ilce_adi",
            "mahalle_adi",
            "mahalle_koy_uavt",
            "cok_agir_hasarli_bina_sayisi",
            "agir_hasarli_bina_sayisi",
            "orta_hasarli_bina_sayisi",
            "hafif_hasarli_bina_sayisi",
            "can_kaybi_sayisi",
            "gecici_barinma"
        };

        private static readonly string[] RequiredBuildingHeaders =
        {
            "mahalle_uavt",
            "1980_oncesi",
            "1980-2000_arasi",
            "2000_sonrasi"
        };

        public static List<IbbSeismicRecord> Parse(string scenarioCsv, string buildingCountCsv)
        {
            if (string.IsNullOrWhiteSpace(scenarioCsv))
                throw new FormatException("Senaryo CSV içeriği boş.");

            if (string.IsNullOrWhiteSpace(buildingCountCsv))
                throw new FormatException("Bina sayısı CSV içeriği boş.");

            // 1. Bina sayısı CSV'sini oku ve UAVT tablosu oluştur
            var buildingLines = SplitLines(buildingCountCsv);
            var (buildingHeaders, buildingHeaderRow) = ReadHeaders(buildingLines);
            ValidateHeaders(buildingHeaders, RequiredBuildingHeaders);

            var uavtColB = buildingHeaders["mahalle_uavt"];
            var before1980Col = buildingHeaders["1980_oncesi"];
            var range1980Col = buildingHeaders["1980-2000_arasi"];
            var after2000Col = buildingHeaders["2000_sonrasi"];

            var buildingMap = new Dictionary<int, (int Before1980, int Range1980To2000, int After2000)>();

            for (var i = buildingHeaderRow + 1; i < buildingLines.Count; i++)
            {
                var line = buildingLines[i];
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var columns = line.Split(';');
                var maxCol = Math.Max(Math.Max(uavtColB, before1980Col), Math.Max(range1980Col, after2000Col));
                if (columns.Length <= maxCol)
                    continue;

                if (!TryParseInt(columns[uavtColB], out var uavt) ||
                    !TryParseInt(columns[before1980Col], out var before1980) ||
                    !TryParseInt(columns[range1980Col], out var range1980) ||
                    !TryParseInt(columns[after2000Col], out var after2000))
                {
                    continue;
                }

                buildingMap[uavt] = (before1980, range1980, after2000);
            }

            // 2. Senaryo CSV'sini oku ve birleştir
            var scenarioLines = SplitLines(scenarioCsv);
            var (scenarioHeaders, scenarioHeaderRow) = ReadHeaders(scenarioLines);
            ValidateHeaders(scenarioHeaders, RequiredScenarioHeaders);

            var ilceCol = scenarioHeaders["ilce_adi"];
            var mahalleCol = scenarioHeaders["mahalle_adi"];
            var uavtColS = scenarioHeaders["mahalle_koy_uavt"];
            var cokAgirCol = scenarioHeaders["cok_agir_hasarli_bina_sayisi"];
            var agirCol = scenarioHeaders["agir_hasarli_bina_sayisi"];
            var ortaCol = scenarioHeaders["orta_hasarli_bina_sayisi"];
            var hafifCol = scenarioHeaders["hafif_hasarli_bina_sayisi"];
            var canKaybiCol = scenarioHeaders["can_kaybi_sayisi"];
            var barinmaCol = scenarioHeaders["gecici_barinma"];

            var maxScenarioCol = new[] { ilceCol, mahalleCol, uavtColS, cokAgirCol, agirCol, ortaCol, hafifCol, canKaybiCol, barinmaCol }.Max();

            var records = new List<IbbSeismicRecord>();

            for (var i = scenarioHeaderRow + 1; i < scenarioLines.Count; i++)
            {
                var line = scenarioLines[i];
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var columns = line.Split(';');
                if (columns.Length <= maxScenarioCol)
                    continue;

                var districtName = columns[ilceCol].Trim().Trim('"', '\'');
                var neighborhoodName = columns[mahalleCol].Trim().Trim('"', '\'');
                if (string.IsNullOrWhiteSpace(districtName) || string.IsNullOrWhiteSpace(neighborhoodName))
                    continue;

                if (!TryParseInt(columns[uavtColS], out var uavt) ||
                    !TryParseInt(columns[cokAgirCol], out var cokAgir) ||
                    !TryParseInt(columns[agirCol], out var agir) ||
                    !TryParseInt(columns[ortaCol], out var orta) ||
                    !TryParseInt(columns[hafifCol], out var hafif) ||
                    !TryParseInt(columns[canKaybiCol], out var canKaybi) ||
                    !TryParseInt(columns[barinmaCol], out var barinma))
                {
                    continue;
                }

                if (!buildingMap.TryGetValue(uavt, out var buildings))
                    continue; // İç birleşim: bina sayısı olmayan satır atlanır

                records.Add(new IbbSeismicRecord(
                    districtName,
                    neighborhoodName,
                    uavt,
                    buildings.Before1980,
                    buildings.Range1980To2000,
                    buildings.After2000,
                    cokAgir,
                    agir,
                    orta,
                    hafif,
                    canKaybi,
                    barinma));
            }

            return records;
        }

        private static List<string> SplitLines(string text)
        {
            var raw = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            return raw.ToList();
        }

        private static (Dictionary<string, int> Headers, int HeaderRowIndex) ReadHeaders(List<string> lines)
        {
            for (var i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var columns = line.Split(';');
                var dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                for (var c = 0; c < columns.Length; c++)
                {
                    var name = columns[c].Trim().Trim('"', '\'').Trim('\uFEFF');
                    if (!string.IsNullOrEmpty(name))
                    {
                        dict.TryAdd(name, c);
                    }
                }

                return (dict, i);
            }

            throw new FormatException("Başlık satırı bulunamadı.");
        }

        private static void ValidateHeaders(Dictionary<string, int> headers, string[] requiredHeaders)
        {
            foreach (var req in requiredHeaders)
            {
                if (!headers.ContainsKey(req))
                    throw new FormatException($"Eksik başlık adı: {req}");
            }
        }

        private static bool TryParseInt(string? text, out int value)
        {
            value = 0;
            if (string.IsNullOrWhiteSpace(text))
                return false;

            var cleaned = text.Trim().Trim('"', '\'');
            if (int.TryParse(cleaned, NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
                return true;

            cleaned = cleaned.Replace(".", "").Replace(",", "").Replace(" ", "");
            return int.TryParse(cleaned, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
        }
    }
}
