# GÖREV: Ana Sayfa Arama Modülü — Kademeli İl/İlçe + Dinamik Gelişmiş Arama (Düzeltme ve Tamamlama)

> Bu prompt, ekran kaydı (video) ve projenin kaynak kodu incelenerek hazırlandı. Satır numaraları 2026-10-06 tarihli çalışma ağacına göredir; dosya değiştiyse **önce doğrula**.

---

## 0. Rolün ve çalışma şeklin

- Bu repoda (FibiEmlakDanismanlik) kıdemli bir ASP.NET Core + jQuery geliştiricisisin.
- **İlk iş: repo kökündeki `rules.md` dosyasını baştan sona oku.** Bu görevde yazacağın her satır `rules.md`'ye uymak zorunda. Bu prompt ile `rules.md` çelişirse `rules.md` geçerlidir; çelişkiyi bana raporla.
- Çalışma sırası: **Faz 0 → (onay) → Faz 1 → (onay) → Faz 2 → (onay) → Faz 3.** Her fazın sonunda build al, §6'daki formatta rapor ver ve **onayımı almadan sonraki faza geçme.**
- Başlamadan önce `git status` ve `git diff` ile commit edilmemiş değişiklikleri incele. `fix/home-search-cascade` adında yeni bir branch aç. Ben söylemeden commit atma.

---

## 1. Beklenen davranış (spesifikasyon)

Ana sayfadaki arama bölümü (`_SearchFieldSectionComponentPartial`) iki sekmeden oluşur:

- **Satın Al:** `usageType = 1`
- **Kirala:** `usageType = 2`

Her sekme kendi durumunu bağımsız olarak tutar.

### 1.1 Kademeli (cascade) akış — her sekme için ayrı

| Durum | İlan Tipi | İl | İlçe | Gelişmiş Arama paneli |
|---|---|---|---|---|
| **S0 Açılış** | Aktif; yalnızca o sekmede ≥1 aktif ilanı olan tipler listelenir | **Disabled**, placeholder: "Önce ilan tipi seçin" | **Disabled**, placeholder: "Önce il seçin" | Açılabilir. "Detaylı filtreler için önce ilan tipi seçin" bilgisi görünür, filtreler disabled |
| **S1 Tip seçildi** | Seçili | Önce "Yükleniyor...", sonra aktif. Yalnızca (usage, tip) için aktif ilanı olan iller listelenir. Placeholder: "İl seçiniz" | **Disabled**, placeholder: "Önce il seçin" | Alanlar tipin kategorisine göre gelir; seçenekler (usage, tip) kapsamında |
| **S2 İl seçildi** | | Seçili | Önce yükleniyor, sonra aktif. **Yalnızca seçilen ilin** aktif ilanı olan ilçeleri listelenir | Seçenekler (usage, tip, il) kapsamına daralır |
| **S3 İlçe seçildi** | | | Seçili | Seçenekler (usage, tip, il, ilçe) kapsamına daralır |

**Geçiş kuralları:**

- **Tip değiştiğinde:** İl, İlçe ve tüm gelişmiş filtre değerleri sıfırlanır, İl yeniden yüklenir.
- **Tip temizlendiğinde (X):** Durum S0'a döner.
- **İl değiştiğinde:** İlçe ve gelişmiş filtre değerleri sıfırlanır, İlçe yüklenir.
- **İl temizlendiğinde:** İlçe temizlenir ve **disabled** olur.
- **İlçe değiştiğinde veya temizlendiğinde:** Yalnızca gelişmiş filtre seçenekleri yeni kapsama göre yeniden yüklenir.
- **Sonuç boş geldiğinde:** İlgili select disabled kalır, placeholder "Bu seçimde aktif ilan yok" olur.
- **Geç gelen AJAX cevapları:** Eski bir cevap güncel durumu **asla ezemez**.
- **Sekme değiştiğinde:** Açık Gelişmiş Arama paneli kapanır. Diğer sekmenin durumu korunur.
- **Placeholder kuralı:** "Önce ilan tipi seçin" yazısı, aktif (enabled) bir alanda **hiçbir zaman** görünmez.

### 1.2 "Aktif ilan" tanımı (tek doğruluk kaynağı)

Bu tanım listeleme sayfalarıyla **birebir aynı** olmalı. Dropdown'da görünen her il veya ilçe, listeleme sayfasında sonuç döndürmeli.

- **Statü koşulu:** `PropertyStatus != null && PropertyStatuses.ActiveSet.Contains(PropertyStatus)`
  - Aktif sayılan değerler: `Aktif` ve `Opsiyonlu`
  - Kaynak: `Core/FibiEmlakDanismanlik.Application/Constants/PropertyStatuses.cs`
  - **String literal yazma, her zaman `ActiveSet` sabitini kullan.**
