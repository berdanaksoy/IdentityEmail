<h1 align="center"> MailMate: Kurumsal Seviyede E-posta Yönetim Sistemi </h1>

<p align="center"> Kullanıcıların birbirleriyle mesajlaşmasını, mesajlarını kategorize etmesini ve tüm e-posta süreçlerini merkezi bir panel üzerinden yönetmesini sağlayan gelişmiş, full-stack bir e-posta uygulaması. </p>

<p align="center">
  <img alt="Build" src="https://img.shields.io/badge/Build-Passing-brightgreen?style=for-the-badge">
  <img alt="Framework" src="https://img.shields.io/badge/Framework-ASP.NET%20Core%2010-512bd4?style=for-the-badge">
  <img alt="Database" src="https://img.shields.io/badge/Database-SQL%20Server-red?style=for-the-badge">
  <img alt="Architecture" src="https://img.shields.io/badge/Architecture-MVC-blue?style=for-the-badge">
</p>

---

## 🌟 Genel Bakış

**MailMate**, kullanıcıların platform içinde birbirlerine e-posta gönderip aldığı, mesajlarını klasörler ve kategoriler aracılığıyla organize edebildiği, profesyonel bir e-posta yönetim ekosistemidir. Gmail ve Outlook gibi modern e-posta istemcilerinden ilham alınarak tasarlanmış bu sistem; taslak, spam, çöp kutusu, yıldızlı mesajlar, toplu işlemler ve detaylı kullanıcı istatistikleri gibi kapsamlı bir özellik setiyle birlikte gelir.

### Problem
> Çoğu MVC eğitim projesinde kullanıcılar arasındaki mesajlaşma yüzeysel düzeyde kalır: tek yönlü form gönderimi, ham metin içeriği ve temel CRUD işlemleri. Gerçek dünyada bir e-posta istemcisinin sunduğu kategori sistemi, spam yönetimi, taslak akışı, kişi etiketleme ve per-kullanıcı durum takibi gibi karmaşık özellikler çoğu projede göz ardı edilir.

### Çözüm
MailMate, her mesajın her kullanıcı için bağımsız bir durum kaydına (`UserMessageBox`) sahip olduğu ilişkisel bir veri modeli üzerine kurulmuştur. Bu sayede aynı mesaj bir kullanıcı için yıldızlı, diğeri için spam, bir diğeri için kategorili olarak görünebilir. Sistem, **ASP.NET Core Identity** ile güvenli kimlik doğrulamasını, **MailKit** ile e-posta doğrulamasını ve **Summernote** ile zengin metin düzenleyiciyi bir araya getirir.

---

## ✨ Temel Özellikler

### 📬 Tam Donanımlı Posta Kutusu
* **Klasör Sistemi:** Gelen Kutusu, Gönderilenler, Taslaklar, Spam, Çöp Kutusu ve Yıldızlı — tam anlamıyla çalışan bir e-posta istemcisi deneyimi.
* **Per-Kullanıcı Durum Takibi:** Her mesaj, her kullanıcı için bağımsız okundu/okunmadı, yıldızlı, spam, çöp kutusu durumlarına sahiptir.
* **Toplu İşlemler:** Birden fazla mesajı aynı anda taşıma, silme, kategorize etme, okundu/okunmadı işaretleme ve yıldızlama.
* **Kalıcı Silme:** Çöp kutusundaki mesajlar veritabanından tamamen silinebilir; diğer kullanıcıların kayıtlarına dokunulmaz.

### ✍️ Mesaj Oluşturma & Taslak Sistemi
* **Zengin Metin Düzenleyici:** Summernote entegrasyonu ile kalın, italik, renkli metin, listeler ve bağlantı ekleme desteği.
* **Akıllı Taslak Akışı:** Taslaklar düzenlenebilir, güncellenebilir ve gönderildiğinde eski taslak otomatik silinir.
* **Kullanıcı Otomatik Tamamlama:** Alıcı alanında kayıtlı kullanıcılar anlık olarak önerilir.
* **Cevaplama:** Mesaj detayından doğrudan cevap yazma ve cevabı taslak olarak kaydetme.

