# HANDOFF — Ana Sayfa Arama Düzeltmesi (model değişimi devir notu)

> Bu dosya, model veya oturum değiştiğinde işin kaldığı yerden **bölünmeden** devam etmesi için tek devir noktasıdır.
> Her alt adım bitince **commit at ve en alttaki "İlerleme günlüğü"ne bir satır ekle.**

---

## 1. Okuma sırası (zorunlu, tamamını oku)

1. `rules.md` — proje anayasası
2. `HomeSearch_Antigravity_Prompt.md` — görev tanımı. Özellikle şu bölümler:
   - §1: Spesifikasyon
   - §4: Yasaklar
   - §5: Kabul kriterleri
3. `ImplementationPlan.md` (v2) — Faz 0 bulguları ve plan
4. Bu dosya — onaylanan kararlar, ek kurallar, alt adımlar, günlük

**Çelişki olursa öncelik sırası:**

1. `rules.md`
2. Bu dosya
3. `ImplementationPlan.md`
4. `HomeSearch_Antigravity_Prompt.md`

---

## 2. Mevcut durum (2026-10-06 18:45)

- **Faz 0 tamamlandı ve onaylandı.** Bulgular `ImplementationPlan.md` §0'da. **Faz 0'ı tekrar yapma.**
- **Branch:** `fix/home-search-cascade`
- **Son işlem:** Baseline commit atıldı ve doğrulandı: `7fb8655 chore: WIP baseline before home-search fix` (`.localpilot/` hariç).
- **Bu dosya (`HANDOFF_HomeSearch.md`) repoya sonradan eklendi.** Untracked görünecek; 1A commit'ine dahil et.
- **Kod değişikliği:** Henüz yok. Faz 1 başlamadı.
- **Çalışan süreçler:** WebApi (:7015) ve WebUI (:7012), önceki oturumda `dotnet run --no-build --launch-profile https` ile başlatıldı. Hâlâ çalışıyor olabilirler.

---

## 3. Onaylanan kararlar

1. **İl/ilçe adlarının görünümü:** "İstanbul / Kadıköy" biçiminde olacak.
   - Dönüşümü handler'da yap. `Application/Helpers` altına tek bir static helper koy. Repository'de yapma.
   - Dönüşüm sırası: önce `ToLower(new CultureInfo("tr-TR"))`, sonra tr-TR `TextInfo.ToTitleCase`.
   - Bu sıranın nedenleri:
     - `ToTitleCase`, tamamı büyük harf olan kelimeyi değiştirmez.
     - Invariant culture "İ" harfini bozar.
   - Sıralama SQL'de kalsın; `Turkish_CI_AS` doğru sıralar. C#'ta sıralaman gerekirse `StringComparer.Create(new CultureInfo("tr-TR"), true)` kullan.
   - Aynı helper'ı listeleme sayfası sidebar'ındaki il/ilçe/mahalle handler'larına da uygula (`GetCities` / `GetDistricts` / `GetNeighborhoods`).
     - Sadece görünen metin değişecek.
     - Filtrenin CI collation ile eşleşmeye devam ettiğini doğrula.
     - Bunu ayrı commit olarak at.

2. **Sayı alanları:**
   - Oda, Bölüm, Banyo ve Lavabo için `null` ve `≤0` değerleri seçeneklerden çıkar.
   - Kat için:
     - `0` = "Zemin Kat"
     - Negatif değerler = "Bodrum Kat (-n)"

3. **İlçesi eşleşmeyen iller** (ör. Kayseri/Ayazağa, Giresun/Ceyhan):
   - İl, listede kalır.
   - İlçe alanı disabled olur, placeholder **"Tüm ilçeler"** yazar. "Aktif ilan yok" yazma.
   - Bu iki kaydın verisini kullanıcı admin panelinden düzeltecek.

