# Demo ilan verisini gerçekçi hale getiren SQL'i üretir: tools/sql/demo-listings-refresh.sql
# Fiyatlar FİBİ emsal verisindeki ilçe/il m² değerlerinden türetilir; mahalleler Neighborhoods tablosundaki gerçek adlardır.
# Görseller wwwroot/images/listings (CC0, CREDITS.md). Kimlikler (Id) değişmez. Adres metni mahalle düzeyindedir (kapı no yok).
# Kullanım: powershell -ExecutionPolicy Bypass -File tools/sql/demo-listings-refresh.ps1
# Çalıştırmadan önce: tabloların yedeği (bak20261019 şeması) alınmış olmalı.
# UYARI (T5 2. aşama): PropImgUrl1..30 kolonları silindi; bu betiğin ürettiği SQL artık çalışmaz. Yeniden kullanılacaksa ImgSql, görselleri ListingImages tablosuna yazacak şekilde değiştirilmeli.

$ErrorActionPreference = 'Stop'
$tr = [Globalization.CultureInfo]::GetCultureInfo('tr-TR')
$rng = New-Object System.Random 20261019
function Noise([double]$pct) { 1 + (($rng.NextDouble() * 2 - 1) * $pct) }
function RoundTo([double]$v, [double]$step) { [math]::Round($v / $step) * $step }
function Q([string]$s) { if ($null -eq $s) { 'NULL' } else { "N'" + $s.Replace("'", "''") + "'" } }
function B($v) { if ($null -eq $v) { 'NULL' } elseif ($v) { '1' } else { '0' } }
function D($v) { if ($null -eq $v) { 'NULL' } else { ([decimal]$v).ToString([Globalization.CultureInfo]::InvariantCulture) } }
function TitleTr([string]$s) { $tr.TextInfo.ToTitleCase($s.ToLower($tr)) }

# Türkçe ek uyumu: son ünlüye göre ilgi (-in) ve bulunma (-de) eki; ünlüyle bitende kaynaştırma "n", sert ünsüzde "t"
function LastVowel([string]$w) { $l = $w.ToLower($tr); for ($i = $l.Length - 1; $i -ge 0; $i--) { if ('aeıioöuü'.Contains($l[$i])) { return $l[$i] } }; 'e' }
function Gen([string]$w) {
    $v = LastVowel $w; $s = @{ 'a'='ın'; 'ı'='ın'; 'e'='in'; 'i'='in'; 'o'='un'; 'u'='un'; 'ö'='ün'; 'ü'='ün' }[[string]$v]
    $endsVowel = 'aeıioöuü'.Contains($w.ToLower($tr)[-1])
    "$w'$(if ($endsVowel) { 'n' } else { '' })$s"
}
function Loc([string]$w) {
    # Rakamla bitenlerde okunuşa göre (Evka 3 → "üç" → 'te)
    $spoken = $w
    $digitWords = @{ '0'='sıfır'; '1'='bir'; '2'='iki'; '3'='üç'; '4'='dört'; '5'='beş'; '6'='altı'; '7'='yedi'; '8'='sekiz'; '9'='dokuz' }
    if ([char]::IsDigit($w[-1])) { $spoken = $digitWords[[string]$w[-1]] }
    $v = LastVowel $spoken; $s = if ('aıou'.Contains($v)) { 'da' } else { 'de' }
    if ('çfhkpsşt'.Contains($spoken.ToLower($tr)[-1])) { $s = 't' + $s.Substring(1) }
    "$w'$s"
}
function N0([double]$v) { $v.ToString('N0', $tr) }
# "Merkez" mahallesi tek başına belirsiz; ilçe adıyla birlikte yazılır
function PlaceName([string]$n, $d) { if ($n -eq 'Merkez') { "$($d.District) Merkez" } else { $n } }

# İlçe kimliği → (il, ilçe, il id, satış m², kira m²)
$districts = @{
    431 = @{ City='İstanbul'; District='Beşiktaş'; CityId=34; Sale=140000; Rent=520 }
    444 = @{ City='İstanbul'; District='Kadıköy'; CityId=34; Sale=125000; Rent=480 }
    452 = @{ City='İstanbul'; District='Silivri'; CityId=34; Sale=42000; Rent=170 }
    456 = @{ City='İstanbul'; District='Şişli'; CityId=34; Sale=100000; Rent=430 }
    460 = @{ City='İstanbul'; District='Zeytinburnu'; CityId=34; Sale=65000; Rent=290 }
    64  = @{ City='Ankara'; District='Çankaya'; CityId=6; Sale=47500; Rent=205 }
    69  = @{ City='Ankara'; District='Gölbaşı'; CityId=6; Sale=36000; Rent=150 }
    74  = @{ City='Ankara'; District='Keçiören'; CityId=6; Sale=30500; Rent=135 }
    78  = @{ City='Ankara'; District='Polatlı'; CityId=6; Sale=22000; Rent=95 }
    87  = @{ City='Antalya'; District='Döşemealtı'; CityId=7; Sale=42000; Rent=180 }
    96  = @{ City='Antalya'; District='Konyaaltı'; CityId=7; Sale=63000; Rent=275 }
    100 = @{ City='Antalya'; District='Muratpaşa'; CityId=7; Sale=57500; Rent=250 }
    202 = @{ City='Bursa'; District='Nilüfer'; CityId=16; Sale=48000; Rent=200 }
    344 = @{ City='Eskişehir'; District='Tepebaşı'; CityId=26; Sale=33000; Rent=140 }
    360 = @{ City='Giresun'; District='Espiye'; CityId=28; Sale=22000; Rent=95 }
    365 = @{ City='Giresun'; District='Merkez'; CityId=28; Sale=30000; Rent=125 }
    467 = @{ City='İzmir'; District='Bornova'; CityId=35; Sale=53000; Rent=218 }
    477 = @{ City='İzmir'; District='Karşıyaka'; CityId=35; Sale=64500; Rent=265 }
    489 = @{ City='İzmir'; District='Torbalı'; CityId=35; Sale=34000; Rent=145 }
    490 = @{ City='İzmir'; District='Urla'; CityId=35; Sale=70000; Rent=260 }
    525 = @{ City='Kayseri'; District='Kocasinan'; CityId=38; Sale=31000; Rent=130 }
    560 = @{ City='Kocaeli'; District='Kartepe'; CityId=41; Sale=36000; Rent=153 }
    657 = @{ City='Muğla'; District='Bodrum'; CityId=48; Sale=112000; Rent=430 }
}
$premium = @('BEBEK','ETİLER','LEVENT','FENERBAHÇE','SUADİYE','CAFERAĞA','ÇUKURAMBAR','ÜMİT','MAVİŞEHİR','BOSTANLI','YALIKAVAK','TÜRKBÜKÜ','TEŞVİKİYE','ÖZLÜCE','ŞİRİNYALI','HURMA')
function NFactor([string]$n) { if ($premium -contains $n.ToUpper($tr)) { 1.22 } else { 1.0 } }