### 🏷️ Kategori & Etiket Sistemi
* **Özel Kategoriler:** Her kullanıcı en fazla 5 kategori oluşturabilir; her kategori için isim ve renk seçimi yapılabilir.
* **Mesaj Kategorilendirme:** Gelen mesajlara manuel olarak kategori atanabilir.
* **Kişi Kategorilendirme:** Bir göndericiye kategori atandığında, o kişiden gelen tüm yeni mesajlar otomatik olarak ilgili kategoriye düşer.
* **Sol Panel Entegrasyonu:** Oluşturulan kategoriler sol navigasyon panelinde renkli noktalarla listelenir ve tıklanarak filtrelenebilir.

### 🚫 Spam & Engelleme Sistemi
* **Göndericiye Göre Engelleme:** Bir kullanıcı spam olarak işaretlendiğinde, o kişiden gelen tüm mesajlar gelen kutusunda görünmez; Spam klasörüne yönlendirilir.
* **Mesaj Bazlı Spam:** Tek bir mesajı spam olarak işaretleme ve spam'den çıkarma.
* **Toplu Spam İşlemleri:** Seçili mesajlar toplu olarak spam'e alınabilir veya spam'den çıkarılabilir.

### 👤 Profil & İstatistikler
* **Profil Sayfası:** Kullanıcı bilgilerini güncelleme, şifre değiştirme ve profil fotoğrafı yönetimi.
* **Kategori Yönetimi:** Profil sayfasından kategoriler eklenip silinebilir.
* **Kullanım İstatistikleri:** Gönderilen, alınan, okunmamış, yıldızlı, taslak, spam ve çöp kutusu sayıları kart görünümünde.
* **Son 12 Ay Grafiği:** Gelen mesajların aylık dağılımını gösteren interaktif bar grafiği (Chart.js).
* **Kategori Dağılımı:** Hangi kategoride kaç mesaj olduğunu gösteren doughnut grafiği.

### 🔐 Kimlik Doğrulama
* **ASP.NET Core Identity:** Kayıt, giriş ve şifre yönetimi.
* **E-posta Doğrulama:** MailKit ile kayıt sonrası e-posta doğrulama kodu gönderme ve hesap onaylama.

---

## 🛠️ Teknoloji Yığını & Mimari

Proje, veri, iş mantığı ve arayüzü birbirinden ayıran temiz bir **MVC (Model-View-Controller)** mimarisi üzerine kurulmuştur.

### Kullanılan Teknolojiler

| Teknoloji | Amacı | Neden Tercih Edildi |
| :--- | :--- | :--- |
| **ASP.NET Core 10** | Ana Backend Framework | Kurumsal seviye performans, güvenlik ve platform bağımsızlık sağlar. |
| **C#** | Programlama Dili | Güçlü tip sistemi ve modern özellikler ile sürdürülebilir iş mantığı sunar. |
| **Entity Framework Core 10** | ORM / Veri Erişimi | Migration desteği ile veritabanı işlemlerini kolaylaştırır. |
| **SQL Server** | İlişkisel Veritabanı | Veri tutarlılığı ve gelişmiş sorgulama desteği sağlar. |
| **ASP.NET Core Identity** | Kimlik Doğrulama | Güvenli kullanıcı yönetimi, şifre hashleme ve oturum kontrolü. |
| **MailKit** | E-posta Gönderimi | SMTP üzerinden güvenilir ve modern e-posta doğrulama akışı. |
| **Summernote** | Zengin Metin Editörü | Mesaj oluşturmada WYSIWYG HTML editör desteği sağlar. |
| **Chart.js** | Veri Görselleştirme | İstatistik sayfasında interaktif bar ve doughnut grafikleri. |
| **Razor / ViewComponents** | Şablon Motoru | Modüler ve yeniden kullanılabilir UI geliştirmeyi kolaylaştırır. |
| **Bootstrap & jQuery** | Frontend Tasarım | Responsive tasarım ve dinamik kullanıcı etkileşimi. |
| **Mendy Admin Template** | UI Şablonu | Profesyonel ve modern yönetim paneli arayüzü. |

---

## 📁 Proje Yapısı

