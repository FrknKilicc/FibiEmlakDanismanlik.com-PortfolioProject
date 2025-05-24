# 🏠 Fibi Emlak Danışmanlık

**Fibi Emlak Danışmanlık**, ASP.NET Core, Entity Framework ve CQRS mimarisi ile geliştirilen çok katmanlı bir emlak ilan sistemidir.

---

## 📁 Proje Yapısı

Bu proje, SOLID prensiplerine uygun şekilde katmanlı mimari ile organize edilmiştir:

- **Domain**: Temel varlık sınıfları (Entityler)
- **Application**: CQRS tabanlı `Query`, `Command`, `Handler`, `ViewModel`, `Result` sınıfları
- **Persistence**: Entity Framework Core ile DB işlemleri ve Repository’ler
- **WebApi**: Swagger HTTP API endpoint’leri,
- **WebUI**: HTML, CSS, JS (jQuery) ile fonksiyonel arayüz tasarımı
- **DTO**: Backend ve UI arasında veri transferi

---

## ⚙️ Kullanılan Teknolojiler

- ASP.NET Core 7.0
- Entity Framework Core
- CQRS + MediatR
- AutoMapper
- SQL Server
- Swagger
- jQuery / AJAX

---

## 🚀 Kurulum

```bash
git clone https://github.com/furkankilic/FibiEmlakDanismanlik.git
```
