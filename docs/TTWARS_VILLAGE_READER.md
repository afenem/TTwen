# TTWars Köy Okuyucu

## Amaç

Bu modül, bağlı TTWars hesabındaki köylerin canlı web durumunu yalnızca okuma amacıyla toplar.

İlk sürümde okunan temel veriler:

- köy kimliği ve adı
- başkent işareti
- koordinatlar
- nüfus
- odun, tuğla, demir ve tahıl stokları
- depo ve ambar kapasitesi
- saatlik kaynak üretimleri

Bu modül henüz hiçbir bina, asker, pazar veya otomasyon işlemi başlatmaz.

## Katmanlar

~~~text
TTWars HTML
    |
    v
TTWarsVillageReader
    |
    v
TTWarsVillagePageData
    |
    v
TTWarsVillageSnapshotMapper
    |
    v
VillageSnapshot
    |
    v
VillagesPage
~~~

Reader yalnızca web/Playwright selector ayrıntılarından sorumludur.

Mapper ham web verisini domain snapshot'ına dönüştürür.

Snapshot canlı gözlemdir ve kalıcı otomasyon ayarlarından ayrı tutulur.

## Okuma stratejisi

Reader önce açık sayfada bilinen köy değiştirme bağlantılarını arar. Bulamazsa dorf3.php üzerinden hesap köy listesini toplamaya çalışır.

Her köy için:

1. dorf1.php?newdid=<id> açılır.
2. Sayfa tek JavaScript değerlendirmesinde kaynak, üretim, nüfus ve koordinat alanlarını çıkarır.
3. Ham veri mapper ile VillageSnapshot oluşturur.
4. Köy okuma hatası diğer köylerin sonuçlarını mümkün olduğunca koruyacak şekilde ayrı raporlanır.
5. İşlem sonunda ilk sayfa geri yüklenmeye çalışılır.

## Selector yaklaşımı

Reader tek bir kırılgan selector'a bağlı değildir. Klasik Travian/T3.6 işaretlerinin yanında yaygın sidebar ve data-did varyantları da kontrol edilir.

Kaynak çubuğu için klasik #l1 ... #l4 işaretleri, üretim tablosu için #production, köy listesi için #overview, #vlist, sidebar newdid bağlantıları ve data-did kayıtları desteklenir.

## Canlı TTWars doğrulaması

TTWars sunucusunun gerçek HTML yapısı bu uyumluluk selector'larıyla aynı olmak zorunda değildir.

Okuyucu köy listesi bulamazsa mevcut URL, sayfa başlığı ve selector sayımlarını Details alanında raporlar.

Bu nedenle ilk canlı bağlantı testinde:

- Köyler ekranında yenileme çalıştırılmalı.
- Başarısızlık olursa ekrandaki Details içeriği incelenmeli.
- Gerekirse Network/Fetch verisiyle gerçek HTML yapısı doğrulanmalıdır.
- Sonrasında yalnızca TTWarsVillageReader selector katmanı güncellenmelidir.

## Performans

Reader bir sayfada temel alanları tek EvaluateAsync çağrısında toplar.

Köy listesi bulunabiliyorsa mevcut sayfadan kullanılır; yalnızca gerekli durumda dorf3.php açılır.

Her köy için yalnızca gerekli dorf1.php sayfası açılır.

İşlem sonunda başlangıç sayfası geri yüklenmeye çalışılır.

## Bilinmeyen değerler

HTML alanı bulunamazsa değer null olarak korunur.

0 gerçek sıfır değerini, null ise alanın okunamadığını temsil eder.

Bu ayrım ileride kaynak yönetimi ve otomasyon kararlarının yanlış veriye dayanmasını önler.

## Sınırlamalar

İlk sürümde:

- bina seviyeleri okunmaz
- asker sayıları okunmaz
- kahraman bilgisi okunmaz
- inşa/eğitim kuyrukları okunmaz
- otomatik işlem yapılmaz

Bu alanlar ayrı modüllerde ve canlı HTML doğrulamasıyla eklenecektir.