$agents = 1, 2, 1002, 1003
$img = @{
    living = 1..9; kitchen = 1..12; bedroom = 1..8; bathroom = 1..7; building = 1..2; modernbuilding = 1..4; villa = 1..5
    office = 1..4; officebuilding = 1..5; cafe = 1..5; shopinterior = 1..1; warehouse = 1..1; land = 1..8; olive = 1..1
}
$imgCursor = @{}
function Pick([string]$cat) {
    if (-not $imgCursor.ContainsKey($cat)) { $imgCursor[$cat] = 0 }
    $list = $img[$cat]; $i = $list[$imgCursor[$cat] % $list.Count]; $imgCursor[$cat]++
    "/images/listings/{0}-{1:D2}.jpg" -f $cat, $i
}
function ImageSet([string]$kind) {
    switch ($kind) {
        'apartment' { @((Pick 'modernbuilding'), (Pick 'living'), (Pick 'kitchen'), (Pick 'bedroom'), (Pick 'living'), (Pick 'bathroom')) }
        'classic'   { @((Pick 'building'), (Pick 'living'), (Pick 'kitchen'), (Pick 'bedroom'), (Pick 'bathroom')) }
        'villa'     { @((Pick 'villa'), (Pick 'villa'), (Pick 'living'), (Pick 'kitchen'), (Pick 'bedroom'), (Pick 'bathroom')) }
        'office'    { @((Pick 'officebuilding'), (Pick 'office'), (Pick 'office'), (Pick 'kitchen')) }
        'shop'      { @((Pick 'cafe'), (Pick 'cafe'), (Pick 'shopinterior')) }
        'warehouse' { @((Pick 'warehouse'), (Pick 'officebuilding')) }
        'land'      { @((Pick 'land'), (Pick 'land')) }
        'olive'     { @((Pick 'olive'), (Pick 'land'), (Pick 'land')) }
    }
}
function ImgSql([string[]]$urls) {
    $parts = @()
    for ($k = 1; $k -le 30; $k++) { $u = if ($k -le $urls.Count) { $urls[$k - 1] } else { $null }; $parts += "PropImgUrl$k = $(Q $u)" }
    $parts -join ', '
}
$created = [datetime]'2026-10-17'
function NextDate { $script:created = $script:created.AddHours(-1 * (18 + $rng.Next(0, 30))); $script:created.ToString('yyyy-MM-dd HH:mm:ss') }
$propNo = 2610000
function NextNo { $script:propNo += 7 + $rng.Next(0, 13); $script:propNo }

function AgeBucket([int]$age) {
    if ($age -le 0) { '0' } elseif ($age -le 5) { '1-5' } elseif ($age -le 10) { '6-10' } elseif ($age -le 15) { '11-15' } elseif ($age -le 20) { '16-20' } else { '21 ve üzeri' }
}
function AgeFactor([int]$age) { if ($age -le 0) { 1.12 } elseif ($age -le 5) { 1.06 } elseif ($age -le 10) { 1.0 } elseif ($age -le 20) { 0.92 } else { 0.84 } }

$sql = New-Object System.Collections.Generic.List[string]
$sql.Add("-- Üretildi: tools/sql/demo-listings-refresh.ps1 ($(Get-Date -Format 'yyyy-MM-dd HH:mm')). Elle düzenlemeyin; betiği değiştirip yeniden üretin.")
$sql.Add("SET NOCOUNT ON; SET XACT_ABORT ON; BEGIN TRAN;")
$amen = New-Object System.Collections.Generic.List[string]