```
IdentityEmail/
├── 📄 Program.cs              # Uygulama başlangıç noktası & DI yapılandırması
├── 📄 appsettings.json        # Veritabanı ve SMTP yapılandırma ayarları
├── 📂 Context/                # EF Core DbContext ve veritabanı bağlantısı
├── 📂 Controllers/            # İş mantığı ve request yönetimi
│   ├── LayoutController.cs    # Ortak ViewBag verilerini dolduran temel controller
│   ├── MessageController.cs   # Mesajlaşma, klasörler, toplu işlemler
│   ├── ProfileController.cs   # Profil, şifre değiştirme, istatistikler
│   └── LoginController.cs     # Kayıt, giriş ve e-posta doğrulama
├── 📂 Dtos/                   # Veri transfer nesneleri (InboxDto, MessageDetailDto)
├── 📂 Entities/               # Veri modelleri
│   ├── AppUser.cs             # Identity kullanıcı modeli
│   ├── Message.cs             # Mesaj entity
│   ├── UserMessageBox.cs      # Per-kullanıcı mesaj durum kaydı
│   ├── MessageCategory.cs     # Kullanıcı kategorileri
│   ├── UserContactCategory.cs # Kişi-kategori eşleştirme
│   └── SpamSender.cs          # Engellenen gönderici listesi
├── 📂 Migrations/             # EF Core veritabanı migration dosyaları
├── 📂 Models/                 # View modelleri ve doğrulama nesneleri
├── 📂 Views/                  # Razor arayüz dosyaları
│   ├── Message/               # Inbox, MessageDetail, CreateMessage
│   ├── Profile/               # Profil ve istatistik sayfaları
│   ├── Login/                 # Kayıt ve giriş sayfaları
│   └── Shared/                # _Layout, _ViewStart
├── 📂 wwwroot/                # Statik dosyalar (CSS, JS, Görseller)
│   └── mendy-admin/           # Admin şablon asset'leri
└── 📂 Database/               # SQL test verisi scriptleri
    └── script.sql             # Örnek kullanıcılar, mesajlar ve kategoriler
```

---

## 🗃️ Veritabanı Mimarisi

MailMate'in çekirdeğinde, her mesajın her kullanıcı için bağımsız bir duruma sahip olmasını sağlayan `UserMessageBox` tablosu yatar.

| Tablo | Açıklama |
| :--- | :--- |
| **AspNetUsers** | ASP.NET Identity kullanıcı tablosu (Name, Surname, ImageUrl ek alanları ile) |
| **Messages** | Gönderilen tüm mesajların içerik kaydı |
| **UserMessageBoxes** | Her kullanıcı-mesaj çifti için bağımsız durum (IsRead, IsStarred, IsSpam, IsTrash, IsDraft, CategoryId) |
| **MessageCategory** | Kullanıcıya özel kategoriler (max 5, renk ve isim) |
| **UserContactCategories** | Göndericiye atanan kategori — yeni mesajları otomatik kategorize eder |
| **SpamSenders** | Kullanıcının engellediği gönderici e-posta adresleri |

---

## 📸 Ekran Görüntüleri

### 📬 Gelen Kutusu
Okunmamış mesajlar pembe kenarlık ile vurgulanır; toplu işlem araç çubuğu, arama ve klasör navigasyonu.

<img width="2560" height="1288" alt="Screenshot 2026-05-05 124102" src="https://github.com/user-attachments/assets/8d5db71c-bd7a-43a6-abd3-c40caf113a92" />

<details>
<summary><strong>📸 Diğer Ekran Görüntülerini İncelemek İçin Tıklayın</strong></summary>
<br>

**Mesaj Detayı:**
<img width="2560" height="1290" alt="Screenshot 2026-05-05 124209" src="https://github.com/user-attachments/assets/03cf012b-7ebb-45b3-a011-baef2e1a703f" />

**Yeni Mesaj Oluştur:**
<img width="2560" height="1288" alt="Screenshot 2026-05-05 124228" src="https://github.com/user-attachments/assets/6ac7db63-c840-4035-8984-5e87bd89cbd4" />

