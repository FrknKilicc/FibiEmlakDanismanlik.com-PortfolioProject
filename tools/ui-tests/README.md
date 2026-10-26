# Fibi Emlak — UI Duman Testleri (Site Smoke Tests)

Bu dizin, Fibi Emlak sitesinin temel kullanıcı arayüzü ve sayfalarını doğrulamak için harici hiçbir npm paketi (`puppeteer`, `playwright` vb.) kullanmadan, yalnızca **Node.js 24 yerleşikleri** (`fetch`, `WebSocket`, `node:child_process`, `node:fs`) ve **Chrome DevTools Protocol (CDP)** ile çalışan bağımsız bir tarayıcı duman testi takımı içerir.

---

## Gereksinimler

1. **Node.js 24+**: Yerleşik `fetch` ve global `WebSocket` desteği için gereklidir.
2. **Google Chrome**: Sisteme yüklü olmalıdır.
   - Varsayılan aranan yol: `C:/Program Files/Google/Chrome/Application/chrome.exe`
   - Farklı bir konumda ise `CHROME_PATH` ortam değişkeni ile belirtilebilir:
     ```powershell
     $env:CHROME_PATH="C:\Custom\Path\chrome.exe"
     ```
3. **Site ve API**: Fibi Emlak web sitesi ve backend API'si test çalıştırılmadan önce ayakta olmalıdır (varsayılan: `http://localhost:5146`).

---

## Dosya Yapısı

- `cdp.mjs`: Headless Chrome sürecini başlatan, WebSocket üzerinden CDP ile bağlanan ve sayfa yönetimi sağlayan ortak modül (`go`, `ev`, `setViewport`, `screenshot`, `consoleErrors`, `close`).
- `site-smoke.mjs`: Sitenin statik ve dinamik sayfalarını, HTTP yanıtlarını, konsol hatalarını, kırık görsellerini ve responsive uyumluluğunu test eden ana duman testi.
- `README.md`: Kullanım kılavuzu ve açıklamalar.

---

## Çalıştırma Komutları

### 1. Varsayılan Ayarlarla Çalıştırma
Varsayılan olarak `http://localhost:5146` adresini hedefler:
```bash
node tools/ui-tests/site-smoke.mjs
```

### 2. Özel Site Adresi (URL) Belirtme
Parametre olarak vererek:
```bash
node tools/ui-tests/site-smoke.mjs http://localhost:5146
```

Ortam değişkeni ile (PowerShell):
```powershell
$env:SITE_URL="http://localhost:5146"; node tools/ui-tests/site-smoke.mjs
```

Ortam değişkeni ile (CMD / Bash):
```bash
SITE_URL=http://localhost:5146 node tools/ui-tests/site-smoke.mjs
```

### 3. Ekran Görüntülerini Kaydetme (`SHOTS_DIR`)
Test edilen sayfaların masaüstü ekran görüntülerini bir klasöre kaydetmek için:
```powershell
# PowerShell
$env:SHOTS_DIR="./tools/ui-tests/screenshots"; node tools/ui-tests/site-smoke.mjs
```
```bash
# Bash
SHOTS_DIR=./tools/ui-tests/screenshots node tools/ui-tests/site-smoke.mjs
```

---

## Kontrol Edilen Sayfalar ve Kriterler

### Kapsam
- **Statik Sayfalar**: `/`, `/satilik`, `/kiralik`, `/blog`, `/Contact`, `/AboutUs`, `/kvkk-aydinlatma-metni`, `/gizlilik-politikasi`, `/kullanim-kosullari`, `/haber-ver`, `/Compare`
- **Dinamik İlan Detayı**: `/satilik` sayfasındaki ilk 3 parçalı ve sonu `-<sayı>` ile biten link (ör. `/satilik/konut/ornek-ilan-123`).
- **Dinamik Blog Detayı**: `/blog` sayfasındaki ilk `/blog/...-<sayı>` linki.
- **Hata Sayfası (404)**: `/satilik/konut/olmayan-ilan-999999999` adresinin HTTP 404 dönmesi.
- **SEO Dosyaları**: `robots.txt` (HTTP 200, boş değil) ve `sitemap.xml` (HTTP 200, XML formatı ve en az bir `<loc>` kaydı).

### Sayfa Başına Yapılan Doğrulamalar
1. **HTTP Durumu**: `fetch` ile yönlendirmeler takip edilerek HTTP 200 yanıtı alındığı doğrulanır.
2. **JS Konsol ve İstisna Kontrolü**: Sayfa açılışında yakalanmamış istisna veya hata logu olmamalıdır. Dış kaynaklı hatalar (`challenges.cloudflare.com`, `tile.openstreetmap.org`, `google-analytics` vb.) uyarı (`WARN`) sayılır, testi başarısız kılmaz.
3. **Kırık Görsel Denetimi**: Lazy-load görselleri tetiklemek için sayfa sonuna kadar kaydırılıp beklenir; ardından `naturalWidth === 0` olan aynı origin kaynaklı görsellerin olmadığı doğrulanır.
4. **Title ve H1**: Sayfa başlığının (`<title>`) dolu olduğu ve sayfada tam olarak 1 adet `<h1>` bulunduğu (`/Compare` ve `/haber-ver` hariç) kontrol edilir.
5. **Mobil Görünüm (390x844)**: Mobil viewport'a geçilerek yatay taşma (`scrollWidth <= innerWidth + 1`) olmadığı doğrulanır.

---

## Çıktının Anlamı

- `OK   <sayfa> — <kontrol>`: İlgili kontrol başarıyla geçti.
- `FAIL <sayfa> — <kontrol> — <ayrıntı>`: İlgili kontrol başarısız oldu.
- `WARN <sayfa> — <ayrıntı>`: Dış kaynaklı hata veya bilgilendirme (testi fail etmez).
- **Özet**: En altta `PASS n FAIL m` raporlanır. Eğer `FAIL > 0` ise süreç çıkış kodu `process.exitCode = 1` olarak ayarlanır.

---

## Panel Testi Notu

Yönetim paneli girişi Cloudflare Turnstile CAPTCHA koruması kullanmaktadır. İleride eklenebilecek panel duman testleri ve headless otomasyonlar için Cloudflare'in resmi test anahtarlarının ortam değişkeni olarak tanımlanması gerekir:

- **Web (İstemci)**: `Captcha__TurnstileSiteKey=1x00000000000000000000AA`
- **API (Backend)**: `Captcha__TurnstileSecretKey=1x0000000000000000000000000000000AA`

> *Not: Mevcut duman testi yalnızca genel ziyaretçi (site) arayüzünü kapsamaktadır; panel duman testi henüz bulunmamaktadır.*