- **Kullanım tipi koşulu:** İlanın `ListingType.UsageType` değeri sekmeyle aynı olmalı (`UsageType.ForSale` veya `UsageType.ForRent`).
- **Referans implementasyonlar:**
  - `Infrastructure/FibiEmlakDanismanlik.Persistance/Repositories/PropertyRepositories/PropertyRepository.cs`
  - `BuildForSaleListingQuery()` (satır ~29-40)
  - `BuildRentalHousingQuery` (~1554), `BuildRentalLandQuery` (~1648), `BuildRentalCommercialQuery` (~1723)
- **Listelenmeyecek ilanlar:** Statüsü Pasif, Satıldı, Kiralandı veya null olan ilanların il ve ilçeleri listelenmez.

### 1.3 Gelişmiş Arama alan matrisi

İlanın kategorisi `Core/FibiEmlakDanismanlik.Domain/Enums/PropertyListingType.cs` enum'undan belirlenir:

- 1 = Satılık Arsa
- 2 = Satılık Konut
- 3 = Satılık İşyeri
- 7 = Kiralık Arsa
- 8 = Kiralık Konut
- 9 = Kiralık İşyeri

| Alan | Konut (2, 8) | İşyeri (3, 9) | Arsa (1, 7) |
|---|---|---|---|
| Oda / Bölüm | "Oda Sayısı": distinct `NumberOfRoom` ("1+1", "2+1"…, doğal sıralı) | "Bölüm Sayısı": distinct `NumberOfSection` | Gizli |
| Banyo / Lavabo | "Banyo Sayısı": `NumberOfBathRoom` | "Lavabo Sayısı": `NumberOfBathrooms` | Gizli |
| Kat | `NumberOfFloors` (0 → "Zemin Kat") | `NumberOfFloors` | Gizli |
| İmar Durumu | Gizli | Gizli | distinct `ZoningStatus` |
| Fiyat aralığı | Satılıkta `Price`, kiralıkta `Rent`: gerçek min–max | Aynı | Aynı |
| m² aralığı | `GrossArea` | `GrossArea` | `Area` |
| Danışman | Kapsamda aktif ilanı olan danışmanlar (distinct `AgentId` + ad) | Aynı | Aynı |
| Sıralama | En Yeni, En Eski, Fiyat Artan, Fiyat Azalan | Aynı | Aynı |
| Konuma Uzaklık | **Gizle** (backend desteği yok) | Gizle | Gizle |

**Sıralama seçeneklerinin backend karşılıkları:**

- En Yeni → `CreatedDate desc`
- En Eski → `CreatedDate asc`
- Fiyat Artan → `Price asc`
- Fiyat Azalan → `Price desc`
- Bunlar `PropertyRepository` (~1485 ve ~1883) içinde desteklenen değerlerdir.
- "En Popüler" ve "Lüks" seçeneklerini **kaldır**; backend'de karşılıkları yok.

**Seçenek kuralları:**

- Seçenek listeleri **hardcoded olmayacak**. Tamamı mevcut kapsamdaki aktif ilan verisinden gelecek.
- Liste boşsa alan disabled olur.
- Slider için min = max ise slider bozulmamalı. Bu durumda `max = min + step` kullan.

---

## 2. Mevcut durum: kök nedenler (kanıtlı)

**Videoda gözlenenler:**

- (a) "Gelişmiş Arama" tıklanınca panel açılmıyor. DevTools'ta `.switch_btn_one` class'ı yanıp sönüyor ama `active` kalmıyor.
- (b) İşyeri seçilince:
  - İl alanı aktifleşiyor ama "Önce İlan Tipi Seçin" placeholder'ı kalıyor.
  - İlçe, **il seçilmeden** aktifleşiyor ve farklı illerin ilçelerini gösteriyor (Ayazağa, Beşiktaş, Ceyhan, Espiye).
- (c) İl olarak İstanbul seçilince İlçe "No results found" gösteriyor.
- (d) Satılık Arsa'da İl "No results found" gösteriyor.
- (e) Console'da 4 hata var.

Aşağıda `Default.cshtml` dediğim dosyanın tam yolu: `Frontends/FibiEmlakDanismanlik.WebUI/Views/Shared/Components/_SearchFieldSectionComponentPartial/Default.cshtml`

### K1 — Modül iki kez init ediliyor (Gelişmiş Arama'nın açılmamasının sebebi)

**Ne oluyor:**

- jQuery, `Frontends/FibiEmlakDanismanlik.WebUI/Views/UILayout/Index.cshtml` dosyasında **sayfanın en altında** yükleniyor.
- Bu yüzden partial içindeki ilk init çağrısı `false` dönüyor.
- `Default.cshtml` satır 1060-1071: `setInterval` polling, jQuery hazır olunca init'i bir kez çalıştırıyor.
- Ardından `window.load` listener'ı `initSearchModule()`'ü **koşulsuz olarak tekrar çağırıyor**.

**Sonuç:**

