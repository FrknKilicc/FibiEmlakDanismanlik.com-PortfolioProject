# Fibi Emlak — Canlıya geçiş ve yayın rehberi

Bu klasör, siteyi kendi VDS sunucumuza taşımak ve sonraki güncellemeleri otomatik yayınlamak için gereken her şeyi içerir.
Komutların hepsi tek satırdır, kopyala-yapıştır yapılabilir. `<IP>` gördüğün yere sunucunun IP adresini, `<E-POSTA>` yerine sertifika bildirimlerinin gideceği e-posta adresini yaz.

## Mimari (tek sunucu, ücretsiz bileşenler)

| Parça | Adres | Sunucuda |
|---|---|---|
| Site (WebUI) | https://fibiemlak.com (www ve fibiemlak.online buraya yönlenir) | `fibi-site` servisi, 127.0.0.1:5001 |
| Yönetim paneli (AdminUI) | https://yonetim.fibiemlak.com | `fibi-panel` servisi, 127.0.0.1:5002 |
| WebApi | fibiemlak.com/api/ altında sınırlı açık: yalnızca okuma + 3 ziyaretçi formu; giriş ve yönetim uçları dışarıdan kapalı. Görseller fibiemlak.com/uploads | `fibi-api` servisi, 127.0.0.1:5000 |
| Veritabanı | dışarıya kapalı | SQL Server 2025 Express, yalnızca 127.0.0.1 |
| HTTPS | Let's Encrypt (ücretsiz, otomatik yenilenir) | nginx + certbot |

- **Güncelleme akışı:** `feature/*` → PR → `dev` → PR → `master`. `master`'a merge edilen her şey otomatik yayınlanır (GitHub Actions, yaklaşık 5 dakika).
- **Her yayında:** önce veritabanı yedeği alınır, sonra migration'lar uygulanır, sonra uygulamalar tek tek açılır. Bir uygulama sağlık kontrolünden (`/health`) geçemezse **otomatik olarak önceki sürüme döner** ve yayın durur.
- **Panel değişiklikleri** (ilan, metin, görsel) veritabanına yazıldığı an sitede görünür; site metin önbelleği en fazla 30 saniyedir. Bunun için yayın gerekmez.
- **Gizli bilgiler** (DB şifresi, JWT anahtarı, captcha anahtarı) yalnızca sunucuda `/etc/fibi/*.env` dosyalarında durur, repoya girmez.

| Dosya | Görevi |
|---|---|
| `server/setup.sh` | Sunucuyu sıfırdan kurar (bir kez) |
| `server/scripts/fibi-db-init` | Yerel veritabanı yedeğini sunucuya ilk kez yükler |
| `server/scripts/fibi-deploy` | Yayın + otomatik geri dönüş (GitHub Actions çağırır) |
| `server/scripts/fibi-migrate` | Yedek + migration |
| `server/scripts/fibi-backup` | Gece yedeği (03:30) |
| `server/scripts/fibi-harden-ssh` | SSH'ta şifreli girişi kapatır |
| `../.github/workflows/deploy.yml` | master → canlı yayın |
| `../.github/workflows/ci.yml` | Her PR'da derleme + test |

---

## 0. Ön hazırlık (sunucudan önce)

1. **Kod hazır olsun:** Antigravity/Claude branch'leri `dev`'e merge edilmiş, yerel `dev` push edilmiş olsun (`git push origin dev`). Sonra GitHub'da `dev` → `master` PR'ı açılıp merge edilir. Canlıya giden şey `master`'dır.
2. **VDS:** Ubuntu 24.04, en az 4 GB RAM, 2 çekirdek, 60 GB SSD, Türkiye'de (KVKK). Sağlayıcı "snapshot/yedek" sunuyorsa aç.
3. **Cloudflare Turnstile (ücretsiz, zorunlu):** https://dash.cloudflare.com → hesap aç → Turnstile → Add widget. Hostname olarak `fibiemlak.com`, `yonetim.fibiemlak.com`, `yeni.fibiemlak.com` ekle, mod "Managed". Sana bir **Site Key** ve bir **Secret Key** verir. *Bunlar olmadan panel girişi çalışmaz* (güvenlik için bilerek kapalı başarısız).
4. **Natro:** alan adında "Otomatik Yenileme"yi aç. "Profesyonel DNS Servisi → Yönet" ekranındaki tüm kayıtların ekran görüntüsünü al (geri dönüş için gerekir; özellikle MX/e-posta kayıtları).

## 1. Sunucuyu kur (JetSite'ı etkilemez)

Windows'ta **PowerShell** aç. Sağlayıcının verdiği şifreyle bağlan:

```
ssh root@<IP>
```

Sunucuda (her satırı ayrı çalıştır):

```
apt-get update && apt-get install -y git
```
```
git clone --depth 1 https://github.com/FrknKilicc/FibiEmlakDanismanlik.com-PortfolioProject.git /opt/fibi-src
```
```
bash /opt/fibi-src/deploy/server/setup.sh
```

