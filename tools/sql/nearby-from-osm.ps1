# İlanların yakınındaki yerleri OpenStreetMap verisinden (Nominatim kategori araması) üretir ve ListingNearbyPlaces tablosuna yazar.
# Her ilan koordinatı için yaklaşık 2 km'lik kutuda 8 kategoride adı olan yerler aranır, en yakını alınır; mesafe gerçek kuş uçuşu mesafedir.
# Koordinat yaklaşık (mahalle merkezi) ise mesafeler de yaklaşıktır. Stars alanı puan değil, yakınlık derecesidir (5 = 500 m altı).
# Nominatim kullanım politikası: saniyede en fazla 1 istek; aynı koordinatlar tekrar sorgulanmaz.
# -OnlyMissing: yalnızca yakın yeri olmayan ilanları işler, mevcut kayıtları silmez.
# Kullanım: powershell -ExecutionPolicy Bypass -File tools/sql/nearby-from-osm.ps1 -Server "<SUNUCU>\<INSTANCE>" -Database "fibiemlak.com"
param([Parameter(Mandatory = $true)][string]$Server, [string]$Database = 'fibiemlak.com', [switch]$DryRun, [switch]$OnlyMissing)

$ErrorActionPreference = 'Stop'
$inv = [Globalization.CultureInfo]::InvariantCulture
$endpoint = 'https://nominatim.openstreetmap.org/search'
$headers = @{ 'User-Agent' = 'FibiEmlakNearby/1.0 (fibiemlak.com)' }
$boxDeg = 0.018

# NearbyCategories.Id → sırayla denenecek aramalar (ilk sonuç veren kullanılır)
# Anahtarlar bilerek metin: [ordered] sözlükte tamsayı anahtar konumsal indeks gibi davranır ($c[1] ikinci öğeyi döndürür)
$categories = [ordered]@{
    '1' = @('amenity=school', 'amenity=university')
    '2' = @('amenity=hospital', 'amenity=clinic', 'amenity=pharmacy')
    '3' = @('q=[station]', 'amenity=bus_station', 'amenity=ferry_terminal')
    '4' = @('q=[supermarket]', 'q=[mall]')
    '5' = @('amenity=cafe', 'amenity=restaurant')
    '6' = @('q=[park]')
    '7' = @('q=[sports centre]', 'q=[fitness centre]')
    '8' = @('amenity=post_office', 'amenity=townhall', 'amenity=police')
}
function Haversine($lat1, $lon1, $lat2, $lon2) {
    $r = 6371.0; $dLat = ($lat2 - $lat1) * [math]::PI / 180; $dLon = ($lon2 - $lon1) * [math]::PI / 180
    $a = [math]::Sin($dLat / 2) * [math]::Sin($dLat / 2) + [math]::Cos($lat1 * [math]::PI / 180) * [math]::Cos($lat2 * [math]::PI / 180) * [math]::Sin($dLon / 2) * [math]::Sin($dLon / 2)
    2 * $r * [math]::Asin([math]::Sqrt($a))
}
function Stars([double]$km) { if ($km -lt 0.5) { 5 } elseif ($km -lt 1) { 4 } elseif ($km -lt 1.5) { 3 } elseif ($km -lt 2) { 2 } else { 1 } }

function Search($term, $lat, $lon) {
    $vb = '{0},{1},{2},{3}' -f ($lon - $boxDeg).ToString($inv), ($lat + $boxDeg).ToString($inv), ($lon + $boxDeg).ToString($inv), ($lat - $boxDeg).ToString($inv)
    $kv = $term.Split('=', 2)
    $url = "$endpoint`?$($kv[0])=$([uri]::EscapeDataString($kv[1]))&viewbox=$vb&bounded=1&format=jsonv2&limit=10&accept-language=tr"
    for ($try = 1; $try -le 3; $try++) {
        try { $res = Invoke-RestMethod $url -Headers $headers -TimeoutSec 40; Start-Sleep -Milliseconds 1100; return @($res) }
        catch {
            $code = 0; if ($_.Exception.Response) { $code = [int]$_.Exception.Response.StatusCode }
            if ($code -eq 429 -or $code -eq 403) { throw "Nominatim istekleri sınırladı (HTTP $code); daha sonra tekrar deneyin." }
            Start-Sleep -Seconds (3 * $try)
        }
    }
    @()
}

