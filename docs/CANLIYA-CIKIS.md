# Canlıya Çıkış — Durum ve Kontrol Listesi

> **Claude (Desktop / Code) için:** Canlıya çıkış, sunucu, DNS, deploy veya canlı veritabanıyla ilgili her soruda **önce bu dosyayı**, sonra adım adım komutlar için [`deploy/README.md`](../deploy/README.md) dosyasını oku. Bu dosya "ne durumdayız, sırada ne var, neye dokunulmaz" sorusunun cevabıdır; komutların kendisi `deploy/README.md`'dedir.
>
> Son güncelleme: 2026-10-31

---

## 1. Hedef mimari (tek VDS)

| Parça | Adres | Sunucuda |
|---|---|---|
| Site (WebUI) | https://fibiemlak.com (+ www, fibiemlak.online yönlenir) | `fibi-site`, 127.0.0.1:5001 |
| Yönetim paneli (AdminUI) | https://yonetim.fibiemlak.com | `fibi-panel`, 127.0.0.1:5002 |
| WebApi | fibiemlak.com/api/ altında **sınırlı** açık (GET + 3 FİBİ formu; Auth/Admin/Me dışarıya kapalı) | `fibi-api`, 127.0.0.1:5000 |
| Veritabanı | dışarıya kapalı | SQL Server 2025 Express, 127.0.0.1 |
| HTTPS | Let's Encrypt (certbot, otomatik yenileme) | nginx |