**Taslak Düzenleme:**
<img width="2560" height="1288" alt="Screenshot 2026-05-05 124243" src="https://github.com/user-attachments/assets/ab282531-4129-4355-943a-cf96502ccae1" />

**Spam Klasörü:**
<img width="2560" height="1284" alt="Screenshot 2026-05-05 124257" src="https://github.com/user-attachments/assets/d4371646-1e4f-4103-88e5-ecd7e9df9271" />

**Çöp Kutusu:**
<img width="2560" height="1288" alt="Screenshot 2026-05-05 124311" src="https://github.com/user-attachments/assets/3dc2669a-abef-4950-b081-536677267a3b" />

**Toplu İşlem Araç Çubuğu:**
<img width="2560" height="1290" alt="Screenshot 2026-05-05 124332" src="https://github.com/user-attachments/assets/ef055f6a-ac5f-4680-bc9a-b7cc7d02b002" />

</details>

<br>

### 👤 Profil & İstatistikler
Kullanıcı bilgileri, kullanım özeti ve son 12 aya ait mesaj grafiği.

<img width="2560" height="1284" alt="Screenshot 2026-05-05 125636" src="https://github.com/user-attachments/assets/6a4dfab0-ea69-4eeb-b3eb-1c77f02988bf" />

<details>
<summary><strong>📊 Tüm Profil Sayfaları İçin Tıklayın</strong></summary>
<br>

**Profil Düzenleme:**
<img width="2560" height="1288" alt="Screenshot 2026-05-05 125642" src="https://github.com/user-attachments/assets/83ea246e-54b9-47b8-8e7c-19e9d9578cd9" />

**İstatistik:**
<img width="2560" height="1288" alt="Screenshot 2026-05-05 125648" src="https://github.com/user-attachments/assets/b4d51cb1-b839-436c-adfb-92fe80f0c304" />

</details>

<br>

### 🔐 Kimlik Doğrulama
Modern tasarımlı kayıt, giriş ve e-posta doğrulama sayfaları.

<img width="2560" height="1288" alt="Screenshot 2026-05-05 124000" src="https://github.com/user-attachments/assets/38426939-4fe5-4d4f-869c-bea171d57c81" />
<br>
<img width="2560" height="1293" alt="Screenshot 2026-05-05 124030" src="https://github.com/user-attachments/assets/76368e20-8530-4de3-8758-af9aa2edd60f" />
<br>
<img width="2560" height="1288" alt="Screenshot 2026-05-05 124036" src="https://github.com/user-attachments/assets/7115a09e-01d2-447d-974b-7db69ddc76fb" />


---

## 🚀 Kurulum

### Gereksinimler
* **.NET SDK 10.0** veya üstü
* **SQL Server** (LocalDB veya Express)
* **Visual Studio 2022** veya **VS Code + C# Dev Kit**
* **SMTP Hesabı** (e-posta doğrulama için — Gmail App Password önerilir)

### Kurulum Adımları

1. **Repository'yi Klonlayın**
    ```bash
    git clone https://github.com/berdanaksoy/MailMate.git
    cd MailMate
    ```