- `$(document).on(...)` ile bağlanan **tüm** handler'lar 2 kez bağlanmış oluyor.
- Toggle handler (satır 1025-1042) aynı tıklamada `active` class'ını ekleyip geri siliyor.

**Kanıt (headless Chromium ile tekrar üretildi):**

- Mevcut durum:
  - `jQuery._data(document,'events').click.filter(h => h.selector === '.search__toggler').length` sonucu `2`.
  - Tıklama sonrası class `switch_btn_one` olarak kalıyor (değişmiyor).
- Tek init guard eklenince:
  - Handler sayısı `1`.
  - Tıklama sonrası class `switch_btn_one active` oluyor ve panel açılıyor.

**Aynı sebebin diğer etkileri:** İlan tipi ve il change handler'ları da 2 kez çalışıyor. Bu da çift isteklere ve yarış koşullarına yol açıyor.

### K2 — İşyeri için isim bazlı özel dal, cascade'i atlıyor (İlçe'nin il seçilmeden açılmasının sebebi)

**Ne oluyor:**

- `Default.cshtml` satır 672-867'de `selectedText.indexOf('İşyeri')` ile ayrılan özel bir dal var.
- Bu dal `api/CommercialSearch/filters` üzerinden **tüm** işyeri ilanlarını çekiyor. Statü filtresi yok, veri ham liste olarak geliyor.
- İl, ilçe ve fiyat bilgileri JS tarafında bu ham listeden çıkarılıyor.
- İl ve İlçe **aynı anda** doldurulup ikisi birden enable ediliyor (satır 812-830). İlçe listesinin il ile bağı yok.

**İlçe isteğinin 400 dönmesi:**

- İl option'larının value'su il **adı**, ID değil.
- Örneğin `available-districts?cityId=İstanbul` isteği gidiyor.
- `[ApiController]` model binding bunu kabul etmiyor ve 400 dönüyor. Sonuç: boş liste.
- 2 handler × (network hatası + `console.error`) = muhtemelen console'daki 4 hata.

**Placeholder sorunu:** Option'lar `.empty().append()` ile değiştiriliyor ama Select2 yeniden init edilmiyor. Bu yüzden eski placeholder kalıyor.

**Döngü sorunu:** Programatik `.trigger('change')` çağrıları cascade handler'larını yeniden tetikliyor ve döngü/yarış oluşturuyor.

### K3 — Backend il/ilçe sorguları aktif ilan filtresi uygulamıyor

- Dosya: `Infrastructure/FibiEmlakDanismanlik.Persistance/Repositories/LocationRepositories/LocationRepository.cs`
- Metotlar: `GetAvailableCitiesAsync` (satır ~65) ve `GetAvailableDistrictsAsync` (~102)
- Statü filtresi yok.
- UsageType filtresi yok.
- Hangi tablonun sorgulanacağı `listingType.Name.Contains("Konut" / "Arsa" / "İş")` ile seçiliyor. Bu yaklaşım kırılgan.

### K4 — Gelişmiş filtre handler'ı hatalı ve statik

Dosya: `Core/FibiEmlakDanismanlik.Application/Features/Handlers/ListingTypeHandlers/GetAdvancedFiltersByListingTypeQueryHandler.cs`

- **İsim kontrolü tutmuyor:** Kod "İş Yeri" (boşluklu) arıyor, ama ilan tipinin adı "İşyeri". Bu yüzden işyeri ilanları konut seçeneklerini alıyor.
- **Oda seçenekleri eşleşmiyor:** Seçenek değerleri "1", "2" gibi. Listeleme ise `NumberOfRoom == "2+1"` ile filtreliyor, bu yüzden asla eşleşme olmuyor.
- **Kiralık tespiti hatalı:** `isRental` değeri `ListingType.UsageType`'tan çıkarılıyor. UsageType = 3 ("Both") olan tiplerde bu bozuluyor.
- **Sınırlar sabit:** Fiyat ve m² sınırları hardcoded.

### K5 — "Ara" butonu hiçbir yere gitmiyor

- `Default.cshtml` satır 976-1006 yalnızca `console.log` yapıyor.
- Kullanılan parametre adları (`rooms`, `baths`, tek başına `cityId` vb.) listeleme sayfalarının beklediği parametrelerle uyumsuz.

### K6 — `rules.md`'yi ihlal eden geçici yapılar

- **Takip edilmeyen controller:** `Presentation/FibiEmlakDanismanlik.WebApi/Controllers/CommercialSearchController.cs`
  - Ham ilan listesi dönüyor.
  - İş mantığı client tarafında yapılıyor.
- **Boş klasörler:** `Core/FibiEmlakDanismanlik.Application/Features/CommercialSearch/{Handlers,Queries,Requests,Results}`
  - CQRS klasör standardının dışında.
- **JS tarafındaki hack'ler:**
  - Select2 CDN'den dinamik olarak yükleniyor (`loadSelect2Lib`).
  - `setInterval` ile polling yapılıyor.
  - `killNiceSelect` hack'i kullanılıyor.