Kurulum 5-10 dakika sürer. Sonunda ekrana bir **özel anahtar** ve bir **parmak izi satırı** yazar; 3. adımda lazım, şimdi kopyalayıp bir yere not et. Sonra anahtar dosyasını sil:

```
shred -u /root/fibi-ci-key /root/fibi-ci-key.pub
```

Turnstile anahtarlarını gir (açılan dosyada `<TURNSTILE_...>` yazan yerleri değiştir, `Ctrl+O` `Enter` `Ctrl+X` ile kaydet):

```
nano /etc/fibi/api.env
```
```
nano /etc/fibi/site.env
```
```
nano /etc/fibi/panel.env
```

### SSH'ı anahtara geçir (önerilir)

Kendi bilgisayarında **yeni** bir PowerShell penceresinde:

```
ssh-keygen -t ed25519
```
```
type $env:USERPROFILE\.ssh\id_ed25519.pub | ssh root@<IP> "mkdir -p ~/.ssh && cat >> ~/.ssh/authorized_keys"
```

Artık şifresiz girebildiğini dene (`ssh root@<IP>`). Girebiliyorsan sunucuda:

```
fibi-harden-ssh root
```

Bu pencereyi kapatmadan yeni bir pencereden tekrar girmeyi dene. Giremezsen eski pencerede ekrandaki geri alma komutunu çalıştır.

## 2. Veriyi taşı

> **Önemli:** Bu adım o anki verinin bir kopyasını alır. Bundan sonra **yerel panelden veri girme**; girersen geçiş günü bu adımı `--replace` ile tekrarla.

Kendi bilgisayarında PowerShell (yönetici olarak), proje klasöründe (`cd` ile repo köküne geç):

```
sqlcmd -S ".\FURKANKILIC" -E -Q "BACKUP DATABASE [fibiemlak.com] TO DISK = N'C:\Users\Public\fibiemlak.bak' WITH COPY_ONLY, INIT, CHECKSUM"
```
```
scp C:\Users\Public\fibiemlak.bak root@<IP>:/root/fibiemlak.bak
```
```
scp -r .\Presentation\FibiEmlakDanismanlik.WebApi\uploads\* root@<IP>:/var/lib/fibi/uploads/
```

Sunucuda:

```
chown -R fibi:fibi /var/lib/fibi/uploads
```
```
fibi-db-init /root/fibiemlak.bak
```

Bu betik:
- Veritabanı zaten varsa **durur** (canlı veriyi ezmez).
- Yedeği geri yükler, `fibiemlak_app` (okuma/yazma) ve `fibiemlak_migrator` (migration/yedek) hesaplarını oluşturur.
- **2FA kayıtlarını sıfırlar:** 2FA anahtarları yerel Windows'un şifreleme anahtarlarıyla şifreli, sunucuda çözülemez. Sahip ve Yönetici ilk girişte QR kodu yeniden okutur. Şifreler değişmez.
- Görsel adreslerindeki `https://localhost:7015` gibi yerel adresleri önce **önizler**, sen `e` dersen `https://fibiemlak.com` yapar.
- Son satırda yerelden kalan kullanılmayan DB kullanıcılarını listeler (ör. `denemeKullanici`); bunlar silinmez, sadece bilgi.

## 3. Otomatik yayını bağla

GitHub → repo → **Settings → Secrets and variables → Actions**:

- **Secrets** sekmesi → New repository secret:
  - `DEPLOY_HOST` = sunucu IP'si
  - `DEPLOY_SSH_KEY` = 1. adımda ekrana yazılan özel anahtar (BEGIN ve END satırları dahil, tamamı)
  - `DEPLOY_KNOWN_HOSTS` = 1. adımda yazılan parmak izi satırı
- **Variables** sekmesi → New repository variable: `DEPLOY_ENABLED` = `true`

Sonra **Actions → Deploy → Run workflow → master**. Yeşil tik gelirse üç uygulama sunucuda çalışıyor demektir. Kırmızı olursa adımın loguna bak; sunucu otomatik olarak önceki sürümde kalır.

## 4. Gerçek HTTPS ile dene (JetSite hâlâ yayında)

Natro → Alan Adı Yönetimi → fibiemlak.com → Profesyonel DNS Servisi → Yönet → **yeni kayıt ekle** (mevcutlara dokunma):

- `A` kaydı, ad `yonetim`, değer `<IP>`
- `A` kaydı, ad `yeni`, değer `<IP>`

10-30 dakika bekle, sonra sunucuda:

```
certbot --nginx -d yeni.fibiemlak.com -d yonetim.fibiemlak.com --redirect --agree-tos -m <E-POSTA> --no-eff-email
```

Deneme listesi:
- [ ] https://yeni.fibiemlak.com açılıyor; ana sayfa, satılık/kiralık listeleri, ilan detayı, fotoğraflar, harita
- [ ] Tarayıcıdan API çağıran yerler: ana sayfa arama önerileri, liste filtreleri (il/ilçe/özellik), "Haritada İlanlar", FİBİ Hizmetler formları, "Haber ver" formu
- [ ] İletişim formu gönderiliyor (captcha görünüyor)
- [ ] https://yonetim.fibiemlak.com girişi → 2FA kurulumu (QR) → panel açılıyor
- [ ] Panelde ilan düzenle + fotoğraf yükle → 30 saniye içinde yeni.fibiemlak.com'da görünüyor
- [ ] Loglarda hata yok: `journalctl -u fibi-api -u fibi-site -u fibi-panel -n 100 --no-pager`

