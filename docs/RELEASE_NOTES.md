# TTwen Sürüm Notları

## v0.5.1 — Windows Test Dağıtımı Düzeltmesi

Durum: Test sürümüne hazırlanıyor.

Düzeltmeler:

- Unpackaged WinUI 3 uygulaması için Windows App SDK self-contained dağıtımı etkinleştirildi.
- Windows x64 test paketinin hedef makinede Windows App SDK runtime eksikliği nedeniyle sessizce başlamama riskinin giderilmesi hedeflendi.
- Test artifact sürümü v0.5.1 olarak güncellendi.

Testte ayrıca önceki v0.5.0 kapsamındaki TTWars bağlantısı, köy, bina ve kaynak okuma zinciri tekrar kontrol edilecektir.


## v0.5.0 — Kaynak Okuyucu + Kaynak Durumu

Durum: Test sürümüne hazırlanıyor.

Eklenenler:

- Seçili köye özel dorf1.php canlı okuma akışı.
- Kaynak Yönetimi ekranı.
- Odun, tuğla, demir ve tahıl stok gösterimi.
- Depo ve ambar kapasitesi gösterimi.
- Saatlik kaynak üretimi gösterimi.
- Doluluk yüzdesi hesaplama.
- Düşük stok ve kapasite taşma riski göstergeleri.
- Eksik HTML alanlarında güvenli bilinmeyen durumları.
- Kaynak durumu hesaplama unit testleri.
- Windows x64 self-contained test publish adımı.

Test edilecekler:

- TTWars bağlantısı.
- Köy listesinin okunması.
- Aktif köy seçimlerinin modüller arasında korunması.
- Seçili köyün bina slotlarının okunması.
- Seçili köyün kaynak stoklarının okunması.
- Depo / ambar kapasitesi.
- Saatlik üretimler.
- Doluluk oranları.
- Düşük stok / taşma riski eşikleri.
- Eksik veri durumunda uygulamanın kararlı kalması.

Mimari not:

Kaynak ekranı HTML selector'larını tekrar etmez. Mevcut köy reader'ının
normalize ettiği snapshot üzerinden çalışır ve analiz mantığını Application
katmanındaki VillageResourceStatusCalculator'a bırakır.

## v0.4.0 — Aktif Köy Bağlamı

Durum: Yayında.

Eklenenler:

- Uygulama genelinde seçili köy context'i.
- Köyler ekranında tekil köy seçimi.
- Seçili köyün kimliğinin ortak application state içinde tutulması.
- Binalar ekranının seçili köye göre bina okuması.
- Seçim durumunu doğrulayan unit testleri.

Mimari hedef:

Binalar, Kaynaklar, Askerler, Kahraman, Farm ve diğer modüller aynı aktif köy context'ini kullanabilecek.

Kapsam dışı:

- Köy seçiminin disk üzerinde kalıcı saklanması.
- Otomatik köy değiştirme politikaları.

## v0.3.0 — TTWars Bina Okuyucu

Durum: Yayında.

Eklenenler:

- dorf2.php bina haritası okuyucusu.
- Modern buildingSlot ve klasik T3.6 bina işaretleme desteği.
- Slot, bina adı, seviye, GID ve inşaat durumu çıkarımı.
- Bina listesi ekranı.
- Okuma tanılayıcıları ve unit test desteği.

## v0.2.0 — TTWars Köy Okuyucu

Durum: Yayında.

Eklenenler:

- TTWars köy listesi okuyucusu.
- dorf1.php üzerinden köy temel bilgilerinin çıkarılması.
- Kaynak ve üretim alanlarının ilk normalize modeli.
- Köyler kontrol merkezi ekranı.
- Başarılı / başarısız köy okuma sonuçlarının ayrıştırılması.
- Okuma tanılayıcıları ve mapper unit testleri.

## v0.1.0 — TTWars Bağlantı Temeli

Durum: Arşivlendi.

Eklenenler:

- TTWars sunucu adres modeli.
- Playwright persistent Chromium oturumu.
- TTWars bağlantı servisi.
- Sunucu bağlantı ekranı.
- Playwright kurulum / tanı akışı.