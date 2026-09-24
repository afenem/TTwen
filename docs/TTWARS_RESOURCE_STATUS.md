# TTWars Kaynak Durumu

## Amaç

v0.5.0 ile seçili köyün kaynak verileri ayrı bir kontrol merkezi ekranında
okunur ve normalize edilir. Kaynak ekranı aynı Active Village Context üzerinden
çalışır; böylece Binalar ve diğer modüllerle aynı köy hedeflenir.

## Okunan alanlar

- Odun
- Tuğla
- Demir
- Tahıl
- Depo kapasitesi
- Ambar kapasitesi
- Dört kaynağın saatlik üretimi

Canlı okuma dorf1.php üzerinden yapılır. Selector mantığı Infrastructure
katmanındaki TTWarsVillageReader içinde tutulur; UI doğrudan HTML selector bilgisi
kullanmaz.

## Durum hesapları

Doluluk yüzdesi:

  mevcut stok / kapasite * 100

Yüzde 20'nin altındaki değer düşük stok olarak, yüzde 90 ve üzerindeki değer
taşma riski olarak gösterilir. Kapasite verisi okunamadığında doluluk bilinmeyen
olarak korunur.

Bu eşikler otomasyon komutu değildir. Yalnızca test sürümündeki gözlem ve
sunum durumlarını standartlaştırır; ileride Settings üzerinden yapılandırılabilir.

## Test kapsamı

v0.5.0 test sürümünde kullanıcı şu zinciri kontrol etmelidir:

1. TTWars Sunucu ekranından sunucuya bağlanma.
2. Köyler ekranından köy listesini okuma.
3. Bir köy seçme ve seçimin ortak context'e aktarılması.
4. Binalar ekranından seçili köyün bina slotlarını okuma.
5. Kaynak Yönetimi ekranından aynı seçili köyün dorf1.php verilerini yeniden okuma.
6. Odun, tuğla, demir ve tahıl stoklarını kontrol etme.
7. Depo ve ambar kapasitesini kontrol etme.
8. Saatlik üretim değerlerini kontrol etme.
9. Doluluk yüzdelerinin stok / kapasite ile uyuştuğunu kontrol etme.
10. Eksik veri varsa uygulamanın çökmeden "VERİ YOK" veya "KAPASİTE BİLİNMİYOR"
göstermesini kontrol etme.
11. Köy değiştirildiğinde Binalar ve Kaynak Yönetimi ekranlarının yeni köyü
hedeflediğini kontrol etme.

## Bilinen sınır

Canlı TTWars HTML yapısı bu sürümde henüz kullanıcının gerçek hesabı üzerinde
doğrulanmamıştır. HTML selector uyumsuzluğu görülürse ilgili dorf1.php HTML
örneği ve selector tanısı kullanılarak bir sonraki düzeltme yapılacaktır.