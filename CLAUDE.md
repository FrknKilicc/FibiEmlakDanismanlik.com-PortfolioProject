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
- [ ] Detay linklerine ilan türü ekle (id + listingTypeId): şu an detay sayfası yalnızca id ile konut/arsa/ticari tablolarına sırayla bakıyor; çakışma id aralıklarıyla (konut 1M+, arsa 2M+, ticari 3M+) geçici olarak önlendi
- [ ] Veri: kırık ilan görselleri (imaj.emlakjet.com URL'leri 404) gerçek görsellerle değiştirilmeli; harici sitelere hotlink yerine görseller kendi depolamamıza alınmalı
- [ ] Global exception middleware + handler'larda null kontrolü (G9)
- [ ] PropertyLocationController → Query/Handler/Result (G5)
- [ ] FluentValidation + MediatR ValidationBehavior pipeline (G6)
- [ ] ListingImageSelection.ListingTypeId alanı + migration (H1)
- [ ] Admin: 2 hatalı ilanın ilçesini düzelt (Kayseri-Ayazağa, Giresun-Ceyhan)
- [ ] İlan giriş formunda il/ilçe cascade dropdown
- [x] LandCategories tablosunu doldur (2026-10-12, migration ListingIdRangesAndLandCategories)
- [ ] Teknik borç: kiralık listeleme bellekten DB sorgusuna taşı (T2)
- [ ] Teknik borç: facet sayaçları null statüyü sayıyor (T3)
- [ ] localhost:7015 hardcode'ları appsettings'e taşı (G11)

---

## Bilinen Teknik Borçlar

- `PropertyRepository.cs` 3002 satır, DRY ihlali (T1)
- 30 ayrı `PropImgUrl1..30` kolonu — ayrı tablo olmalı (T5)
- Katman ihlali: WebUI → Application direkt referans (T6)
- `Persistance` yazım hatası klasör adında (T7)
- 12 deneme-yanılma migration — production öncesi squash (T9)
- BaseEntity yok — her entity kendi Id'sini taşıyor (T10)
- Soft delete yok — fiziksel silme yapılıyor (T10)

---

## Çalışma Komutları

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
