# Yönetim Paneli Tasarımı (taslak, 2026-10-23)

Adres: `yonetim.fibiemlak.com` · Proje: `Frontends/FibiEmlakDanismanlik.AdminUI` (aynı çözüm, ayrı proje) · Tema: NiceAdmin (BootstrapMade, ücretsiz sürüm; footer'daki kredi linki kalır)

AdminUI, WebUI gibi WebApi'yi `HttpClient` ile çağırır. JWT tarayıcıya verilmez; AdminUI sunucusunda HttpOnly cookie oturumunda tutulur ve isteklerde `Authorization: Bearer` olarak gönderilir. **Tüm yetki kuralları WebApi'de zorlanır**; AdminUI'de menü gizlemek yalnızca görünüm içindir.

---

## 1. Roller

`Agents` tablosuna `Role` (enum `AgentRole`: `Owner = 1` Sahip, `Manager = 2` Yönetici, `Consultant = 3` Danışman). Sahip 1-2 kişi olabilir.

| Yetki | Sahip | Yönetici | Danışman |
|---|---|---|---|
| Sahip ekleme / rol değiştirme | ✔ | ✘ | ✘ |
| Yönetici ekleme / çıkarma | ✔ | ✘ | ✘ |
| Danışman ekleme / çıkarma | ✔ | ✔ | ✘ |
| Toplam kazanç, komisyon ve şirket raporları | ✔ | ✔ | ✘ |
| Hak ediş ve işlem tutarları (kapora, komisyon, prim) | ✔ | ✔ | ✘ (hiçbir tutar görmez) |
| İlanlar | Hepsi | Hepsi | Kendi ilanları + izin verilenler |
| İlan erişim izni verme | ✔ | ✔ | ✘ |
| İçerik (blog, Hakkımızda, SSS, banner, sosyal medya) | ✔ | ✔ | ✘ |
| Talepler / randevular | Hepsi | Hepsi | Kendi ilanlarına gelen ve kendisine atananlar |
| Ayarlar, denetim kaydı | ✔ | Görüntüleme | ✘ |

Kurallar:
- Son kalan Sahip silinemez, pasife alınamaz, rolü düşürülemez.
- Kimse kendi rolünü yükseltemez.
- Rol, şifre veya pasiflik değişince o kullanıcının açık oturumları düşer (`Agents.SecurityStamp`).

### Danışmanın ilan görünürlüğü
- `Agents.CanViewAllListings` (bool): işaretliyse tüm ilanları **görür**, düzenleyemez.
- `ListingAccessGrants` tablosu: `AgentId`, `ListingId`, `ListingTypeId`, `Access` (`View` / `Edit`), `GrantedByAgentId`, `GrantedAt`. Belirli ilanları görme ya da düzenleme izni verir.
- Kendi ilanı: ilandaki `AgentId` danışmanın kendisi → görür ve düzenler.
- Kontrol, API'de kaynak tabanlı yetkilendirmeyle (`IAuthorizationHandler`) yapılır.

---

## 2. Güvenlik (panel dışa kapalı)

Katmanlar, dıştan içe:
1. **Erişim kapısı:** karar (2026-10-23): ek kapı yok, koruma uygulama içinde. İleride gerekirse Cloudflare Access veya IP izin listesi eklenebilir.
2. **Bot koruması:** girişte Cloudflare Turnstile (reCAPTCHA muadili, ücretsiz). Sunucu tarafında doğrulanır.
3. **İki adımlı doğrulama (TOTP):** Google/Microsoft Authenticator. Sahip ve Yönetici için zorunlu; Danışman için isteğe bağlı. Kurtarma kodları tek kullanımlık ve hash'li saklanır.
4. **Giriş sertleştirme:**
   - IP başına ve hesap başına deneme sınırı.
   - 5 hatalı denemede 15 dakika kilit.
   - Hata mesajı genel ("e-posta veya şifre hatalı").
   - Tüm denemeler `LoginAuditLogs` tablosuna yazılır (IP, user-agent, sonuç).
5. **Oturum:**
   - Cookie `HttpOnly`, `Secure`, `SameSite=Strict`.
   - 30 dk hareketsizlikte düşer; mutlak süre 8 saat.
   - JWT 60 dk; süresi dolarsa AdminUI yeniden giriş ister.
   - `SecurityStamp` değişirse oturum düşer.
6. **İstek güvenliği:**
   - Tüm POST'larda antiforgery.
   - Güvenlik başlıkları: CSP, `X-Frame-Options: DENY`, HSTS, `Referrer-Policy`, `Permissions-Policy`.
   - `X-Robots-Tag: noindex, nofollow` ve `robots.txt: Disallow: /`.
7. **Şifre:**
   - En az 12 karakter, BCrypt (mevcut).
   - İlk girişte şifre değiştirme zorunlu.
   - Şifre sıfırlamayı Sahip/Yönetici yapar; e-postayla sıfırlama SMTP kurulunca eklenir.
8. **Denetim kaydı:** `AuditLogs` tablosu tutulur: kim, ne zaman, hangi kaydı, ne değiştirdi (önce/sonra JSON). Silme ve rol değişiklikleri dahil.
9. **Dosya yükleme:**
   - Uzantı, MIME ve dosya imzası (magic bytes) kontrolü, boyut sınırı.
   - Görsel yeniden kodlanır; EXIF (GPS dahil) silinir.
   - Dosya adını sunucu üretir.
10. **API:**
    - Write uçları role/politikaya göre kilitli.
    - Canlıda WebApi'nin yazma uçlarına yalnızca AdminUI sunucusundan erişilmesi hedeflenir (ağ seviyesi).

---

## 3. Fotoğraf yapısı (T5) — mevcut sistemi bozmadan

Bugün: 6 ilan tablosunda `PropImgUrl1..30` kolonları var.

Hedef: yeni `ListingImages` tablosu.
- Kolonlar: `ListingImageId`, `ListingId`, `ListingTypeId`, `Url`, `ThumbUrl`, `MediumUrl`, `SortOrder`, `IsCover`, `AltText`, `Width`, `Height`, `CreatedDate`.
- İndeks: (`ListingTypeId`, `ListingId`, `SortOrder`).

Geçiş, sırayla:
1. Migration tabloyu oluşturur ve mevcut `PropImgUrl1..30` değerlerini sırasıyla aktarır (backfill). Eski kolonlar **silinmez**.
2. Okuma tarafı tek yardımcıya taşınır: `IListingImageService` / repository'de `GetImages(listingTypeId, listingId)`. Bu yardımcıyı kullanacak yerler:
   - ilan detay galerisi
   - liste kartları ve vitrin
   - benzer ilanlar
   - harita popup'ı
   - karşılaştırma sayfası
   - Open Graph görseli ve JSON-LD
   - danışman sayfası
3. Yazma tarafı (admin) yalnızca `ListingImages`'a yazar. Geçiş süresince ilk 30 görsel `PropImgUrl` kolonlarına da kopyalanır (çift yazma); böylece taşınmamış bir yer bozulmaz.
4. Tüm okumalar taşınınca `PropImgUrl` kolonları ayrı bir migration ile kaldırılır (onayla).

Yükleme hattı:
- Tek bir arayüz arkasında: `IFileStorage` (yerelde WebApi `wwwroot/uploads/listings/{yıl}/{ay}`, ileride blob).
- Her görsel için 3 boy üretilir: 1600, 800 ve 400 piksel, WebP. Liste kartları küçük boyu kullanır; detay sayfası `srcset` ile duyarlı boyut seçer (TODO: WebP + srcset).
- Görsel kütüphanesi: SkiaSharp (MIT). ImageSharp'ın lisansı ticari kullanımda koşullu.

---

## 4. İşlemler ve hak ediş

### `ListingDeals` (işlem kaydı)
| Alan | Açıklama |
|---|---|
| `ListingId`, `ListingTypeId` | İlgili ilan |
| `DealType` | Satış / Kiralama |
| `Status` | Kapora Alındı → Tamamlandı / İptal (kapora iade edildi / yandı) |
| `AgreedPrice` | Anlaşılan satış bedeli veya aylık kira |
| `DepositAmount`, `DepositDate` | Kapora |
| `BuyerCommissionAmount`, `SellerCommissionAmount` | Komisyon kimden alındıysa o taraf dolar: yalnızca alıcı, yalnızca satıcı ya da ikisi. Satışta isteğe bağlı oran (%) girilince tutar hesaplanır, elle düzeltilebilir |
| (kiralama) | Komisyon genellikle kiracıdan, bir kira bedeli; **tutar** olarak girilir (varsayılan = aylık kira), oran yok |
| `VatIncluded` | Komisyona KDV dahil mi |
| `CompletedDate`, `Notes` | |
| `CustomerName`, `CustomerPhone` | KVKK kapsamında |

Danışman işlem ekranını ve hiçbir tutarı görmez; yalnızca ilanın "kapora alındı / satıldı / kiralandı" durumunu görür.

Tamamlanınca ilan durumu "Satıldı / Kiralandı" olur ve sitede yayından kalkar.

### `DealShares` (hak ediş payı)
| Alan | Açıklama |
|---|---|
| `DealId`, `AgentId` | |
| `RoleAtDeal` | O andaki rol (rol değişse de geçmiş bozulmaz) |
| `SharePercent` | Komisyondan alınan pay (%) |
| `Amount` | Hesaplanan prim |
| `PaymentStatus`, `PaidDate` | Bekliyor / Ödendi |

- Varsayılan prim oranları ayarlardan gelir (rol bazlı) ve danışman bazında ezilebilir: `Agents.DefaultSharePercent`.
- Her işlemde oranlar ayrıca elle değiştirilebilir.
- Şirket payı = toplam komisyon − dağıtılan primler.
- Raporlar: aylık/yıllık ciro (komisyon), danışman performansı, ödenecek primler.

---

## 5. Müşteri ve talep takibi

- Mevcut `CustomerContacts` (iletişim formu ve randevu) ve `FibiServiceLeads` tek bir "Talepler" ekranında birleşir.
- Talep durumu: Yeni → Arandı → Randevu → Teklif → Kapora → Kazanıldı / Kaybedildi.
- Talep bir danışmana atanır; notlar ve hareket geçmişi `LeadActivities` tablosunda tutulur.
- "X ilanının müşteri durumu": ilan detayında o ilana bağlı talepler, durumları ve randevuları.
- "Haber bekleyenler": "Benzer ilan çıkınca haber ver" talepleri (yeni `ListingAlerts` tablosu: kriterler, iletişim, KVKK onayı). Kriterlere uyan yeni ilan eklenince panelde "haber verilecekler" listesi oluşur. Otomatik e-posta/SMS sonraki adımda.

---

## 6. Kontrol paneli ve raporlar

- **Sayaç kartları (animasyonlu):**
  - aktif ilan sayısı, satılık/kiralık ayrımıyla
  - kategori dağılımı (konut, arsa, işyeri)
  - yeni talepler
  - bugünkü ve yaklaşan randevular
  - haber bekleyenler
  - bu ayki kapora ve işlemler
  - toplam komisyon ve ödenecek primler (Danışman göremez)
- **Grafikler:** Chart.js 4.
  - aylık işlem ve komisyon (çizgi)
  - ilan dağılımı (halka)
  - talep hunisi (yatay çubuk)
  - danışman performansı (çubuk)
  - en çok talep alan ilanlar
- Tarih aralığı filtresi; rapor tabloları CSV/Excel dışa aktarılır.
- Tüm ekranlar mobil uyumlu; tablolar mobilde kart görünümüne geçer.

---

## 7. Fazlar

| Faz | İçerik |
|---|---|
| 0 | `todo-batch2` tarayıcı testi; bekleyen branch zincirinin dev'e birleştirilmesi |
| 1 | API güvenlik ve roller: `Role`, `SecurityStamp`, politikalar, giriş sertleştirme, denetim kaydı, TOTP, Turnstile doğrulama |
| 2 | Fotoğraf yapısı: `ListingImages`, backfill, okuma yardımcısı (her yer), yükleme ucu |
| 3 | AdminUI iskeleti: NiceAdmin ayıklama ve Türkçeleştirme, giriş + 2FA + captcha, layout, boş kontrol paneli |
| 4 | İlan yönetimi: satılık/kiralık filtreli liste, ekle/düzenle, fotoğraflar, vitrin, harita pini, durum, erişim izinleri |
| 5 | Talepler: durumlar, atama, notlar, ilan bazlı müşteri durumu, haber bekleyenler |
| 6 | İşlemler ve hak ediş: kapora, komisyon, prim payları, ödemeler |
| 7 | Kontrol paneli grafikleri ve raporlar |
| 8 | Kullanıcı yönetimi, içerik ekranları (blog/Hakkımızda/SSS/banner/sosyal medya/iletişim), ayarlar |
