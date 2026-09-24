# Proje klasör yapısı

Klasörler dosya depolamak için değil, sorumluluk sınırlarını görünür yapmak için kullanılır.

src = üretim kodu.
tests = otomatik testler.
docs = mimari ve teknik açıklamalar.

TTwen.Domain = oyun kavramları. HTML bilmez.
TTwen.Core = temel abstraction'lar.
TTwen.Application = iş akışları.
TTwen.Infrastructure = Playwright, TTWars ve dış sistemler.
TTwen.Simulator = savaş ve ekonomi hesapları.
TTwen.App = WinUI 3 arayüzü.

## TTWars bağlantı katmanı

`src/TTwen.Domain/ValueObjects/TTWarsServerAddress.cs`
TTWars adresini doğrular ve normalize eder. URL kuralları tek yerde tutulur.

`src/TTwen.Application/Interfaces/ITTWarsConnectionService.cs`
UI'ın bağlantı hizmetiyle konuştuğu abstraction'dır.

`src/TTwen.Application/Interfaces/IBrowserSetupService.cs`
Playwright tarayıcı çalışma zamanını hazırlamak için kullanılan abstraction'dır.

`src/TTwen.Infrastructure/TTWars/TTWarsConnectionService.cs`
Kalıcı Playwright oturumunu ve TTWars client'ını birleştirir.

`src/TTwen.Infrastructure/Browser/PlaywrightBrowserSetupService.cs`
Playwright Chromium bileşenlerinin açıkça hazırlanmasını sağlar.

`src/TTwen.App/Services/AppServiceProvider.cs`
WinUI composition root'udur; somut Infrastructure servislerini burada oluşturur.

`src/TTwen.App/Views/Pages/TTWarsServerPage.xaml`
Sunucu bağlantı ekranının görünümüdür.

`src/TTwen.App/Views/Pages/TTWarsServerPage.xaml.cs`
Kullanıcı komutlarını Application servislerine iletir ve sonucu gösterir.

## Kural

Bir dosya başka klasöre yalnızca sorumluluğu değiştiği için taşınır. Sırf düzenli görünsün diye gereksiz klasör oluşturulmaz.

Yeni bir modül gerçek davranış kazanmadan önce:
1. Sözleşmesi belirlenir.
2. Domain modelleri belirlenir.
3. Infrastructure sınırı belirlenir.
4. UI yüzeyi eklenir.
5. Unit testler yazılır.
6. GitHub Actions doğrulaması geçmeden sonraki modüle geçilmez.
