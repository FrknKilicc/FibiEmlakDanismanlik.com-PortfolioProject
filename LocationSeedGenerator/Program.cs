using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace LocationSeedGenerator
{
    internal class Program
    {
        static string Esc(string s) => (s ?? "").Replace("'", "''").Trim();

        static void Main(string[] args)
        {
            if (args.Length < 4)
            {
                // DEBUG FALLBACK (argüman gelmiyorsa buradan devam)
                args = new[]
 {
    @"C:\Users\3060\source\repos\FibiEmlakDanismanlik\Infrastructure\FibiEmlakDanismanlik.Persistance\SeedData\sehirler.json",
    @"C:\Users\3060\source\repos\FibiEmlakDanismanlik\Infrastructure\FibiEmlakDanismanlik.Persistance\SeedData\ilceler.json",
    @"C:\Users\3060\source\repos\FibiEmlakDanismanlik\Infrastructure\FibiEmlakDanismanlik.Persistance\SeedData\mahalleler-1.json;C:\Users\3060\source\repos\FibiEmlakDanismanlik\Infrastructure\FibiEmlakDanismanlik.Persistance\SeedData\mahalleler-2.json;C:\Users\3060\source\repos\FibiEmlakDanismanlik\Infrastructure\FibiEmlakDanismanlik.Persistance\SeedData\mahalleler-3.json;C:\Users\3060\source\repos\FibiEmlakDanismanlik\Infrastructure\FibiEmlakDanismanlik.Persistance\SeedData\mahalleler-4.json",
    @"C:\Temp\Seed_Location.sql"
};

                Console.WriteLine("Arguman gelmedi, debug fallback kullaniliyor...");
            }
            // Kullanım:
            // LocationSeedGenerator "iller.json" "ilceler.json" "mahalleler1;mahalleler2;..." "output.sql"
            //
            // Örnek:
            // LocationSeedGenerator "C:\...\SeedData\iller.json" "C:\...\SeedData\ilceler.json"
            //   "C:\...\SeedData\mahalleler-1.json;C:\...\SeedData\mahalleler-2.json;C:\...\SeedData\mahalleler-3.json;C:\...\SeedData\mahalleler-4.json"
            //   "C:\Temp\Seed_Location.sql"

            if (args.Length < 4)
            {
                Console.WriteLine("Kullanim:");
                Console.WriteLine("  LocationSeedGenerator \"iller.json\" \"ilceler.json\" \"mahalleler1;mahalleler2;...\" \"output.sql\"");
                return;
            }

            var citiesPath = args[0];
            var districtsPath = args[1];
            var neighborhoodsPaths = args[2].Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var outSqlPath = args[3];

            if (!File.Exists(citiesPath)) { Console.WriteLine("Bulunamadi: " + citiesPath); return; }
            if (!File.Exists(districtsPath)) { Console.WriteLine("Bulunamadi: " + districtsPath); return; }
            foreach (var p in neighborhoodsPaths)
                if (!File.Exists(p)) { Console.WriteLine("Bulunamadi: " + p); return; }

            var cities = LoadCities(citiesPath)
                .Select(c => c with { CityName = Clean(c.CityName) })
                .ToList();

            var districts = LoadDistricts(districtsPath)
                .Select(d => d with { CityName = Clean(d.CityName), DistrictName = Clean(d.DistrictName) })
                .ToList();

            var neighborhoods = new List<NeighborhoodRow>();
            foreach (var p in neighborhoodsPaths)
                neighborhoods.AddRange(LoadNeighborhoods(p));

            neighborhoods = neighborhoods
                .Select(n => n with
                {
                    CityName = Clean(n.CityName),
                    DistrictName = Clean(n.DistrictName),
                    NeighborhoodName = Clean(n.NeighborhoodName)
                })
                .ToList();

            // SQL üret
            var sb = new StringBuilder();
            sb.AppendLine("-- Auto-generated location seed (TR flat json)");
            sb.AppendLine("SET NOCOUNT ON;");
            sb.AppendLine();

            // 1) CITIES
            sb.AppendLine("-- CITIES");
            foreach (var c in cities.OrderBy(x => x.CityId))
            {
                // PlateCode'ı sehir_id olarak basıyoruz (işimizi kolaylaştırıyor)
                sb.AppendLine($"IF NOT EXISTS (SELECT 1 FROM Cities WHERE PlaceCode = {c.CityId})");
                sb.AppendLine($"    INSERT INTO Cities (Name, PlaceCode) VALUES (N'{Esc(c.CityName)}', {c.CityId});");
            }
            sb.AppendLine();

            // 2) DISTRICTS
            // 2) DISTRICTS
            sb.AppendLine("-- DISTRICTS");
            foreach (var d in districts.OrderBy(x => x.CityId).ThenBy(x => x.DistrictName))
            {
                sb.AppendLine($@"
IF NOT EXISTS (
    SELECT 1
    FROM Districts
    WHERE CityId = (SELECT TOP 1 CityId FROM Cities WHERE PlaceCode = {d.CityId})
      AND Name = N'{Esc(d.DistrictName)}'
)
BEGIN
    INSERT INTO Districts (CityId, Name)
    SELECT TOP 1 CityId, N'{Esc(d.DistrictName)}'
    FROM Cities
    WHERE PlaceCode = {d.CityId};
END
");
            }
            sb.AppendLine();


            // 3) NEIGHBORHOODS
            // 3) NEIGHBORHOODS
            sb.AppendLine("-- NEIGHBORHOODS");
            foreach (var n in neighborhoods.OrderBy(x => x.CityId).ThenBy(x => x.DistrictName).ThenBy(x => x.NeighborhoodName))
            {
                sb.AppendLine($@"
IF NOT EXISTS (
    SELECT 1
    FROM Neighborhoods
    WHERE DistrictId = (
        SELECT TOP 1 d.DistrictId
        FROM Districts d
        JOIN Cities c ON c.CityId = d.CityId
        WHERE c.PlaceCode = {n.CityId}
          AND d.Name = N'{Esc(n.DistrictName)}'
    )
    AND Name = N'{Esc(n.NeighborhoodName)}'
)
BEGIN
    INSERT INTO Neighborhoods (DistrictId, Name)
    SELECT TOP 1 d.DistrictId, N'{Esc(n.NeighborhoodName)}'
    FROM Districts d
    JOIN Cities c ON c.CityId = d.CityId
    WHERE c.PlaceCode = {n.CityId}
      AND d.Name = N'{Esc(n.DistrictName)}';
END
");
            }

            File.WriteAllText(outSqlPath, sb.ToString(), Encoding.UTF8);

            Console.WriteLine("Bitti!");
            Console.WriteLine($"Cities: {cities.Count}, Districts: {districts.Count}, Neighborhoods: {neighborhoods.Count}");
            Console.WriteLine("SQL: " + outSqlPath);
        }

        static string Clean(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "";
            s = s.Trim();
            while (s.Contains("  ")) s = s.Replace("  ", " ");
            return s;
        }

        // ---- LOADERS ----

        static List<CityRow> LoadCities(string path)
        {
            using var doc = JsonDocument.Parse(File.ReadAllText(path, Encoding.UTF8));
            var list = new List<CityRow>();

            foreach (var el in AsArray(doc.RootElement))
            {
                var id = GetInt(el, "sehir_id");
                var name = GetString(el, "sehir_adi");

                if (id == null || string.IsNullOrWhiteSpace(name)) continue;
                list.Add(new CityRow(id.Value, name!));
            }

            return list;
        }

        static List<DistrictRow> LoadDistricts(string path)
        {
            using var doc = JsonDocument.Parse(File.ReadAllText(path, Encoding.UTF8));
            var list = new List<DistrictRow>();

            foreach (var el in AsArray(doc.RootElement))
            {
                var cityId = GetInt(el, "sehir_id");
                var cityName = GetString(el, "sehir_adi") ?? "";
                var districtName = GetString(el, "ilce_adi");

                if (cityId == null || string.IsNullOrWhiteSpace(districtName)) continue;
                list.Add(new DistrictRow(cityId.Value, cityName, districtName!));
            }

            return list;
        }

        static List<NeighborhoodRow> LoadNeighborhoods(string path)
        {
            using var doc = JsonDocument.Parse(File.ReadAllText(path, Encoding.UTF8));
            var list = new List<NeighborhoodRow>();

            foreach (var el in AsArray(doc.RootElement))
            {
                var cityId = GetInt(el, "sehir_id");
                var cityName = GetString(el, "sehir_adi") ?? "";
                var districtName = GetString(el, "ilce_adi");
                var neighborhoodName = GetString(el, "mahalle_adi");

                if (cityId == null || string.IsNullOrWhiteSpace(districtName) || string.IsNullOrWhiteSpace(neighborhoodName))
                    continue;

                list.Add(new NeighborhoodRow(cityId.Value, cityName, districtName!, neighborhoodName!));
            }

            return list;
        }

        static IEnumerable<JsonElement> AsArray(JsonElement root)
        {
            if (root.ValueKind == JsonValueKind.Array)
                return root.EnumerateArray();

            if (root.ValueKind == JsonValueKind.Object)
            {
                if (root.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Array)
                    return data.EnumerateArray();
            }

            throw new InvalidOperationException("JSON root array degil (ya da {data:[...]} degil).");
        }

        static int? GetInt(JsonElement obj, string prop)
        {
            if (!obj.TryGetProperty(prop, out var v)) return null;
            if (v.ValueKind == JsonValueKind.Number && v.TryGetInt32(out var i)) return i;
            if (v.ValueKind == JsonValueKind.String && int.TryParse(v.GetString(), out var j)) return j;
            return null;
        }

        static string? GetString(JsonElement obj, string prop)
        {
            if (!obj.TryGetProperty(prop, out var v)) return null;
            if (v.ValueKind == JsonValueKind.String) return v.GetString();
            return null;
        }

        record CityRow(int CityId, string CityName);
        record DistrictRow(int CityId, string CityName, string DistrictName);
        record NeighborhoodRow(int CityId, string CityName, string DistrictName, string NeighborhoodName);
    }
}
