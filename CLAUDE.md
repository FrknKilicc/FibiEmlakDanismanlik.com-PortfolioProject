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
│   └── FibiEmlakDanismanlik.Persistance/     (klasör "Persistance", namespace "Persistence")
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

---

## Güvenlik Kuralları

- ~~**G1 KRİTİK:** Tüm POST/PUT/DELETE uçları kimliksiz~~ — ÇÖZÜLDÜ (2026-10-12): JWT (`POST api/Auth/login`, Agent Mail + şifre) ve 29 write ucunda `[Authorize]`. Bilerek anonim: `CustomerContact` (iletişim formu), `ForSales/filter`, `ForRental/filter`. İmzalama anahtarı User Secrets'ta `Jwt:Key` (en az 32 bayt), Issuer/Audience/süre appsettings'te
- ~~**G2 KRİTİK:** Agent şifreleri plain text~~ — ÇÖZÜLDÜ (2026-10-12): Create/UpdateAgent BCrypt ile hash'ler, mevcut kayıtlar hash'lendi
- ~~**G3 KRİTİK:** `sa` şifresi git geçmişinde~~ — ÇÖZÜLDÜ (2026-10-12): uygulama `fibiemlak_app` login'iyle bağlanır (yalnızca db_datareader + db_datawriter), connection string User Secrets'ta; `sa` şifresi değiştirildi ve devre dışı
- **G4:** `@Html.Raw` kullanan alanlar XSS açığı — HtmlSanitizer eklenecek
- Yeni yazılan tüm write endpoint'lere `[Authorize]` eklenir
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

- [x] **[KRİTİK]** sa şifresini değiştir, ayrı SQL login aç (2026-10-12)
- [x] **[KRİTİK]** JWT + [Authorize] tüm write endpoint'lere (2026-10-12)
- [x] **[KRİTİK]** Agent şifrelerini BCrypt ile hash'le (2026-10-12)
- [x] Agents.Mail için unique index + migration; Create/UpdateAgent aynı maile 409 döner (2026-10-12)
- [ ] Html.Raw alanlarına HtmlSanitizer ekle (G4)
- [ ] Admin paneli eklenince: login ekranı + JWT'nin saklanması (HttpOnly cookie / sunucu tarafı oturum) ve write isteklerinde `Authorization: Bearer` gönderilmesi; süre dolunca yeniden login
- [ ] Admin paneli eklenince: ilan formunda "Vitrin ilanı" (BestDeals) checkbox'ı; satılık ve kiralıkta ayrı ayrı en fazla 5 (BestDealConstants.MaxPerUsageType) uyarısı. BestDeals şu an sadece Create komutlarında ve ForSaleCommercial Update'te var; diğer Update komutlarına eklenmeli
- [x] (2026-10-19, `feature/antigravity-todo-batch1`) Detay linklerine ilan türü ekle (id + listingTypeId): şu an detay sayfası yalnızca id ile konut/arsa/ticari tablolarına sırayla bakıyor; çakışma id aralıklarıyla (konut 1M+, arsa 2M+, ticari 3M+) geçici olarak önlendi
- [x] (2026-10-19) Veri: kırık ilan görselleri yerine `wwwroot/images/listings` altındaki CC0 demo görseller (`CREDITS.md`); gerçek ilan fotoğrafları admin yüklemesiyle gelmeli
- [ ] Global exception middleware + handler'larda null kontrolü (G9)
- [x] (2026-10-19, `feature/antigravity-todo-batch1`) PropertyLocationController → Query/Handler/Result (G5)
- [x] (2026-10-19, `feature/antigravity-todo-batch1`) FluentValidation + MediatR ValidationBehavior pipeline (G6); validator'lar: CustomerContact, Create/UpdateAgent, Login; ValidationException → 400 (`ExceptionHandlers/ValidationExceptionHandler`). Diğer write komutlarına da validator eklenmeli
- [ ] ListingImageSelection.ListingTypeId alanı + migration (H1)
- [x] (2026-10-19, demo veri yenilemesi) Admin: 2 hatalı ilanın ilçesini düzelt (Kayseri-Ayazağa, Giresun-Ceyhan). Bu ikisinde `DistrictId` boş; ilçe düzeltilip kaydedilince SaveChanges ID'yi kendisi doldurur
- [ ] İlan giriş formunda il/ilçe cascade dropdown
- [ ] Admin: ana sayfa bölüm görselleri için dosya yükleme ucu (`HomeSectionImages` şu an yalnızca URL alıyor). Karar: dosyalar WebApi `wwwroot/uploads/home-sections` altına yazılsın ve statik sunulsun; depolama bir arayüzün arkasında dursun, ileride blob'a taşınabilsin. Uzantı/MIME/boyut kontrolü yapılsın, dosya adı sunucuda üretilsin, uç `[Authorize]` olsun
- [x] (2026-10-19, `feature/antigravity-todo-batch1`) Mahalle için `NeighborhoodId`: il/ilçede olduğu gibi (ILocatedListing + SaveChanges çözümlemesi + migration backfill) ilanlara eklensin; filtre şu an mahalleyi metinle eşleştiriyor
- [x] (2026-10-19, `feature/antigravity-todo-batch1`) `tools/sql/seed-popular-places-demo.sql` doğrudan SQL ile eklediği için yeni demo ilanlarda `CityId`/`DistrictId` boş kalıyor; betiğin sonuna ListingLocationIds migration'ındaki backfill UPDATE'leri eklensin
- [x] LandCategories tablosunu doldur (2026-10-12, migration ListingIdRangesAndLandCategories)
- [x] (2026-10-19, `feature/antigravity-todo-batch1`) Teknik borç: kiralık listeleme bellekten DB sorgusuna taşı (T2)
- [x] (2026-10-19, `feature/antigravity-todo-batch1`) Teknik borç: facet sayaçları null statüyü sayıyor (T3)
- [x] (2026-10-19) localhost:7015 hardcode'ları appsettings'e taşı (G11): benzer ilanlar (satılık/kiralık) ve iletişim formu `Url:ApiUrl` kullanıyor; yalnızca `_SearchFieldSectionComponentPartial` içinde yedek değer olarak kaldı
- [x] (2026-10-19, `feature/antigravity-todo-batch1`) Kiralık fiyat dönemi: demo veride kira AYLIK kabul edildi; ilan detayında ve harita sayfasında "/ ay" gösteriliyor. Liste kartları, ana sayfa vitrini ve benzer ilan kartlarına da eklenmeli; eklendi. Admin formunda dönem alanı hâlâ netleşmeli
- [ ] Ana sayfa vitrini: kiralık vitrin 5 sınırında dolu (4 işyeri + 1 arsa); yeni işaretlenen ilan en eski işaretliyi düşürür
- [ ] Repo temizliği: dev'e birleşmiş feature branch'leri (yerel + origin) silinsin mi karar ver; `.claude/launch.json` commit'lenecek mi yoksa .gitignore'a mı eklenecek