- **Artık selector:** Satır 651'de `#ilanTipiDropdown` seçicisi kalmış.

### K7 — Veri kalitesi riski (raporla, düzeltme)

- İlan tablolarında il ve ilçe serbest metin olarak tutuluyor (`City` ve `District` string alanları).
- Ekranda "Ayazağa" (aslında bir mahalle) ilçe olarak görünüyor.
- `Cities` / `Districts` tablolarıyla eşleşmeyen değerler yeni yapıda listeden düşecek. Bu ilanlar ilçe filtresiyle bulunamaz hale gelecek.
- **Veriyi değiştirme. Faz 0'da raporla.**

---

## 3. Uygulama planı

### Faz 0 — Teşhis ve doğrulama (yalnızca okuma, kod değişikliği yok)

**1. Uygulamayı çalıştır ve gözlemle**

- WebApi: `https://localhost:7015`
- WebUI: `https://localhost:7012`
- Ana sayfada Console ve Network sekmelerini kaydet.
- K1 ve K2'yi doğrula. Handler sayısı için: `jQuery._data(document,'events')`

**2. Aşağıdaki SQL sorgularını salt okunur çalıştır ve sonuçları raporla**

```sql
-- 1) İlan tipleri
SELECT ListingTypeId, Name, UsageType FROM listingTypes ORDER BY ListingTypeId;

-- 2) Tablo bazında ListingTypeId ve statü dağılımı (enum eşlemesini doğrulamak için)
SELECT 'ForSaleHousing' T, ListingTypeId, PropertyStatus, COUNT(*) C FROM forSaleHousingPropertyListings GROUP BY ListingTypeId, PropertyStatus
UNION ALL SELECT 'ForSaleLand', ListingTypeId, PropertyStatus, COUNT(*) FROM forSaleLandListings GROUP BY ListingTypeId, PropertyStatus
UNION ALL SELECT 'ForSaleCommercial', ListingTypeId, PropertyStatus, COUNT(*) FROM forSaleCommercialPropertyListings GROUP BY ListingTypeId, PropertyStatus
UNION ALL SELECT 'RentalHousing', ListingTypeId, PropertyStatus, COUNT(*) FROM rentalHousingListings GROUP BY ListingTypeId, PropertyStatus
UNION ALL SELECT 'RentalLand', ListingTypeId, PropertyStatus, COUNT(*) FROM rentalLandListings GROUP BY ListingTypeId, PropertyStatus
UNION ALL SELECT 'RentalCommercial', ListingTypeId, PropertyStatus, COUNT(*) FROM rentalCommercialPropertyListings GROUP BY ListingTypeId, PropertyStatus;

-- 3) Aktif ilanlarda Cities/Districts ile eşleşmeyen il/ilçe metinleri (veri kalitesi)
WITH L AS (
  SELECT 'ForSaleHousing' T, City, District FROM forSaleHousingPropertyListings WHERE PropertyStatus IN (N'Aktif', N'Opsiyonlu')
  UNION ALL SELECT 'ForSaleLand', City, District FROM forSaleLandListings WHERE PropertyStatus IN (N'Aktif', N'Opsiyonlu')
  UNION ALL SELECT 'ForSaleCommercial', City, District FROM forSaleCommercialPropertyListings WHERE PropertyStatus IN (N'Aktif', N'Opsiyonlu')
  UNION ALL SELECT 'RentalHousing', City, District FROM rentalHousingListings WHERE PropertyStatus IN (N'Aktif', N'Opsiyonlu')
  UNION ALL SELECT 'RentalLand', City, District FROM rentalLandListings WHERE PropertyStatus IN (N'Aktif', N'Opsiyonlu')
  UNION ALL SELECT 'RentalCommercial', City, District FROM rentalCommercialPropertyListings WHERE PropertyStatus IN (N'Aktif', N'Opsiyonlu')
)
SELECT L.T, L.City, L.District,
       CASE WHEN c.CityId IS NULL THEN 'IL_ESLESMIYOR' ELSE 'ILCE_ESLESMIYOR' END AS Sorun
FROM L
LEFT JOIN Cities c ON c.Name = L.City
LEFT JOIN Districts d ON d.CityId = c.CityId AND d.Name = L.District
WHERE c.CityId IS NULL OR d.DistrictId IS NULL;

-- 4) DB collation (Türkçe İ/ı karşılaştırması için)
SELECT DATABASEPROPERTYEX(DB_NAME(), 'Collation') AS DbCollation;
```

**3. Enum eşlemesini doğrula**

- `PropertyListingType` enum'undaki ID'ler (1, 2, 3, 7, 8, 9), `listingTypes` tablosu ve ilan tablolarındaki `ListingTypeId` dağılımıyla tutarlı olmalı.
- **Tutarsızlık varsa dur ve raporla.**

**4. Planı yaz ve onay bekle**