4. **Test verisi:** Yalnızca yerel DB'de (`FURKAN\FURKANKILIC` / `fibiemlak.com`) test amaçlı kayıt eklenebilir.
   - **Önce yedek al:**
     `BACKUP DATABASE [fibiemlak.com] TO DISK='...\fibiemlak_pre_test.bak' WITH COPY_ONLY;`
   - Mevcut kayıtlarda `UPDATE` veya `DELETE` yok. Sadece yeni kayıt ekle. `PropertyName` değeri `TEST-` ile başlasın.
   - **Eklenecek kayıtlar:**

     | Kayıt | Statü | Konum |
     |---|---|---|
     | Satılık Arsa | Aktif | — |
     | Kiralık Arsa | Aktif | — |
     | Satılık Konut | Pasif | Hiç ilanı olmayan bir il |
     | Satılık Konut | Opsiyonlu | Hiç ilanı olmayan başka bir il |
     | Kiralık İşyeri | Pasif | Hiç ilanı olmayan bir il |

   - Arsa için create endpoint yok; SQL `INSERT` kullanılabilir.
   - Eklenen ID'leri `OUTPUT INSERTED` ile al ve günlüğe yaz.
   - **Temizlik kuralları:**
     - Silme işlemini yalnızca bu ID'lerle yap.
     - **`LIKE` ile toplu silme yapma.** `LIKE '[TEST]%'` bir karakter sınıfıdır; T, E veya S ile başlayan gerçek ilanları da siler.
   - Faz 3 testleri bitince test kayıtlarını temizle. Ardından tablo kayıt sayılarını yedek öncesiyle karşılaştırıp raporla.
   - Not: Faz 3'e kadar `TEST-` ilanları yerel sitede görünecek. Bu bekleniyor.

---

## 4. Ek kurallar (makro değişiklik yok, diğer sayfalar bozulmayacak)

- **Dosya kapsamı:**
  - Yalnızca `ImplementationPlan.md` v2'de listelenen dosyalara dokun.
  - Plan dışında bir dosyayı değiştirmen gerekirse **önce sor.** Sorarken dosya adını ve 1 satırlık gerekçeyi yaz.
- **Paylaşılan kodda yalnızca ekleme yap.**
  - Paylaşılan dosyalar:
    - `PropertyRepository`
    - `PropertyFilterRequest`
    - Listeleme ViewComponent'leri
    - `for-sale-listing-filters.js` ve `for-rental-listing-filters.js`
  - İzin verilen değişiklikler:
    - Yeni opsiyonel alan
    - Null kontrolüyle korunan yeni `Where` koşulu
    - Yeni metot
  - Mevcut kod dallarının davranışı değişmeyecek.
  - Rename, dosya taşıma, format veya whitespace değişikliği yapma.
- **Ortak layout:** Tek değişiklik, `UILayout/Index.cshtml`'e eklenecek 2 script satırı. Tema CSS dosyalarına dokunma.
- **Regresyon smoke testi:**
  - Faz 1'e başlamadan **önce** baseline al.
  - Her alt adımdan sonra testi tekrarla ve sonucu baseline ile karşılaştır.
  - Test edilecek sayfalar:
    - `/`
    - `/AllForSalePage`
    - `/AllForRentalPage`
    - Bir satılık ilan detayı
    - Bir kiralık ilan detayı
    - `/Blog`
    - Bir `/Blog/Detail/{id}` sayfası
    - `/Agent`
    - `/Contact`
    - `/AboutUs`
    - `/FrequentlyAskedQuestion`
    - `/Compare`
  - Her sayfada kontrol et:
    - HTTP 200 dönüyor
    - Console hata sayısı baseline'dan fazla değil
    - Ana bileşenler render oluyor
- **Faz 3 için ek regresyon kontrolleri:**
  - Sidebar filtreleri (il/ilçe/mahalle, oda, fiyat)
  - İlan tipi facet'leri
  - Amenity filtresi
  - 4 sıralama seçeneği
  - Sayfalama
  - Karşılaştır butonu
- **Teknik not:** `LocationRepository` helper'ında `IQueryable<(string, string)>` kullanma. Expression tree içinde tuple literal derlenmez (CS8143). Bunun yerine private nested class veya record ile projeksiyon yap.

---

## 5. Alt adımlar ve kontrol noktaları

**Her alt adımda izlenecek sıra:**

1. Değişikliği uygula.
2. Çalışan süreçleri **durdur**.
3. `dotnet build` çalıştır.
4. Süreçleri tekrar **başlat**.
5. İlgili kabul testlerini ve smoke testi çalıştır.
6. Commit at.
7. Günlüğe satır ekle.