$points = sqlcmd -S $Server -d $Database -E -C -h -1 -W -s "|" -Q "SET NOCOUNT ON; SELECT g.ListingId, g.ListingTypeId, CAST(g.Latitude AS varchar(20)), CAST(g.Longitude AS varchar(20)) FROM ListingGeoPoints g $(if ($OnlyMissing) { 'WHERE NOT EXISTS (SELECT 1 FROM ListingNearbyPlaces n WHERE n.ListingId = g.ListingId AND n.ListingTypeId = g.ListingTypeId)' });" |
    Where-Object { $_ -match '^\d' } | ForEach-Object { $p = $_.Split('|'); [pscustomobject]@{ Id = [int]$p[0]; Type = [int]$p[1]; Lat = [double]::Parse($p[2], $inv); Lon = [double]::Parse($p[3], $inv) } }
Write-Output "$($points.Count) ilan koordinatı"

$cache = @{}
$rows = New-Object System.Collections.Generic.List[string]
foreach ($pt in $points) {
    $key = '{0:F5},{1:F5}' -f $pt.Lat, $pt.Lon
    if (-not $cache.ContainsKey($key)) {
        $found = @{}
        foreach ($cat in $categories.Keys) {
            foreach ($term in $categories[$cat]) {
                $best = $null; $bestKm = [double]::MaxValue
                foreach ($e in (Search $term $pt.Lat $pt.Lon)) {
                    if (-not $e.name) { continue }
                    $km = Haversine $pt.Lat $pt.Lon ([double]::Parse($e.lat, $inv)) ([double]::Parse($e.lon, $inv))
                    if ($km -lt $bestKm) { $bestKm = $km; $best = $e.name }
                }
                if ($best) { $found[$cat] = @{ Name = $best; Km = $bestKm }; break }
            }
        }
        $cache[$key] = $found
    }
    $found = $cache[$key]; $sort = 0
    foreach ($cat in $categories.Keys) {
        if (-not $found.ContainsKey($cat)) { continue }
        $sort++
        $name = $found[$cat].Name.Replace("'", "''"); if ($name.Length -gt 150) { $name = $name.Substring(0, 150) }
        $km = $found[$cat].Km
        $rows.Add("INSERT INTO dbo.ListingNearbyPlaces (ListingId, ListingTypeId, NearbyCategoryId, PlaceName, DistanceKm, Stars, SortOrder, IsActive) VALUES ($($pt.Id), $($pt.Type), $cat, N'$name', $([math]::Round($km, 2).ToString($inv)), $(Stars $km), $sort, 1);")
    }
    Write-Output ("  {0}/{1}: {2} yer" -f $pt.Id, $pt.Type, $sort)
}

$sqlText = "SET NOCOUNT ON; SET XACT_ABORT ON; BEGIN TRAN; $(if (-not $OnlyMissing) { 'DELETE FROM dbo.ListingNearbyPlaces;' })`n" + ($rows -join "`n") + "`nCOMMIT;"
$out = Join-Path $env:TEMP 'fibi-nearby.sql'
[IO.File]::WriteAllText($out, $sqlText, (New-Object Text.UTF8Encoding($true)))
if ($DryRun) { Write-Output "DryRun: $out ($($rows.Count) satır)"; return }
if ($rows.Count -eq 0) { Write-Output 'Hiç yer bulunamadı; tablo değiştirilmedi.'; return }
sqlcmd -S $Server -d $Database -E -C -b -f 65001 -i $out
Write-Output "Yazıldı: $($rows.Count) yakın yer"
