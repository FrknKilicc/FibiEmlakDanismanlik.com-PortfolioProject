# FibiEmlakDanismanlik - AI DEVELOPMENT RULES & PROJECT CONSTITUTION

## PROJECT IDENTITY

Bu proje:

FibiEmlakDanismanlik

adında gerçek bir emlak danışmanlık platformudur.

Proje aşağıdaki teknolojiler ile geliştirilmiştir:

- ASP.NET Core .NET 8
- Clean Architecture
- CQRS Pattern
- MediatR
- Entity Framework Core
- SQL Server
- Repository Pattern
- MVC Razor Architecture
- jQuery
- AJAX


Bu proje yeni oluşturulacak bir proje değildir.

Bu proje mevcut çalışan bir yazılım sistemidir.

Senin görevin:

MEVCUT MİMARİYİ ANALİZ ETMEK,
KORUMAK,
VE AYNI STANDARTLARDA YENİ GELİŞTİRMELER YAPMAKTIR.


---

# CRITICAL RULE

## DO NOT RESTRUCTURE THIS PROJECT

Aşağıdaki işlemler kesinlikle yasaktır:


- Mevcut klasör yapısını değiştirme.
- Yeni mimari oluşturma.
- Clean Architecture yapısını değiştirme.
- CQRS patternini kaldırma.
- Repository Pattern'i kaldırma.
- Namespace yapılarını değiştirme.
- Var olan çalışan kodları gereksiz yere refactor etme.
- Framework değiştirme.
- MVC yapısını değiştirme.
- React, Vue veya başka frontend framework ekleme.
- Database yapısını değiştirme.


Bu proje "yeniden yazılacak" bir proje değildir.

Bu proje "mevcut mimariye uygun geliştirilecek" bir projedir.


---

# EXISTING SOLUTION STRUCTURE


Mevcut solution yapısı:


FibiEmlakDanismanlik

│
├── Core
│   │
│   ├── FibiEmlakDanismanlik.Domain
│   │
│   └── FibiEmlakDanismanlik.Application
│
│
├── Infrastructure
│   │
│   └── FibiEmlakDanismanlik.Persistence
│
│
├── Presentation
│   │
│   └── FibiEmlakDanismanlik.WebApi
│
│
└── Frontends
    │
    ├── FibiEmlakDanismanlik.Dto
    │
    └── FibiEmlakDanismanlik.WebUI



Bu yapı korunacaktır.


---

# ARCHITECTURE DEPENDENCY RULE


Bağımlılık yönü:


WebUI

↓

WebApi

↓

Application

↓

Domain



Persistence

↓

Application

↓

Domain



Kurallar:


Domain hiçbir katmana bağımlı olamaz.


Application sadece Domain katmanını bilir.


Persistence:

- Domain
- Application

referanslarını kullanabilir.


WebApi:

- Application
- Persistence

kullanabilir.


---

# DOMAIN LAYER RULES


Konum:


Core/FibiEmlakDanismanlik.Domain


Domain katmanı sadece:


- Entities
- DTOs
- Enums
- Domain modelleri


içindir.


Örnek yapı:


Domain

│

├── Entities

├── DTOs

└── Enums



Domain içinde kesinlikle bulunamaz:


- DbContext
- Repository
- Service
- Controller
- MediatR
- AutoMapper Profile
- EF Core Logic
- API Model


Domain bağımsız ve temiz kalmalıdır.


---

# APPLICATION LAYER RULES


Konum:


Core/FibiEmlakDanismanlik.Application



Application bütün iş akışlarının merkezidir.


Mevcut yapı:


Application

│

├── Constants

├── Features

├── Interfaces

├── Mapping

├── Services

├── Helpers

└── ViewModels



Yeni özellikler bu yapıya uygun eklenmelidir.


---

# CQRS IMPLEMENTATION STANDARD


Bu projede CQRS zorunludur.


Yeni feature oluştururken aşağıdaki yapı kullanılmalıdır.


Örnek:

Property Feature


Features


├── Commands

│
│   └── PropertyCommands

│
│       ├── CreatePropertyCommand.cs

│       ├── UpdatePropertyCommand.cs

│       └── DeletePropertyCommand.cs



├── Queries

│
│   └── PropertyQueries

│
│       ├── GetPropertyQuery.cs

│       └── GetPropertyByIdQuery.cs



├── Handlers

│
│   └── PropertyHandlers

│
│       ├── CreatePropertyCommandHandler.cs

│       ├── UpdatePropertyCommandHandler.cs

│       └── GetPropertyQueryHandler.cs



└── Results

    │
    └── PropertyResults

        ├── GetPropertyResult.cs

        └── PropertyListResult.cs



Bu yapı tüm yeni modüllerde korunmalıdır.


---

# COMMAND RULES


Command sınıfları veri değiştiren işlemler içindir.


Örnek:


- Create
- Update
- Delete


Command içerisinde:


✔ Request bilgileri

✔ Validation

✔ Kullanıcıdan gelen veriler


bulunabilir.


Command içinde:


YASAK:


- DbContext kullanmak
- Direkt SQL çalıştırmak
- Database erişimi yapmak



Database işlemleri Repository üzerinden yapılmalıdır.


---

# QUERY RULES


Query sadece veri okumak içindir.


Örnek:


- GetAllProperty
- GetPropertyById
- GetFeaturedProperty


Query:


- Veri değiştiremez.
- Insert yapamaz.
- Update yapamaz.
- Delete yapamaz.



---

# HANDLER RULES


Handler iş mantığının merkezidir.


Handler içinde:


✔ Business logic

✔ Validation

✔ Mapping

✔ Repository kullanımı


olabilir.