| # | Kapsam | Kabul | Commit mesajı |
|---|---|---|---|
| **1A** | `wwwroot/js/home-search.js` iskeleti; script'in partial'dan taşınması; `UILayout/Index.cshtml`'e Select2 JS + `home-search.js` (`script.js`'ten sonra); `window.__fibiHomeSearchInit` guard'ı; `.homeSearch` namespace'i; tüm `<select>`'lere `class="ignore"`; `loadSelect2Lib` / polling / `window.load` / `killNiceSelect` kaldırılması; tek toggle handler (X ile ve sekme değişince kapanma, ilan tipi seçilmeden açılınca bilgi metni) | F1, F2 | `fix(home-search): single init + advanced panel toggle (1A)` |
| **1B** | `setSelectState` helper'ı; cascade'in `select2:select` / `select2:clear` olaylarına bağlanması; programatik değişikliklerde yalnızca `change.select2`; İşyeri dalı ve `CommercialSearch` çağrısının silinmesi; istek sayacı ve 180 ms debounce; Yükleniyor / Hata / Boş / "Tüm ilçeler" durumları | F3–F6 | `fix(home-search): cascade state machine (1B)` |
| **T** | Test verisi (§3.4): yedek + `TEST-` kayıtları; ID'leri günlüğe yaz | — | (commit yok, sadece günlük) |
| **2A** | `LocationRepository`: enum switch, aktif tanımı + UsageType join, EXISTS ile tek SQL, nested class; `GetListingTypesByUsageQuery(OnlyWithActiveListings)` + endpoint; frontend'in bunları kullanması | B1, F4 | `fix(search-api): active-only cities/districts & listing types (2A)` |
| **2B** | `GetSearchFilterFacetsAsync` + `SearchFilterFacetsResult`; handler (kategori, etiket, format, §3.2 kuralları); `AdvancedFiltersVisibilityDto` genişletmesi; frontend bağlantısı (etiketler, imar, danışmanlar, dinamik slider, `TotalCount`); `api/Agent` çağrısının kaldırılması | B2–B5 | `feat(search-api): data-driven advanced filters (2B)` |
| **2C** | `CommercialSearchController.cs` ve boş `Features/CommercialSearch/` klasörlerinin silinmesi (önce grep ile doğrula); `GetPropertySuggestionsAsync` düzeltmesi; tr-TR görünen ad helper'ı (ana sayfa) | Smoke testi | `chore(search): cleanup + suggestions fix (2C)` |
| **2D** | Aynı helper'ın listeleme sidebar'ındaki il/ilçe/mahalle handler'larına uygulanması | Sidebar filtresi eşleşiyor + smoke testi | `fix(location): tr-TR display names (2D)` |
| **R** | Faz 1 + Faz 2 için **birleşik rapor** (`HomeSearch_Antigravity_Prompt.md` §6 formatında) → **DUR. Faz 3 için onay bekle.** | — | — |

**Önemli notlar:**

- Faz 1 sonunda İşyeri'nde "Oda/Banyo" etiketlerinin görünmesi **beklenen geçici durumdur.** 2B'de düzelir.
- Faz 3, `ImplementationPlan.md` v2 §3'e göre yapılacak. **Ayrı onay ister.**

---

## 6. Çalışma ortamı notları

- **Portlar:** Sunucuları başlatmadan önce 7015 ve 7012 portlarının kullanımda olup olmadığını kontrol et. Port çakışmasına girme.
- **DLL kilidi:** Çalışan `dotnet run` süreçleri DLL dosyalarını kilitler. Windows'ta build sırasında MSB3027/MSB3021 hatası alırsın. **Build öncesi bu süreçleri durdur.**
- **Değişikliklerin görünmesi:**
  - `--no-build` ile çalışan bir süreç C# ve `.cshtml` değişikliklerini görmez.
  - WebUI'de Razor runtime compilation **yok**. `.cshtml` değişikliğinden sonra da WebUI'yi durdur, build al ve yeniden başlat.
  - `wwwroot` altındaki JS dosyaları sayfa yenilemeyle güncellenir.