# ---------------------------------------------------------------- Konut (satılık + kiralık)
# id, ilçe, mahalle (DB adı), oda, brüt m², yaş, kat, bina kat, site, eşyalı, kategori, görsel tipi
$saleHousing = @(
    @(6, 460, 'VELİEFENDİ', '3+1', 145, 8, 4, 8, $false, $false, 1, 'classic'),
    @(1000001, 444, 'CAFERAĞA', '2+1', 95, 32, 3, 5, $false, $false, 1, 'classic'),
    @(1000002, 444, 'FENERBAHÇE', '3+1', 150, 6, 5, 9, $true, $false, 1, 'apartment'),
    @(1000003, 444, 'GÖZTEPE', '3+1', 135, 14, 2, 7, $false, $false, 1, 'apartment'),
    @(1000004, 444, 'SUADİYE', '4+1', 190, 3, 8, 12, $true, $false, 5, 'apartment'),
    @(1000005, 460, 'KAZLIÇEŞME', '2+1', 105, 2, 6, 15, $true, $false, 2, 'apartment'),
    @(1000006, 460, 'MERKEZEFENDİ', '3+1', 125, 18, 1, 6, $false, $false, 1, 'classic'),
    @(1000007, 460, 'SÜMER', '1+1', 65, 0, 4, 10, $true, $true, 2, 'apartment'),
    @(1000008, 64, 'ÇUKURAMBAR', '3+1', 160, 4, 11, 22, $true, $false, 2, 'apartment'),
    @(1000009, 64, 'AYRANCI', '3+1', 140, 26, 2, 5, $false, $false, 1, 'classic'),
    @(1000010, 64, 'BAHÇELİEVLER', '4+1', 175, 12, 3, 6, $false, $false, 1, 'classic'),
    @(1000011, 64, 'ÜMİT', '4+1', 210, 7, 2, 4, $true, $false, 5, 'apartment'),
    @(1000012, 64, 'ORAN', '2+1', 110, 1, 9, 18, $true, $false, 1, 'apartment'),
    @(1000013, 477, 'BOSTANLI', '3+1', 140, 10, 4, 8, $false, $false, 1, 'apartment'),
    @(1000014, 477, 'MAVİŞEHİR', '3+1', 155, 9, 12, 20, $true, $false, 1, 'apartment'),
    @(1000015, 477, 'ALAYBEY', '2+1', 90, 24, 3, 5, $false, $false, 1, 'classic'),
    @(1000016, 100, 'ŞİRİNYALI', '3+1', 150, 5, 2, 6, $true, $false, 1, 'apartment'),
    @(1000017, 100, 'GÜZELOBA', '2+1', 100, 2, 1, 4, $true, $true, 1, 'apartment'),
    @(1000018, 202, 'ÖZLÜCE', '3+1', 145, 6, 7, 14, $true, $false, 1, 'apartment'),
    @(1000019, 202, 'ATAEVLER', '4+1', 170, 15, 4, 8, $false, $false, 1, 'classic'),
    @(1000020, 657, 'YALIKAVAK', '4+1', 240, 4, 1, 2, $true, $false, 3, 'villa')
)
$rentHousing = @(
    @(1, 460, 'TELSİZ', '3+1', 130, 12, 3, 7, $false, $false, 1, 'classic'),
    @(1000001, 431, 'LEVENT', '2+1', 110, 6, 14, 30, $true, $true, 2, 'apartment'),
    @(1000002, 431, 'ETİLER', '3+1', 165, 9, 3, 6, $true, $false, 1, 'apartment'),
    @(1000003, 431, 'BEBEK', '2+1', 120, 35, 2, 4, $false, $true, 1, 'classic'),
    @(1000004, 431, 'ABBASAĞA', '2+1', 85, 28, 4, 5, $false, $false, 1, 'classic'),
    @(1000005, 431, 'SİNANPAŞA', '1+1', 60, 15, 2, 6, $false, $true, 1, 'classic'),
    @(1000006, 431, 'AKAT', '3+1', 175, 3, 6, 12, $true, $false, 1, 'apartment'),
    @(1000007, 460, 'KAZLIÇEŞME', '2+1', 100, 4, 9, 18, $true, $false, 2, 'apartment'),
    @(1000008, 460, 'VELİEFENDİ', '3+1', 125, 16, 2, 6, $false, $false, 1, 'classic'),
    @(1000009, 460, 'YENİDOĞAN', '2+1', 90, 22, 1, 5, $false, $false, 1, 'classic'),
    @(1000010, 467, 'KAZIMDİRİK', '1+1', 55, 1, 3, 8, $false, $true, 1, 'apartment'),
    @(1000011, 467, 'ERZENE', '2+1', 85, 12, 2, 5, $false, $true, 1, 'classic'),
    @(1000012, 467, 'EVKA 3', '3+1', 120, 20, 4, 6, $true, $false, 1, 'classic'),
    @(1000013, 467, 'MEVLANA', '3+1', 135, 3, 5, 10, $true, $false, 1, 'apartment'),
    @(1000014, 74, 'ETLİK', '3+1', 130, 10, 3, 8, $false, $false, 1, 'classic'),
    @(1000015, 74, 'KALABA', '2+1', 95, 18, 2, 5, $false, $false, 1, 'classic'),
    @(1000016, 344, 'BATIKENT', '3+1', 140, 5, 4, 9, $true, $false, 1, 'apartment'),
    @(1000017, 344, 'HOŞNUDİYE', '1+1', 60, 2, 3, 6, $false, $true, 9, 'apartment'),
    @(1000018, 344, 'ULUÖNDER', '2+1', 95, 14, 1, 5, $false, $false, 1, 'classic'),
    @(1000019, 344, 'SÜTLÜCE', '3+1', 125, 8, 2, 7, $false, $false, 1, 'apartment'),
    @(1000020, 96, 'HURMA', '2+1', 105, 2, 1, 4, $true, $true, 1, 'apartment')
)
$coastal = @('BOSTANLI','MAVİŞEHİR','ALAYBEY','ŞİRİNYALI','GÜZELOBA','YALIKAVAK','HURMA','BEBEK','FENERBAHÇE','SUADİYE','CAFERAĞA','KAZLIÇEŞME')
$catName = @{ 1='Daire'; 2='Rezidans'; 3='Villa'; 4='Müstakil Ev'; 5='Dubleks'; 9='Stüdyo' }

