# TTWars Bina Okuyucu

## Amaç

Bu modül aktif köyün dorf2.php sayfasından bina slotlarını canlı olarak okur.

İlk sürümde slot kimliği, bina adı, seviye, GID, dolu/boş durumu ve inşa/yükseltme işareti okunur.

## Okuma sırası

Reader önce modern buildingSlot yapısını kontrol eder.

Ardından klasik Travian/T3.6 village_map bina sınıflarını ve level overlay alanlarını kontrol eder.

Son olarak map2/map1 build.php bağlantıları uyumluluk geri dönüşü olarak taranır.

## Performans

Tüm bina slotları tek EvaluateAsync çağrısında çıkarılır.

Her bina için ayrı build.php navigasyonu yapılmaz.

## Veri doğruluğu

Level null ise seviye güvenilir biçimde okunmamıştır.

Gid null ise bina grup kimliği güvenilir biçimde okunmamıştır.

IsOccupied false ise slotun bina arsası olduğu kabul edilir.

IsUnderConstruction true ise sayfada inşa veya yükseltme sinyali bulunmuştur.

## Canlı doğrulama

Binalar ekranındaki BİNALARI OKU düğmesi aktif köy için dorf2.php okuması yapar.

Slotlar bulunamazsa reader URL, sayfa başlığı, modern slot sayısı, klasik bina görseli sayısı, level sayısı ve build bağlantısı sayısını tanılama olarak döndürür.

Gerçek TTWars HTML'i farklıysa Network/Fetch veya sayfa kaynağıyla doğrulama yapılıp yalnızca TTWarsBuildingReader selector katmanı güncellenmelidir.
