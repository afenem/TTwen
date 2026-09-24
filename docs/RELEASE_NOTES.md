# TTwen Sürüm Notları

## v0.4.0 — Aktif Köy Bağlamı

Durum: Feature branch / CI doğrulaması bekleniyor.

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