function HousingRecord($r, [bool]$isRent) {
    $id = $r[0]; $d = $districts[$r[1]]; $nRaw = $r[2]; $n = TitleTr $nRaw; $rooms = $r[3]; $gross = [double]$r[4]; $age = [int]$r[5]
    $floor = [int]$r[6]; $totalFloors = [int]$r[7]; $site = [bool]$r[8]; $furn = [bool]$r[9]; $cat = [int]$r[10]; $kind = $r[11]
    $net = [math]::Round($gross * (0.82 + $rng.NextDouble() * 0.06))
    $roomCount = [int]($rooms.Split('+')[0])
    $baths = if ($gross -ge 170) { 3 } elseif ($gross -ge 120) { 2 } else { 1 }
    $balc = if ($gross -ge 150) { 2 } elseif ($gross -ge 80) { 1 } else { 0 }
    $heating = if ($site -and $age -le 6) { 'Yerden Isıtma' } elseif ($age -ge 21) { 'Merkezi (Pay Ölçer)' } else { 'Doğalgaz (Kombi)' }
    if ($r[1] -eq 657 -or $r[1] -eq 96) { $heating = 'Klima' }
    $facades = 'Güney', 'Güney-Batı', 'Güney-Doğu', 'Batı', 'Doğu', 'Kuzey-Batı'; $facade = $facades[$rng.Next(0, $facades.Count)]
    $elevator = $totalFloors -ge 5; $parking = $site -or $age -le 10
    $factor = (NFactor $nRaw) * (AgeFactor $age) * $(if ($site) { 1.08 } else { 1.0 }) * (Noise 0.06)
    $dues = if ($site) { RoundTo ($gross * (11 + $rng.Next(0, 9))) 50 } elseif ($elevator) { RoundTo ($gross * 5) 50 } else { $null }
    $ageText = AgeBucket $age
    $catTxt = $catName[$cat]
    $view = if ($coastal -contains $nRaw -and $floor -ge 3) { 'deniz manzaralı' } elseif ($floor -ge 8) { 'şehir manzaralı' } else { $null }
    $usage = if ($isRent) { 'Boş' } else { @('Boş', 'Mülk Sahibi', 'Kiracılı')[$rng.Next(0, 3)] }

    $titleParts = @()
    $titleParts += (Loc $n)
    if ($site) { $titleParts += 'site içinde' }
    if ($view) { $titleParts += $view }
    if ($furn -and $isRent) { $titleParts += 'eşyalı' }
    $titleParts += "$rooms $(if ($cat -eq 1) { 'daire' } else { $catTxt.ToLower($tr) })"
    if ($age -le 1) { $titleParts = @($titleParts[0], 'yeni binada') + $titleParts[1..($titleParts.Count - 1)] }
    $title = (($titleParts -join ' ').Substring(0, 1).ToUpper($tr)) + ($titleParts -join ' ').Substring(1)
    $title = "$title - $(if ($isRent) { 'Kiralık' } else { 'Satılık' })"

    $floorTxt = if ($floor -eq 0) { 'giriş katında' } else { "$floor. katında" }
    $p1 = "$(Gen $d.District) $n Mahallesi'nde, $totalFloors katlı binanın $floorTxt yer alan $gross m² brüt ($net m² net) kullanım alanına sahip $rooms $($catTxt.ToLower($tr)). Konut $(if ($age -eq 0) { 'yeni tamamlanmış bir binada' } else { "$age yıllık bir binada" }) bulunuyor ve $($facade.ToLower($tr)) cepheli olduğu için gün boyu ışık alıyor."
    $p2parts = @("$roomCount yatak odası", "$baths banyo")
    if ($balc -gt 0) { $p2parts += "$balc balkon" }
    $p2 = "Dairede $($p2parts -join ', ') bulunuyor; ısınma $($heating.ToLower($tr)) ile sağlanıyor."
    if ($site) { $p2 += ' Site içinde 7/24 güvenlik, otopark ve sosyal alanlar mevcut.' } elseif ($parking) { $p2 += ' Binada otopark imkânı var.' }
    if ($elevator) { $p2 += ' Bina asansörlü.' }
    if ($view) { $p2 += " Üst katlardan $view." }
    $p3 = if ($isRent) { "Kira bedeli aylık olup $(if ($furn) { 'daire eşyalı teslim edilir' } else { 'daire boş teslim edilir' }). Depozito ve sözleşme koşulları için danışmanımızla görüşebilirsiniz." } else { "Tapu durumu kat mülkiyetli$(if ($age -le 10) { ', konut kredisine uygundur' } else { '' }). Detaylı bilgi ve yerinde görme randevusu için danışmanımızla iletişime geçebilirsiniz." }
    $desc = "$p1`n`n$p2`n`n$p3"
    $addr = "$n Mahallesi, $($d.District) / $($d.City)"
    $images = ImageSet $kind
    $agent = $agents[$rng.Next(0, $agents.Count)]

    if ($isRent) {
        $rent = RoundTo ($d.Rent * $gross * $factor * $(if ($furn) { 1.12 } else { 1.0 })) 500
        $deposit = $rent * $(if ($rng.Next(0, 2) -eq 0) { 1 } else { 2 })
        $set = @("PropertyName = $(Q $title)", "PropertyDescription = $(Q $desc)", "PropertyNo = $(NextNo)", "CreatedDate = '$(NextDate)'",
            "City = $(Q $d.City)", "District = $(Q $d.District)", "CityId = $($d.CityId)", "DistrictId = $($r[1])", "Neighborhood = $(Q $n)", "AddressDesc = $(Q $addr)",
            "Rent = $(D $rent)", "Deposit = $(D $deposit)", "GrossArea = $gross", "NetArea = $net", "OpenArea = NULL", "NumberOfRoom = $(Q $rooms)",
            "NumberOfBathRoom = $baths", "NumberOfBalconies = $balc", "BuildingAge = $(Q $ageText)", "TotalNumberOfFloor = $totalFloors", "NumberOfFloors = $floor",
            "Heating = $(Q $heating)", "Facade = $(Q $facade)", "IsElevator = $(B $elevator)", "ParkingLot = $(B $parking)", "Furnished = $(B $furn)",
            "WithinTheComplex = $(B $site)", "Dues = $(D $dues)", "BlackBox = NULL", "HousingCategoryId = $cat", "AgentId = $agent", "PropertyStatus = N'Aktif'", (ImgSql $images))
        $sql.Add("UPDATE dbo.rentalHousingListings SET $($set -join ', ') WHERE RentalHousingListId = $id;")
        $table = 'RentalHousingListingAmenities'; $col = 'RentalHousingListId'
    } else {
        $price = RoundTo ($d.Sale * $gross * $factor) 10000
        $loan = $age -le 15
        $set = @("PropertyName = $(Q $title)", "PropertyDescription = $(Q $desc)", "PropertyNo = $(NextNo)", "CreatedDate = '$(NextDate)'",
            "City = $(Q $d.City)", "District = $(Q $d.District)", "CityId = $($d.CityId)", "DistrictId = $($r[1])", "Neighborhood = $(Q $n)", "AddressDesc = $(Q $addr)",
            "Price = $(D $price)", "GrossArea = $gross", "NetArea = $net", "OpenArea = $(if ($kind -eq 'villa') { 380 } else { 'NULL' })", "NumberOfRoom = $(Q $rooms)",
            "NumberOfBathRoom = $baths", "NumberOfBalconies = $balc", "BuildingAge = $(Q $ageText)", "TotalNumberOfFloor = $totalFloors", "NumberOfFloors = $floor",
            "Heating = $(Q $heating)", "Facade = $(Q $facade)", "IsElevator = $(B $elevator)", "ParkingLot = $(B $parking)", "Furnished = $(B $furn)",
            "UsageStatus = $(Q $usage)", "WithinTheComplex = $(B $site)", "Dues = $(D $dues)", "Exchange = $(B ($rng.Next(0, 4) -eq 0))",
            "TitleDeedStatus = N'Kat Mülkiyetli'", "HomeLoan = $(B $loan)", "BlackBox = NULL", "HousingCategoryId = $cat", "AgentId = $agent", "PropertyStatus = N'Aktif'", (ImgSql $images))
        $sql.Add("UPDATE dbo.forSaleHousingPropertyListings SET $($set -join ', ') WHERE ForSaleHousingListId = $id;")
        $table = 'ForSaleHousingListingAmenities'; $col = 'ForSaleHousingListId'
    }
    # Özellikler (AmenityDefinitions): ilanın kendi alanlarından türetilir
    $a = @()
    if ($elevator) { $a += 3 }
    if ($site) { $a += 20, 5, 26 } elseif ($parking) { $a += 21 }
    if ($site -and $gross -ge 130) { $a += 24, 25 }
    if ($site) { $a += 22 }
    if ($heating -eq 'Doğalgaz (Kombi)') { $a += 12 } elseif ($heating -eq 'Yerden Isıtma') { $a += 13 } elseif ($heating -like 'Merkezi*') { $a += 2 }
    if ($heating -eq 'Klima' -or $age -le 5) { $a += 1 }
    if ($baths -ge 2) { $a += 14 }
    if ($balc -gt 0) { $a += 15 }
    if ($kind -eq 'villa' -or ($floor -eq $totalFloors - 1 -and $gross -ge 150)) { $a += 16 }
    if ($furn) { $a += 17 }
    if ($age -le 12) { $a += 18 }
    if ($view -eq 'deniz manzaralı') { $a += 7 } elseif ($view -eq 'şehir manzaralı') { $a += 28 }
    foreach ($am in ($a | Select-Object -Unique)) { $amen.Add("INSERT INTO dbo.$table ($col, AmenityId) VALUES ($id, $am);") }
}
$sql.Add("DELETE FROM dbo.ForSaleHousingListingAmenities; DELETE FROM dbo.RentalHousingListingAmenities;")
foreach ($r in $saleHousing) { HousingRecord $r $false }
foreach ($r in $rentHousing) { HousingRecord $r $true }

