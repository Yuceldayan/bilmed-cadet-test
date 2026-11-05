# Bilmed Cadet Test

Orijinal depo: https://github.com/bilmedbilgisayar/cadet-test

## Gereksinimler
- .NET 10 SDK: https://dotnet.microsoft.com/download/dotnet/10.0
- Visual Studio 2026, VS Code veya JetBrains Rider (tercihinize göre)

## Çalıştırma
Visual Studio ile `CadetTest.sln` dosyasını açıp F5 ile çalıştırabilir ya da komut satırından:

```
cd CadetTest
dotnet run
```

Uygulama https://localhost:44338 adresinde açılır. Hazırladığım ekran: https://localhost:44338/frontend.html

Tarayıcı sertifika uyarısı verirse geliştirme sertifikasını bir kez onaylayın:

```
dotnet dev-certs https --trust
```

## Yapılanlar

### Faz 1
`wwwroot/frontend.html`: Sayfa açılınca `User/authenticate` ile giriş yapılıyor, kayıtlar `POST api/Consents` ile 10'ar 10'ar okunuyor. Her paketten sonra Recipient kolonuna saati yazan bir satır ekleniyor. API boş liste döndüğünde döngü bitiyor.

- Her satır tablonun en üstüne ekleniyor. İlk gelen kayıtlar aşağı itildiği için en küçük Id en altta kalıyor.
- İstekler sırayla gidiyor, bir paket gelmeden sonraki istenmiyor.
- Sonraki paketin `startId` değeri son gelen kaydın Id'si + 1. Arada silinmiş Id olsa da kayıt atlanmıyor ya da tekrar gelmiyor.
- Saat `HH:mm:ss.mmm` formatında, çünkü paketler milisaniyeler arayla geliyor.

### Faz 2
Backend'e ekleme, güncelleme ve silme metotları eklendi (EF Core + LINQ). Ekranda "Yeni Kayıt" butonu ve her satırda "Düzenle" / "Sil" butonları var.

| Metot | Rota | Cevap |
|---|---|---|
| GET | `api/Consents/{id}` | 200, yoksa 404 |
| POST | `api/Consents/create` | 201 + oluşan kayıt, hatalı veride 400 |
| PUT | `api/Consents/{id}` | 200 + güncel kayıt, 400, 404 |
| DELETE | `api/Consents/{id}` | 204, yoksa 404 |

Hepsi token ister (`Authorization: Bearer <jwtToken>`). Örnek:

```
POST api/Consents/create
{ "type": "MESAJ", "recipient": "905551112233", "status": "ONAY", "recipientType": "TACIR" }

201 Created
{ "id": 1001, "type": "MESAJ", "recipient": "905551112233", "status": "ONAY", "recipientType": "TACIR" }
```

Sunucudan başarılı cevap geldikten sonra ekranda sadece ilgili satır değişiyor. Sayfa yenilenmiyor ve kayıtlar tekrar çekilmiyor:
- Yeni kayıt tablonun en üstüne ekleniyor, çünkü Id'si en büyük.
- Güncellenen satır yerinde güncelleniyor.
- Silinen satır tablodan kaldırılıyor.

Değişen dosyalar: `wwwroot/frontend.html`, `Models/ConsentSaveRequest.cs` (yeni), `Services/DataService.cs`, `Controllers/ConsentsController.cs`.

## Kararlar
- **Rotalar:** `POST api/Consents` okuma için kullanıldığı ve Faz 1 ona bağlı olduğu için değiştirmedim. Eklemeyi `POST api/Consents/create` olarak ayırdım.
- **Katmanlar:** Mevcut yapı korundu: Controller → `IDataService` → `DataContext`.
- **İstek modeli:** Ekleme ve güncellemede entity yerine `ConsentSaveRequest` kullanılıyor. Bu modelde Id yok, Id'yi veritabanı veriyor. Alanlar `[Required]`, `[StringLength]` ve `[AllowedValues]` ile kontrol ediliyor, geçersiz istekte otomatik 400 dönüyor. İzin verilen değerleri İYS izin tiplerine göre seçtim: EPOSTA/MESAJ/ARAMA, ONAY/RET, BIREYSEL/TACIR.
- **Giriş:** Sayfa açılır açılmaz çalışsın diye Program.cs'teki ilk kullanıcıyla otomatik giriş yapılıyor. Gerçek bir uygulamada bu bilgiler bir giriş formundan alınır.
- **Token:** Token 5 dakikada bitiyor. Bütün istekler `apiFetch` fonksiyonundan geçiyor; 401 gelirse yeniden giriş yapılıp istek bir kez tekrarlanıyor.
- **Pencereler:** Hazır UI elementi kullanmamak için `alert` / `confirm` yerine HTML/CSS ile kendi pencerelerimi yazdım.
- **Güvenlik:** Gelen metinler tabloya `textContent` ile yazılıyor, `innerHTML` kullanılmadı.
- **Yükleme sırasında ekleme:** "Yeni Kayıt" butonu yükleme bitince açılıyor. Yükleme sürerken eklenen kayıt döngü tarafından da okunup tabloda iki kez görünürdü.

## Sınırlamalar
- Veritabanı InMemory, uygulama kapanınca değişiklikler kayboluyor.
- Aynı kayıt aynı anda iki kişi tarafından güncellenirse son kaydeden kazanıyor.