- Bulgularını raporla.
- Repo kökündeki `ImplementationPlan.md` dosyasını "v2" olarak güncelle: kök nedenler + bu plan.
- **Onayımı bekle.**

### Faz 1 — Frontend stabilizasyonu (WebUI)

**1. Script'i partial'dan ayrı dosyaya taşı**

- Yeni dosya: `Frontends/FibiEmlakDanismanlik.WebUI/wwwroot/js/home-search.js`
- Mevcut `wwwroot/js/for-sale-listing-filters.js` dosyasının desenini izle.
- Razor değerlerini data attribute ile geçir:
  ```html
  <section class="search-field-section" id="homeSearchRoot"
           data-api="@ViewBag.ApiUrl"
           data-sale-url="@Url.Action("Index","AllForSalePage")"
           data-rent-url="@Url.Action("Index","AllForRentalPage")">
  ```
- Mevcut id ve class isimlerini **koru**: `listingTypeDropdown-forsale`, `cityDropdown-forrent`, `.search__toggler` vb.

**2. Script yükleme sırasını düzelt**

- `Views/UILayout/Index.cshtml` dosyasında `script.js` satırından **sonra**, şu sırayla ekle:
  1. Select2 JS. `Views/ThemeScaff/ScaffTemplate.cshtml` ile aynı sürümü kullan (`select2@4.1.0-rc.0`).
  2. `<script src="~/js/home-search.js" asp-append-version="true"></script>`
- Partial içinden şunları **tamamen kaldır:**
  - `loadSelect2Lib`
  - `setInterval` polling
  - `window.load` ile yapılan init

**3. Tek init garantisi**

- Dosyanın başına guard ekle:
  ```js
  if (window.__fibiHomeSearchInit) return; window.__fibiHomeSearchInit = true;
  ```
- Tüm event'leri `.homeSearch` namespace'i ile ve `off(...).on(...)` kalıbıyla bağla.

**4. Nice-select çakışmasını gider**

- Arama bileşenindeki tüm `<select>` elementlerine `class="ignore"` ekle.
- Tema bu sınıfı zaten tanıyor: `wwwroot/Realshed/assets/js/script.js` satır 524 → `$('select:not(.ignore)').niceSelect()`
- `killNiceSelect` hack'ini kaldır.

**5. Select2'yi tek helper üzerinden yönet**

- Helper: `setSelectState($select, { options, placeholder, disabled })`
  - Option'ları yeniden kurar.
  - Select2'yi yeni placeholder ile destroy edip yeniden init eder.
  - `disabled` durumunu uygular.
- Bu helper dışında hiçbir yerde `.empty().append()` kullanılmayacak.
- Gelişmiş panel içindeki select'lerde `dropdownParent` olarak paneli ver.

**6. Cascade'i yalnızca kullanıcı olaylarına bağla**

- Kullanılacak event'ler: `select2:select` ve `select2:clear`.
- Örnek desen: `for-sale-listing-filters.js` satır 181-206.
- Programatik değişikliklerde yalnızca `trigger('change.select2')` kullan.
- **`trigger('change')` kullanmak yasak.**

**7. Yarış koşullarını engelle**

- Her bağımlı istek türü (cities, districts, filters) için, sekme bazında `AbortController` veya artan bir istek sayacı kullan.
- Eski isteklerden gelen cevapları yok say.
- İl veya ilçe değiştiğinde gelişmiş filtre isteğini 150-200 ms debounce et.

**8. İşyeri özel dalını kaldır**

- Satır 672-867'deki dalı ve `CommercialSearch` çağrısını tamamen sil.
- Tüm ilan tipleri aynı cascade'den geçsin.
- **JS'te ilan tipi adına göre dallanma olmayacak.**
- Bu fazda mevcut `showRooms`, `showBath`, `showFloor`, `showPrice`, `showArea` bayraklarını kullan. Kategori ve etiket bilgisi Faz 2'de backend'den gelecek.

**9. Gelişmiş Arama toggle'ını düzelt**

- Tek bir handler olsun.
- En yakın `.switch_btn_one` elementine `active` class'ını ekleyip kaldırsın.
- İkon `fa-angle-down` ile `fa-angle-up` arasında değişsin.
- Panel şu durumlarda kapansın:
  - X (kapat) butonuna basıldığında.
  - Sekme değiştiğinde. Tema tab handler'ı `script.js` satır ~150'de.
- Panel ilan tipi seçilmeden açılırsa bilgi metni göster.

**10. Kullanıcı geri bildirimleri**

- Yükleme sırasında: "Yükleniyor..."
- Hata durumunda: "Yüklenemedi, tekrar deneyin"
- Console'a anlamlı log yaz.

**11. CSS'e dokunma**

- Mevcut `<style>` bloğunu ve Realshed CSS dosyalarını değiştirme.
- Yalnızca zorunluysa minimal ekleme yap.