# ---------------------------------------------------------------- Arsa
# id, ilçe, mahalle, m², imar (ZoningStatus), LandCategoryId, m² fiyatı (satış) veya aylık kira, KAKS, TAKS, plan notu, görsel
$saleLand = @(
    @(2000001, 560, 'MAŞUKİYE', 750, 'Konut İmarlı', 2, 9200, 0.60, 0.30, 'Ayrık nizam, 2 kat (Hmax 6,50 m)', 'land'),
    @(2000002, 452, 'SELİMPAŞA', 620, 'Konut İmarlı', 2, 14500, 1.05, 0.35, 'Ayrık nizam, 3 kat', 'land'),
    @(2000003, 69, 'İNCEK', 540, 'Konut İmarlı', 2, 12500, 0.75, 0.25, 'Villa imarlı, 2 kat', 'land'),
    @(2000004, 490, 'ZEYTİNALANI', 8600, 'Zeytinlik', 9, 1650, $null, $null, 'Tarım arazisi, imar planı dışında', 'olive'),
    @(2000005, 87, 'YEŞİLBAYIR', 5200, 'Tarla', 7, 1350, $null, $null, 'Tarım arazisi, imar planı dışında', 'land'),
    @(2000006, 489, 'AYRANCILAR', 2400, 'Sanayi İmarlı', 5, 9400, 1.00, 0.55, 'Sanayi alanı, ayrık nizam', 'land')
)
$rentLand = @(
    @(2000001, 452, 'GÜMÜŞYAKA', 5000, 'Tarla', 7, 3000, $null, $null, 'Tarım arazisi', 'land'),
    @(2000002, 78, 'GÜLVEREN', 20000, 'Tarla', 7, 6500, $null, $null, 'Tarım arazisi, sulama kanalına yakın', 'land'),
    @(2000003, 489, 'TORBALI', 3200, 'Sanayi İmarlı', 5, 48000, 0.80, 0.50, 'Açık depolama / otopark kullanımına uygun', 'land')
)
function LandRecord($r, [bool]$isRent) {
    $id = $r[0]; $d = $districts[$r[1]]; $n = TitleTr $r[2]; $area = [double]$r[3]; $zoning = $r[4]; $lcat = $r[5]; $val = [double]$r[6]
    $kaks = $r[7]; $taks = $r[8]; $plan = $r[9]
    $addr = "$n Mahallesi, $($d.District) / $($d.City)"
    $images = ImageSet $r[10]
    $agent = $agents[$rng.Next(0, $agents.Count)]
    $ada = 100 + $rng.Next(0, 800); $parsel = 1 + $rng.Next(0, 60); $pafta = "$(@('F','G','H','I','J')[$rng.Next(0,5)])$(20 + $rng.Next(0,15))-$(@('a','b','c','d')[$rng.Next(0,4)])-$(1 + $rng.Next(0,4))"
    $zLower = $zoning.ToLower($tr)
    if ($isRent) {
        $rent = RoundTo ($val * (Noise 0.05)) 500
        $title = "$(Loc $n) $(N0 $area) m² $zLower$(if ($zoning -ne 'Tarla') { ' arazi' } else { '' }) - Kiralık"
        $desc = "$(Gen $d.District) $n Mahallesi'nde $(N0 $area) m² yüzölçümlü, $zLower vasıflı arazi kiralıktır. $plan.`n`nArazi uzun dönem kiralamaya uygundur; kira bedeli aylıktır ve sözleşme süresi, ödeme planı ile kullanım koşulları için danışmanımızla görüşebilirsiniz.`n`nAda/parsel bilgisi ve yerinde görme randevusu ofisimiz üzerinden paylaşılır."
        $set = @("PropertyName = $(Q $title)", "PropertyDescription = $(Q $desc)", "PropertyNo = $(NextNo)", "CreatedDate = '$(NextDate)'",
            "City = $(Q $d.City)", "District = $(Q $d.District)", "CityId = $($d.CityId)", "DistrictId = $($r[1])", "Neighborhood = $(Q $n)", "AddressDesc = $(Q $addr)",
            "Area = $area", "Rent = $(D $rent)", "PricePerSquareMeter = $(D ([math]::Round($rent / $area, 2)))", "ZoningStatus = $(Q $zoning)", "LandCategoryId = $lcat",
            "FloorAreaRatio = $(D $kaks)", "BaseAreaRatio = $(D $taks)", "ZoningPlan = $(Q $plan)", "TitleDeedStatus = N'Müstakil Tapulu'", "DevelopmentRight = NULL",
            "ParcelNumber = N'$parsel'", "PlotNumber = N'$ada'", "MapSheetNumber = N'$pafta'", "AgentId = $agent", "PropertyStatus = N'Aktif'", (ImgSql $images))
        $sql.Add("UPDATE dbo.rentalLandListings SET $($set -join ', ') WHERE RentalLandListingId = $id;")
        $table = 'RentalLandListingAmenities'; $col = 'RentalLandListingId'
    } else {
        $ppsqm = RoundTo ($val * (Noise 0.05)) 50
        $price = RoundTo ($ppsqm * $area) 10000
        $title = "$(Loc $n) $(N0 $area) m² $zLower arsa - Satılık"
        if ($zoning -eq 'Zeytinlik' -or $zoning -eq 'Tarla') { $title = "$(Loc $n) $(N0 $area) m² $zLower - Satılık" }
        $imarTxt = if ($kaks) { "Emsal (KAKS) $($kaks.ToString('0.00', $tr)), TAKS $($taks.ToString('0.00', $tr)); $plan." } else { "$plan." }
        $desc = "$(Gen $d.District) $n Mahallesi'nde $(N0 $area) m² yüzölçümlü, $zLower arsa satılıktır. $imarTxt`n`nArsa $(if ($kaks) { 'yola cepheli olup altyapı hizmetlerine yakındır' } else { 'tarımsal kullanıma ve hobi bahçesine uygundur' }). Tapu müstakil tapuludur$(if ($zoning -eq 'Konut İmarlı') { '; arsa kredisine uygundur' } else { '' }).`n`nAda/parsel bilgisi, imar durum belgesi ve yerinde görme randevusu ofisimiz üzerinden paylaşılır."
        $set = @("PropertyName = $(Q $title)", "PropertyDescription = $(Q $desc)", "PropertyNo = $(NextNo)", "CreatedDate = '$(NextDate)'",
            "City = $(Q $d.City)", "District = $(Q $d.District)", "CityId = $($d.CityId)", "DistrictId = $($r[1])", "Neighborhood = $(Q $n)", "AddressDesc = $(Q $addr)",
            "Area = $area", "Price = $(D $price)", "PricePerSquareMeter = $(D $ppsqm)", "ZoningStatus = $(Q $zoning)", "LandCategoryId = $lcat",
            "FloorAreaRatio = $(D $kaks)", "BaseAreaRatio = $(D $taks)", "ZoningPlan = $(Q $plan)", "TitleDeedStatus = N'Müstakil Tapulu'", "SharePercentage = NULL",
            "DevelopmentRight = $(B ([bool]$kaks))", "LandLoan = $(B ($zoning -eq 'Konut İmarlı'))", "Exchange = $(B ($rng.Next(0,3) -eq 0))",
            "ParcelNumber = N'$parsel'", "PlotNumber = N'$ada'", "MapSheetNumber = N'$pafta'", "AgentId = $agent", "PropertyStatus = N'Aktif'", (ImgSql $images))
        $sql.Add("UPDATE dbo.forSaleLandListings SET $($set -join ', ') WHERE ForSaleLandListingId = $id;")
        $table = 'ForSaleLandListingAmenities'; $col = 'ForSaleLandListId'
    }
    $a = @(31)
    if ($kaks) { $a += 9 }
    if ($rng.Next(0, 3) -eq 0) { $a += 8 }
    foreach ($am in $a) { $amen.Add("INSERT INTO dbo.$table ($col, AmenityId) VALUES ($id, $am);") }
}
$sql.Add("DELETE FROM dbo.ForSaleLandListingAmenities; DELETE FROM dbo.RentalLandListingAmenities;")
foreach ($r in $saleLand) { LandRecord $r $false }
foreach ($r in $rentLand) { LandRecord $r $true }

