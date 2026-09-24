# TTWars bağlantı katmanı

## Amaç

TTwen'in ilk gerçek çalışma sınırı TTWars sunucu bağlantısıdır. Bu katman yalnızca tarayıcı oturumunu açmak, sunucu adresini doğrulamak ve sayfanın erişilebilir olduğunu doğrulamakla ilgilenir.

Köy, kaynak, asker, vaha ve rapor okuma işlemleri bağlantı doğrulandıktan sonra ayrı reader/mapper bileşenleri olarak eklenecektir.

## Adres kuralları

`TTWarsServerAddress`:

- `http://` ve `https://` kabul eder.
- Şema belirtilmezse HTTPS ekler.
- Host bulunmayan adresleri reddeder.
- URL içinde kullanıcı adı/parola kabul etmez.
- Sondaki gereksiz yolu normalize eder; örneğin `/dorf1.php/` → `/dorf1.php`.

## Tarayıcı oturumu

`PlaywrightBrowserSession` kalıcı Chromium context kullanır.

Profil yolu:

`%LOCALAPPDATA%/TTwen/Playwright/Profile`

Bu profil yalnızca TTwen oturumunun saklanması içindir. Böylece kullanıcı normal tarayıcı profili ile bot oturumunu birbirinden ayırabilir.

## Bağlantı

Akış:

`TTWarsServerPage`
→ `ITTWarsConnectionService`
→ `TTWarsConnectionService`
→ `PlaywrightBrowserSession`
→ `TTWarsClient`
→ TTWars

Bağlantı başarılı olduğunda:

- normalize edilmiş sunucu adresi,
- açılan sayfanın başlığı,
- bağlı/bağlı değil durumu

UI'a aktarılır.

## Chromium hazırlığı

Playwright belirli browser binary sürümlerine ihtiyaç duyduğu için uygulama ilk çalıştırmada Chromium'u kendiliğinden indirmek yerine kullanıcıya açık bir hazırlama düğmesi sunar. Gerekli browser binary'leri Playwright'ın resmi kurulum mekanizması üzerinden hazırlanır.

## Sonraki aşama

Bağlantı katmanı doğrulandıktan sonra sıradaki adım:

1. TTWars giriş/oturum durumunu tespit etmek.
2. Köy sayfasının gerçek HTML yapısını analiz etmek.
3. `VillageReader` eklemek.
4. Ham HTML verisini Domain `Village` modeline map etmek.
5. Gerçek köy verilerini Dashboard'a bağlamak.

Bağlantı katmanı içinde oyun verisi selector'ları bulunmamalıdır.