**Faz 1 kabul:** §5'teki F1-F6 senaryoları geçmeli. Build al, raporla ve **onay bekle**.

### Faz 2 — Backend veri doğruluğu (Application / Persistence / WebApi)

**1. `LocationRepository` sorgularını düzelt**

Metotlar: `GetAvailableCitiesAsync` ve `GetAvailableDistrictsAsync`. İmzaları değişmeyecek.

- **Tablo seçimi:** `(PropertyListingType)listingTypeId` switch'i ile yap. İsimle eşleştirme olmayacak.
- **Kullanım tipi kontrolü:** `usageType`, ilan tipiyle uyuşmuyorsa boş liste dön.
- **Aktif ilan filtresi:** §1.2'deki tanımı ve `ListingType.UsageType` join'ini uygula. Sınıf içinde tek bir private helper kullan (DRY).
- **Sorgu yapısı:** Tek SQL sorgusu olsun (EXISTS veya JOIN). İsim listesini belleğe çekme.
- **Karşılaştırma:**
  - İsim eşleştirmesini SQL tarafında, DB collation'ı ile yap.
  - C# tarafında `ToLower()` veya kültüre bağlı karşılaştırma kullanma.
  - `Trim` farklarını SQL içinde ele al.
- **İlçe sorgusu:** Yalnızca seçili ilin (`City.Name`) aktif ilanlarından gelsin.

**2. İlan tipi listesini aktif ilana göre filtrele**

- `GetListingTypesByUsageQuery`'ye `bool OnlyWithActiveListings` parametresi ekle (varsayılan: `false`).
- `true` ise yalnızca §1.2'ye göre ≥1 aktif ilanı olan tipleri dön.
- Endpoint: `GET api/listingtypes?usageType=1&onlyWithActiveListings=true`
- Bu endpoint'i yalnızca ana sayfa araması kullanıyor. Bunu grep ile doğrula.

**3. Gelişmiş filtreleri veriye dayalı hale getir**

- **Query:** `GetAdvancedFiltersByListingTypeQuery` şu alanları alsın:
  - `ListingTypeId`
  - `UsageType`
  - `int? CityId`
  - `int? DistrictId`
- **Endpoint (geriye uyumlu):**
  - `GET api/listingtypes/advanced-filters?listingTypeId=&usageType=&cityId=&districtId=`
  - Controller yalnızca `_mediator.Send` çağırsın.
- **Yeni repository metodu:**
  - İmza: `IPropertyRepository.GetSearchFilterFacetsAsync(int usageType, int listingTypeId, int? cityId, int? districtId)`
  - Dönüş tipi: `Core/FibiEmlakDanismanlik.Application/Features/Results/PropertyResults/SearchFilterFacetsResult.cs` (yeni dosya)
  - Result alanları: `TotalCount`, `MinPrice`, `MaxPrice`, `MinArea`, `MaxArea`, `Rooms`, `Bathrooms`, `Floors`, `Sections`, `ZoningStatuses`, `Agents`
  - Repository yalnızca ham distinct değerleri ve min/max'ları döner. İş kuralı içermez.
  - Satılık için mümkünse mevcut `BuildForSaleListingQuery()` metodunu, kiralık için mevcut `BuildRental*Query(...)` metotlarını yeniden kullan. Böylece aktif ilan tanımının aynı olduğu garanti edilir.
- **Handler sorumlulukları:**
  - Kategoriyi `PropertyListingType` enum'undan belirler.
  - Alanların görünürlüğüne karar verir.
  - Etiketleri belirler: "Oda Sayısı" / "Bölüm Sayısı", "Banyo Sayısı" / "Lavabo Sayısı".
  - Metinleri formatlar: "Zemin Kat", "3 Bölüm" vb.
  - Odaları doğal sıraya koyar.
- **`AdvancedFiltersVisibilityDto`'yu genişlet.** Mevcut alanları silme. Eklenecek alanlar:
  - `Category`: `"housing"` | `"land"` | `"commercial"`
  - `RoomLabel`, `BathLabel`
  - `ShowZoning`, `ZoningOptions`
  - `AgentOptions`
  - `TotalCount`
  - `ShowDistance` her zaman `false`.
- **Kaldırılacaklar:** Hardcoded fiyat ve m² sınırları ile "İş Yeri" isim kontrolü.

**4. Frontend'i yeni alanlara bağla**

- Yeni DTO alanlarını kullan: etiketler, imar durumu, danışmanlar, dinamik slider sınırları, `TotalCount`.
- Tüm danışmanları çeken `api/Agent` çağrısını, kapsama göre filtrelenmiş `AgentOptions` ile değiştir.

**5. Temizlik**

- `CommercialSearchController.cs` dosyasını sil.
- Boş `Features/CommercialSearch/` klasörlerini sil.
- Silmeden önce hiçbir yerde referans verilmediğini grep ile doğrula.

**6. Öneri listesi düzeltmesi (küçük)**