# ---------------------------------------------------------------- İşyeri
# id, ilçe, mahalle, tür (office/shop/warehouse), brüt m², bölüm, kat sayısı, yaş, cephe
$saleCommercial = @(
    @(3, 456, 'MECİDİYEKÖY', 'office', 420, 8, 1, 12, 'Güney'),
    @(1002, 444, 'CAFERAĞA', 'shop', 120, 2, 1, 30, 'Caddeye Bakan'),
    @(3000001, 444, 'KOZYATAĞI', 'office', 180, 5, 1, 8, 'Batı'),
    @(3000002, 460, 'MERKEZEFENDİ', 'shop', 95, 1, 1, 15, 'Caddeye Bakan'),
    @(3000003, 64, 'KIZILAY', 'office', 160, 6, 1, 25, 'Güney'),
    @(3000004, 477, 'BOSTANLI', 'shop', 85, 1, 1, 10, 'Caddeye Bakan'),
    @(3000005, 100, 'FENER', 'shop', 140, 2, 2, 6, 'Caddeye Bakan'),
    @(3000006, 202, 'BEŞEVLER', 'office', 220, 7, 1, 4, 'Güney-Doğu'),
    @(3000007, 657, 'GÜMBET', 'shop', 160, 2, 1, 9, 'Caddeye Bakan')
)
$rentCommercial = @(
    @(2, 431, 'LEVENT', 'office', 150, 5, 1, 7, 'Kuzey'),
    @(3, 525, 'SAHABİYE', 'shop', 155, 2, 1, 14, 'Caddeye Bakan'),
    @(4, 365, 'HACI HÜSEYİN', 'shop', 195, 2, 2, 18, 'Caddeye Bakan'),
    @(5, 360, 'MERKEZ', 'shop', 170, 2, 1, 11, 'Caddeye Bakan'),
    @(3000001, 431, 'GAYRETTEPE', 'office', 210, 7, 1, 9, 'Güney'),
    @(3000002, 460, 'KAZLIÇEŞME', 'warehouse', 650, 3, 1, 20, 'Doğu'),
    @(3000003, 467, 'ERZENE', 'shop', 110, 1, 1, 6, 'Caddeye Bakan'),
    @(3000004, 74, 'ETLİK', 'shop', 90, 1, 1, 16, 'Caddeye Bakan'),
    @(3000005, 344, 'HOŞNUDİYE', 'office', 135, 4, 1, 3, 'Batı'),
    @(3000006, 96, 'LİMAN', 'shop', 120, 2, 1, 5, 'Caddeye Bakan')
)
function CommercialRecord($r, [bool]$isRent) {
    $id = $r[0]; $d = $districts[$r[1]]; $nRaw = $r[2]; $n = TitleTr $nRaw; $type = $r[3]; $gross = [double]$r[4]; $sections = [int]$r[5]
    $floors = [int]$r[6]; $age = [int]$r[7]; $facade = $r[8]
    $net = [math]::Round($gross * 0.88)
    $typeTxt = @{ office = 'ofis'; shop = 'dükkan'; warehouse = 'depo' }[$type]
    $mult = @{ office = 1.25; shop = 1.45; warehouse = 0.55 }[$type]
    $rentMult = @{ office = 1.55; shop = 1.9; warehouse = 0.6 }[$type]
    $factor = (NFactor $nRaw) * (AgeFactor $age) * (Noise 0.07)
    $heating = if ($type -eq 'office' -and $age -le 10) { 'Merkezi (Klima)' } elseif ($type -eq 'office') { 'Merkezi' } elseif ($type -eq 'warehouse') { 'Yok' } else { 'Klima' }
    $baths = if ($gross -ge 200) { 2 } else { 1 }
    $kitchens = if ($type -eq 'office') { 1 } else { 0 }
    $addr = "$n Mahallesi, $($d.District) / $($d.City)"
    $images = ImageSet $(if ($type -eq 'office') { 'office' } elseif ($type -eq 'warehouse') { 'warehouse' } else { 'shop' })
    $agent = $agents[$rng.Next(0, $agents.Count)]
    $onStreet = $facade -eq 'Caddeye Bakan'
    $title = "$(Loc (PlaceName $n $d)) $(if ($onStreet) { 'cadde üzeri ' } else { '' })$gross m² $typeTxt - $(if ($isRent) { 'Kiralık' } else { 'Satılık' })"
    $p1 = "$(Gen $d.District) $n Mahallesi'nde, $gross m² brüt ($net m² net) kullanım alanına sahip $(if ($sections -gt 1) { "$sections bölümlü " } else { '' })$typeTxt. $(if ($onStreet) { 'Cadde cepheli olup yaya ve araç trafiği yüksek bir noktada yer alıyor.' } else { "$($facade.ToLower($tr)) cepheli, aydınlık bir kullanım alanı sunuyor." })"
    $p2 = switch ($type) {
        'office' { "Ofis $(if ($age -le 10) { 'yeni nesil bir iş merkezinde' } else { 'merkezi konumdaki bir iş hanında' }); $baths WC ve mutfak bölümü bulunuyor, ısıtma-soğutma $($heating.ToLower($tr)) ile sağlanıyor. Binada asansör ve otopark mevcut." }
        'shop' { "Dükkan $(if ($floors -gt 1) { "$floors katlı olup " } else { '' })vitrin cephesine, $baths WC'ye ve depo alanına sahip; perakende, kafe-restoran veya hizmet sektörüne uygun." }
        'warehouse' { "Depo yüksek tavanlı, tır yanaşmasına uygun giriş ve geniş manevra alanına sahip; lojistik ve stoklama kullanımına uygun." }
    }
    $p3 = if ($isRent) { 'Kira bedeli aylıktır (KDV hariç). Depozito, sözleşme süresi ve kira artış koşulları için danışmanımızla görüşebilirsiniz.' } else { 'Tapu kat mülkiyetlidir. Detaylı bilgi, kira getirisi ve yerinde görme randevusu için danışmanımızla iletişime geçebilirsiniz.' }
    $desc = "$p1`n`n$p2`n`n$p3"
    if ($isRent) {
        $rent = RoundTo ($d.Rent * $rentMult * $gross * $factor) 500
        $deposit = $rent * 2
        $set = @("PropertyName = $(Q $title)", "PropertyDescription = $(Q $desc)", "PropertyNo = $(NextNo)", "CreatedDate = '$(NextDate)'",
            "City = $(Q $d.City)", "District = $(Q $d.District)", "CityId = $($d.CityId)", "DistrictId = $($r[1])", "Neighborhood = $(Q $n)", "AddressDesc = $(Q $addr)",
            "Rent = $(D $rent)", "Deposit = $(D $deposit)", "GrossArea = $gross", "NetArea = $net", "NumberOfSection = $sections", "NumberOfKitchens = $kitchens",
            "NumberOfBathrooms = $baths", "NumberOfFloors = $floors", "BuildingAge = $(Q (AgeBucket $age))", "Heating = $(Q $heating)", "Facade = $(Q $facade)",
            "TitleDeedStatus = N'Kat Mülkiyetli'", "AgentId = $agent", "PropertyStatus = N'Aktif'", (ImgSql $images))
        $sql.Add("UPDATE dbo.rentalCommercialPropertyListings SET $($set -join ', ') WHERE RentalCommercialListId = $id;")
        $table = 'RentalCommercialListingAmenities'; $col = 'RentalCommercialListId'
    } else {
        $price = RoundTo ($d.Sale * $mult * $gross * $factor) 10000
        $set = @("PropertyName = $(Q $title)", "PropertyDescription = $(Q $desc)", "PropertyNo = $(NextNo)", "CreatedDate = '$(NextDate)'",
            "City = $(Q $d.City)", "District = $(Q $d.District)", "CityId = $($d.CityId)", "DistrictId = $($r[1])", "Neighborhood = $(Q $n)", "AddressDesc = $(Q $addr)",
            "Price = $(D $price)", "GrossArea = $gross", "Area = $net", "NumberOfSection = $sections", "NumberOfKitchens = $kitchens", "NumberOfBathrooms = $baths",
            "NumberOfFloors = $floors", "Facade = $(Q $facade)", "TitleDeedStatus = N'Kat Mülkiyetli'", "SharePercentage = NULL", "LandLoan = 0",
            "Exchange = $(B ($rng.Next(0,4) -eq 0))", "Transferable = $(B ($type -eq 'shop' -and $rng.Next(0,3) -eq 0))", "AgentId = $agent", "PropertyStatus = N'Aktif'", (ImgSql $images))
        $sql.Add("UPDATE dbo.forSaleCommercialPropertyListings SET $($set -join ', ') WHERE ForSaleCommercialListingId = $id;")
        $table = 'ForSaleCommercialListingAmenities'; $col = 'ForSaleCommercialListingId'
    }
    $a = @(18)
    if ($type -eq 'office') { $a += 3, 20, 1, 6 }
    if ($type -eq 'shop') { $a += 1, 34; if ($onStreet) { $a += 33 } }
    if ($type -eq 'warehouse') { $a += 21, 34, 6 }
    foreach ($am in ($a | Select-Object -Unique)) { $amen.Add("INSERT INTO dbo.$table ($col, AmenityId) VALUES ($id, $am);") }
}
$sql.Add("DELETE FROM dbo.ForSaleCommercialListingAmenities; DELETE FROM dbo.RentalCommercialListingAmenities;")
foreach ($r in $saleCommercial) { CommercialRecord $r $false }
foreach ($r in $rentCommercial) { CommercialRecord $r $true }

foreach ($line in $amen) { $sql.Add($line) }

# Adresi değişen ilanların koordinatları yeniden üretilecek: tüm geo noktaları (yanlış konumdaki 2 manuel pin dahil) silinir;
# listing-geocode sağlayıcısı mahalle düzeyinde yeniden konumlar. Yakın yerler ayrıca nearby betiğiyle doldurulur.
$sql.Add("DELETE FROM dbo.ListingGeoPoints;")
$sql.Add("DELETE FROM dbo.ListingNearbyPlaces;")
$sql.Add("COMMIT;")

$out = Join-Path $PSScriptRoot 'demo-listings-refresh.sql'
[IO.File]::WriteAllLines($out, $sql, (New-Object Text.UTF8Encoding($true)))
Write-Output "Yazıldı: $out ($($sql.Count) satır, $($amen.Count) özellik ataması)"