2. **Veritabanı Ayarı**

    `appsettings.json` içerisindeki connection string'i güncelleyin:
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=YOUR_SERVER;Database=IdentityEmailDb;Trusted_Connection=True;TrustServerCertificate=True;"
    }
    ```

3. **SMTP Yapılandırması** ⚠️

    E-posta doğrulama özelliği için `LoginController.cs` (kayıt ve e-posta doğrulama action'ları) içindeki aşağıdaki kod bloğunu **kendi SMTP bilgilerinizle** değiştirin:

    ```csharp
    var smtp = new SmtpClient("smtp.gmail.com")
    {
        Port = 587,
        Credentials = new NetworkCredential("KENDI_EMAILINIZ@gmail.com", "GMAIL_APP_PASSWORD"),
        EnableSsl = true
    };
    ```

    > **Gmail App Password nasıl alınır?**
    > Google Hesabınız → Güvenlik → 2 Adımlı Doğrulama → Uygulama Şifreleri → "Posta" için şifre oluşturun.
    >
    > Bu kod bloğu `LoginController.cs` dosyasında e-posta gönderilen her yerde geçmektedir — tüm yerleri güncellemeyi unutmayın.

4. **Migration'ları Uygulayın**

    Package Manager Console'da:
    ```powershell
    $WarningPreference = "SilentlyContinue"
    Update-Database
    ```

5. **Test Verilerini Yükleyin (Opsiyonel)**

    Projeyi hemen test etmek için örnek kullanıcılar ve mesajlarla dolu bir veri seti hazırlanmıştır.

    **Adım 1 — Aşağıdaki kullanıcıları `/Login/Register` üzerinden kayıt edin:**

    | Ad | Soyad | Kullanıcı Adı | E-posta | Şifre |
    | :--- | :--- | :--- | :--- | :--- |
    | Berdan | Aksoy | berdanaksoy | berdan0227@gmail.com | Aa123! |
    | Ahmet | Yılmaz | ahmetyilmaz | ahmet.yilmaz@gmail.com | Aa123! |
    | Zeynep | Kaya | zeynepkaya | zeynep.kaya@gmail.com | Aa123! |
    | Mehmet | Demir | mehmetdemir | mehmet.demir@gmail.com | Aa123! |
    | Ayşe | Çelik | aysecelik | ayse.celik@gmail.com | Aa123! |
    | Mustafa | Öztürk | mustafaozturk | mustafa.ozturk@gmail.com | Aa123! |
    | Fatma | Şahin | fatmasahin | fatma.sahin@gmail.com | Aa123! |
    | İbrahim | Güneş | ibrahimgunes | ibrahim.gunes@gmail.com | Aa123! |
    | Elif | Yıldız | elifyildiz | elif.yildiz@gmail.com | Aa123! |

    > ⚠️ Tüm kullanıcıların şifresi `Aa123!` olarak belirlenmiştir. Gerçek ortamda mutlaka değiştirin.

    **Adım 2 — SQL scriptini çalıştırın:**

    Tüm kullanıcıları kayıt ettikten sonra SSMS'i açın ve `Database/script.sql` dosyasını çalıştırın. Script; kategorileri, mesajları, kullanıcı kutularını, spam gönderenlerini ve kişi kategorilendirmelerini otomatik olarak oluşturur.

    > ⚠️ **Önemli:** SQL scriptini çalıştırmadan önce tüm kullanıcıların kayıt ve e-posta doğrulamasını tamamlamış olması gerekir. Aksi takdirde foreign key hataları alabilirsiniz.

6. **Projeyi Çalıştırın**
    ```bash
    dotnet run --project IdentityEmail
    ```

---

## 🔧 Kullanım

### Kayıt & Giriş
`/Login/Register` adresinden kayıt olun, e-posta doğrulama kodunu girin ve `/Login/UserLogin` üzerinden giriş yapın.

### Mesajlaşma
* **Yeni Mesaj:** Sol paneldeki **Yeni Mesaj Oluştur** butonuna tıklayın
* **Cevaplama:** Mesaj detay sayfasının altındaki cevap alanını kullanın
* **Taslak:** Mesaj yazarken **Taslak Olarak Kaydet** butonuna basın

### Kategori Sistemi
* Profil sayfasındaki **Düzenle** sekmesinden kategori oluşturun (maksimum 5)
* Mesaj detayından **Kategori Ata** ile mesajı kategorize edin
* **Göndericiye Kategori Ata** ile o kişiden gelecek tüm mesajları otomatik kategorize edin

### Spam Yönetimi
* Mesaj detayından veya toplu işlem araç çubuğundan **Spam** seçeneğini kullanın
* Spam olarak işaretlenen göndericiden gelen tüm mesajlar otomatik olarak Spam klasörüne yönlendirilir

---

## 🤝 Katkıda Bulunma

MailMate'e katkılarınızı memnuniyetle karşılıyoruz!

### Nasıl Katkı Sağlanır

1. Fork alın
2. Yeni branch oluşturun (`git checkout -b feature/yeni-ozellik`)
3. Geliştirme yapın
4. Test edin
5. Commit atın (`git commit -m 'feat: yeni özellik eklendi'`)
6. Push edin (`git push origin feature/yeni-ozellik`)
7. Pull Request açın