- Metot: `PropertyRepository.GetPropertySuggestionsAsync` (~2525)
- §1.2'deki aktif ilan tanımını uygula.
- Gelen `listingTypeId` parametresiyle filtrele. Şu an bu parametre yok sayılıyor.

**7. Endpoint'leri test et**

- Her endpoint'i Swagger'dan dene.
- Örnek request ve response'ları rapora ekle.

**Faz 2 kabul:** §5'teki B1-B5 senaryoları geçmeli. Build al, raporla ve **onay bekle**.

### Faz 3 — Arama gönderimi ve listeleme sayfası uyumu

**1. "Ara" butonunu çalışır hale getir**

- Hem "Ara" butonu hem de "Mülk Ara" input'unda Enter tuşu çalışsın.
- Aktif sekmeye göre `data-sale-url` veya `data-rent-url` adresine yönlendir.
- Parametreler, listeleme sayfalarının **mevcut** beklediği adlarla gönderilmeli. Referans dosyalar:
  - `ViewComponents/ForSalePropertyViewComponents/_AllForSaleListingViewComponentPartial.cs`
  - `ViewComponents/ForRentalPropertyViewComponents/_AllForRentalListingViewComponentPartial.cs`
  - `wwwroot/js/for-sale-listing-filters.js` (`initApplyButton`)
- Gönderilecek parametreler:
  - `listingTypeIds`
  - `cityId` ve `city` (ad)
  - `districtId` ve `district` (ad)
  - `minPrice`, `maxPrice`
  - `numberOfRoom`
  - `sortBy`, `sortDir`
  - `page=1`
- URL'i **`URLSearchParams` ile kur.**
  - Elle birleştirirsen "2+1" ifadesindeki `+` karakteri sunucuda boşluğa dönüşür ve filtre eşleşmez.
- Fiyat ve m² değerlerini yalnızca kullanıcı slider'ı hareket ettirdiyse gönder.

**2. Eksik filtreleri listeleme tarafına ekle**

Listeleme tarafında karşılığı olmayan gelişmiş filtreler için `Core/FibiEmlakDanismanlik.Application/Features/Requests/PropertyRequests/PropertyFilterRequest.cs` dosyasına **opsiyonel** alanlar ekle. Değer null ise filtre uygulanmaz, mevcut davranış değişmez.

- Eklenecek alanlar: `MinArea`, `MaxArea`, `NumberOfBathRoom`, `Floor`, `NumberOfSection`, `AgentId`, `ZoningStatus`, `SearchTerm`
- **Satılık:** `GetFilteredForSalePropertyForListingWithFacets` (~1358) içine ilgili `Where` koşullarını ekle.
- **Kiralık:** `BuildRentalHousingQuery`, `BuildRentalLandQuery` ve `BuildRentalCommercialQuery` içine ilgili `Where` koşullarını ekle.
- **ViewComponent'ler:** İki listeleme ViewComponent'i yeni query key'lerini okuyup request'e aktarsın. Key adları:
  - `minArea`, `maxArea`
  - `numberOfBathRoom`
  - `floor`
  - `numberOfSection`
  - `agentId`
  - `zoningStatus`
  - `searchTerm`
- **Sidebar JS dosyaları:** `for-sale-listing-filters.js` ve `for-rental-listing-filters.js` içindeki `initApplyButton` silme listesine bu key'leri ekle. Böylece kullanıcı sidebar'dan "Uygula" dediğinde görünmeyen bir filtre URL'de kalmaz.

**3. Regresyon kontrolü**

- Listeleme sayfasının mevcut filtre, sıralama, sayfalama ve amenity davranışı **bozulmamalı**.

**Faz 3 kabul:** §5'teki S1-S4 senaryoları geçmeli.

---

## 4. Kurallar ve yasaklar (`rules.md`'ye ek olarak bu görev için)

### `rules.md`'den bu görevi doğrudan ilgilendirenler

- CQRS: Query, Handler ve Result sınıfları standart klasörlerde olacak.
- Controller'da yalnızca `MediatR.Send` çağrılacak.
- Handler içinde DbContext kullanılmayacak.
- Entity dışarıya doğrudan dönülmeyecek.
- Veri erişimi yalnızca Repository üzerinden yapılacak.
- Realshed teması korunacak.
- Yeni bir frontend framework eklenmeyecek.
- Namespace ve klasör yapısı değişmeyecek.
- Gereksiz refactor yapılmayacak.

### Bu göreve özel yasaklar

