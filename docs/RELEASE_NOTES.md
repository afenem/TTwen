# TTwen Sürüm Notları

## v0.2.0 — TTWars Köy Okuyucu

**Durum:** Feature branch / canlı TTWars HTML doğrulaması bekleniyor

### Eklenenler

- TTWars hesabındaki köylerin canlı okunması için Playwright reader katmanı.
- Köy kimliği, ad, başkent durumu, koordinat ve nüfus modeli.
- Odun, tuğla, demir ve tahıl stokları.
- Depo ve ambar kapasitesi.
- Saatlik kaynak üretimleri.
- Ham web verisi ile domain snapshot arasında mapper katmanı.
- Kısmi okuma ve hata ayrıntılarını taşıyan sonuç modeli.
- Köyler kontrol merkezi ekranı.
- Okuma durumu, köy sayısı ve son okuma zamanı göstergeleri.
- Eksik veya okunamayan alanların `null` olarak korunması.
- Selector sorunları için tanılama bilgileri.
- Mapper için birim testleri.

### Mimari değişiklik

Yeni katman:

```text
TTWars HTML
    ↓
TTWarsVillageReader
    ↓
TTWarsVillagePageData
    ↓
TTWarsVillageSnapshotMapper
    ↓
VillageSnapshot
    ↓
VillagesPage
```

### Kapsam dışı

Bu sürüm henüz:

- bina seviyelerini,
- asker sayılarını,
- kahraman durumunu,
- inşa/eğitim kuyruklarını,
- otomatik oyun işlemlerini

gerçekleştirmez.

### Doğrulama

- GitHub Actions Build: başarılı
- GitHub Actions Unit Tests: başarılı
- Canlı TTWars HTML doğrulaması: sonraki test aşaması