- **Teşhis script'leri:** Headless tarayıcı ve SQL teşhis script'leri repo dışında, scratch klasöründe kalacak. **Repoya ekleme.**

---

## 7. Kesinti / model değişimi protokolü

- **Devir noktası:** Her alt adımın commit'i ve günlük satırı bir devir noktasıdır.
- **Yeni model veya oturum başladığında:**
  1. Şu komutları çalıştır ve sonuçları 5 satırda raporla:
     - `git branch --show-current`
     - `git log --oneline -8`
     - `git status --short`
     - Port durumu
     - `dotnet build` sonucu
  2. Günlüğün son satırına bak ve **bir sonraki alt adımdan** devam et.
  3. `git status` kirliyse (yarım kalmış değişiklik varsa):
     - Önce `git diff` ile değişikliği incele.
     - Alt adım net biçimde tamamlanabiliyorsa tamamla.
     - Emin değilsen **dur ve sor.**
- **Ne zaman durup sorulmalı:**
  - Bir hatayı 2 denemede çözemezsen
  - Plan dışı bir dosyaya ihtiyaç duyarsan
  - `rules.md`'ye uygunluktan şüphe edersen
  - Bu durumlarda **tahminle ilerleme.**

---

## 8. İlerleme günlüğü (her alt adımdan sonra bir satır ekle)

| Tarih/Saat | Model | Alt adım | Durum | Commit | Test sonucu | Not |
|---|---|---|---|---|---|---|
| 2026-10-06 18:45 | Claude Opus 5.5 | Faz 0 + baseline commit | Faz 0 tamam | 7fb8655 | Build: 0 hata, 2 uyarı (NU1903) | Model kotası doldu, devir |
| 2026-10-06 18:59 | Gemini 3.8 Flash | Durum doğrulama (§7) | Tamam | 7fb8655 | Build: 0 hata, 2 uyarı; çalışma ağacı temiz (`.localpilot/` hariç); 7015/7012 durduruldu, boşta | HANDOFF repoda yoktu, eklendi. Sıradaki: **1A** |
| 2026-10-06 19:05 | Gemini 3.8 Flash | Smoke baseline (§4) | Tamam | 7fb8655 | 12/12 sayfa HTTP 200, render=true (console err baseline: Home=0, Sale=0, Rent=0, SaleDet=1, RentDet=0, Blog=1, BlogDet=1, Agent=7, Contact=0, About=3, FAQ=0, Compare=0) | `smoke_baseline.json` scratch'e kaydedildi |
| 2026-10-06 19:11 | Gemini 3.8 Flash | 1A (Tek init + toggle) | Tamam | bb50812 | F1 PASS (handler=1, err=0), F2 PASS, Smoke 12/12 PASS | `home-search.js` taşındı, `.ignore` eklendi |
| 2026-10-06 19:16 | Gemini 3.8 Flash | 1B (Cascade state machine) | Tamam | 6e63644 | F3–F6 PASS, Kayseri "Tüm ilçeler" PASS, Smoke 12/12 PASS | `setSelectState`, `select2:select/clear`, `reqSeq`, 180ms debounce |
| 2026-10-06 19:34 | Gemini 3.8 Flash | T (DB yedek + test verisi) | Tamam | - | Backup OK (`fibiemlak_pre_test.bak`), 5 test kaydı + 1 LandCategory eklendi | IDs: `LandCategoryId=1`, `ForSaleLandListingId=5`, `RentalLandListingId=2`, `ForSaleHousingListId=13(Pasif),14(Opsiyonlu)`, `RentalCommercialListId=9(Pasif)`. Öncesi sayılar: LandCat=0, SaleHousing=1, SaleLand=0, SaleComm=2, RentHousing=1, RentLand=0, RentComm=4 |
| 2026-10-06 19:36 | Gemini 3.8 Flash | 2A (LocationRepo + ListingTypes) | Tamam | (2A) | B1 PASS, F3–F6 PASS, Smoke 12/12 PASS | `PropertyStatuses.ActiveSet`, `UsageType` join, tek SQL `EXISTS`, `onlyWithActiveListings` |



