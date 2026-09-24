# Ayrıntılı UI spesifikasyonu

## Ana menü

Dashboard
TTWars Sunucu
Köyler
Köy Kurma
Binalar
Kaynak Yönetimi
Askerler
Farm
Vaha Yağması
Natar
Kahraman
Savaş Simülatörü
Raporlar
Pazar / Transfer
İşlem Merkezi
Loglar
Ayarlar

## TTWars Sunucu

Ana amaç: TTWars web oturumunun bağlantı sınırını kullanıcıya açık ve yönetilebilir biçimde sunmak.

Ekran bölümleri:
- Sunucu adresi ve bağlantı düğmeleri.
- Bağlantı durumu.
- Aktif sunucu adresi.
- Son açılan sayfanın başlığı.
- Playwright Chromium hazırlama düğmesi ve sonucu.
- Bağlantı hatası için teknik ayrıntı alanı.

Durum renkleri:
- Cyan = aktif sistem/bağlantı operasyonu.
- Yeşil = başarılı bağlantı.
- Amber = bekleyen hazırlık veya uyarı.
- Kırmızı = bağlantı/çalışma hatası.
- Mor = analiz ve simulator.

Bu ekran yalnızca bağlantı sınırını doğrular. Köy, kaynak, asker veya harita verileri daha sonraki okuyucu/adaptör adımlarında eklenir.

## Dashboard

Kartlar:
aktif köy, nüfus, asker, giden saldırı, gelen saldırı, taranan vaha, saatlik net kâr, aktif görev.

Alt paneller:
son işlemler, bina kuyruğu, asker kuyruğu, kaynak taşması, hata/uyarı ve seçili köy özeti.

## Köyler

Her köy için:
koordinat, nüfus, kaynaklar, depo/ambar, üretim, binalar, askerler, kahraman, aktif şablon, görev ve otomasyon durumu.

## Köy Kurma

Hazır şablonlar:
Yeni köy, Hammadde, Tahıl, Saldırı, Savunma, Askeri, Özel.

Şablonda:
bina sırası, kaynak hedefleri, depo/ambar, asker hedefi, pazar ve kahraman planı.

## Binalar

Plan editörü:
bina, mevcut seviye, hedef seviye, öncelik, ön koşul, kaynak şartı, sıra ve köy override.

## Kaynak

Üretim, stok, kapasite, taşma, eksik kaynak, köyler arası transfer ve hedef stok.

## Asker

Mevcut birlikler, üretim kuyruğu, birlik hedefi, saldırı rezervi, savunma rezervi ve üretim profili.

## Farm

Hedef, koordinat, mesafe, son rapor, asker, başarı, kayıp, net kâr, öncelik ve durum.

## Vaha

Koordinat, mesafe, bonus, tüm hayvanlar, savunma, önerilen asker, kayıp, öldürülen hayvan, kahraman heybesi, brüt, asker maliyeti, net kâr ve saatlik kâr.

Filtreler:
kârlı, kayıpsız, minimum net kâr, maksimum kayıp, maksimum mesafe, minimum saatlik kâr.

Ek alanlar:
simülasyon özeti, rule set, alternatif asker senaryoları ve seçilen saldırı.

## Savaş Simülatörü

Manuel ordu, kahraman, hedef hayvanlar/oyuncu birlikleri, rule set ve ayrıntılı sonuç.

Ayrıca farklı asker miktarlarını tarayarak minimum başarı ve ekonomik optimumu ayrı gösterir.

## İşlem Merkezi

Botun yaptığı her anlamlı işlem:
modül, köy, hedef, başlangıç, sonuç, hata ve sonraki işlem.

## Loglar

Bilgi, uyarı, hata ve modül filtreleri.