Handler içinde:


YASAK:


- DbContext kullanımı
- SQL sorgusu
- Controller işlemleri


Database erişimi sadece Repository üzerinden yapılmalıdır.


---

# RESULT RULES


Entity direkt dışarı dönülmemelidir.


Yanlış:


return property;



Doğru:


Entity -> Result DTO -> Response



Örnek:


Property

↓

GetPropertyResult

↓

API Response



---

# INFRASTRUCTURE LAYER RULES


Konum:


Infrastructure/FibiEmlakDanismanlik.Persistence



Görev:


- EF Core
- DbContext
- Migration
- Repository Implementation



Yapı:


Persistence


├── Context

├── Repositories

└── Migrations



Infrastructure içinde:


Business logic yazılmamalıdır.


---

# REPOSITORY RULES


Repository Pattern korunacaktır.


Akış:


Handler

↓

Repository Interface

↓

Repository Implementation

↓

EF Core

↓

Database



Application içinde:


Interfaces


örneği:


IPropertyRepository


Persistence içinde:


Implementation:


PropertyRepository



---

# WEB API RULES


Konum:


Presentation/FibiEmlakDanismanlik.WebApi



Controller görevleri:


SADECE:


1. Request almak

2. Model binding

3. MediatR.Send() çağırmak

4. Response döndürmek



Controller içinde:


YASAK:


- Business logic
- Repository kullanımı
- DbContext
- SQL
- Mapping



Controller ince ve sade kalmalıdır.


---

# EXISTING DOMAIN MODULES


Bu projede mevcut ana modüller:


AboutUs

Agent

Author

Blog

Contact

CustomerContact

ForSaleHousing

ForSaleCommercialProperty

ForSaleLand

RentalHousing

RentalCommercialProperty

Service

FAQ

MainBanner

Location

Map

Nearby

OurPartners

SocialMedia

Testimonials



Yeni geliştirme yapılırken mevcut modül yapısı analiz edilmeli ve aynı standart uygulanmalıdır.


---

# REAL ESTATE DOMAIN RULES


Bu proje basit bir web sitesi değildir.


Ana domain:

EMLAK YÖNETİM PLATFORMU


Özellikle:


ForSaleHousing

ForSaleCommercialProperty

ForSaleLand

RentalHousing

RentalCommercialProperty

Location

Map

Nearby



modülleri kritik öneme sahiptir.


Yeni emlak özelliklerinde:


- Property ilişkileri
- Location ilişkileri
- Map bilgileri
- Nearby bilgileri
- Category ayrımları


korunmalıdır.


---

# WEB UI RULES


Konum:


Frontends/FibiEmlakDanismanlik.WebUI



Frontend teknolojisi:


- ASP.NET MVC Razor
- HTML
- CSS
- Javascript
- jQuery
- AJAX



Mevcut yapı korunacaktır.


---

# REALSHED THEME RULES


Projede kullanılan:


Realshed Theme


korunmalıdır.


Aşağıdakiler yapılmayacaktır:


- CSS yapısını değiştirme
- Tema dosyalarını silme
- HTML componentlerini bozma
- Asset yollarını değiştirme
- Yeni frontend framework ekleme


Yeni sayfa oluşturulurken mevcut Realshed componentleri kullanılmalıdır.


---

# NAMING CONVENTION


Entity:


Property

Blog

Agent

Location



Command:


CreatePropertyCommand

UpdatePropertyCommand

DeletePropertyCommand



Query:


GetPropertyQuery

GetPropertyByIdQuery



Handler:


CreatePropertyCommandHandler

GetPropertyQueryHandler



Result:


PropertyResult

GetPropertyResult

PropertyListResult



Dosya isimlendirme mevcut proje standardına uygun olmalıdır.


---

# DEVELOPMENT PROCESS


Her yeni geliştirmeden önce:


1.

İlgili mevcut feature incelenmelidir.


Örneğin:


Yeni Property özelliği yapılacaksa:


Önce:


ForSaleHousing


incelenmelidir.



2.

Benzer çalışan yapı örnek alınmalıdır.



3.

Yeni kod aynı pattern ile yazılmalıdır.



4.

Çalışan kodlara dokunulmamalıdır.



---

# CODE QUALITY RULES


Kod yazarken:


- Clean Code prensiplerine uy.
- Mevcut isimlendirme standardını koru.
- Gereksiz abstraction oluşturma.
- Gereksiz class oluşturma.
- Var olan yapıyı tekrar etme.
- DRY prensibini uygula.


---

# ABSOLUTE PROHIBITIONS


Kesinlikle yapılmayacak:


❌ Yeni mimari oluşturmak

❌ CQRS kaldırmak

❌ Repository kaldırmak

❌ Controller içine logic yazmak

❌ Domain içine servis koymak

❌ DbContext'i Application içinde kullanmak

❌ UI framework değiştirmek

❌ Realshed temasını değiştirmek

❌ Gereksiz refactor yapmak

❌ Namespace değiştirmek

❌ Mevcut çalışan kodu bozmak



---

# FINAL OBJECTIVE


Amaç:


FibiEmlakDanismanlik projesini yeniden tasarlamak değildir.


Amaç:


Mevcut mimariyi tamamen koruyarak yeni özellikler geliştirmektir.


Her yeni geliştirme:


- Mevcut klasör yapısına
- Clean Architecture kurallarına
- CQRS patternine
- Repository Pattern'e
- Naming Convention'a
- Realshed UI yapısına


tam uyumlu olmalıdır.



Kod yazmadan önce mevcut yapıyı analiz et.

Mevcut yapıya uy.

Yeni mimari oluşturma.

Bu projeyi olduğu gibi geliştir.