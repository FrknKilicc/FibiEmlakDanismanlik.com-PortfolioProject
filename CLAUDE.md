# Fibi Emlak Danışmanlık — CLAUDE.md

Bu dosya Claude Code'un projeyi tanıması için repo root'ta bulunur.
Her oturumda otomatik okunur, context kaybı olmaz.

---

## Proje Kimliği
- **Ad:** Fibi Emlak Danışmanlık Sistemi
- **Geliştirici:** Furkan (youkali40@gmail.com)
- **AI Ajanları:** Claude Code (bu) + Antigravity (koordineli)
- **Framework:** .NET 8 (appsettings'de 7.0 yazar ama csproj net8.0)

---

## Gerçek Folder Structure (kodda olan)

```
FibiEmlakDanismanlik.sln
├── Core/
│   ├── FibiEmlakDanismanlik.Domain/
│   │   ├── Entities/          39 entity
│   │   ├── Enums/
│   │   └── DTOs/              (katman ihlali — ileride taşınacak)
│   └── FibiEmlakDanismanlik.Application/
│       ├── Constants/
│       ├── Features/
│       │   ├── Commands/[Feature]Commands/    Create/Update/Remove
│       │   ├── Queries/[Feature]Queries/      GetX / GetXById
│       │   ├── Handlers/[Feature]Handlers/    her komut için ayrı handler
│       │   └── Results/[Feature]Results/      GetXResult / GetXByIdResult
│       ├── Helpers/
│       ├── Interfaces/
│       ├── Mapping/
│       ├── Services/
│       └── ViewModels/
├── Infrastructure/
│   └── FibiEmlakDanismanlik.Persistence/     (klasör adı 2026-10-27 düzeltildi)
│       ├── Context/
│       ├── Migrations/        40 migration
│       └── Repositories/[Feature]Repositories/
├── Presentation/
│   └── FibiEmlakDanismanlik.WebApi/
│       └── Controllers/       26 controller
└── Frontends/
    ├── FibiEmlakDanismanlik.Dto/              ResultXDto, CreateXDto
    └── FibiEmlakDanismanlik.WebUI/            MVC + ViewComponent + Realshed tema + jQuery
```

**Akış:** WebUI (HttpClient) → WebApi Controller → IMediator.Send → Handler → IRepository → EF Core → SQL Server

---

## Kod Yazma Kuralları (Mevcut Pattern)

### Command
`Features/Commands/{Feature}Commands/{Create|Update|Remove}{Feature}Command.cs`
- `IRequest` uygular
- Remove komutu `int Id` ile constructor alır

### Query
`Features/Queries/{Feature}Queries/Get{Feature}Query.cs`
- `IRequest<List<Get{Feature}Result>>` veya `IRequest<Get{Feature}ByIdResult>` döner

### Handler
`Features/Handlers/{Feature}Handlers/{CommandOrQuery}Handler.cs`
- Basit CRUD → `IRepository<TEntity>`
- Karmaşık sorgular → `I{Feature}Repository`
- Mapping elle yapılır (AutoMapper sadece rental listingde var)

### Result
`Features/Results/{Feature}Results/Get{Feature}Result.cs`
- Entity asla dışarı dönmez

### Controller
```csharp
[Route("api/[controller]")]
[ApiController]
// Sadece IMediator enjekte edilir
// GET → Ok(result)
// POST/PUT → Ok("Türkçe başarı mesajı")
// DELETE → int id query parametresi
```

### Entity
`Domain/Entities`, `[Key] public int {Entity}Id`
Fluent API konfigürasyonu `OnModelCreating` içine eklenir.

### WebUI
ViewComponent + `IHttpClientFactory` + `_configuration["Url:ApiUrl"]`
Dto projesinde `Result{X}Dto` kullanılır. Realshed teması bozulmaz.
UI değişikliklerinde mevcut tasarım dili (Realshed sitesi, NiceAdmin paneli: renk, font, bileşen) korunur; yeniden tasarım yalnızca Furkan isterse yapılır. Değişiklik masaüstü + mobil ekran görüntüsüyle (Chrome DevTools MCP) kontrol edilir.

---

## Güvenlik Kuralları

- ~~**G1 KRİTİK:** Tüm POST/PUT/DELETE uçları kimliksiz~~ — ÇÖZÜLDÜ (2026-10-12): JWT (`POST api/Auth/login`, Agent Mail + şifre) ve 29 write ucunda `[Authorize]`. Bilerek anonim: `CustomerContact` (iletişim formu), `ForSales/filter`, `ForRental/filter`. İmzalama anahtarı User Secrets'ta `Jwt:Key` (en az 32 bayt), Issuer/Audience/süre appsettings'te
- ~~**G2 KRİTİK:** Agent şifreleri plain text~~ — ÇÖZÜLDÜ (2026-10-12): Create/UpdateAgent BCrypt ile hash'ler, mevcut kayıtlar hash'lendi
- ~~**G3 KRİTİK:** `sa` şifresi git geçmişinde~~ — ÇÖZÜLDÜ (2026-10-12): uygulama `fibiemlak_app` login'iyle bağlanır (yalnızca db_datareader + db_datawriter), connection string User Secrets'ta; `sa` şifresi değiştirildi ve devre dışı
- **G4:** `@Html.Raw` kullanan alanlar XSS açığı — HtmlSanitizer eklenecek
- Yeni yazılan tüm write endpoint'lere `[Authorize]` eklenir
- AdminUI CSP'si `script-src 'self'`: görünümlerde satır içi `<script>` ve `onclick=` gibi öznitelikler ÇALIŞMAZ. Davranışlar `wwwroot/js` altında harici dosyada, `data-*` öznitelikleriyle (`admin-ui.js`: `data-open-modal`, `data-filter-input`, `data-preview-img`, `data-count-target`, `data-help`, `data-confirm`)
- Plain text şifre asla DB'ye yazılmaz, asla log'a düşmez

---

## Makro Değişiklik Kuralları (KRİTİK)

Herhangi bir method, property, parametre değiştirilmeden önce:
1. Bağımlılık taraması: Controller, Handler, AutoMapper profili, DTO, ViewModel, JS/AJAX
2. Etki analizi yaz: "Bu değişiklik şu dosyaları etkiler: [liste]. Devam edeyim mi?"
3. Tüm bağımlıları tek seferde güncelle, derleme hatası bırakma
4. UI-API uyumunu kontrol et: response field adı değişirse jQuery kodu da güncellenir

---

## Branch Stratejisi

```
master          ← production (stabil)
└── dev         ← aktif geliştirme
    ├── feature/claude-xxx       ← Claude Code branch'leri
    └── feature/antigravity-xxx  ← Antigravity branch'leri
```

- Her görev başında hangi dosyalara dokunulacağı belirtilir
- Aynı dosya aynı anda iki ajan tarafından düzenlenmez

---

## Antigravity'ye iş devri (önce Antigravity modelleri, hepsi bitince Claude)

Küçük işler dahil (tek satırlık düzeltme bile) her kod değişikliği önce Antigravity CLI'a (`agy`) gider. Sen yönetici ve kontrolcüsün: keşfi yaparsın, görevi tam dosya yollarıyla yazarsın, build/test/git'i sen çalıştırırsın, çıktıyı kontrol edip onaylarsın, hata varsa düzelttirirsin.

Kodu kendin **YALNIZCA** şu iki durumda yazarsın:
- Listedeki tüm modeller denendi ve hepsi kota hatası verdi (çıkış kodu `10`)
- `agy` hiç çalışmıyor: kurulu değil / oturum kapalı (çıkış kodu `3`)

Kod yazma gerektirmeyen işleri (soru cevaplama, kod okuma, keşif, build/test/git) kendin yaparsın.

### Akış

1. `dev`'den `feature/antigravity-<konu>` branch'i aç.
2. Görevi tek, net bir prompt olarak yaz: amaç, uyulacak kalıp (bu dosyadaki kurallar) ve dokunulacak dosyaların **tam yolları**. Keşfi (hangi dosyalar, nerede ne var) sen yap ve gereken bağlamı prompt'a koy; Antigravity'den dosya araması veya proje taraması isteme (headless modda arama aracı yok, dosyaları tek tek açarak 15 dakikalık zaman aşımına takılıyor). Prompt'un sonuna her zaman şunu ekle: "Terminal komutu çalıştırma; sadece dosya okuma ve yazma araçlarını kullan. Build ve testi ben çalıştıracağım." (Headless modda komutlar izinsiz reddediliyor; build/test/git işini sen yaparsın.)
   **Oluşturulacak veya düzenlenecek her dosyanın tam yolu prompt'ta geçmeli.** Headless agy yazma izni soramaz ve klasör desenleri eşleşmez; betik prompt'taki repo içi tam dosya yolları için geçici `write_file(<yol>)` izni ekler, iş bitince `settings.json`'ı geri yükler. Yolu geçmeyen dosyaya agy yazamaz. Prompt'ta alt ajan kullanmamasını da iste (alt ajanlar dosya içeriği uydurabiliyor).
3. Prompt'u scratchpad'de bir dosyaya yaz ve çalıştır (uzun ve tırnaklı metin `-Task` ile komut satırında bölünür):
   `powershell -ExecutionPolicy Bypass -File tools/agy-delegate.ps1 -TaskFile <prompt.md>`
   Betik `tools/agy-models.txt`'teki modelleri sırayla dener. Bir model kota (`AGY_MODEL_QUOTA_EXHAUSTED`), izin (`AGY_NEEDS_PERMISSION`), yarım kalma (`AGY_INCOMPLETE`) veya genel hata (`AGY_ERROR`) verirse görevi listedeki **sıradaki modele** verir. Önceki model işe başlamışsa sıradaki model önce aynı konuşmayı (`--conversation`) sürdürmeyi dener; olmazsa görev başına "Önceki model işi yarıda bıraktı, git diff'e bak ve kaldığı yerden devam et" notu eklenerek yeniden gönderilir. `SUCCESS` gelip hiçbir dosya değişmediyse bu da yarım kalma sayılır (değişiklik beklenmeyen işler için `-AllowNoChanges`). Betik `--dangerously-skip-permissions` kullanmaz ve `useG1Credits`'i her zaman `false` tutar.
4. Çıkış koduna göre:
   - `0` → **Kontrol et** (aşağıda). Çıktıdaki `MODEL` ve `CONVERSATION_ID`'yi not al.
   - `10` (tüm modeller kota hatası verdi) veya `3` (agy yok / oturum kapalı) → yarım kalan değişiklikleri `git stash` ile kenara al, `feature/claude-<konu>` branch'ine geç, işi kendin yap. Kullanıcıya tek satırla "Antigravity modellerinin kotası bitti, devam ediyorum" (ya da "agy çalışmıyor, devam ediyorum") de.
   - `4` (izin), `5` (yarım kaldı), `1` (genel hata) → betik bunları zaten sıradaki modelle denedi; bu kodlar **liste bittiği hâlde** en az bir model kota dışı bir sebeple başarısız olduğunda döner. Kodu kendin yazma: çıktıdaki hataları özetleyip kullanıcıya bildir ve nasıl devam edileceğini sor (prompt'u düzeltip yeniden göndermek gibi).

Betik çıktısını `Select-Object -First N` gibi bir şeye pipe'lama: pipeline erken kesilirse betik yarıda ölür, `finally` çalışmaz ve geçici `write_file` izni `settings.json`'da kalır. Çıktıyı değişkene al, sonra kırp.

### Kontrol et, test et, onay ver

1. `git diff` ile değişiklikleri incele: bu dosyadaki kurallar, Makro Değişiklik Kuralları (bağımlılıklar güncellenmiş mi), güvenlik kuralları.
2. `dotnet build` çalıştır. Test projesi varsa `dotnet test` çalıştır.
3. Sorun varsa düzeltmeyi yine Antigravity'ye yaptır, aynı konuşmada:
   `powershell -ExecutionPolicy Bypass -File tools/agy-delegate.ps1 -TaskFile <düzeltme-prompt.md> -Conversation <CONVERSATION_ID>`
   Düzeltme prompt'unda da build/test çıktısı, inceleme notların ve düzenlenecek dosyaların tam yolları olsun.
4. En fazla 3 düzeltme turu. Üçüncüden sonra hâlâ sorun varsa kullanıcıya bildir ve sor. Düzeltme sırasında kod `10` veya `3` gelirse kalan düzeltmeyi kendin yap.
5. Her şey geçince commit at, `dev`'e PR aç; PR açıklamasında işi hangi modelin yaptığını ve kaç düzeltme turu gerektiğini yaz.

---

## Aktif TODO (Öncelik Sırasıyla)

### Yarın (2026-10-28) — önce bunlar
- [ ] **Elle kontrol (VS'te):** ilan formunda çoklu cephe kaydı (bir test turunda cephe NULL kaydedilmişti, sebep doğrulanmadı), koordinatla konum girme, il/ilçe/mahalle aramalı listeler, açıklama editörü, "Yakın Yerler" kartı; Site Metinleri ekranı (değişiklik sitede ≤30 sn); işlem formunda "Bağlı talep"; Ayarlar → Kullanılmayan görseller (yalnızca listele, silmeden önce bak); kontrol panelinde senkronizasyon hatası uyarısı; kenar menüde ElenAI (Yakında)
- [ ] **dev → master birleştirmesi:** (2026-10-31, Furkan'ın kararıyla elle kontrol beklenmeden) yerelde birleştirildi, build + 110 test geçti; **push edilmedi**: master push'u `deploy.yml`'i tetikler (`DEPLOY_ENABLED=true` ise canlıya migration + yayın, T5 kolon silme dahil). Push öncesi: origin'deki birleşmemiş `feature/claude-public-api-url` (canlı API adresi düzeltmesi) dev'e alınacak mı, karar
- [ ] **Canlı veritabanı:** `tools/sql/deploy/README.md` sırasıyla (yedek → idempotent migration betiği: Komple Bina, SiteTexts, LinkDealToLead → `data-fixes.sql` AboutUs görsel yolu). Canlıya erişim yalnızca Furkan'da
- [x] **ElenAI kullanıcısı (karar 2026-10-31):** ElenAI bir yapay zekâ/bot; insan kullanıcı gibi değerlendirilmez (2FA, gerçek e-posta, insan girişi yok). Yerel DB'de Sahip, Id 1046, `elenai@fibiemlak.com`, sitede gizli, rastgele bilinmeyen şifre, görsel `Realshed/assets/images/agents/Fibi-Emlak-Danismanlik-ElenAI.jpg`. Öneri: ElenAI yazılırken şifreyle giren Sahip hesabı yerine yalnızca gereken uçlara yetkili servis kimliği (API anahtarı/servis politikası) kullanılsın; Sahip rolü o zaman düşürülsün. Paneldeki "ElenAI (Yakında)" menüsü tıklanamaz, sayfası yok
- [x] **Teknik borç kararları:** (1) ~~`PropImgUrl1..30` kaldırma~~ 2026-10-28 2. aşama da bitti (kolonlar silindi, aşağıda T5); (2) ~~`PropertyRepository` bölme~~ 2026-10-27 partial class; (3) ~570 derleme uyarısı: (2026-10-31 karar) toplu temizlik YAPILMAYACAK, ciddi sorun çıkarmadıkça dokunulmaz; (4) migration squash ÖNERİLMİYOR (canlıda migration geçmişi var)
- [ ] Veri: 3 blog görseli dış sitelerden hotlink (harvard.edu, onthemarket, britannica): telif riski, panelden yerel görselle değiştirilmeli
- [ ] Veri: ilan 1000034 yakın yerlerinde "Kamu" kategorisinde "DHL" (OSM etiketi); panelde Yakın Yerler kartından düzeltilmeli
- [ ] Not: yerel DB değişikliklerinin (demo yorumlar pasif, Jeneratör birleştirme, kırık yazar görseli) yedekleri `bak20261019.*_before_20261026`; canlıda aynı veri sorunları varsa ayrıca bakılmalı

- [x] **[KRİTİK]** sa şifresini değiştir, ayrı SQL login aç (2026-10-12)
- [x] **[KRİTİK]** JWT + [Authorize] tüm write endpoint'lere (2026-10-12)
- [x] **[KRİTİK]** Agent şifrelerini BCrypt ile hash'le (2026-10-12)
- [x] Agents.Mail için unique index + migration; Create/UpdateAgent aynı maile 409 döner (2026-10-12)
- [x] (2026-10-23, `feature/antigravity-todo-batch2`) Html.Raw alanlarına HtmlSanitizer ekle (G4)
- [x] (Faz 3, AdminUI cookie oturumu, JWT sunucuda) Admin paneli eklenince: login ekranı + JWT'nin saklanması (HttpOnly cookie / sunucu tarafı oturum) ve write isteklerinde `Authorization: Bearer` gönderilmesi; süre dolunca yeniden login
- [x] (Faz 4, panel ilan düzenlemede vitrin kartı, ≤5) Admin paneli eklenince: ilan formunda "Vitrin ilanı" (BestDeals) checkbox'ı; satılık ve kiralıkta ayrı ayrı en fazla 5 (BestDealConstants.MaxPerUsageType) uyarısı. BestDeals şu an sadece Create komutlarında ve ForSaleCommercial Update'te var; diğer Update komutlarına eklenmeli
- [x] (2026-10-19, `feature/antigravity-todo-batch1`) Detay linklerine ilan türü ekle (id + listingTypeId): şu an detay sayfası yalnızca id ile konut/arsa/ticari tablolarına sırayla bakıyor; çakışma id aralıklarıyla (konut 1M+, arsa 2M+, ticari 3M+) geçici olarak önlendi
- [x] (2026-10-19) Veri: kırık ilan görselleri yerine `wwwroot/images/listings` altındaki CC0 demo görseller (`CREDITS.md`); gerçek ilan fotoğrafları admin yüklemesiyle gelmeli
- [x] (2026-10-24, `feature/antigravity-security-hardening`) Global exception middleware + handler'larda null kontrolü (G9): `ExceptionHandlers/GlobalExceptionHandler` (KeyNotFoundException → 404, diğerleri ayrıntısız Türkçe 500 + traceId), 20 handler null'da KeyNotFoundException fırlatır; `GetMainBannerById` yanlış repository tipiyle her istekte 500 veriyordu, düzeltildi. WebUI'nin API kapalıyken 500 vermesi ayrı madde
- [x] (2026-10-19, `feature/antigravity-todo-batch1`) PropertyLocationController → Query/Handler/Result (G5)
- [x] (2026-10-19, `feature/antigravity-todo-batch1`) FluentValidation + MediatR ValidationBehavior pipeline (G6); validator'lar: CustomerContact, Create/UpdateAgent, Login; ValidationException → 400 (`ExceptionHandlers/ValidationExceptionHandler`). Diğer write komutlarına da validator eklenmeli
- [x] (2026-10-24) ~~ListingImageSelection.ListingTypeId alanı (H1)~~ — gereksiz: kat planı artık `ListingImages.Kind` ile; `ListingImageSelections` tablosu (2 demo satır) okunmuyor, ileride onayla silinebilir
- [x] (2026-10-19, demo veri yenilemesi) Admin: 2 hatalı ilanın ilçesini düzelt (Kayseri-Ayazağa, Giresun-Ceyhan). Bu ikisinde `DistrictId` boş; ilçe düzeltilip kaydedilince SaveChanges ID'yi kendisi doldurur
- [x] (Faz 4'te yapılmıştı, 2026-10-25'te doğrulandı) İlan giriş formunda il/ilçe/mahalle cascade dropdown
- [x] (2026-10-24, `feature/antigravity-admin-complete`: `POST api/ContentImages` klasör `home`, panelde "Bilgisayardan yükle") Admin: ana sayfa bölüm görselleri için dosya yükleme ucu. Eski not: (`HomeSectionImages` yalnızca URL alıyordu). Karar: dosyalar WebApi `wwwroot/uploads/home-sections` altına yazılsın ve statik sunulsun; depolama bir arayüzün arkasında dursun, ileride blob'a taşınabilsin. Uzantı/MIME/boyut kontrolü yapılsın, dosya adı sunucuda üretilsin, uç `[Authorize]` olsun
- [x] (2026-10-19, `feature/antigravity-todo-batch1`) Mahalle için `NeighborhoodId`: il/ilçede olduğu gibi (ILocatedListing + SaveChanges çözümlemesi + migration backfill) ilanlara eklensin; filtre şu an mahalleyi metinle eşleştiriyor
- [x] (2026-10-19, `feature/antigravity-todo-batch1`) `tools/sql/seed-popular-places-demo.sql` doğrudan SQL ile eklediği için yeni demo ilanlarda `CityId`/`DistrictId` boş kalıyor; betiğin sonuna ListingLocationIds migration'ındaki backfill UPDATE'leri eklensin
- [x] LandCategories tablosunu doldur (2026-10-12, migration ListingIdRangesAndLandCategories)
- [x] (2026-10-19, `feature/antigravity-todo-batch1`) Teknik borç: kiralık listeleme bellekten DB sorgusuna taşı (T2)
- [x] (2026-10-19, `feature/antigravity-todo-batch1`) Teknik borç: facet sayaçları null statüyü sayıyor (T3)
- [x] (2026-10-19) localhost:7015 hardcode'ları appsettings'e taşı (G11): benzer ilanlar (satılık/kiralık) ve iletişim formu `Url:ApiUrl` kullanıyor; yalnızca `_SearchFieldSectionComponentPartial` içinde yedek değer olarak kaldı
- [x] (2026-10-19, `feature/antigravity-todo-batch1`) Kiralık fiyat dönemi: demo veride kira AYLIK kabul edildi; ilan detayında ve harita sayfasında "/ ay" gösteriliyor. Liste kartları, ana sayfa vitrini ve benzer ilan kartlarına da eklenmeli; eklendi. Admin formunda dönem alanı hâlâ netleşmeli
- [ ] Ana sayfa vitrini: kiralık vitrin 5 sınırında dolu (4 işyeri + 1 arsa); yeni işaretlenen ilan en eski işaretliyi düşürür
- [ ] **[GÜVENLİK]** GitHub reposu herkese açık (kimlik doğrulamasız erişilebiliyor). Repoda: kod, `appsettings`'teki `info@fibiemlak.com`, danışman ad ve telefonlarının geçtiği SQL demo betikleri, iş kuralları ve yönetim paneli tasarım belgesi; şifre ve anahtar yok. Yönetim paneli ve hak ediş modülü eklenmeden repo private yapılmalı: GitHub → Settings → Danger Zone → Change visibility
- [ ] Repo temizliği: (2026-10-27) dev'e birleşmiş 48 yerel branch silindi; origin'deki 23 birleşmiş branch'i Furkan silecek (`git branch -r --merged dev` listesi, `git push origin --delete ...`). (2026-10-27: `.claude/launch.json` ve `.claude/settings.local.json` .gitignore'a eklendi)

### Footer (2026-10-19'da DB'den dinamik yapıldı)
Footer (`_FooterLayoutViewComponentPartial`) `AboutUs/current` (en küçük id), `Contact` (ilk kayıt), `SocialMedia` (aktif, yalnızca http/https URL'ler) ve `Blog/GetLast3BlogWithAuthor` (son 2) uçlarından beslenir, 5 dk `IMemoryCache`'te tutulur (admin değişikliği en geç 5 dk'da görünür). "Sayfalar" sütunu bilerek sabit route listesi.
- [x] (Faz 8) Admin paneli: Hakkımızda, İletişim, Sosyal Medya ve Blog ekranları (`[Authorize]` CRUD uçları mevcut; footer, iletişim ve hakkımızda sayfaları bu tablolardan okuyor)
- [x] (2026-10-23, `feature/antigravity-todo-batch2`) `AboutUsController`'da liste/aktif kayıt ucu yok; footer ve Hakkımızda sayfası `AboutUs/1`'e sabit bağlı. "Aktif/ilk kaydı getir" ucu eklenip ikisi de ona geçmeli
- [x] (2026-10-24) Veri: `SocialMedias` bozuk kayıtları: Google Plus silindi, Twitter pasife alındı (doğru adres gelince panelden düzeltilip aktif edilir); yedek `bak20261019.SocialMedias_before_20261024`. Not: Facebook adresi `facebook.com/ankayapiemlakk`, doğru sayfa olduğu teyit edilmeli
- [ ] Veri: `Contacts.EMail1` = `info@fibiEmlak.com` (footer küçük harfe çeviriyor); eski footer'da elle yazılı `(0212) 510 01 14` vardı, DB'de `05442603193` — doğru ofis telefonu teyit edilmeli
- [x] (2026-10-20, `feature/antigravity-seo-and-fixes`) İletişim sayfası (`_GetContactInfoViewComponentPartial`) telefon linkleri sabit ve yanlış: `tel:+23055873407` / `tel:+23055873408` (tema kalıntısı); footer'daki gibi DB numarasından üretilmeli
- [x] (2026-10-20, `feature/antigravity-seo-and-fixes`) Footer kozmetik: blog küçük görselleri eşit 90x90 değil (tema CSS'i `width/height`'ı eziyor), "Devamını Oku" linki yeşil yerine gri görünüyor
- [x] (2026-10-23, `feature/antigravity-todo-batch2`) Footer "Kullanım Koşulları" (`/kullanim-kosullari`) ve "Gizlilik Politikası" (`/gizlilik-politikasi`) sayfaları yazıldı ve bağlandı. Metinler taslak: canlı öncesi hukukçu okumalı; ayrıntılı KVKK aydınlatma metni ayrı madde
- [x] (2026-10-31, karar: panele eklenmeyecek, kodda sabit) "Sayfalar" sütunu header ile uyumlu: Satılık, Kiralık, Haritada İlanlar, İlan Çıkınca Haber Ver, FİBİ Hizmetler, Ekibimiz, Hakkımızda, Blog, SSS, İletişim

### İlan verisi ve detay sayfası (2026-10-19)
Demo ilanlar `tools/sql/demo-listings-refresh.ps1` ile üretilen `demo-listings-refresh.sql` ile yenilendi (gerçek mahalleler, FİBİ emsal m² değerlerinden fiyat, türetilmiş özellikler, CC0 görseller); yakın yerler `tools/sql/nearby-from-osm.ps1` ile OpenStreetMap'ten (Nominatim) dolduruldu. Öncesinin yedeği `bak20261019` şemasında. Detay sayfasındaki "İlan Özellikleri" ortak bileşen: `Models/ListingSpecs` + `Views/Shared/_ListingSpecs.cshtml`; ada/parsel/pafta değerleri bilerek gösterilmez.
- [ ] `bak20261019` yedek şeması, yeni veri doğrulandıktan sonra silinsin (`DROP` işlemleri onayla)
- [x] (2026-10-19, `feature/antigravity-todo-batch1`) Kullanılmayan eski partial'lar silinsin: `_ForSaleDetailPropertyDetails_*`, `_ForRentalDetailPropertyDetails_*`, `_AmenitiesListViewComponentPartial` (detay sayfası artık `_ListingSpecs` kullanıyor; başka yerde kullanılmadığı doğrulanmalı)
- [x] (2026-10-19, `feature/antigravity-todo-batch1`) Benzer ilanlar çok katı (aynı il + fiyat ±%30 + m² ±%30); gerçekçi veride çoğu ilanda boş dönüyor. Sonuç yoksa kademeli gevşetme (önce fiyat, sonra il) eklenmeli
- [x] (2026-10-20, `feature/antigravity-seo-and-fixes`) Detay sayfası danışman kutusundaki ofis adresi sabit metin; `Contacts` tablosundan gelmeli
- [ ] Yakın yerler ilan koordinatı yaklaşık (mahalle merkezi) olduğu için yaklaşık mesafeler; admin kesin pin girince `nearby-from-osm.ps1 -OnlyMissing` (o ilanın kayıtları silinerek) yeniden çalıştırılmalı; ileride sync sağlayıcısına dönüştürülebilir
- [ ] Demo görseller repo içinde (~9 MB, `wwwroot/images/listings`); gerçek fotoğraflar geldiğinde klasör ve CREDITS kaldırılmalı, yükleme deposu (blob) kararı verilmeli

### Gözlemler ve öneriler (2026-10-19, kullanıcı gün içinde soracak; sırayla birlikte yapılacak)

**Git ve süreç**
- [x] (2026-10-20) Push + dev'e birleştirme: önce `feature/antigravity-fibi-services-phase2`, sonra `feature/antigravity-listings-map` (harita branch'i Faz 2'den açıldı, sync altyapısını kullanıyor). Footer, dil ve iç sayfa düzeltmeleri de bu branch'lerde; kullanıcı onayıyla
- [x] (2026-10-26, `feature/antigravity-tests-and-cleanup`: `Tests/FibiEmlakDanismanlik.Unit.Tests` 91 test; `tools/ui-tests/site-smoke.mjs` CDP duman testi, README'de) Testler: (2026-10-25) `Tests/FibiEmlakDanismanlik.Persistence.Tests` (xUnit) açıldı; repository'leri LocalDB'de (`(localdb)\MSSQLLocalDB`) her çalıştırmada yeni oluşturulup silinen geçici veritabanına karşı çalıştırır (SQL Server'a özgü sequence/NEWID/GETDATE yüzünden SQLite/InMemory olmaz). Şu an: talep hunisi (kümülatif) ve işlem günü (UTC 22:30 → Türkiye ertesi gün). Çalıştırma: `dotnet test Tests/FibiEmlakDanismanlik.Persistence.Tests`. Eksikler: `ListingSpecsBuilder`, `KfeBulletinParser`, `IbbSeismicCsvParser`, `MarketIndexApplier`, `TurkishTextHelper` için xUnit testleri; headless Chrome kontrol betikleri (şu an oturum scratchpad'inde: harita, detay, taşma taraması) `tools/ui-tests` altına alınıp duman testi olarak çalıştırılabilir hale getirilsin
- [x] (2026-10-25, `feature/antigravity-site-batch3`, tarayıcıda) Yeni veriyle doğrulama: ana sayfa bölümleri dolu, kırık yerel görsel yok; satılık liste tür/oda/fiyat filtreleri doğru; karşılaştırma tablosu dolu. Bulunan ve düzeltilenler: (0) sayılı özellikler filtrede gizlendi; menüdeki `/Compare` seçim varken boş sayfa açıyordu (artık seçime yönlenir; `compare-listing.js` önbellek kırıcı sürümle yüklenir). Kalan: ana sayfada 3 görsel dış siteden (onthemarket, britannica, cukurcafotosafari — sonuncusu kırık; muhtemelen müşteri yorumu demo verisi, panelden düzeltilmeli); bülten başlığı İngilizce ("Sign Up To Our Newsletter", bülten maddesiyle birlikte); "Jeneratör" özelliği iki ayrı kayıt olarak listede iki kez çıkıyor (veri); (2026-10-27) liste yan panelinde Bina Yaşı / Isıtma / Tapu Durumu filtreleri (tek ilan türü seçiliyken, seçenekler `listingtypes/advanced-filters`'tan); arama önerileri denenmedi
- [x] (2026-10-26, `Restore-Boms`) Antigravity satır sonlarını LF yapıyor ve BOM'u siliyor (her turda elle geri ekleniyor); `tools/agy-delegate.ps1` iş bitince değişen dosyalarda BOM'u HEAD'deki duruma göre otomatik geri yüklesin

**Hata ve tutarsızlıklar**
- [x] (2026-10-24, `feature/antigravity-small-fixes`) WebUI sayfaları API kapalıyken 500 veriyordu: tüm HttpClient'larda `Services/ApiUnavailableHandler` bağlantı hatasını 503 yanıtına çevirir (mevcut `IsSuccessStatusCode` kontrolleri boş içerik yoluna düşer) ve 15 sn devre kesici uygular (Windows'ta kapalı porta her deneme ~2 sn sürüyordu, ana sayfa 20 çağrıyla 45 sn'de açılıyordu → ~4 sn); 5 ViewComponent `View(null)` yerine boş liste döner. İlan/blog detayı ve il açılış sayfaları API hatasında bilerek 500 verir (SEO kuralı)
- [x] (2026-10-20, `feature/antigravity-ui-polish`) Tüm sayfalarda `<title>` sabit ("Fibi Yapı Emlak & Danışmanlık", `_HeadLayoutViewComponentPartial`); `ViewData["Title"]` kullanılmıyor ve 8 view'da "Index" yazıyor. Sayfa başına başlık + meta description; ilan detayında ilan adı
- [x] (2026-10-20, `feature/antigravity-ui-polish`) Mobil menüde (`ScaffTemplate.cshtml`) tema kalıntıları: sahte telefon `+88 01682648101`, `info@example.com` linki, sosyal linkler `/Realshed/index.html`; footer gibi `Contact` ve `SocialMedia` tablolarından gelmeli
- [x] (2026-10-20, `feature/antigravity-ui-polish`) Header "Sayfalar" mega menüsü İngilizce tema demo linkleri (About Us, Our Services, Pricing Table, Compare Properties → `/Realshed/*.html`); gerçek sayfalara bağlanmalı veya kaldırılmalı
- [x] (2026-10-21, `feature/antigravity-seo-urls-cleanup`) Sayfa başlığı arka plan görselleri (9 dosya) yerel: `wwwroot/images/page-title/page-title-bg.jpg` (`CREDITS.md`; StockCake lisansı canlı öncesi teyit edilmeli). İletişim sayfasındaki pexels "harita" fotoğrafı hâlâ hotlink (aşağıdaki harita maddesiyle çözülecek)
- [x] (2026-10-20, `feature/antigravity-ui-polish`) Detay sayfası `h1` her ilanda "Satılık İlanlar"/"Kiralık İlanlar" (sayfa başlığı bandı); breadcrumb ilan adını göstermiyor
- [x] (2026-10-23) `Blogs` Id 15 başlığı Id 10'un, metni Id 1'in kopyasıydı; yeni yazıyla değiştirildi ("Ev Alırken Tapu Öncesi Kontrol Listesi", kategori Ev Alım Satım Süreçleri, yerel görsel). Eski kayıt `bak20261019.Blogs_15_before_20261023` ve `BlogTags_15_before_20261023`
- [x] (2026-10-20, `feature/antigravity-ui-polish`) `MapRepository.GetListingMarkerAsync` nullable uyarıları (dönüş tipi `ListingMarkerResult` ama null dönüyor); imza `Task<ListingMarkerResult?>` yapılmalı
- [ ] Danışman e-postaları kişisel Gmail adresleri (`...fibi@gmail.com`); kurumsal alan adı adresleri tercih edilmeli
- [x] (2026-10-20, `feature/antigravity-ui-polish`) Liste ve detayda ilan "Durum" etiketi (Aktif/Opsiyonlu) ziyaretçiye gösterilmiyor; "Opsiyonlu" ilanlarda rozet gösterilmesi düşünülmeli

**Beklenen / önerilen geliştirmeler**
- [x] (2026-10-23, `feature/antigravity-todo-batch2`) Danışman kutusuna WhatsApp ile yaz ve tek tıkla ara butonları (mobilde sabit alt çubuk)
- [x] (2026-10-23, `feature/antigravity-todo-batch2`) İlan detayında kredi taksit hesaplayıcı (FİBİ Finans modülüyle ortak; `BankLoanRates` verisi). Oranlar `BankLoanRates` tahmini başlangıç verisi; banka sağlayıcıları yazılınca güncellenir
- [x] (2026-10-23, `feature/antigravity-todo-batch2`) Galeri: küçük resim şeridi, fotoğraf sayısı rozeti, tam ekran (fancybox zaten yüklü)
- [x] (2026-10-23, `feature/antigravity-todo-batch2`) SEO: ilan detayına schema.org `RealEstateListing`/`Offer` yapılandırılmış verisi, Open Graph görseli, `sitemap.xml` ve `robots.txt`
- [x] (2026-10-25, `feature/antigravity-site-batch3`) Görseller: demo görsellerin (`wwwroot/images/listings`, 72 JPEG) yanına `-400.webp`/`-800.webp` üretildi (SkiaSharp, kalite 80, +5.4 MB); `ResponsiveImageTagHelper` ve `listings-map.js` `sizedImageUrl` bu dosyalara srcset verir (`shortcut:` yorumlu; demo klasörü silinince kalıp da kaldırılmalı). Panelden yüklenen fotoğraflar zaten 400/800/1600 WebP
- [x] (2026-10-20, `feature/antigravity-seo-and-fixes`) Spam koruması: iletişim ve randevu formlarına rate limit (FİBİ lead formundaki gibi) ve isteğe bağlı captcha
- [x] (Faz 5'te yapıldı: `ListingAlerts`, `/haber-ver`) "Benzer ilan çıkınca haber ver" talep formu
- [x] (2026-10-25, `feature/antigravity-site-batch3`) Liste sayfalarında "Liste | Harita" geçişi: `listings-map.js` gömülü mod (`data-embedded="true"`, URL'ye yazmaz, gizliyken kurulmaz, `lm:show` olayıyla açılır), başlangıç filtresi listedeki tür/il/ilçe/fiyat/odadan; seçim `sessionStorage` `fibi-list-view`; `wwwroot/js/listing-view-toggle.js`
- [ ] Harita sayfası: (2026-10-25 yapıldı: fiyat aralığı ve oda filtresi, URL'de `minPrice`/`maxPrice`/`rooms`); kalan: "bu alanda ara" (bbox) ile API tarafı filtreleme
- [x] (2026-10-26: Serilog günlük dosya `logs/`, 14 gün; sync hatası LogError + kontrol panelinde uyarı) İzleme: yapılandırılmış log (Serilog) ve sync sağlayıcıları için hata bildirimi (Nominatim/TCMB/İBB başarısız olursa)

### SEO ve form güvenliği (2026-10-20, branch `feature/antigravity-seo-and-fixes`)
İlan detay adresi `/satilik|kiralik/{konut|arsa|isyeri}/{ilce-ilan-adi}-{id}`; tek kaynak `Models/ListingUrl.cs` (`DetailPath`, `Slugify`), harita için aynı kural `wwwroot/js/listings-map.js` içinde (değişirse ikisi birlikte güncellenmeli). Eski `/AllFor*DetailPage/Index/{id}` adresleri ve yanlış slug 301 ile asıl adrese gider; ilan yoksa 404, API hatası 500. Canonical ve JSON-LD mutlak adresleri `Seo:SiteBaseUrl` ayarından (boşsa istekten). Form koruması `Services/FormSpamGuard.cs` (IP başına sınır `FormSpam:*`, honeypot `Website` alanı, isteğe bağlı Turnstile `Captcha:*`), formlarda `<partial name="_FormSpamFields" />`.
- [x] SEO dostu ilan adresleri + 301, canonical etiketi, detayda BreadcrumbList (JSON-LD), blog detayında başlık/açıklama, görsel alt metinleri, Türkçe 404/hata sayfası (`noindex`), olmayan ilanda API 404
- [x] İletişim ve randevu formlarına IP başına sınır + honeypot + isteğe bağlı captcha; iletişim formu artık POST'ta API'ye gönderiyor; randevu dönüşü yalnızca aynı host'a
- [ ] Canlıya çıkarken: `Seo:SiteBaseUrl` canlı alan adıyla ayarlanmalı; Google Search Console kaydı ve sitemap gönderimi
- [x] (2026-10-23, `feature/antigravity-todo-batch2`) `sitemap.xml` (yukarıdaki SEO maddesi) ilan adreslerini `ListingUrl.DetailPath` ile üretmeli
- [x] (2026-10-23, `feature/antigravity-todo-batch2`) Open Graph ile birlikte Twitter Card etiketleri
- [x] (2026-10-21, `feature/antigravity-seo-urls-cleanup`) Blog adresleri `/blog/{slug}-{id}` (`Models/BlogUrl.cs`), liste `/blog`; eski `/Blog/Detail/{id}` ve `/Blog/Index` 301; olmayan blogda 404, API hatasında 500. Öneri ucu `blog/suggest` (JSON'a `url` alanı eklendi)
- [x] (2026-10-21, `feature/antigravity-seo-urls-cleanup`) Liste adresleri `/satilik`, `/kiralik`; açılış sayfaları `/satilik/{il}` ve `/satilik/{il}/{ilce}` (`Services/ListingLocationResolver.cs`, il/ilçe API listesinden slug ile, 6 saat önbellek; bulunamazsa 404). Detay route'u `listingcategory` constraint'i ile ayrışır (`konut|arsa|isyeri|ilan`; bir il bu adlardan birini alamaz). Eski `/AllForSalePage`, `/AllForRentalPage` 301
- [x] (2026-10-25, `feature/antigravity-site-batch3`) Kategori + konum açılış sayfaları: `/satilik|kiralik/{konut|arsa|isyeri}[/{il}[/{ilce}]]` (`ListingLocationResolver.ResolveCategoryAsync`, ortak `Controllers/ListingLandingResult.Render`; 3 parçalı adres detay route'una düşer, id yoksa açılış sayfası sayılır; `ilan` kategorisi 404), sitemap'te. (2026-10-27) Yalnızca `cityId`/`city`/`districtId`/`district`/`page=1` içeren `/satilik|kiralik?..` adresi açılış sayfasına 301 (`ListingLocationResolver.FindNamesByIdsAsync`); açılış sayfaları zaten sitemap'teydi
- [ ] Captcha'yı açmak için Cloudflare Turnstile anahtarı: `Captcha:TurnstileSiteKey`, `Captcha:ScriptUrl`, `Captcha:VerifyUrl` appsettings'e, `Captcha:TurnstileSecretKey` User Secrets'a; dördü de doluysa formlarda kendiliğinden açılır
- [x] (2026-10-25, `feature/antigravity-panel-fixes`) WebUI'ye ForwardedHeaders + `ForwardedHeaders:KnownProxies` eklendi (WebApi ile aynı kalıp, varsayılan loopback); canlıda proxy IP'si ayara yazılmalı
- [ ] `FormSpamGuard` sayaçları bellek içi; WebUI birden fazla örnekle çalışırsa paylaşılmaz (gerekirse dağıtık önbellek)
- [ ] API CORS (`AllowLocalhost`) yalnızca sabit localhost portlarına izin veriyor; harita sayfası API'yi tarayıcıdan çağırdığı için canlı alan adı ayardan okunmalı
- [x] (2026-10-24, G9 ile) API'deki diğer GetById handler'ları da kayıt yoksa null kontrolsüz olabilir (ör. blog detayı); G9 kapsamında taranmalı
- [ ] Veri: `Contacts.PhoneNumber2` = `PhoneNumber1` (iletişim sayfasında aynı numara iki kez), `EMail2` kişisel Gmail (`fibiinfo@gmail.com`)
- [x] (2026-10-25, `feature/antigravity-site-batch3`) Kozmetik: detay başlık bandı `page-title--listing` (clamp 26–42px, en fazla 2 satır); 404/hata sayfası mobilde `padding-top: 170px` (header absolute ~132px)
- [ ] Veri: `Contacts.EMail1` iletişim sayfasında olduğu gibi `info@fibiEmlak.com` (büyük harfli) görünüyor; link küçük harfe çevriliyor ama kayıt düzeltilmeli
- [x] (2026-10-27, `feature/antigravity-small-batch2`, karar: kaldır) Ana sayfadaki bülten aboneliği formu tema kalıntısıydı (`action="contact.html"`), bölüm kaldırıldı
- [x] (2026-10-21'de yapılmıştı, 2026-10-25'te doğrulandı) Blog detayında yazar adı linki kaldırıldı (düz başlık)
- [x] (2026-10-25, `feature/antigravity-site-batch3`) İletişim sayfasında gerçek harita: Leaflet, `wwwroot/js/office-map.js`, koordinat `appsettings` `Office:Latitude/Longitude/Zoom` + "Yol tarifi al". **Koordinat yaklaşık** (Telsiz Mahallesi merkezi; 85/2. Sokak OSM'de yok): ofisin kesin konumu ayara yazılmalı. Site haritalarında OSM karoları alt alan adsız `tile.openstreetmap.org`
- [x] (2026-10-23) Bu branch dev'e birleştirildi ve push edildi (aşağıdaki elle test listesi ayrıca yapılmadı; otomatik duman testiyle doğrulandı)

**Elle test listesi (bu branch için, kullanıcı 2026-10-21'de yapacak)**
- [ ] Eski ilan linki (`/AllForSaleDetailPage/Index/{id}?listingTypeId=..`) yeni adrese 301 ile gidiyor; tarayıcı adres çubuğunda okunabilir adres görünüyor
- [ ] Satılık ve kiralık her ilan türünde (konut, arsa, işyeri) detay açılıyor; olmayan id Türkçe 404 sayfası veriyor
- [ ] Ana sayfa vitrini, sıcak fırsatlar, liste sayfaları, benzer ilanlar, danışman sayfası ve harita popup'larındaki ilan linkleri doğru ilana gidiyor
- [ ] Sayfa kaynağında `<link rel="canonical">` ve ilan detayında `application/ld+json` var; filtreli liste sayfasında canonical filtresiz
- [ ] İletişim formu: gönderim başarılı mesajı, kayıt `customerContacts` tablosunda; 5 gönderimden sonra (10 dk) "çok fazla deneme" uyarısı
- [ ] İlan detayında randevu formu: gönderim sonrası aynı ilana dönüyor, toastr mesajı çıkıyor
- [ ] İletişim sayfası telefon/e-posta linkleri telefonda tıklanınca arama/e-posta açıyor
- [ ] Footer'da "Devamını Oku" yeşil, blog küçük görselleri kare; blog detayında sekme başlığı yazının adı
- [ ] VS ile çalıştırırken (WebUI 5146, API 7015) harita sayfası ilanları gösteriyor

### Haritada İlanlar (2026-10-19, branch `feature/antigravity-listings-map`)
`ListingsMap` sayfası `GET api/Map/listings` ucundan satılık + kiralık tüm aktif ilanları tek haritada gösterir. Koordinatı olmayan ilanlara `listing-geocode` sağlayıcısı (sync altyapısı, 6 saatte bir) OSM Nominatim ile mahalle > ilçe > il düzeyinde yaklaşık konum yazar (`ListingGeoPoints.Source = nominatim`, `Precision = neighborhood|district|city`); haritada kesikli çerçeveyle gösterilir.
- [x] (Faz 4'te yapıldı: `api/AdminListings` harita pini, panelde "Konumu Kaydet"; 2026-10-25'te harita karo sorunu da düzeldi) Admin paneli: ilan formunda haritadan kesin pin seçme (Leaflet tıkla/sürükle); kaydedince `ListingGeoPoints` kaydı `Source = manual`, `Precision = building` olarak yaklaşık kaydın yerine yazılsın (şu an `AddGeoPointsAsync` mevcut kaydı ezmiyor, güncelleme ucu ayrıca yazılmalı)
- [x] (2026-10-19, `tools/sql/demo-listings-refresh.ps1`) Veri: demo ilanların çoğunda mahalle alanı il/ilçeyle uyumsuz (ör. Kadıköy ve Çankaya ilanlarında "Veliefendi"); bu yüzden 68 ilanın 47'si yalnızca ilçe düzeyinde konumlandı. Mahalle düzeltilince ilgili `nominatim` kayıtları silinirse sağlayıcı yeniden konumlar
- [x] (2026-10-19, `feature/antigravity-todo-batch1`) İlan güncellenip ili/ilçesi/mahallesi değişirse `nominatim` kaynaklı eski koordinat kalıyor; adres değişikliğinde yaklaşık kayıt silinmeli (manuel pin hariç)
- [ ] Harita sayfası tüm ilanları tek istekte alıp istemci tarafında filtreliyor (şimdilik 70 ilan); ilan sayısı birkaç bini geçerse `Map/listings` ucunun mevcut `usageType/category/cityId` filtreleri ve harita sınırlarına göre (bbox) sorgu kullanılmalı
- [ ] Nominatim: User-Agent'a iletişim eklendi (2026-10-23, `FibiMarketDataSync:UserAgentContact` = info@fibiemlak.com; adresin çalıştığı teyit edilmeli). Canlıda yoğun kullanımda kendi geocoder'ımız veya ücretli servis değerlendirilmeli
- [x] (2026-10-23, `feature/antigravity-todo-batch2`) Leaflet ve MarkerCluster unpkg CDN'den yükleniyor (detay sayfası da öyle); CDN erişilemezse harita çalışmaz, `wwwroot/lib` altına alınabilir
- [x] (2026-10-21, `feature/antigravity-seo-urls-cleanup`) Tema demo `.html` linkleri temizlendi (danışman karuseli, satılık liste ikonu, blog yazar linki, Hakkımızda'daki İngilizce uygulama bölümü); kullanılmayan `_FeaturedRentalPropertViewComponentPartial` silindi. Yalnızca ana sayfa bülten formu (`action="contact.html"`) kaldı, kendi maddesinde

---

### Yönetim Paneli (karar 2026-10-23, tasarım `docs/admin-panel-tasarim.md`)
Ayrı proje `Frontends/FibiEmlakDanismanlik.AdminUI`, adres `yonetim.fibiemlak.com`, tema NiceAdmin (ücretsiz sürüm; footer kredi linki kalır, Quill kullanılmaz, TinyMCE 6.x'te kalınır, ECharts/ApexCharts/Boxicons/Remixicon atılır). Roller: Sahip (1-2 kişi, her şey + Yönetici/Danışman yönetimi), Yönetici (Sahip yönetimi hariç her şey, Danışman ekler/çıkarır), Danışman (kendi ilanları + izin verilen ilanlar; kazanç ve üye yönetimi yok). Yetki API'de zorlanır.
- [x] (2026-10-23) Faz 0: `todo-batch2` tarayıcı testi (sayfalar, 127 sitemap adresi, OG/JSON-LD, galeri, mobil çubuk, kredi hesabı) ve seo-and-fixes → seo-urls-cleanup → listing-badges → todo-batch2 zincirinin dev'e birleştirilip push edilmesi
- [x] (2026-10-23, `feature/antigravity-admin-security`, 27/27 uçtan uca test) Faz 1 — API güvenlik ve roller: `Agents.Role`, `SecurityStamp`, `CanViewAllListings`, `ListingAccessGrants`; politikalar; giriş sertleştirme (deneme sınırı, kilit, `LoginAuditLogs`); `AuditLogs`; TOTP 2FA; Turnstile doğrulama; son Sahip korunur
- [x] (2026-10-23, `feature/antigravity-listing-images`) Faz 2 — Fotoğraf yapısı (T5): `ListingImages` tablosu (316 görsel aktarıldı), eski `PropImgUrl1..30` kolonlarına otomatik senkron (okuyucular değişmedi), yükleme ucu (dosya imzası, 15 MB, EXIF yönü, GPS dahil metadata silinir, 1600/800/400 WebP, `/uploads` statik), `fibi-responsive` tag helper ile kart/vitrin/benzer/karşılaştırma/galeri/harita duyarlı görsel. Eski kolonlar ancak tüm okumalar yeni tabloya geçince (onayla) kaldırılacak
- [ ] Faz 2 kalanları: (kat planı 2026-10-24'te yapıldı: `ListingImages.Kind` Fotoğraf/Kat Planı/Kroki/İmar Planı + `Caption`; planlar galeriye ve eski `PropImgUrl` kolonlarına girmez, kapak olamaz; ilan detayında "Planlar ve Krokiler"); (`SkiaImageProcessor` bitmap sızıntısı maddesi eskimiş: `OrientBitmap` kaynağı dispose ediyor); Linux VPS'te `SkiaSharp.NativeAssets.Linux` eklenmeli; canlıda `FileStorage:PublicBaseUrl` ve uploads klasörü yedeği ayarlanmalı
- [x] (2026-10-23, `feature/antigravity-admin-ui`, tarayıcı testi 17/17) Faz 3 — AdminUI iskeleti: NiceAdmin ayıklama + Türkçe, giriş + 2FA + captcha, cookie oturumu (JWT sunucuda), güvenlik başlıkları, noindex. AdminUI her API isteğinde ziyaretçinin gerçek IP'sini `X-Forwarded-For` ile iletmeli (yoksa API deneme sınırı, Turnstile ve giriş kayıtları AdminUI sunucusunun IP'sini görür); API bunu yalnızca bilinen proxy'den (aynı VPS'te loopback) kabul eder
- [x] (2026-10-24, `feature/antigravity-security-hardening`) Güvenlik iyileştirmesi: TOTP kodu 30 sn penceresi içinde yeniden kullanılabiliyordu; `ITwoFactorService.VerifyCodeOnce` son kabul edilen zaman adımını hesap bazında BELLEKTE tutar (şema değişikliği yok). Sınırlama: tek sunucu içindir, uygulama yeniden başlarsa 30 sn içinde aynı kod bir kez daha geçebilir; birden çok örnekte `Agents`'a kolon + migration gerekir
- [x] (2026-10-23, `feature/antigravity-admin-listings`, API testi 32/32, panel testi 16/16) Faz 4 — İlan yönetimi: `api/AdminListings` (tek liste, detay, kaydet, durum, vitrin ≤5, harita pini, izinler, Sahip'e özel kalıcı silme), `IListingAccessService` (Danışman kendi ilanı + izin), panelde liste/form/fotoğraf/harita/durum/vitrin/izin/silme; eski dağınık yazma uçları yalnızca Sahip/Yönetici
- [x] Faz 4 kalanları: (2026-10-25, `feature/antigravity-panel-fixes`) Sahip'e özel izinler ve kalıcı silme kartları geçici Sahip hesabıyla tarayıcıda denendi: izin ver/kaldır, "SİL" onayı, silinen ilan 404. Bulunan hata düzeltildi: izin eklenince API `true` dönerken panel liste bekliyordu, kayıt olduğu hâlde hata uyarısı çıkıyordu. Panel ilan haritası boş/"Access blocked" çıkıyordu: panelin `Referrer-Policy: no-referrer` başlığı OSM karo politikasına takılıyordu; `listing-map.js` karo katmanında `referrerPolicy: 'strict-origin-when-cross-origin'` (yalnızca karolara köken gider) ve alt alan adsız `tile.openstreetmap.org`. (Tapu filtresi maddesi gereksiz: seçenekler ilanlardaki değerlerden facet olarak üretiliyor, o değerde ilan girilince kendiliğinden görünür.) 2026-10-24'te `feature/antigravity-small-fixes` ile düzeltildi: "Sorumlu Danışman" listesi tüm aktif çalışanları gösterirken kayıt yalnızca Danışman rolünü kabul ediyordu (Yönetici/Sahip'e atanmış ilanlar panelden kaydedilemiyordu; artık aktif herhangi bir çalışan); panel kaydı ilan numarasını (`PropertyNo`, formda alan yok) siliyordu (artık gönderilmezse korunur; veri kaybı olmamıştı). Not: panelden kayıt mahalle adını normalize eder ("Veliefendi" → "Veliefendi Mahallesi"), bu da yaklaşık `nominatim` koordinatını sildirir (sağlayıcı 6 saat içinde yeniden konumlar)
- [x] (2026-10-24, `feature/antigravity-admin-leads`; API testleri 31/31 + 38/38, panel 15/15 + 17/18, site 10/10) Faz 5 — Talepler: `LeadStatus` (Yeni → Arandı → Randevu → Teklif → Kapora → Kazanıldı/Kaybedildi), atama, `LeadActivities` (not, arama, durum, atama), ilan düzenlemede "Müşteri Talepleri" kartı; randevu talebi ilanın danışmanına otomatik atanır. Haber bekleyenler: `ListingAlerts` + `ListingAlertMatches`, site formu (`/haber-ver`, ilan detay kutusu, liste çağrı kutusu; KVKK onayı, aynı telefonla en fazla 5 aktif kayıt), panelde liste/detay ve ilan kartında "Bu İlanı Bekleyenler". Eşleşme: kayıttan SONRA eklenen aktif ilan, aynı tür/il(/ilçe), fiyat ve oda; otomatik e-posta/SMS yok
- [ ] Faz 5 kalanları: (2026-10-26 toplu sorguya çevrildi, `GetPendingCountsAsync` + test) haber bekleyenlerde "eşleşeni olanlar" filtresi ve özet her aktif kayıt için ayrı sorgu çalıştırıyor (kayıt yüzleri geçince toplu sorguya çevrilmeli); (2026-10-24, `feature/antigravity-kvkk-settings`: `customerContacts.KvkkApprovedAt` eklendi; iletişim formuna da KVKK kutusu eklendi, API onaysız talebi 400 ile reddeder, panel talep detayında onay zamanı görünür. Aynı branch'te eski hata düzeltildi: e-postasız iletişim formu ve e-postasız/mesajsız randevu talebi API'de İngilizce "required" hatasıyla reddediliyor, kaydedilmiyordu); sitedeki iletişim ve randevu formlarında antiforgery doğrulaması (2026-10-24'te eklendi); (2026-10-27, `feature/antigravity-small-batch2`) tarih kuralı: iş tarihleri (talep, haber bekleyen, ilan oluşturma, işlem tarihleri, panel "bugün") `Domain/Common/TurkeyClock` ile sunucu saat diliminden bağımsız Türkiye saati, `*Utc` alanları UTC; panel ikisini de Türkiye saatinde gösterir. Piyasa verisi senkronizasyonu hâlâ `DateTime.Now` (yalnızca süre hesabı); (2026-10-27) ilan fiyat/kira/aidat/depozito decimal alanlarına `HasPrecision(18, 2)` (şema değişmedi, EF uyarısı kalmadı)
- [x] (2026-10-24, `feature/antigravity-admin-deals`; API testi 64/64, panel 28/28) Faz 6 — **Hak ediş**: `ListingDeals` (kapora, anlaşılan bedel, alıcı/satıcı komisyonu oran veya tutar; kiralıkta varsayılan komisyon bir kira) + `DealShares` (çalışan başına oran veya tutar prim, o andaki rol, ödeme durumu); `api/AdminDeals` yalnızca Sahip/Yönetici; panelde `/islemler` liste/özet/filtre, yeni işlem (ilan düzenleme kartından), detay (prim ekle/düzenle/sil, ödendi, tamamla, iptal + kapora iadesi). İşlem açılınca ilan Opsiyonlu, tamamlanınca Satıldı/Kiralandı, iptalde Aktif'e döner. Kurallar: ilana bir açık işlem, primler komisyonu aşamaz, ödenmiş prim silinemez/tutarı değişmez, işlemi olan ilan kalıcı silinemez. Danışman hiçbir tutar görmez
- [ ] Faz 6 kalanları: (2026-10-25 düzeltildi: işlem tarihi kaporasız/tamamlanmamış işlemde artık `CreatedAtUtc.AddHours(3).Date`, Türkiye günü); (2026-10-26 yapıldı: işlem formunda bağlı talep, açılınca Kapora, tamamlanınca Kazanıldı, iptalde not; migration `LinkDealToLead`) işlem kaydı talep ile otomatik bağlanmıyordu. (Panel ilan formundaki ondalık sorunu 2026-10-24'te düzeltildi: AdminUI'de `Infrastructure/InvariantDecimalModelBinder` tüm decimal/double alanları önce noktalı, virgül varsa tr-TR biçimiyle okur; enlem/boylam da etkileniyordu)
- [x] (2026-10-24, `feature/antigravity-admin-dashboard`; API testi 18/19 — tek başarısızlık test beklentisi, panel 18/18) Faz 7 — Kontrol paneli: `api/AdminDashboard` (ilan/talep/haber bekleyen özetleri, aralıkta talep hunisi, bugünkü/7 günlük randevu, en çok talep alan 5 ilan, bu ay kapora/tamamlanan, son 12 ay işlem-komisyon serisi, çalışan performansı; Danışman'a finans yok, yalnızca kendi satırı ve tutarlar 0). Panelde animasyonlu sayaçlar + Chart.js (aylık, ilan dağılımı, talep hunisi, danışman performansı), tarih aralığı; `/raporlar` (Sahip/Yönetici) aylık özet ve çalışan tabloları, CSV indirme (UTF-8 BOM, `;`, formül enjeksiyonu koruması): aylık, çalışanlar, işlem listesi
- [ ] Faz 7 kalanları: (2026-10-25 düzeltildi: talep hunisi kümülatif: aralıkta oluşturulan her talep, şu anki durumu ve `LeadActivities` durum geçmişinden bulunan en ileri aşamaya kadar her aşamada bir kez sayılır; Kaybedildi yalnızca şu anki durumsa sayılır; testi var); (2026-10-27 düzeltildi) CSV'de sayı olan hücreler formül önekini almaz; "en çok talep alan ilanlar"da silinmiş ilan "Silinmiş ilan" olarak linksiz görünür
- [x] Faz 8 — (2026-10-24, `feature/antigravity-admin-users`, dev'e birleştirildi b2f25cc) Kullanıcılar (`/kullanicilar`: liste/filtre, ekle, düzenle, rol, aktiflik, 2FA sıfırlama, kilit; Yönetici yalnızca Danışman yönetir — panel testi 24/24), Denetim kaydı (`/denetim-kaydi`, şifre alanları maskeli), İçerik (`/icerik`: Hakkımızda, İletişim, Sosyal medya, SSS, Ana sayfa banner — API 21/21, panel 20/20; sosyal medya yazma uçları ve içerik doğrulayıcıları eklendi), Blog (`/icerik/blog`; API kategori + doğrulama + 404, API testi 14/14). Blog panel tarayıcı testi 43/44 (tek başarısızlık test betiğinin kendi 404 isteği). Test sırasında bulunan hata düzeltildi: panel boş metin alanlarını `null` gönderiyordu, API `[ApiController]` örtük zorunluluk kontrolüyle İngilizce 400 dönüyor ve İletişim'deki isteğe bağlı alanlar temizlenemiyordu; artık boş dize gidiyor ve FluentValidation'ın Türkçe mesajları görünüyor
- [x] (2026-10-24, `feature/antigravity-admin-content2`; tarayıcı testi 50/50 + blog regresyonu 43/44, tek başarısızlık betiğin kendi 404'ü) Faz 8 kalanları: blog yazarları (`/icerik/blog/yazarlar`) ve kategorileri (`/icerik/blog/kategoriler`) ekranları + API (yazısı olan yazar/kategori silinemez → 409; `Blogs.AuthorId` FK'si CASCADE olduğu için yazar silmek yazılarını da silerdi), yazar güncellemesinde görsel adresi kaydedilmiyordu (düzeltildi); blog etiketleri (formda virgülle, mevcut etiket önerileri, `GET api/Blog/tags`; yalnızca işlemde çıkarılıp yetim kalan etiket silinir); içerik görseli yükleme (`POST api/ContentImages`, klasörler blog/banner/about/author/agent, 1600px WebP, `uploads/content/...`) ve blog, Hakkımızda, banner, yazar, kullanıcı formlarında "Bilgisayardan yükle"; site içi göreli görseller panelde `Url:SiteUrl` ile önizlenir; Kullanıcılar formu boş alanları boş dize gönderir; görsel adresi kuralı `ContentUrlRules.IsSafeImageUrl` (`//` ve `\` reddedilir; danışman fotoğrafına da eklendi). Veri: `AboutUs.AboutUsImgUrl` `\realshed\assets\images\fibi emlak danışmanlık başarı yolunda.png` idi (yalnızca Windows'ta çalışır, yeni kural da reddeder) → `/Realshed/assets/images/Fibi Emlak Danışmanlık başarı yolunda.png`; **canlı DB'de de aynı düzeltme yapılmalı**
- [x] (2026-10-24, `feature/antigravity-kvkk-settings`; tarayıcı testi Ayarlar 16/16 + KVKK 8/8) Ayarlar ekranı (iş kuralları): `BusinessSettings` tek satırlık tablo (varsayılan 2/2/1/5/5), `api/AdminSettings` (GET Yönetici+, PUT yalnızca Sahip), panelde `/ayarlar` yalnızca Sahip'e; satış alıcı/satıcı komisyon oranı ve kiralık komisyon (kira sayısı) yeni işlem formunda öneri olarak doldurulur (API'deki "boşsa bir kira" yedeği aynı kaldı), vitrin sınırı ve haber bekleyen telefon başına sınır ayardan okunur. Sosyal medya bozuk kayıtları da temizlendi (yukarıda)
- [x] (2026-10-24, `feature/antigravity-admin-complete`; tarayıcı testi 23/23) İçerik eksikleri: Müşteri yorumları ve İş ortakları (`IsActive` + `DisplayOrder`, mevcut kayıtlar yayında), Ana sayfa bölüm görselleri (yuva kartları), Şehir galerisi ve Kredi oranları (CRUD uçları + ekranlar), içerik görseli klasörleri home/testimonial/partner/city/bank. Not: sitedeki 3 müşteri yorumu hâlâ demo metin ("Başlık 1 Deneme"); panelden düzeltilmeli/gizlenmeli
- [ ] Faz 8 kalanları (2): canlıda AdminUI `Url:SiteUrl` (site adresi, önizleme için) ve WebApi `FileStorage:PublicBaseUrl` ayarlanmalı (yüklenen görseller mutlak adresle kaydediliyor, alan adı değişirse eski kayıtlar eski adresi gösterir); içerik görselleri için tablo yok: görsel değiştirilince eski dosya `uploads/content` altında kalır (2026-10-26: Ayarlar → Kullanılmayan görseller, yalnızca Sahip, 24 saat eşiği); (2026-10-25 düzeltildi: blog silinince etiket bağları ve başka yazıda kullanılmayan etiketler de silinir)

### Panel UX ve hesap (2026-10-24, `feature/antigravity-admin-complete`)
Testler: UX 24/24, Hesabım 15/15, kat planı 17/17, içerik 23/23; açık/koyu ve 390 px mobil ekran görüntüsü incelemesi.
- [x] Tema: Açık / Koyu / Sistem (`data-bs-theme`, `localStorage` `fibi-theme`, `js/theme-init.js` head'de senkron, `js/theme.js`, `css/theme.css` `--fibi-*` değişkenleri). Yeni görünümlerde `bg-white`/`bg-light`/`text-dark` yerine `bg-body`/`bg-body-tertiary`/`text-body`
- [x] Bildirimler: `window.fibiToast(mesaj, tür, { items })`; sayfa içi başarı uyarıları otomatik toast'a döner, hatalar sayfada da kalır; gönderimde düğmede spinner
- [x] Zorunlu alanlar: etiketlerde yıldız otomatik, formlar `novalidate` + ortak doğrulama (eksik alanları adlarıyla listeleyen bildirim, ilk alana kaydırma, yalnızca hatalı alan kırmızı); alan ipuçları `data-help` (ⓘ tooltip) ilan, kullanıcı, işlem, ayar ve içerik formlarında
- [x] İçerik Merkezi `/icerik` (gruplu kutucuklar, sayaçlar, demo/pasif uyarıları, arama), tek satır kaydırılabilir içerik menüsü, SSS/Banner/Sosyal medya düzenleme modallarla, ortak silme onayı (`data-confirm`)
- [x] İlan: tür seçimi sayfası `/ilanlar/yeni`, yeni ilanda adım adım sihirbaz, düzenlemede bölüm gezinmesi + tamamlanma göstergesi + canlı önizleme, para alanları tr-TR binlik ayraçlı (gönderimde ham değere çevrilir), karakter sayaçları, kaydedilmemiş değişiklik uyarısı, Ctrl+S; sunucunun zorunlu tuttuğu alanlar (fiyat/kira, il, ilçe, kategori) istemcide de zorunlu
- [x] Hesabım `/hesabim`: profil, şifre değiştirme (tüm oturumlar kapanır), kurtarma kodu yenileme, tüm cihazlardan çıkış, giriş geçmişi
- [x] Ayarlar `/ayarlar` (Sahip): komisyon varsayılanları, vitrin ve haber bekleyen sınırları (`BusinessSettings`)
- (2026-10-27) İlan düzenlemede değişiklik yapılmasa da (ve kaydettikten sonra) "kaydedilmemiş değişiklik" çıkış uyarısı çıkıyordu: temel görüntü sabit 800 ms'de alınıyor, TinyMCE/konum listeleri sonra yazıyordu. Artık ilk gerçek kullanıcı etkileşiminde (pointerdown/keydown, TinyMCE için `fibi-user-intent`) alınır; dokunulmadıysa uyarı yok (`listing-form-ux.js`)
- Bulunan eski hatalar düzeltildi: demo görselli ilanlarda panel fotoğraf göstermiyordu (Width/Height null); düz metin API yanıtında fotoğraf silme/sıralama başarılıyken hata gösteriyordu; e-postasız iletişim/randevu talepleri reddediliyordu; dashboard grafikleri sözdizimi hatasıyla çizilmiyordu
- [ ] Kalan: ilan listesi, talepler, işlemler ve raporlar sayfaları yalnızca tema/bildirim katmanından yararlanıyor; bu sayfalara özel tasarım turu yapılmadı

### Site mobil UX (2026-10-31, `feature/antigravity-mobile-ux`)
26 sayfa × 360/390/768 px otomatik tarandı (yatay taşma, ekran dışı öğe, dokunma hedefi, yazı boyutu) + 390 px kaydırmalı ekran görüntüleri. Düzeltilenler: tema demo renk değiştiricisi (palet simgesi, içeriği örtüyordu, çerezle site rengini değiştirebiliyordu) kaldırıldı; ≤767 px iç sayfa başlıkları absolute header altında kalıyordu (`.page-title` 155px üst boşluk); liste sayfalarında sonuç/Liste|Harita satırı ekran soluna taşıyordu; mobilde filtreler "Filtrele" düğmesiyle katlanır (`fibi-filter-panel`, `listing-view-toggle.js`), ilanlar ilk ekranda; Danışmanlar sayfasında liste yan sütundan önce; yukarı çık düğmesi footer alt bağlantılarını örtmüyor; KVKK onay kutuları 18px. Kurallar `propertyListStyle.css` sonunda "Mobil düzeltmeler" bloğu. Sonrası: 78 sayfa/genişlikte taşma 0.
- [ ] Veri (mobil taramada görüldü): danışman kartlarında açıklama "string" (AgentDescription demo değeri); Hakkımızda "Yorumlar" bölüm metni "Lorem ipsum"; panelden düzeltilmeli
- [ ] İsteğe bağlı: harita sayfasında mobilde 71 ilanlık liste haritanın altında ~14 ekran; sayfalama/"daha fazla" düşünülebilir

### FİBİ Hizmetler Faz 2 (branch `feature/antigravity-fibi-services-phase2`, 2026-10-18'de yarıda kaldı)
Bitti: kademeli emsal şeması + migration (yerel DB'ye uygulandı), mahalle > ilçe > il > Türkiye fallback'i, İBB senaryosundan deprem tehlikesi, senkronizasyon çekirdeği, Infrastructure projesi (seed / İBB / TCMB KFE bülteni / opsiyonel EVDS sağlayıcıları, arka plan servisi, `[Authorize]` sync uçları), KvkkApprovedAt, ForwardedHeaders.
- [x] (2026-10-19) Canlı veri senkronizasyonunu uçtan uca test et (WebApi'yi scratchpad'e derleyip ayrı portta çalıştır; seed + İBB 959 mahalle + TCMB bülteni DB'ye yazılmalı, `sync/logs` kontrol edilmeli; Risk/Bölge uçları verisi olmayan bir ilçede 404 yerine il/Türkiye verisiyle dönmeli)
- [ ] Banka faizi sağlayıcıları: her banka için kendi açık sayfasından oran okuyan `IMarketDataProvider` (oranlar HTML'de değil hesaplama araçlarında; banka banka incelenmeli). Kırılan banka son oranında kalır
- [x] (2026-10-19) WebUI: konum seçimini Türkiye geneline aç (mevcut `api/Location/cities|districts|neighborhoods` cascade); `api/FibiServices/locations` ucu ve `GetFibiServiceLocations*` kaldırılsın; sonuçlarda veri kapsamı/kaynak etiketi ve İBB deprem özeti gösterilsin
- [ ] Fibi Finans modülü (kredi + kira başabaş, banka karşılaştırma, tapu günü gizli maliyetler)
- [ ] Fibi Eksper modülü (şerefiye katsayılı değerleme bandı + satıcı lead)
- [ ] Fibi Yatırım modülü (bütçe + strateji ile en iyi 5 bölge)
- [ ] EVDS anahtarı alınırsa `dotnet user-secrets set "FibiMarketDataSync:EvdsApiKey" "<anahtar>"`; sağlayıcının yanıt biçimi o zaman doğrulanmalı
- [ ] KVKK aydınlatma metni: (2026-10-25, `feature/antigravity-site-batch3`) `/kvkk-aydinlatma-metni` taslak sayfası yazıldı (veri sorumlusu bilgileri `Contacts`'tan; footer, sitemap, Gizlilik Politikası ve 6 formdaki onay metni bağlandı; "taslak" notu yalnızca Development'ta görünür). Kalan: canlı öncesi hukukçu okumalı; saklama süreleri (talep sonrası 2 yıl, haber ver 1 yıl hareketsizlik) metne yazıldı ama kodda otomatik silme yok
- [ ] Canlıda reverse proxy IP'si `ForwardedHeaders:KnownProxies` ayarına yazılmalı

---

## Bilinen Teknik Borçlar

- ~~`PropertyRepository` DRY ihlali (T1)~~ — (2026-10-31) projeksiyonlar da birleşti: tür başına tek `ForSale{Housing|Land|Commercial}Rows()` / `Rental{Housing|Land|Commercial}Rows()` (durum filtresiz); satılık liste = bunlar + aktif durum filtresi, detay = aynı satır + özellikler + fotoğraflar (satılmış ilan detayı dönmeye devam eder), kiralık `GetAllForRentalPropertyForListing` = aynı satırlar. 566 yanıt önce/sonra: kaybolan alan yok, yalnızca boş alanlar doldu (listede danışman tel/mail, işyerinde cephe/alan/kredi/takas, arsa detayında özellikler ilk kez, kiralık detayda tür kimlik alanı); işyeri artık m² dolu olduğu için alan filtresine ve benzer ilan eşleşmesine giriyor; kullanılmayan kiralık liste ucunda `propertyType` "Kiralık". Kiralık filtre sorguları (`BuildRental*Query`, farklı sonuç tipi) ve "son 10" sorguları ayrı kaldı. Geçmiş: (2026-10-27) Davranış değiştirmeden `partial class` olarak bölündü: `PropertyRepository.cs` (kurucu + ortak), `.ForSale.cs`, `.Rental.cs`, `.Similar.cs`, `.Search.cs` (satır satır doğrulandı); dosyalar UTF-8 BOM. (2026-10-28) Kiralık 3 sorgu kurucusundaki ortak filtreler (tür, il/ilçe/mahalle, fiyat, danışman, arama) `ApplyCommonRentalFilters<T>`, 6 detaydaki özellik yükleme `LoadAmenitiesAsync` (ikisi `PropertyRepository.cs`); ölü `GetFilteredForSalePropertyForListing` silindi; 380 API yanıtı önce/sonra birebir aynı. Kalan (bilerek): konut/arsa/işyeri Select projeksiyonları alan alan farklı (by-id yanıtında `CityId` yok, durum filtresi yok); birleştirmek JSON'u değiştirir, ayrı karar ister
- ~~30 ayrı `PropImgUrl1..30` kolonu (T5)~~ — çözüldü (2026-10-28): 2. aşamada entity alanları, eski uçlardaki atamalar, `SyncLegacyColumnsAsync` ve kolonlar (migration `RemoveLegacyPropImgUrlColumns`, 6 tabloda 180 kolon) silindi. API/DTO/Result `PropImgUrl1..30` alanları `ListingImages`'tan doldurulmaya devam eder (JSON aynı; 380 yanıt önce/sonra birebir). Yerel DB'ye uygulandı (yedek `fibiemlak_before_t5_drop_20261028.bak`, öncesinde eski kolon/`ListingImages` tutarlılığı 317/317); **canlıya da uygulanmalı**. `tools/sql/demo-listings-refresh.ps1` artık çalışmaz (başında uyarı)
- ~~Katman ihlali: WebUI → Application direkt referans (T6)~~ — çözüldü (2026-10-28): WebUI yalnızca Dto'ya bağlı (`CreateCustomerContactDto`, `PropertyFilterRequestDto` API'dekiyle aynı alanlar, biri değişirse diğeri de); site metinleri API'den, API kapalıyken son başarılı değerler
- ~~`Persistance` yazım hatası klasör adında (T7)~~ — çözüldü (2026-10-27): klasör `FibiEmlakDanismanlik.Persistence`
- 12 deneme-yanılma migration — production öncesi squash (T9)
- BaseEntity yok — her entity kendi Id'sini taşıyor (T10)
- ~~Soft delete yok (T10)~~ — (2026-10-27) ilanlar (6 tablo) ve blog: `ISoftDeletable.DeletedAtUtc` + global filtre, `SaveChanges` silmeyi işarete çevirir; Sahip'e özel kalıcı silme `AllowHardDelete` ile fiziksel. Çalışanlar kapsam dışı (zaten pasife alınıyor; filtre ilanları gizlerdi). Silinmişleri geri alma ekranı yok. Migration `ListingBlogSoftDelete` yerel DB'ye uygulandı (yedek `fibiemlak_before_softdelete_20261027.bak`); canlıya da uygulanmalı
- ~570 derleme uyarısı, çoğu nullable (CS8618/CS8602/CS8603); yeni kodda yenisi eklenmemeli; toplu temizlik yapılmayacak (karar 2026-10-31) (T11)
- ~~Detay sayfası ilanı yalnızca id ile buluyor (T12)~~ — çözüldü: linkler listingTypeId taşıyor; parametresiz eski linkler için sıralı arama yedek olarak duruyor
- İlan görselleri harici sitelere hotlink (emlakjet, depositphotos, unsplash); kaynak silinince kırılıyor (T13)
- ~~Kiralık entity'lerde `ListingTypeId` int? (T14)~~ — çözüldü (2026-10-27): `int`, FK NO ACTION açıkça yapılandırıldı; migration `RentalListingTypeIdRequired` (yerel DB'ye 2026-10-27'de uygulandı; canlıya da uygulanmalı)
- `GetUnifiedForSale/RentalPropertyById` ve listeleme sorguları PropertyRepository içinde tekrar ediyor; BestDealRepository ayrı tutuldu (T1 ile birlikte ele alınmalı)

---

## Çalışma Komutları

Kullanıcı Visual Studio'da hata ayıklarken `bin` klasörleri kilitlidir: CLI'dan derleme gerekiyorsa `-p:OutDir=<scratchpad>/` ile ayrı klasöre derle, iş bitince `dotnet build-server shutdown`. Test için başlatılan WebApi/WebUI süreçleri iş biter bitmez kapatılır; VS oturumu açıkken sunucu başlatmadan önce kullanıcıya sorulur.

```bash
# WebApi çalıştır
cd Presentation/FibiEmlakDanismanlik.WebApi
dotnet run

# WebUI çalıştır
cd Frontends/FibiEmlakDanismanlik.WebUI
dotnet run

# Migration ekle
dotnet ef migrations add <MigrationName> --project Infrastructure/FibiEmlakDanismanlik.Persistence --startup-project Presentation/FibiEmlakDanismanlik.WebApi

# DB güncelle
dotnet ef database update --project Infrastructure/FibiEmlakDanismanlik.Persistence --startup-project Presentation/FibiEmlakDanismanlik.WebApi
```

Yeni makinede WebApi'nin açılması için JWT imzalama anahtarı gerekir (yoksa uygulama başlarken hata verir):

```powershell
cd Presentation/FibiEmlakDanismanlik.WebApi
dotnet user-secrets set "Jwt:Key" "<en az 32 baytlık rastgele değer>"
```

Uygulama login'i (`fibiemlak_app`) yalnızca okuma/yazma yetkisine sahip, şema değiştiremez.
`dotnet ef database update` için Windows kimlik doğrulamasıyla bağlan; DbContext bu ortam değişkenini User Secrets'tan önce okur (PowerShell):

```powershell
$env:ConnectionStrings__DefaultConnection = "Server=<SUNUCU>\<INSTANCE>;Database=fibiemlak.com;Integrated Security=True;TrustServerCertificate=True"
dotnet ef database update --project Infrastructure/FibiEmlakDanismanlik.Persistence --startup-project Presentation/FibiEmlakDanismanlik.WebApi
Remove-Item Env:ConnectionStrings__DefaultConnection
```

## graphify

This project has a knowledge graph at graphify-out/ with god nodes, community structure, and cross-file relationships.

Rules:
- When graphify-out/graph.json exists, run `graphify query "<question>"` before your first grep, search or source-file read for a codebase question, and put this rule in every subagent prompt that explores code. Read raw files only for lines the graph or an earlier read already located. Use `graphify path "<A>" "<B>"` for relationships and `graphify explain "<concept>"` for focused concepts. These return a scoped subgraph, usually much smaller than GRAPH_REPORT.md or raw grep output.
- If graphify-out/wiki/index.md exists, use it for broad navigation instead of raw source browsing.
- Read graphify-out/GRAPH_REPORT.md only for broad architecture review or when query/path/explain do not surface enough context.
- After modifying code, run `graphify update .` to keep the graph current (AST-only, no API cost).
- A committed graphify-out/graph.json is rewritten whole on every build, so a large diff to it in a pull request is expected. Do not flag it as churn or suggest skipping the graph rebuild or its hooks.
