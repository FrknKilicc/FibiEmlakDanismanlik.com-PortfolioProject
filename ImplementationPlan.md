# Implementation Plan v2: Ana Sayfa Arama Modülü — Kademeli İl/İlçe + Dinamik Gelişmiş Arama

> Branch: `fix/home-search-cascade` · v1 git geçmişinde duruyor · Tüm geliştirme `rules.md`'ye tabidir.
> Akış: **Faz 0 (teşhis) → onay → Faz 1 (Frontend) → onay → Faz 2 (Backend) → onay → Faz 3 (Arama gönderimi)**

---

## 0. Faz 0 Bulguları (2026-10-06, kanıtlı)

### 0.1 Veri durumu (salt okunur SQL)

| ListingTypeId | Name | UsageType | Tablo | Aktif ilan |
|---|---|---|---|---|
| 1 | Arsa | 1 | forSaleLandListings | **0 (tablo boş)** |
| 2 | Konut | 1 | forSaleHousingPropertyListings | 1 |
| 3 | İşyeri | 1 | forSaleCommercialPropertyListings | 2 |
| 7 | Arsa | 2 | rentalLandListings | **0 (tablo boş)** |
| 8 | Konut | 2 | rentalHousingListings | 1 |
| 9 | İşyeri | 2 | rentalCommercialPropertyListings | 4 |

- Enum `PropertyListingType` (1,2,3,7,8,9) ↔ `listingTypes` ↔ tablo `ListingTypeId` dağılımı **tutarlı**. `UsageType = 3 (Both)` olan tip yok.
- Tüm kayıtların statüsü `Aktif`. `Opsiyonlu`, `Pasif`, `Satıldı`, `Kiralandı`, `null` statülü kayıt **yok**.
- DB ve ilgili tüm kolonların collation'ı: `Turkish_CI_AS`. Bu sayede `İstanbul = İSTANBUL` ve `Kadıköy = KADIKÖY` eşleşmesi SQL'de doğru çalışıyor.
- `Cities` (81) ve `Districts` (973) tablolarında isimler **BÜYÜK HARF** tutuluyor ("İSTANBUL", "KADIKÖY").

### 0.2 Veri kalitesi (K7, yalnızca rapor, veriye dokunulmadı)