### Footer (2026-10-19'da DB'den dinamik yapıldı)
Footer (`_FooterLayoutViewComponentPartial`) `AboutUs/1`, `Contact` (ilk kayıt), `SocialMedia` (aktif, yalnızca http/https URL'ler) ve `Blog/GetLast3BlogWithAuthor` (son 2) uçlarından beslenir, 5 dk `IMemoryCache`'te tutulur (admin değişikliği en geç 5 dk'da görünür). "Sayfalar" sütunu bilerek sabit route listesi.
- [ ] Admin paneli: Hakkımızda, İletişim, Sosyal Medya ve Blog ekranları (`[Authorize]` CRUD uçları mevcut; footer, iletişim ve hakkımızda sayfaları bu tablolardan okuyor)
- [ ] `AboutUsController`'da liste/aktif kayıt ucu yok; footer ve Hakkımızda sayfası `AboutUs/1`'e sabit bağlı. "Aktif/ilk kaydı getir" ucu eklenip ikisi de ona geçmeli
- [ ] Veri: `SocialMedias` tablosunda bozuk kayıtlar var (Google Plus → `blog-1.html`, Twitter → `/UIlayout/Index`); footer bunları gizliyor ama düzeltilmeli/pasife alınmalı. Google+ kapandı, kaydı silinebilir
- [ ] Veri: `Contacts.EMail1` = `info@fibiEmlak.com` (footer küçük harfe çeviriyor); eski footer'da elle yazılı `(0212) 510 01 14` vardı, DB'de `05442603193` — doğru ofis telefonu teyit edilmeli
- [x] (2026-10-20, `feature/antigravity-seo-and-fixes`) İletişim sayfası (`_GetContactInfoViewComponentPartial`) telefon linkleri sabit ve yanlış: `tel:+23055873407` / `tel:+23055873408` (tema kalıntısı); footer'daki gibi DB numarasından üretilmeli
- [x] (2026-10-20, `feature/antigravity-seo-and-fixes`) Footer kozmetik: blog küçük görselleri eşit 90x90 değil (tema CSS'i `width/height`'ı eziyor), "Devamını Oku" linki yeşil yerine gri görünüyor
- [ ] Footer "Kullanım Koşulları" ve "Gizlilik Politikası" linkleri `#`; sayfalar yazılınca bağlanmalı (KVKK aydınlatma metni ile birlikte)
- [ ] İstenirse "Sayfalar" sütunu admin'den yönetilebilir menü olur (`FooterLink` tablosu: başlık, URL, sıra, aktif + CRUD + migration)

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
- [ ] Test projesi yok: `ListingSpecsBuilder`, `KfeBulletinParser`, `IbbSeismicCsvParser`, `MarketIndexApplier`, `TurkishTextHelper` için xUnit testleri; headless Chrome kontrol betikleri (şu an oturum scratchpad'inde: harita, detay, taşma taraması) `tools/ui-tests` altına alınıp duman testi olarak çalıştırılabilir hale getirilsin
- [ ] Yeni veriyle doğrulanmamış sayfalar: ana sayfa (vitrin, popüler yerler, son ilanlar), satılık/kiralık liste filtreleri (yeni değer biçimleri: bina yaşı "0/1-5/6-10/11-15/16-20/21 ve üzeri", ısıtma, tapu "Kat Mülkiyetli/Müstakil Tapulu", kullanım durumu), karşılaştırma (Compare) sayfası, arama önerileri
- [ ] Antigravity satır sonlarını LF yapıyor ve BOM'u siliyor (her turda elle geri ekleniyor); `tools/agy-delegate.ps1` iş bitince değişen dosyalarda BOM'u HEAD'deki duruma göre otomatik geri yüklesin

**Hata ve tutarsızlıklar**
- [ ] WebUI sayfaları API kapalıyken 500 veriyor (ör. Hakkımızda, İletişim ViewComponent'leri try/catch'siz); footer ve benzer ilanlardaki gibi boş içerikle açılmalı. API tarafı G9 ile birlikte ele alınmalı
- [x] (2026-10-20, `feature/antigravity-ui-polish`) Tüm sayfalarda `<title>` sabit ("Fibi Yapı Emlak & Danışmanlık", `_HeadLayoutViewComponentPartial`); `ViewData["Title"]` kullanılmıyor ve 8 view'da "Index" yazıyor. Sayfa başına başlık + meta description; ilan detayında ilan adı
- [x] (2026-10-20, `feature/antigravity-ui-polish`) Mobil menüde (`ScaffTemplate.cshtml`) tema kalıntıları: sahte telefon `+88 01682648101`, `info@example.com` linki, sosyal linkler `/Realshed/index.html`; footer gibi `Contact` ve `SocialMedia` tablolarından gelmeli
- [x] (2026-10-20, `feature/antigravity-ui-polish`) Header "Sayfalar" mega menüsü İngilizce tema demo linkleri (About Us, Our Services, Pricing Table, Compare Properties → `/Realshed/*.html`); gerçek sayfalara bağlanmalı veya kaldırılmalı
- [ ] Sayfa başlığı arka plan görselleri (detay, liste, blog, SSS, iletişim, karşılaştırma; 9 dosya) `images.stockcake.com` vb. harici sitelerden hotlink; `wwwroot` altına alınmalı
- [x] (2026-10-20, `feature/antigravity-ui-polish`) Detay sayfası `h1` her ilanda "Satılık İlanlar"/"Kiralık İlanlar" (sayfa başlığı bandı); breadcrumb ilan adını göstermiyor
- [ ] `Blogs` tablosunda neredeyse aynı iki yazı ("Gayrimenkul/Emlak Yatırımında Lokasyon Neden Her Şeydir?", Id 15 ve 10); biri tekrar kayıt olabilir
- [x] (2026-10-20, `feature/antigravity-ui-polish`) `MapRepository.GetListingMarkerAsync` nullable uyarıları (dönüş tipi `ListingMarkerResult` ama null dönüyor); imza `Task<ListingMarkerResult?>` yapılmalı
- [ ] Danışman e-postaları kişisel Gmail adresleri (`...fibi@gmail.com`); kurumsal alan adı adresleri tercih edilmeli
- [x] (2026-10-20, `feature/antigravity-ui-polish`) Liste ve detayda ilan "Durum" etiketi (Aktif/Opsiyonlu) ziyaretçiye gösterilmiyor; "Opsiyonlu" ilanlarda rozet gösterilmesi düşünülmeli

**Beklenen / önerilen geliştirmeler**
- [ ] Danışman kutusuna WhatsApp ile yaz ve tek tıkla ara butonları (mobilde sabit alt çubuk)
- [ ] İlan detayında kredi taksit hesaplayıcı (FİBİ Finans modülüyle ortak; `BankLoanRates` verisi)
- [ ] Galeri: küçük resim şeridi, fotoğraf sayısı rozeti, tam ekran (fancybox zaten yüklü)
- [ ] SEO: ilan detayına schema.org `RealEstateListing`/`Offer` yapılandırılmış verisi, Open Graph görseli, `sitemap.xml` ve `robots.txt`
- [ ] Görseller: WebP + duyarlı boyutlar (srcset), liste kartlarında küçük boy; şu an 1024px JPEG her yerde
- [x] (2026-10-20, `feature/antigravity-seo-and-fixes`) Spam koruması: iletişim ve randevu formlarına rate limit (FİBİ lead formundaki gibi) ve isteğe bağlı captcha
- [ ] "Benzer ilan çıkınca haber ver" talep formu (lead olarak `FibiServiceLeads` benzeri tabloya)
- [ ] Liste sayfalarında harita/liste görünüm geçişi aynı sayfada (şu an ayrı Haritada İlanlar sayfası var)
- [ ] Harita sayfasında fiyat aralığı ve oda filtresi; "bu alanda ara" (bbox) ile API tarafı filtreleme
- [ ] İzleme: yapılandırılmış log (Serilog) ve sync sağlayıcıları için hata bildirimi (Nominatim/TCMB/İBB başarısız olursa)

### SEO ve form güvenliği (2026-10-20, branch `feature/antigravity-seo-and-fixes`)
İlan detay adresi `/satilik|kiralik/{konut|arsa|isyeri}/{ilce-ilan-adi}-{id}`; tek kaynak `Models/ListingUrl.cs` (`DetailPath`, `Slugify`), harita için aynı kural `wwwroot/js/listings-map.js` içinde (değişirse ikisi birlikte güncellenmeli). Eski `/AllFor*DetailPage/Index/{id}` adresleri ve yanlış slug 301 ile asıl adrese gider; ilan yoksa 404, API hatası 500. Canonical ve JSON-LD mutlak adresleri `Seo:SiteBaseUrl` ayarından (boşsa istekten). Form koruması `Services/FormSpamGuard.cs` (IP başına sınır `FormSpam:*`, honeypot `Website` alanı, isteğe bağlı Turnstile `Captcha:*`), formlarda `<partial name="_FormSpamFields" />`.
- [x] SEO dostu ilan adresleri + 301, canonical etiketi, detayda BreadcrumbList (JSON-LD), blog detayında başlık/açıklama, görsel alt metinleri, Türkçe 404/hata sayfası (`noindex`), olmayan ilanda API 404
- [x] İletişim ve randevu formlarına IP başına sınır + honeypot + isteğe bağlı captcha; iletişim formu artık POST'ta API'ye gönderiyor; randevu dönüşü yalnızca aynı host'a
- [ ] Canlıya çıkarken: `Seo:SiteBaseUrl` canlı alan adıyla ayarlanmalı; Google Search Console kaydı ve sitemap gönderimi
- [ ] `sitemap.xml` (yukarıdaki SEO maddesi) ilan adreslerini `ListingUrl.DetailPath` ile üretmeli
- [ ] Open Graph ile birlikte Twitter Card etiketleri
- [ ] Blog adresleri de SEO dostu olsun (`/blog/{slug}-{id}`, eski `/Blog/Detail/{id}` 301); geçersiz blog id'sinde 404 dönmeli (şu an ViewComponent `View(null)` dönüyor)
- [ ] Liste sayfası adresleri okunabilir olabilir (`/AllForSalePage` → `/satilik`); il/ilçe açılış sayfaları (`/satilik/istanbul/kadikoy`) SEO için değerlendirilmeli
- [ ] Captcha'yı açmak için Cloudflare Turnstile anahtarı: `Captcha:TurnstileSiteKey`, `Captcha:ScriptUrl`, `Captcha:VerifyUrl` appsettings'e, `Captcha:TurnstileSecretKey` User Secrets'a; dördü de doluysa formlarda kendiliğinden açılır
- [ ] WebUI'de `UseForwardedHeaders` yok: canlıda reverse proxy arkasında `RemoteIpAddress` proxy IP'si olur ve form sınırı tüm ziyaretçileri tek kişi sayar; WebUI'ye de ForwardedHeaders + `KnownProxies` eklenmeli
- [ ] `FormSpamGuard` sayaçları bellek içi; WebUI birden fazla örnekle çalışırsa paylaşılmaz (gerekirse dağıtık önbellek)
- [ ] API CORS (`AllowLocalhost`) yalnızca sabit localhost portlarına izin veriyor; harita sayfası API'yi tarayıcıdan çağırdığı için canlı alan adı ayardan okunmalı
- [ ] API'deki diğer GetById handler'ları da kayıt yoksa null kontrolsüz olabilir (ör. blog detayı); G9 kapsamında taranmalı
- [ ] Veri: `Contacts.PhoneNumber2` = `PhoneNumber1` (iletişim sayfasında aynı numara iki kez), `EMail2` kişisel Gmail (`fibiinfo@gmail.com`)
- [ ] Kozmetik: uzun ilan adlarında detay sayfası üst başlığı iki satıra iniyor; 404 sayfasında mobilde "404" yazısı sabit header'a çok yakın
- [ ] Veri: `Contacts.EMail1` iletişim sayfasında olduğu gibi `info@fibiEmlak.com` (büyük harfli) görünüyor; link küçük harfe çevriliyor ama kayıt düzeltilmeli
- [ ] Ana sayfadaki bülten aboneliği formu tema kalıntısı (`UILayout/Index.cshtml`, `action="contact.html"`): çalışmıyor; ya gerçek bir abonelik ucuna (KVKK onayı + spam koruması ile) bağlanmalı ya kaldırılmalı
- [ ] Blog detayında yazar adı linki tema kalıntısı (`href="blog-details.html"`); yazarın yazılarına filtreli blog listesine gitmeli ya da link kaldırılmalı
- [ ] İletişim sayfasındaki "harita" aslında harici bir stok fotoğraf (pexels hotlink); ofis konumu gerçek harita (Leaflet, ilan detayındaki gibi) ile gösterilmeli
- [ ] Bu branch dev'e birleştirilmedi; aşağıdaki testlerden sonra kullanıcı onayıyla birleştirilip push edilecek

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
- [ ] Admin paneli: ilan formunda haritadan kesin pin seçme (Leaflet tıkla/sürükle); kaydedince `ListingGeoPoints` kaydı `Source = manual`, `Precision = building` olarak yaklaşık kaydın yerine yazılsın (şu an `AddGeoPointsAsync` mevcut kaydı ezmiyor, güncelleme ucu ayrıca yazılmalı)
- [x] (2026-10-19, `tools/sql/demo-listings-refresh.ps1`) Veri: demo ilanların çoğunda mahalle alanı il/ilçeyle uyumsuz (ör. Kadıköy ve Çankaya ilanlarında "Veliefendi"); bu yüzden 68 ilanın 47'si yalnızca ilçe düzeyinde konumlandı. Mahalle düzeltilince ilgili `nominatim` kayıtları silinirse sağlayıcı yeniden konumlar
- [x] (2026-10-19, `feature/antigravity-todo-batch1`) İlan güncellenip ili/ilçesi/mahallesi değişirse `nominatim` kaynaklı eski koordinat kalıyor; adres değişikliğinde yaklaşık kayıt silinmeli (manuel pin hariç)
- [ ] Harita sayfası tüm ilanları tek istekte alıp istemci tarafında filtreliyor (şimdilik 70 ilan); ilan sayısı birkaç bini geçerse `Map/listings` ucunun mevcut `usageType/category/cityId` filtreleri ve harita sınırlarına göre (bbox) sorgu kullanılmalı
- [ ] Nominatim kullanım politikası: User-Agent'a iletişim bilgisi (site adresi veya e-posta) eklenmeli; canlıda yoğun kullanımda kendi geocoder'ımız veya ücretli servis değerlendirilmeli
- [ ] Leaflet ve MarkerCluster unpkg CDN'den yükleniyor (detay sayfası da öyle); CDN erişilemezse harita çalışmaz, `wwwroot/lib` altına alınabilir
- [ ] Site geneli: yalnızca `/Realshed/...` görsel yolları kaldı ama birçok bileşende hâlâ `/Realshed/property-details.html`, `/Realshed/property-list.html`, `/Realshed/blog-details.html` gibi tema demo sayfası linkleri var (ör. `_FeaturedRentalPropertViewComponentPartial`); gerçek route'lara çevrilmeli

---

### FİBİ Hizmetler Faz 2 (branch `feature/antigravity-fibi-services-phase2`, 2026-10-18'de yarıda kaldı)
Bitti: kademeli emsal şeması + migration (yerel DB'ye uygulandı), mahalle > ilçe > il > Türkiye fallback'i, İBB senaryosundan deprem tehlikesi, senkronizasyon çekirdeği, Infrastructure projesi (seed / İBB / TCMB KFE bülteni / opsiyonel EVDS sağlayıcıları, arka plan servisi, `[Authorize]` sync uçları), KvkkApprovedAt, ForwardedHeaders.
- [x] (2026-10-19) Canlı veri senkronizasyonunu uçtan uca test et (WebApi'yi scratchpad'e derleyip ayrı portta çalıştır; seed + İBB 959 mahalle + TCMB bülteni DB'ye yazılmalı, `sync/logs` kontrol edilmeli; Risk/Bölge uçları verisi olmayan bir ilçede 404 yerine il/Türkiye verisiyle dönmeli)
- [ ] Banka faizi sağlayıcıları: her banka için kendi açık sayfasından oran okuyan `IMarketDataProvider` (oranlar HTML'de değil hesaplama araçlarında; banka banka incelenmeli). Kırılan banka son oranında kalır
- [x] (2026-10-19) WebUI: konum seçimini Türkiye geneline aç (mevcut `api/Location/cities|districts|neighborhoods` cascade); `api/FibiServices/locations` ucu ve `GetFibiServiceLocations*` kaldırılsın; sonuçlarda veri kapsamı/kaynak etiketi ve İBB deprem özeti gösterilsin
- [ ] Fibi Finans modülü (kredi + kira başabaş, banka karşılaştırma, tapu günü gizli maliyetler)
- [ ] Fibi Eksper modülü (şerefiye katsayılı değerleme bandı + satıcı lead)
- [ ] Fibi Yatırım modülü (bütçe + strateji ile en iyi 5 bölge)
- [ ] EVDS anahtarı alınırsa `dotnet user-secrets set "FibiMarketDataSync:EvdsApiKey" "<anahtar>"`; sağlayıcının yanıt biçimi o zaman doğrulanmalı
- [ ] KVKK aydınlatma metni sayfası (onay artık `KvkkApprovedAt` ile kaydediliyor)
- [ ] Canlıda reverse proxy IP'si `ForwardedHeaders:KnownProxies` ayarına yazılmalı

---

## Bilinen Teknik Borçlar

- `PropertyRepository.cs` 3002 satır, DRY ihlali (T1). Dosya UTF-8 değil, ISO-8859/Windows-1254 kodlamalı: Antigravity bu dosyada takılabiliyor, düzenlerken kodlama korunmalı (ileride UTF-8'e çevrilmeli)
- 30 ayrı `PropImgUrl1..30` kolonu — ayrı tablo olmalı (T5)
- Katman ihlali: WebUI → Application direkt referans (T6)
- `Persistance` yazım hatası klasör adında (T7)
- 12 deneme-yanılma migration — production öncesi squash (T9)
- BaseEntity yok — her entity kendi Id'sini taşıyor (T10)
- Soft delete yok — fiziksel silme yapılıyor (T10)
- ~570 derleme uyarısı, çoğu nullable (CS8618/CS8602/CS8603); yeni kodda yenisi eklenmemeli (T11)
- ~~Detay sayfası ilanı yalnızca id ile buluyor (T12)~~ — çözüldü: linkler listingTypeId taşıyor; parametresiz eski linkler için sıralı arama yedek olarak duruyor
- İlan görselleri harici sitelere hotlink (emlakjet, depositphotos, unsplash); kaynak silinince kırılıyor (T13)
- Kiralık entity'lerde `ListingTypeId` int?, satılıklarda int — tutarsız (T14)
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
dotnet ef migrations add <MigrationName> --project Infrastructure/FibiEmlakDanismanlik.Persistance --startup-project Presentation/FibiEmlakDanismanlik.WebApi

# DB güncelle
dotnet ef database update --project Infrastructure/FibiEmlakDanismanlik.Persistance --startup-project Presentation/FibiEmlakDanismanlik.WebApi
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
dotnet ef database update --project Infrastructure/FibiEmlakDanismanlik.Persistance --startup-project Presentation/FibiEmlakDanismanlik.WebApi
Remove-Item Env:ConnectionStrings__DefaultConnection
```