- **İlan tipi adına göre string dallanma yok** ("İşyeri", "Konut", "Arsa"). Bu kural hem JS hem C# için geçerli. Kategori her zaman `PropertyListingType` enum'undan belirlenir.
- **Ham ilan listesini client'a çekip JS'te toplama veya ayıklama yapma.**
- **Yeni controller veya yeni feature klasör ağacı oluşturma.** Mevcut `LocationController`, `ListingTypesController` ve `SearchController` genişletilecek.
- **`setInterval` polling, ikinci init ve JS içinden CDN script enjekte etme yok.**
- **Programatik değişikliklerde `.trigger('change')` kullanma.**
- **Migration, şema değişikliği ve `UPDATE`/`DELETE` ile veri düzeltme yok.** Veri sorunlarını yalnızca raporla.
- **Bu görevin kapsamı dışındaki dosyalarda değişiklik yapma.** Gördüğün diğer sorunları raporla ama düzeltme. Örnekler:
  - `GetForSaleListingTypeFacetsAsync` ve `GetForRentalListingTypeFacetsAsync` null statüyü aktif sayıyor (listeleme ile tutarsız).
  - Kiralık filtre, tüm kayıtları belleğe alıp sıralıyor.

### Korunacak mevcut düzeltmeler (commit edilmemiş)

- `rtl.css`'in head'den kaldırılması.
- `boxed_wrapper` elementinden `rtl` class'ının kaldırılması.
- `script.js` içindeki slider guard'ları.

### Diğer

- Kod yorumları mevcut stilde yazılabilir (Türkçe kabul).

---

## 5. Kabul kriterleri / test senaryoları

### Frontend

- **F1:** Sayfa açıldığında console'da hata yok. Document üzerinde `.search__toggler` için **1** click handler var.
- **F2:**
  - Gelişmiş Arama'ya tıklanınca panel açılıyor.
  - Tekrar tıklanınca veya X'e basılınca panel kapanıyor.
  - Sekme değişince panel kapanıyor.
  - Panel, altındaki bölümün arkasında kalmıyor.
- **F3:** Açılışta iki sekmede de İl ve İlçe disabled.
- **F4:**
  - Satılık sekmesinde İşyeri seçilince İl aktifleşiyor ve yalnızca aktif satılık işyeri ilanı olan iller listeleniyor. İlçe **disabled** kalıyor.
  - İl olarak İstanbul seçilince İlçe yalnızca İstanbul'un ilçelerini gösteriyor.
  - İl temizlenince İlçe de temizleniyor ve disabled oluyor.
- **F5:**
  - Tip değiştirilince İl, İlçe ve gelişmiş filtre değerleri sıfırlanıyor.
  - Placeholder'lar duruma uygun gösteriliyor.
- **F6:**
  - Hızlı ardışık seçimlerde eski cevaplar UI'ı ezmiyor.
  - Network sekmesinde `CommercialSearch` çağrısı ve 400 dönen istek yok.

### Backend

- **B1:**
  - Bir ilan `Pasif` yapılınca, o ilde başka aktif ilan yoksa il ve ilçesi listeden düşüyor.
  - İlan tekrar `Aktif` yapılınca geri geliyor.
  - `Opsiyonlu` statüsündeki ilanlar listeleniyor.
- **B2:** Arsa seçilince Oda, Banyo ve Kat gizli; İmar Durumu görünür. m² değerleri arsanın `Area` alanından geliyor.
- **B3:** İşyeri seçilince etiketler "Bölüm Sayısı" ve "Lavabo Sayısı"; seçenekler veriden geliyor.
- **B4:**
  - Konut seçilince oda seçenekleri gerçek değerlerden geliyor ("2+1" vb.).
  - İl ve ilçe seçimi seçenekleri daraltıyor.
- **B5:**
  - Fiyat ve m² slider sınırları, mevcut kapsamın gerçek min–max değerleri.
  - min = max olduğunda slider bozulmuyor.

### Arama

- **S1:** Satılık + Konut + İstanbul + Kadıköy + 2+1 seçilip aranınca şu adrese gidiyor:
  `/AllForSalePage/Index?listingTypeIds=..&cityId=..&city=İstanbul&districtId=..&district=Kadıköy&numberOfRoom=2%2B1&page=1`
  Sonuç sayısı, panelde gösterilen `TotalCount` ile tutarlı.
- **S2:** Kiralık sekmesinde aynı akış `/AllForRentalPage/Index` adresine gidiyor.
- **S3:** Listeleme sayfasının sol filtresinde il ve ilçe seçili olarak geliyor.
- **S4:** Listeleme sayfasının mevcut filtre, sıralama, sayfalama ve amenity davranışı değişmemiş.

### Genel

- `dotnet build` hatasız tamamlanıyor ve yeni uyarı çıkmıyor.
- 375 px genişlikteki mobil görünümde select'ler ve panel kullanılabiliyor.

---

## 6. Her faz sonunda rapor formatı

1. **Dosya değişiklikleri:** Değişen, eklenen ve silinen dosyalar. Her biri için yol ve tek satırlık neden.
2. **`rules.md` uyumu:** Madde madde ✓ / ✗ kontrol listesi.
3. **Testler:** Çalıştırılan testler ve sonuçları, §5'teki ID'lerle.
4. **Açık konular:** Kalan riskler ve bana sorman gereken kararlar.
