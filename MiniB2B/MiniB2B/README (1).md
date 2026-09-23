# MiniB2B – Bayi Sipariş Portalı

MiniB2B, bayilerin ürün kataloğunu görüntüleyip sepet oluşturabildiği, sipariş verebildiği,
siparişlerinin durumunu takip edebildiği ve profillerinde adres/kart bilgilerini
kaydedip görüntüleyebildiği; yöneticilerin ise ürün/kategori yönetimi ve sipariş
süreçlerini (onay, kargo takibi) yönetebildiği bir ASP.NET Core MVC (.NET 8) web
uygulamasıdır.

---

## İçindekiler

1. [Kullanılan Teknolojiler](#kullanılan-teknolojiler)
2. [Mimari ve Teknik Tercihler](#mimari-ve-teknik-tercihler)
3. [Kurulum](#kurulum)
4. [Veritabanı Bağlantı Ayarları](#veritabanı-bağlantı-ayarları)
5. [Veritabanının Oluşturulması (Migration)](#veritabanının-oluşturulması-migration)
6. [Uygulamanın Çalıştırılması](#uygulamanın-çalıştırılması)
7. [Varsayılan Kullanıcı Bilgileri](#varsayılan-kullanıcı-bilgileri)
8. [Proje Yapısı](#proje-yapısı)
9. [Bilinen Sınırlamalar / Notlar](#bilinen-sınırlamalar--notlar)

---

## Kullanılan Teknolojiler

| Katman | Teknoloji |
|---|---|
| Framework | ASP.NET Core MVC (.NET 8.0) |
| ORM | Entity Framework Core 8.0.11 (Code-First + Migrations) |
| Veritabanı | Microsoft SQL Server (SQL Server Express / LocalDB desteklenir) |
| Kimlik Doğrulama | Cookie tabanlı authentication (`Microsoft.AspNetCore.Authentication.Cookies`) — ASP.NET Core Identity **kullanılmamıştır**, kullanıcı/rol yönetimi projeye özgü (custom) yapıdadır |
| Şifreleme | BCrypt.Net-Next 4.2.0 (kullanıcı şifreleri hash'lenerek saklanır) |
| Medya/Görsel Depolama | CloudinaryDotNet 1.29.3 (ürün görselleri Cloudinary üzerinde barındırılır) |
| Oturum Yönetimi | ASP.NET Core Session (In-Memory Distributed Cache) |
| View Engine | Razor Views (.cshtml) |
| Frontend | Bootstrap 5, Bootstrap Icons, vanilla JavaScript |

---

## Mimari ve Teknik Tercihler

- **Katmanlı yapı:** `Controllers` (HTTP/istek yönetimi) → `Services` (iş kuralları/mantık) →
  `Data` (EF Core `DbContext` ve veri erişimi) → `Models` (domain nesneleri). Controller'lar
  doğrudan iş mantığı içermez; `ProductService`, `OrderService`, `UserService`,
  `GridConfigService`, `ISliderService`/`SliderService` gibi servis sınıflarına devredilir.
  Bu ayrım, test edilebilirliği ve bakımı kolaylaştırmak için tercih edilmiştir.

- **Code-First / Migration tabanlı veritabanı yönetimi:** Şema elle SQL script'i ile değil,
  EF Core migration'ları ile yönetilir. `Data/AppDbContext.cs` içindeki `OnModelCreating`
  metodunda bazı referans veriler (`Category`, `GridColumnConfig`) `HasData(...)` ile seed
  edilmiştir; bu veriler migration uygulandığında otomatik olarak eklenir.

- **Cookie tabanlı özel kimlik doğrulama:** ASP.NET Core Identity yerine daha hafif bir
  cookie authentication şeması tercih edilmiştir. Kullanıcı şifreleri `BCrypt` ile
  hash'lenip `PasswordHash`/`PasswordSalt` alanlarında saklanır; yetkilendirme basit bir
  `Role` (`"Admin"` / `"Customer"`) alanı üzerinden yürütülür.

- **Sipariş durum akışı merkezi bir yerde yönetilir:** `OrderService.SiparisAkisi` dizisi,
  sipariş yaşam döngüsündeki (Onay Bekliyor → Onaylandı → … → Teslim Edildi) sıralı adımları
  tek bir yerden tanımlar; yeni bir aşama eklemek/çıkarmak sadece bu diziyi düzenlemeyi
  gerektirir.

- **Ödeme simülasyonu:** Proje kapsamında gerçek bir ödeme sağlayıcısı (iyzico, Stripe vb.)
  entegre edilmemiştir. `OrderController.Payment` action'ı kart bilgilerini biçimsel olarak
  doğrular (Luhn algoritması, son kullanma tarihi, CVV) ve ardından siparişi oluşturur. Gerçek
  bir ödeme altyapısı bağlanacaksa, bu nokta entegrasyon için ayrılmıştır.

- **Görsel depolama için Cloudinary:** Ürün/slider görselleri sunucu dosya sisteminde değil,
  Cloudinary üzerinde barındırılır; bu sayede uygulama sunucusu stateless kalır ve görsel
  optimizasyonu/CDN avantajından yararlanılır.

- **Kullanıcı profili:** `ProfileController`, bayi kullanıcılarının adres ve kart bilgilerini
  kaydedip kendi profillerinde görüntüleyebilmesini sağlar. Kart bilgileri, ödeme akışındaki
  gibi biçimsel doğrulama (Luhn, son kullanma tarihi, CVV) ile birlikte saklanır; gerçek bir
  ödeme sağlayıcısı ile entegre değildir.

---

## Kurulum

### Ön Gereksinimler

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Microsoft SQL Server Express (yerel instance adı: `LAPTOP-5OQRETR9\SQLEXPRESS`) — kurulu ve erişilebilir olmalı; farklı bir makinede çalıştırılacaksa bkz. [Veritabanı Bağlantı Ayarları](#veritabanı-bağlantı-ayarları)
- (Önerilir) Visual Studio 2022 (17.8+) veya VS Code + C# Dev Kit

### Adımlar

1. **Projeyi klonlayın / indirin** ve çözüm dizinine geçin:
   ```bash
   git clone <repo-url>
   cd MiniB2B
   ```

2. **Bağımlılıkları geri yükleyin:**
   ```bash
   dotnet restore
   ```

3. **`appsettings.json` dosyası**, değerlendirme kolaylığı için gerekli bağlantı ve Cloudinary
   bilgileriyle birlikte gelir (bkz. [Veritabanı Bağlantı Ayarları](#veritabanı-bağlantı-ayarları));
   ek bir düzenleme yapmanıza gerek yoktur.

4. **Veritabanını oluşturun** (bkz. [Veritabanının Oluşturulması](#veritabanının-oluşturulması-migration)).

5. **Uygulamayı çalıştırın** (bkz. [Uygulamanın Çalıştırılması](#uygulamanın-çalıştırılması)).

---

## Veritabanı Bağlantı Ayarları

Bağlantı ayarları `appsettings.json` içindeki `ConnectionStrings:DefaultConnection` alanından
okunur. Proje, değerlendirmeyi kolaylaştırmak amacıyla aşağıdaki hazır değerlerle gelir:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=LAPTOP-5OQRETR9\\SQLEXPRESS;Database=MiniB2BDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "CloudinarySettings": {
    "CloudName": "lhmwwnx1",
    "ApiKey": "471691294165233",
    "ApiSecret": "VR_ejFKuBbYbRweGMXMhcWxhIgI"
  }
}
```
---

## Veritabanının Oluşturulması (Migration)

Veritabanı şeması **Entity Framework Core Migrations** ile yönetilmektedir; elle çalıştırılacak
bir `.sql` script'i bulunmamaktadır. `Migrations/` klasöründe sırasıyla uygulanan migration'lar
bulunur 

### Yöntem 1 — .NET CLI (önerilen)

Proje dizininde (`.csproj` dosyasının bulunduğu klasörde):

```bash
dotnet tool install --global dotnet-ef   # dotnet-ef global aracı kurulu değilse
dotnet ef database update
```

Bu komut, `ConnectionStrings:DefaultConnection` içinde belirtilen sunucuda `MiniB2BDb`
veritabanını (yoksa) oluşturur ve tüm migration'ları sırasıyla uygular; `Category` ve
`GridColumnConfig` gibi seed veriler de bu sırada otomatik eklenir.

Package Manager Console'da:

```powershell
Update-Database
```

### Yeni bir migration eklemek istersen (geliştirme sırasında model değiştiyse)

```bash
dotnet ef migrations add MigrationAdi
dotnet ef database update
```

---

## Uygulamanın Çalıştırılması

### .NET CLI ile

Proje dizininde:

```bash
dotnet run
```

Konsol çıktısında uygulamanın dinlediği adres(ler) görüntülenir (varsayılan olarak
`launchSettings.json` yapılandırmasına göre genellikle `https://localhost:XXXX` şeklindedir).


### Visual Studio ile

1. `MiniB2B.sln` dosyasını Visual Studio ile açın.
2. Araç çubuğunda başlangıç projesinin **MiniB2B** olduğundan emin olun.
3. `https` profiliyle **F5** (Debug ile Başlat) veya **Ctrl+F5** (Debug olmadan Başlat) tuşuna basın.
4. Uygulama varsayılan tarayıcıda otomatik olarak açılır.

Uygulama ilk açıldığında `/Product/Index` (ürün kataloğu) sayfasına yönlendirilir.

---

## Varsayılan Kullanıcı Bilgileri

Uygulamayı test etmek için aşağıdaki hesaplar kullanılabilir:

| Rol | Kullanıcı Adı | Şifre |
|---|---|---|
| Admin (Yönetici) | `ebrukaraburk` | `123456` |
| Customer (Müşteri) | `ayseaydin` | `123456` |

Admin hesabıyla giriş yapıldığında Yönetici Paneli (Sipariş Yönetimi, ürün/kategori yönetimi
vb.) erişilebilir hale gelir; Customer hesabı ise ürün kataloğu, sepet, sipariş oluşturma ve
profilde adres/kart bilgilerini görüntüleme gibi bayi tarafı işlemleri için kullanılır.

> Not: Bu hesaplar veritabanına önceden seed edilmemiştir (`Users` tablosu için `HasData`
> tanımı yoktur) — veritabanını ilk kurduğunuzda bu kullanıcıları uygulamanın kayıt
> formu üzerinden oluşturmanız, ardından `ebrukaraburk` kullanıcısının rolünü SQL ile
> `"Admin"` olarak güncellemeniz gerekir:
> ```sql
> UPDATE Users
> SET Role = 'Admin'
> WHERE Username = 'ebrukaraburk';
> ```
> Şifreler `BCrypt` ile hash'lenerek saklandığından, veritabanına elle bir kullanıcı satırı
> eklerken `PasswordHash`/`PasswordSalt` alanlarını SQL üzerinden düz metin olarak yazmak
> **çalışmaz** — yeni kullanıcı oluştururken mutlaka uygulamanın kayıt formunu kullanın,
> SQL üzerinden yalnızca mevcut bir kullanıcının `Role` alanını güncelleyin.

---

## Proje Yapısı

```
MiniB2B/
├── Controllers/          # HTTP isteklerini karşılayan MVC controller'ları
│   ├── OrderController.cs
│   ├── ProductController.cs
│   ├── ProfileController.cs
│   ├── SliderAdminController.cs
│   └── UserController.cs
├── Data/
│   └── AppDbContext.cs   # EF Core DbContext, model konfigürasyonu ve seed veriler
├── Helpers/
│   ├── DynamicPropertyReader.cs
│   └── GridColumnRenderHelper.cs
├── Migrations/           # EF Core migration geçmişi (Code-First şema değişiklikleri)
├── Models/                # Domain nesneleri (Order, Product, User, Basket, ...)
├── Services/              # İş mantığı katmanı (OrderService, ProductService, UserService, ...)
├── Views/                 # Razor View'lar (.cshtml)
├── wwwroot/                # Statik dosyalar (css, js, lib)
├── appsettings.json
└── Program.cs             # Uygulama başlangıç/DI yapılandırması
```

---

## Bilinen Sınırlamalar / Notlar

- Ödeme akışı **simülasyondur**; gerçek bir ödeme sağlayıcısına bağlı değildir. Kart bilgileri
  yalnızca biçimsel olarak doğrulanır (Luhn kontrolü, son kullanma tarihi, CVV uzunluğu),
  hiçbir ödeme çağrısı dış servise gönderilmez.
- Kimlik doğrulama ASP.NET Core Identity yerine hafif bir cookie-based custom yapı ile
  yürütülür;
- `Program.cs` içinde giriş/çıkış yolları `/Account/Login`, `/Account/Logout` ve
  `/Account/AccessDenied` olarak tanımlıdır; 
- Cloudinary API anahtarları olmadan ürün/slider görselleri yüklenemez; bu nedenle kurulum
  öncesi geçerli bir Cloudinary hesabı ve anahtarları gereklidir.
</document_content>