- **Sunucu gereksinimi:** Ubuntu 24.04, en az 4 GB RAM, 2 çekirdek, 60 GB SSD, **Türkiye'de** (KVKK). Sağlayıcının snapshot/yedek özelliği açılmalı.
- **Yayın:** `master`'a push/merge → GitHub Actions `Deploy` (≈5 dk): derle + test → sunucuda DB yedeği → migration → uygulamalar tek tek, `/health` başarısızsa otomatik eski sürüme döner. `DEPLOY_ENABLED` değişkeni `true` değilse yalnızca derleme + test çalışır.
- **Gizli bilgiler** yalnızca sunucuda `/etc/fibi/*.env` dosyalarında (örnekler: `deploy/server/env/*.env.example`). Repoya asla girmez.
- **Şu anki canlı site:** Natro JetSite (paket 24.02.2027'ye kadar ödenmiş). Geçiş DNS ile yapılır, geri dönüş DNS'i eski değerlere almaktır. JetSite'ı iptal etme.

## 2. Mevcut durum (2026-10-31)

**Kod:** `dev` ve `master` aynı ve push edildi (origin/master `9873120`). Build temiz, 100 birim + 10 persistence testi geçiyor. Canlıya özel düzeltmeler master'da: tarayıcıya verilen API adresi `Url:PublicApiUrl=/api/`, panel `yonetim.fibiemlak.com`, SkiaSharp Linux, health check.

**Sunucu:** henüz kurulmadı (rehberin 1. adımı yapılmadı varsayılır; GitHub'da `DEPLOY_ENABLED` durumu Actions → Deploy loglarından görülür).

**Veritabanı:** Canlı veritabanı **yerel veritabanının yedeğinden** kurulur (`fibi-db-init`). Bu yüzden içerik düzeltmeleri **yedek alınmadan önce yerelde** yapılmalı; yedek alındıktan sonra yerel panelden veri girilmez (girilirse geçiş günü `--replace` ile tekrarlanır).
- `tools/sql/deploy/` klasörü (idempotent migration betiği + `data-fixes.sql`) eski "var olan bir canlı SQL veritabanını güncelleme" yöntemi içindir. VDS akışında **gerekmez**: veri yerelden gelir (AboutUs görsel yolu düzeltmesi yerelde zaten yapılı), migration'ları deploy hattı uygular.

### Çıkış öncesi içerik / karar durumu

| Konu | Durum |
|---|---|
| İlanlar (71 demo ilan, 316 stok görsel) | **Canlıda silinip gerçek ilanlarla değiştirilecek** (Furkan'ın kararı). Panelden silme soft delete'tir; demo görsel klasörü `wwwroot/images/listings` gerçek ilanlar girildikten sonra ayrı iş olarak kaldırılır. |
| Hukuki metinler (KVKK, Gizlilik, Kullanım Koşulları) | **Avukat onayladı** (2026-10-31). Taslak notu kaldırıldı. |
| İş ortakları bölümü | Kalacak. Panelden yönetilir: İçerik → İş Ortakları (`/icerik/is-ortaklari`); başlıklar Site Metinleri'nde (`home.partners.*`). |
| İletişim telefonu | `0532 369 78 28` olarak güncellendi (yerel DB, `Contacts.PhoneNumber1`; ikinci telefon boş). |
| Ofis konumu | Plus Code `8GHC2W27+29` (Zeytinburnu) → `Office:Latitude/Longitude` = 41.000062 / 28.913437 (WebUI appsettings). |
| Danışman açıklamaları | Panelde zengin metin editörü (Kullanıcılar formu ve Hesabım). Eski "string" değerleri boşaltıldı; **içerik panelden girilecek**. |
| Demo müşteri yorumları | Pasif. "Yorumlar" bölüm açıklaması Site Metinleri'nden düzenlenmeli (şu an Lorem ipsum olabilir). |
| 3 blog görseli dış sitelerden (Harvard, OnTheMarket, Britannica) | Panelden yerel görselle değiştirilmeli (telif). |
| ElenAI kullanıcısı (Id 1046, Sahip, aktif) | **Karar bekliyor:** ElenAI bot; yazılana kadar canlıda pasif olması önerilir. |
| GitHub reposu herkese açık | **Private yapılmalı.** Dikkat: rehberin 1. adımı repoyu parolasız `git clone` ile çeker; private olunca sunucuda token/deploy key gerekir (o adım birlikte uyarlanır). Actions ile yayın etkilenmez. |

## 3. Furkan'ın yapacakları (sırayla)

Komutlar `deploy/README.md`'de; buradaki numaralar o dosyanın bölümleridir.

1. **Ön hazırlık (§0):** VDS satın al · Cloudflare Turnstile widget'ı (`fibiemlak.com`, `yonetim.fibiemlak.com`, `yeni.fibiemlak.com`; **anahtarlar olmadan panele giriş yapılamaz**) · Natro'da otomatik yenileme + DNS ekranının ekran görüntüsü (özellikle MX/e-posta kayıtları).
2. **İçerik (yerelde, yedekten önce):** Bölüm 2 tablosundaki açık maddeler (danışman açıklamaları, Yorumlar metni, blog görselleri, ElenAI kararı).
3. **Sunucu kurulumu (§1):** `setup.sh`, `/etc/fibi/api.env`, `site.env`, `panel.env` içine Turnstile anahtarları; SSH'ı anahtara geçir.
4. **Veri taşıma (§2):** yerel DB yedeği + `uploads` klasörü → sunucu → `fibi-db-init` (2FA kayıtları sıfırlanır, Sahip/Yönetici ilk girişte QR'ı yeniden okutur; görsel adreslerindeki localhost'u `https://fibiemlak.com` yapmayı onayla).
5. **Otomatik yayın (§3):** GitHub Secrets `DEPLOY_HOST`, `DEPLOY_SSH_KEY`, `DEPLOY_KNOWN_HOSTS` + Variable `DEPLOY_ENABLED=true` → Actions → Deploy → Run workflow (master).
6. **Deneme (§4):** `yeni.` ve `yonetim.` A kayıtları + certbot → deneme listesi. Panelde ayrıca: ilan formunda çoklu cephe kaydı, binlik ayraçlı alanlar (fiyat, m², kapora), fotoğraf yükleme, Yakın Yerler ("Diğer" kategorisi dahil), danışman açıklaması editörü.
7. **Canlıda ilanlar:** demo ilanları sil, gerçek ilanları gir.
8. **Geçiş günü (§5):** yalnızca `@` ve `www` A kayıtları → sunucu IP. MX/TXT'ye dokunma. certbot `--expand`. Deneme listesini fibiemlak.com'da tekrarla.
9. **Çıkıştan sonra:** Google Search Console + `sitemap.xml` gönderimi · sunucu dışı şifreli yedek (`rclone` → `fibi-offsite`) · repo private.

## 4. Bilinen riskler

- **.NET 8 desteği 10 Kasım 2026'da bitiyor.** Site çalışır ama güvenlik yaması gelmez; çıkıştan hemen sonra .NET 10 (LTS) yükseltmesi ayrı iş olarak planlanmalı.
- **Migration geri alınmaz:** yayın düşerse uygulama eski sürüme döner, şema yeni kalır. Kolon silen migration'lar (ör. `RemoveLegacyPropImgUrlColumns`) eski koda geri dönüşü bozar; her yayından önce otomatik yedek alınır (`/var/backups/fibi/db/pre-migrate-*.bak`).
- **Yedekler varsayılan olarak yalnızca sunucuda**; sunucu kaybolursa gider → `rclone` crypt hedefi kurulmalı.
- `FormSpamGuard` ve TOTP tekrar koruması bellek içi; tek sunucuda sorun değil, birden fazla örneğe geçilirse paylaşılmalı.
- Nominatim (OSM) ücretsiz konumlandırma; yoğun kullanımda kendi geocoder/ücretli servis değerlendirilmeli.
- Linux'ta Data Protection anahtarları dosyada (700 izinli, gece yedeğine dahil).

## 5. Claude için kurallar (canlı ortam)

- Canlı sunucuya, canlı veritabanına ve DNS'e **erişim yalnızca Furkan'dadır**. Claude komutları hazırlar ve açıklar; Furkan çalıştırır.
- **Geri alınamaz işlemler onaysız yapılmaz:** canlı DB'de `DELETE`/`DROP`/`--replace`, DNS değişikliği, `master`'a push (deploy tetikler), repo görünürlüğü, sertifika komutları.
- `master`'a push = canlı yayın (DEPLOY_ENABLED açıksa). Push'tan önce: `dotnet build` + `dotnet test` (Unit ve Persistence) geçmeli; Furkan'a deploy tetikleneceği söylenmeli.
- Kolon silen/yeniden adlandıran migration'lar ayrıca planlanır ve Furkan'a sorulur (geri dönüşü bozar).
- Yerel veritabanından canlıya veri taşındıktan sonra yerel DB'deki veri değişiklikleri canlıya **kendiliğinden gitmez**; veri değişikliği gerekiyorsa idempotent migration (`migrationBuilder.Sql` + `IF NOT EXISTS`) ile yapılır (örnek: `AddNearbyCategoryOther`).
- Gizli bilgileri (şifre, JWT, Turnstile secret, connection string) asla yazdırma, loga veya repoya koyma.
- `tools/sql/demo-listings-refresh.ps1` artık çalışmaz ve canlıda **çalıştırılmamalı** (demo veri üretir).
