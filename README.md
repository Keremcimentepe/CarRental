# CarRental
RentGo v2 Kurulum ve Çalıştırma Rehberi

Projeyi kendi bilgisayarınızda çalıştırmak için aşağıdaki adımları sırasıyla uygulayın.
1. Ön Gereksinimler

Bilgisayarınızda şunların kurulu olduğundan emin olun:

    .NET SDK (Projenin derlenmesi için)

    PostgreSQL (Veritabanı için)

    İsteğe Bağlı: PgAdmin 4 veya DBeaver (Veritabanını arayüzden görmek için)

2. Projeyi Bilgisayara İndirme

Terminali (veya Komut İstemini) açın ve projeyi klonlayıp klasörün içine girin:
Bash

git clone <BURAYA_GITHUB_LINKINI_YAZIN>
cd WebProje

3. Veritabanı Bağlantı Ayarlarını Yapma

Projenin veritabanınıza bağlanabilmesi için şifrenizi girmelisiniz.

    CarRentalApp.Web klasörünün içindeki appsettings.json dosyasını açın.

    ConnectionStrings -> DefaultConnection satırını bulun.

    Username= ve Password= kısımlarına kendi PostgreSQL kullanıcı adınızı ve şifrenizi yazıp dosyayı kaydedin. (Örn: Username=postgres;Password=1234)

4. Entity Framework Araçlarını Yükleme

Veritabanını kodlar üzerinden otomatik oluşturmak için EF Core aracının yüklü olması gerekir. Terminalde şu komutu çalıştırın:
Bash

dotnet tool install --global dotnet-ef

(Eğer zaten yüklüyse bu adımı geçebilirsiniz.)
5. Veritabanını Otomatik Oluşturma (Sihirli Adım)

Projenin ana dizininde (klasörlerin hepsini gördüğünüz yerde) terminali açın ve şu komutu çalıştırarak tabloların PostgreSQL'de otomatik oluşturulmasını sağlayın:
Bash

dotnet ef database update --project CarRentalApp.DataAccess --startup-project CarRentalApp.Web

Ekranda "Done." yazısını gördüğünüzde veritabanı başarıyla kurulmuş demektir.
6. Projeyi Çalıştırma

Her şey hazır! Projeyi ayağa kaldırmak için Web klasörüne girin ve çalıştırın:
Bash

cd CarRentalApp.Web
dotnet run

Terminalde çıkan http://localhost:XXXX linkine tıklayarak projeyi tarayıcınızda açabilirsiniz.

⚠️ İlk Giriş ve Admin Hesabı:
Sisteme ilk kez girdiğinizde "Kayıt Ol" ekranından "Admin" rolünü seçin. Sistem sizden bir güvenlik kodu isteyecektir. Kodu admin olarak girerek ilk yönetici hesabınızı oluşturabilirsiniz.