| Tablo | Id | City | District | Sorun |
|---|---|---|---|---|
| rentalCommercialPropertyListings | 3 | Kayseri | Ayazağa | İLÇE_EŞLEŞMİYOR (Ayazağa, İstanbul/Sarıyer'de bir mahalle) |
| rentalCommercialPropertyListings | 4 | Giresun | Ceyhan | İLÇE_EŞLEŞMİYOR (Ceyhan, Adana'nın ilçesi) |

- **Etkisi:** Yeni yapıda Kayseri il listesinde görünecek, ilçe listesi "Bu seçimde aktif ilan yok" olacak. Bu 2 ilan ilçe filtresiyle bulunamayacak.
- Ayrıca RC #3, #4, #5'te `NumberOfSection = 0` ve `NumberOfBathrooms = 0` girilmiş (büyük ihtimalle "girilmedi" anlamında).

### 0.3 Kök nedenlerin doğrulanması

| # | Doğrulama | Sonuç |
|---|---|---|
| K1 | Headless Chrome: `.search__toggler` click handler = **2**, ilan tipi change = 2, il change = 2. Tıklama sonrası class `switch_btn_one` olarak kalıyor. Kod: `Default.cshtml` 1060-1071 (polling + `window.load`). | ✅ Doğrulandı |
| K2 | İşyeri seçilince `CommercialSearch/filters` **2 kez** çağrılıyor. İl aktif ama Select2 placeholder'ı "Önce İlan Tipi Seçin" kalıyor. İlçe il seçilmeden aktif (Kadıköy, Şişli). İstanbul seçilince `available-districts?cityId=İstanbul` → **400 × 2**. Console'da tam **4 hata** var. Kod: 672-867, 812-830. | ✅ Doğrulandı |
| K3 | `LocationRepository` 65-138: statü ve UsageType filtresi yok, tablo seçimi `Name.Contains(...)` ile yapılıyor. `available-cities?usageType=1&listingTypeId=1` → `[]` (gerçekten arsa ilanı yok). | ✅ Doğrulandı |
| K4 | `advanced-filters?listingTypeId=3` (İşyeri) konut seçenekleri ("1 Oda"...) dönüyor: kod "İş Yeri" arıyor, DB'de "İşyeri" var. Oda değerleri "1"/"2", listeleme `"2+1"` bekliyor. Sınırlar hardcoded. | ✅ Doğrulandı |
| K5 | 976-1006 yalnızca `console.log` yapıyor. | ✅ Doğrulandı |
| K6 | `CommercialSearchController` ham entity dönüyor (untracked). `Features/CommercialSearch/{Handlers,Queries,Requests,Results}` boş. `loadSelect2Lib`, `setInterval`, `killNiceSelect`, `#ilanTipiDropdown` (651) mevcut. | ✅ Doğrulandı |

### 0.4 Ek tespitler

- Select2 **CSS**'i head'de zaten var (`_HeadLayoutViewComponentPartial` satır 22, `select2@4.1.0-rc.0`). Yalnızca JS eklenecek.
- Commit edilmemiş `script.js` değişikliği temanın kendi `.search__toggler` handler'ını zaten kaldırmış. Bu değişiklik korunacak; toggle'ı yalnızca `home-search.js` yönetecek.
- Listeleme tarafı il/ilçeyi **isimle** filtreliyor (`x.City == filter.City`, SQL tarafında). `PropertyFilterRequest`'te `CityId` alanı yok. `Cities` tablosundaki büyük harfli isim gönderilse bile `Turkish_CI_AS` sayesinde eşleşir.
- Sıralama switch'leri (1485, 1883) yalnızca `price asc|desc`, `createddate asc` ve varsayılan `createddate desc` değerlerini destekliyor.
- Baseline build: 0 hata, 2 uyarı (`NU1903` AutoMapper 12.0.1, bu görevden önce de vardı).

### 0.5 Kapsam dışı (raporlandı, düzeltilmeyecek)

- `GetForSaleListingTypeFacetsAsync` (1222) ve `GetForRentalListingTypeFacetsAsync` (1526) `null` statüyü aktif sayıyor.
- Kiralık listeleme tüm kayıtları belleğe alıp sıralıyor (1883).
- `rules.md`'de Persistence klasörü "Persistence" yazıyor, gerçek klasör adı "Persistance". Bu bir isimlendirme farkı; değiştirilmeyecek.

---

## 1. Faz 1 — Frontend stabilizasyonu (WebUI)

| Adım | Dosya | İş |
|---|---|---|
| 1 | `wwwroot/js/home-search.js` (yeni) | Script partial'dan taşınır. `for-sale-listing-filters.js` deseni izlenir. Razor değerleri `#homeSearchRoot` üzerindeki `data-api`, `data-sale-url`, `data-rent-url` attribute'larıyla gelir. |
| 2 | `Views/UILayout/Index.cshtml` | `script.js`'ten **sonra**: Select2 JS (4.1.0-rc.0), ardından `~/js/home-search.js` (`asp-append-version`). |
| 3 | `home-search.js` | `window.__fibiHomeSearchInit` guard'ı. Tüm event'ler `.homeSearch` namespace'iyle `off().on()` kalıbında bağlanır. |
| 4 | `Default.cshtml` | Tüm `<select>`'lere `class="ignore"` eklenir. `killNiceSelect`, `loadSelect2Lib`, polling ve `window.load` init kaldırılır. Id ve class isimleri korunur. |
| 5 | `home-search.js` | Tek helper: `setSelectState($el, {options, placeholder, disabled})`. Bu helper option'ları yeniden kurar, Select2'yi destroy edip yeniden init eder, panel içindeki select'lere `dropdownParent` verir. |
| 6 | `home-search.js` | Cascade yalnızca `select2:select` ve `select2:clear` olaylarına bağlanır. Programatik değişikliklerde yalnızca `change.select2` kullanılır. |
| 7 | `home-search.js` | Sekme × {cities, districts, filters} için artan istek sayacı; eski cevaplar yok sayılır. Filtre isteği il/ilçe değişiminde 180 ms debounce edilir. |
| 8 | `Default.cshtml` / js | İşyeri dalı ve `CommercialSearch` çağrısı silinir. Tip adına göre dallanma kalmaz. Mevcut `show*` bayrakları kullanılır. |
| 9 | js | Tek toggle handler: `active` class'ı ve ikon `fa-angle-down`/`fa-angle-up`. Panel X'e basılınca ve sekme değişince (`.tab-btn` click) kapanır. Tip seçilmeden açılırsa bilgi metni gösterilir. |
| 10 | js | "Yükleniyor...", "Yüklenemedi, tekrar deneyin" ve "Bu seçimde aktif ilan yok" durumları. Console'a anlamlı log. |
| 11 | — | CSS'e dokunulmaz (zorunluysa minimal ekleme). |

**Kabul:** F1-F6.

## 2. Faz 2 — Backend veri doğruluğu

1. **`LocationRepository.GetAvailableCitiesAsync` / `GetAvailableDistrictsAsync`** (imzalar aynı kalır)
   - Tablo `(PropertyListingType)listingTypeId` switch'iyle seçilir.
   - Tip ile `usageType` uyuşmazsa boş liste döner.
   - Tek bir private helper `IQueryable<(City, District)>` üretir: `ActiveSet` + `ListingType.UsageType` join.
   - `Cities`/`Districts` ile `EXISTS` kurulur. Tek SQL sorgusu, `Trim` SQL içinde, karşılaştırma DB collation'ıyla.
2. **`GetListingTypesByUsageQuery(UsageType, OnlyWithActiveListings = false)`**
   - Endpoint: `GET api/listingtypes?usageType=&onlyWithActiveListings=true`
   - Kullanıcısı yalnızca ana sayfa araması (grep ile doğrulandı).
3. **`GetAdvancedFiltersByListingTypeQuery(ListingTypeId, UsageType, CityId?, DistrictId?)`**
   - Yeni `IPropertyRepository.GetSearchFilterFacetsAsync(...)` → `SearchFilterFacetsResult` (ham distinct ve min/max değerleri). Mümkün olduğunda `BuildForSaleListingQuery()` ve `BuildRental*Query(...)` yeniden kullanılır.
   - Handler: kategori (enum), görünürlük, etiketler, metin formatı ve doğal oda sıralaması.
   - `AdvancedFiltersVisibilityDto` genişletilir: `Category`, `RoomLabel`, `BathLabel`, `ShowZoning`, `ZoningOptions`, `AgentOptions`, `TotalCount`. `ShowDistance` her zaman `false`.
4. **Frontend bağlantısı:** Etiketler, imar durumu, danışmanlar (`api/Agent` kaldırılır), dinamik slider (min = max ise `max = min + step`), `TotalCount`.
5. **Temizlik:** `CommercialSearchController.cs` ve boş `Features/CommercialSearch/` klasörleri silinir (grep ile referans olmadığı doğrulandı: yalnızca Default.cshtml 684/861/865, bunlar Faz 1'de kalkıyor).
6. **`GetPropertySuggestionsAsync`:** `ActiveSet` + UsageType + `listingTypeId` filtresi.
7. **Swagger örnek request/response** rapora eklenir.

**Kabul:** B1-B5.

## 3. Faz 3 — Arama gönderimi ve listeleme uyumu

1. "Ara" butonu ve Enter tuşu, `URLSearchParams` ile `data-sale-url` / `data-rent-url` adresine yönlendirir. Parametreler: `listingTypeIds`, `cityId`, `city`, `districtId`, `district`, `minPrice`, `maxPrice` (yalnızca slider hareket ettiyse), `numberOfRoom`, `sortBy`, `sortDir`, `page=1`.
2. `PropertyFilterRequest`'e opsiyonel alanlar eklenir: `MinArea`, `MaxArea`, `NumberOfBathRoom`, `Floor`, `NumberOfSection`, `AgentId`, `ZoningStatus`, `SearchTerm`. Bu alanlar satılık sorgusuna (~1358) ve 3 kiralık builder'a uygulanır. İki ViewComponent yeni key'leri okur. Sidebar `initApplyButton` silme listelerine eklenir.
3. Regresyon: listeleme sayfasındaki filtre, sıralama, sayfalama ve amenity davranışı korunur.

**Kabul:** S1-S4 ve genel kriterler (build temiz, 375 px mobil).

---

## 4. Onay bekleyen kararlar

1. **İl/ilçe görünen adı:** `Cities`/`Districts` tabloları büyük harf ("İSTANBUL"). Olduğu gibi mi gösterilsin, yoksa handler'da `tr-TR` ile "İstanbul" biçimine mi çevrilsin? URL'deki `city` değeri her iki durumda da eşleşir.
2. **0 değerleri:** İşyeri `NumberOfSection = 0` ve `NumberOfBathrooms = 0` seçeneklerden çıkarılsın mı? (Öneri: evet. Kat için 0 = "Zemin Kat" olarak kalır.)
3. **Test verisi:** Arsa ilanı ve Opsiyonlu/Pasif ilan yok, bu yüzden B1 ve B2 gerçek veriyle doğrulanamıyor. Admin panelinden test ilanı ekleme veya statü değiştirme işini siz mi yaparsınız? (Bu görevde veri değiştirme yasak.)
