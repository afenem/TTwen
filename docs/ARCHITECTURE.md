# TTwen mimarisi

## Akış

UI → Application → Domain/Core

Infrastructure dış sistemlerle veri alışverişi yapar.
Simulator Application tarafından interface üzerinden kullanılır.

## TTWars bağlantı akışı

TTWarsServerPage kullanıcıdan adres alır.
AppServiceProvider, Application interface'leri üzerinden somut servisleri sağlar.
TTWarsConnectionService adresi doğrular ve PlaywrightBrowserSession başlatır.
TTWarsClient mevcut Playwright sayfasını TTWars adresine yönlendirir.
Bağlantı sonucu Application modeli olarak UI'a döner.

Bu akışta UI hiçbir Playwright selector'ı veya HTML ayrıntısı bilmez.

## Vaha örneği

UI "vahaları tara" komutu verir.
Application tarama iş akışını başlatır.
Infrastructure TTWars haritasını okur.
Reader/Mapper ham veriyi Oasis modeline dönüştürür.
Simulator savaş sonucunu tahmin eder.
OasisProfitService ekonomik sonucu çıkarır.
UI tablo ve ayrıntı panellerini günceller.

## Neden?

TTWars HTML'i değiştiğinde mümkün olduğunca sadece web sınırını değiştirmek.

## Tarayıcı oturumu

Playwright kalıcı Chromium context kullanır. Profil TTwen'e özel LocalAppData altında tutulur.
Tarayıcı başlangıcı bağlantı servisine, selector ve HTML kodu TTWars adapter katmanına aittir.
Chromium eksikse uygulama bunu gizli bir hata olarak bırakmak yerine sunucu ekranından açıkça hazırlama seçeneği sunar.

## Sürekli çalışma

Kullanıcı kapatana kadar görev motoru çalışır. Kullanıcı talebi doğrultusunda rastgele anti-detection gecikmesi, kendini silme, gizli mod veya otomatik kapanma tasarımı yoktur.
