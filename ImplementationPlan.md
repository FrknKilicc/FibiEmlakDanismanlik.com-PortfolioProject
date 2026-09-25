# Implementation Plan: Dinamik Lokasyon ve Şarta Bağlı Gelişmiş Arama

Bu plan, `rules.md` (Clean Architecture & CQRS) kuralları çerçevesinde Anasayfa Arama modülünün (SearchFieldSection) dinamik, birbiriyle ilişkili (cascading) ve ilan tipine duyarlı hale getirilmesini açıklamaktadır.

## 1. Backend Geliştirmeleri (Application & Persistence Katmanları)

### A. CQRS - Location (Lokasyon) Sorguları
Sadece seçilen `UsageType` (Satılık/Kiralık) ve `ListingTypeId` değerlerine karşılık gelen **aktif ilanı bulunan** lokasyonların getirilmesi için yeni Query'ler eklenecektir.

*   **Yeni Query ve Handler'lar:**
    *   `GetAvailableCitiesQuery(int UsageType, int ListingTypeId)`
    *   `GetAvailableDistrictsQuery(int CityId, int UsageType, int ListingTypeId)`
*   **Repository Adaptasyonu (`ILocationRepository` & `LocationRepository`):**
    *   `GetAvailableCitiesAsync(...)` ve `GetAvailableDistrictsAsync(...)` metodları eklenecek.
    *   Bu metodlar; parametrelere göre ilgili entity tablolarını (`ForSaleHousingListing`, `RentalLandListing` vb.) sorgulayacak, aktif ilanların `City` ve `District` string isimlerini `Distinct()` ile çekecek ve ardından `City` / `District` tablolarıyla eşleştirerek sadece aktif ilanı olanları dönecektir. (İlanı olmayanlar listeye hiç eklenmeyecek).

### B. CQRS - ListingType (Gelişmiş Arama Filtreleri) Sorgusu
Seçilen İlan Tipine (Konut, Arsa, İşyeri) göre hangi filtre alanlarının görünür olacağını belirten bir ayar DTO'su dönülecektir.

*   **Yeni Query ve Handler:**
    *   `GetAdvancedFiltersByListingTypeQuery(int ListingTypeId)`
    *   **Dönecek DTO (`AdvancedFiltersVisibilityDto`):** `ShowRooms`, `ShowBath`, `ShowFloor`, `ShowPrice`, `ShowArea` vb. (Örn: "Arsa" seçildiğinde `ShowRooms`, `ShowBath`, `ShowFloor` `false` dönecek).

### C. WebAPI Katmanı (Controllers)
*   **`LocationController`:**
    *   `[HttpGet("available-cities")]` -> `GetAvailableCitiesQuery`
    *   `[HttpGet("available-districts")]` -> `GetAvailableDistrictsQuery`
*   **`ListingTypesController`:**
    *   `[HttpGet("advanced-filters")]` -> `GetAdvancedFiltersByListingTypeQuery`

---

## 2. Frontend Geliştirmeleri (WebUI Katmanı)

### A. View Revizyonu (`_SearchFieldSectionComponentPartial/Default.cshtml`)
*   **Lokasyon Alanı Tasarımı:**
    *   Mevcut tekli "Lokasyon" seçimi yerine, sidebar yapısına sadık kalarak **İl** ve **İlçe** olmak üzere yan yana (veya alt alta uygun tasarımla) iki adet `Select2` dropdown'ı eklenecektir.
*   **Gelişmiş Arama Alanları:**
    *   Gelişmiş arama kutucukları (Oda, Banyo, Kat vb.) özelleştirilmiş CSS class'ları (örn. `filter-room-group`, `filter-bath-group`) ile sarmalanacak, böylece JS üzerinden kolayca gizlenip/gösterilebilecekler.

### B. JavaScript / AJAX Mantığı (Script Bloğu Güncellemesi)
Sayfa yenilenmeden, DOM etkileşimleriyle çalışacak script aşağıdaki akışı uygulayacaktır:

1.  **Select2 Entegrasyonu:**
    *   İl ve İlçe dropdown'ları `Select2` (arama kutusu destekli) modülü ile initialize edilecek (Eğer tema Select2'yi destekliyorsa veya `niceSelect`'i ezip Select2'ye dönüştürüyorsa, `for-sale-listing-filters.js` deki yapı örnek alınarak).
2.  **ListingType (İlan Tipi) Change Event:**
    *   İlan tipi seçildiğinde `API: /api/location/available-cities` çağrılıp sadece ilanı olan İl'ler listelenecek. İlçe alanı sıfırlanıp pasif (disabled) yapılacak.
    *   Eşzamanlı olarak `API: /api/listingtypes/advanced-filters` çağrılacak. Dönen JSON sonucuna göre Arsa ise Oda, Banyo, Kat gibi filtreler `$.fn.hide()` ile DOM'dan gizlenecek.
3.  **City (İl) Change Event:**
    *   İl seçildiğinde `API: /api/location/available-districts` çağrılıp, sadece o İl'e ait ve seçilen kriterlere uyan (aktif ilanı olan) İlçe'ler listelenip aktif (enabled) hale getirilecek.

Bu adımlar onaylandığında sırasıyla Backend > Controller > Frontend adımları olarak geliştirmeyi uygulayacağım.