## 5. Geçiş günü (geri alınabilir)

1. Natro DNS ekranının son halinin ekran görüntüsünü al (geri dönüş için).
2. Natro DNS'te **yalnızca** şu kayıtları değiştir: `@` (fibiemlak.com) ve `www` → `A` `<IP>`. **MX, TXT ve e-postayla ilgili kayıtlara dokunma.** `www` CNAME ise sil ve `A` `<IP>` ekle.
3. fibiemlak.online da Natro'daysa: onun `@` ve `www` kayıtları → `A` `<IP>`.
4. 10-30 dakika sonra sunucuda (fibiemlak.online kullanmıyorsan son iki `-d`'yi çıkar):

```
certbot --nginx --expand -d fibiemlak.com -d www.fibiemlak.com -d yeni.fibiemlak.com -d yonetim.fibiemlak.com -d fibiemlak.online -d www.fibiemlak.online --redirect --agree-tos -m <E-POSTA> --no-eff-email
```

5. 4. adımdaki deneme listesini https://fibiemlak.com üzerinde tekrarla.

**Geri dönüş:** sorun olursa Natro'da `@` ve `www` kayıtlarını 1. adımdaki eski değerlere geri al; site JetSite'a döner (paket 24.02.2027'ye kadar ödenmiş, iptal etme).

**JetSite'ı kapatmadan önce:** Natro'daki "Profesyonel DNS Servisi" ekranında "Web siteniz olduğu için aktif" yazıyor. JetSite paketi bitince DNS servisinin devam edeceğini Natro'ya sor; yoksa alan adı DNS'siz kalır.

---

## Günlük işleyiş

| İş | Nasıl |
|---|---|
| Yeni sürüm yayınla | `dev` → `master` PR'ını merge et. Gerisi otomatik. |
| Yayın durumunu gör | GitHub → Actions → Deploy |
| Eski sürüme dön | Actions → eski başarılı "Deploy" çalıştırması → Re-run all jobs |
| Canlı loglar | `journalctl -u fibi-api -f` (site: `fibi-site`, panel: `fibi-panel`); dosyalar `/var/log/fibi/` |
| Servisi yeniden başlat | `systemctl restart fibi-site` |
| Elle yedek | `fibi-backup` |
| Yedekleri gör | `ls -lh /var/backups/fibi/db /var/backups/fibi/files` |
| Sertifika | Otomatik yenilenir; kontrol: `certbot renew --dry-run` |

### Yedekten geri dönüş

Veritabanını bir yedekten geri yüklemek (önce o anki hali yedeklenir):

```
fibi-backup && systemctl stop fibi-api fibi-site fibi-panel && cp /var/backups/fibi/db/<DOSYA>.bak /root/geri.bak && fibi-db-init /root/geri.bak --replace && systemctl start fibi-api fibi-site fibi-panel
```

> Not: `fibi-db-init` 2FA kayıtlarını da sıfırlar; aynı sunucunun yedeğinden dönerken bu gereksizdir ama zararsızdır (yöneticiler QR'ı yeniden okutur).

### Sunucu dışı yedek (şiddetle önerilir, ücretsiz)

Yedekler varsayılan olarak yalnızca sunucuda durur; sunucu tamamen kaybolursa onlar da gider. Google Drive'a şifreli kopya için sunucuda `rclone config` çalıştır: önce Google Drive için bir hedef oluştur, sonra onun üzerine **crypt** tipinde, adı tam olarak `fibi-offsite` olan bir hedef oluştur. Gece yedeği bunu görünce otomatik kopyalar. (Yedek arşivinde `/etc/fibi` şifreleri de olduğu için crypt şart.)

## Bilinen konular ve riskler

- **.NET 8 desteği 10 Kasım 2026'da bitiyor.** Site çalışmaya devam eder ama güvenlik yaması gelmez. Canlıya geçişten kısa süre sonra .NET 10'a (LTS) yükseltme ayrı bir iş olarak planlanmalı.
- **Linux'ta Data Protection anahtarları** dosyada şifresiz durur (Windows'taki DPAPI yok). Klasör yalnızca `fibi` kullanıcısına açık (700) ve gece yedeğine dahil.
- **Migration geri alınmaz:** bir yayın sağlık kontrolünde düşerse uygulama eski sürüme döner ama veritabanı yeni şemada kalır. EF migration'ları genelde ekleme yaptığı için eski kod çalışır; kolon silen/yeniden adlandıran migration'lar ayrıca planlanmalı. Her yayından önce otomatik yedek alınır (`/var/backups/fibi/db/pre-migrate-*.bak`).
- **Persistence testleri** LocalDB istediği için CI'da değil, yerelde çalışır.
- **Repo public:** Actions logları herkese açıktır; betikler gizli bilgi yazdırmaz, secret'lar GitHub tarafından maskelenir.
