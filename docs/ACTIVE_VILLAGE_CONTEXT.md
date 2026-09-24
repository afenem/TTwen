# Aktif Köy Bağlamı

## Amaç

Köy seçimini tek bir application state üzerinden paylaşmak.

Köyler ekranında seçilen VillageSnapshot, Binalar ve ileride Kaynaklar, Askerler, Kahraman, Farm ve diğer modüller tarafından aynı bağlam üzerinden kullanılacaktır.

## Tasarım

UI doğrudan başka bir Page'a köy verisi taşımaz.

Akış:

VillagesPage
    ↓
IActiveVillageContext
    ↓
BuildingsPage / diğer modüller

ActiveVillageContext process scope içinde yaşar. Kalıcı ayar değildir.

## Davranış

Köyler listesi yenilendiğinde mevcut seçim kimliği korunmaya çalışılır.

Daha önce seçilmiş köy listede yoksa ilk okunan köy seçilir.

Binalar ekranı seçili köyün kimliğini reader'a açıkça verir.

Bu sayede Binalar ekranının tarayıcıda tesadüfen açık kalan köye bağlı kalması önlenir.
