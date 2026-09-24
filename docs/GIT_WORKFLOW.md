# Git Workflow — TTwen

## Amaç

Bu proje için geliştirme akışının temel amacı, `main` dalını her zaman derlenebilir ve testleri geçen çalışan sürüm olarak korumaktır.

Branch yapısı uygulamanın çalışma zamanındaki performansını değiştirmez. Ancak geliştirme, test ve hata ayıklama sürecini daha güvenli ve hızlı hale getirir.

## Branch politikası

Kalıcı ana branch yalnızca:

- `main`: çalışan ve yayınlanabilir sürüm.

Geçici çalışma branch'leri:

- `feature/<konu>`: yeni özellik.
- `fix/<konu>`: hata düzeltmesi.
- `refactor/<konu>`: davranışı değiştirmeden kod yapısını iyileştirme.
- `docs/<konu>`: dokümantasyon değişikliği.
- `chore/<konu>`: build, CI veya proje altyapısı işleri.

`develop` branch'i kullanılmaz. Tek geliştiricili bir projede ikinci bir kalıcı entegrasyon dalı gereksiz merge maliyeti oluşturur.

## Standart geliştirme akışı

Her yeni iş şu sırayla ilerler:

```
main
  |
  +--> feature/xxx
          |
          +--> geliştirme
          |
          +--> Draft PR
          |
          +--> GitHub Actions
          |       |
          |       +--> Restore
          |       +--> Build
          |       +--> Tests
          |
          +--> başarılıysa PR merge
                    |
                    v
                   main
```

Yeni bir özelliğe başlamadan önce branch mutlaka güncel ve yeşil `main` commit'inden oluşturulur.

## Main kuralları

`main` üzerinde doğrudan özellik geliştirilmez.

Bir değişiklik `main`e alınmadan önce:

1. Branch üzerinde kod tamamlanır.
2. GitHub Actions build aşaması başarılı olur.
3. Birim testleri başarılı olur.
4. PR üzerinden gözden geçirilir.
5. PR birleştirilir.
6. Birleştirme sonrasında `main` için CI tekrar doğrulanır.

CI başarısızsa PR `main`e alınmaz.

## Commit düzeni

Commit mesajları kısa ve amaca yönelik tutulur:

```
feat: add village reader
fix: handle invalid server url
refactor: separate village mapper
test: add server address tests
docs: document village automation
chore: update CI workflow
```

Tek commit içine birbiriyle ilgisiz özellikler konulmaz.

## PR düzeni

Özellik tamamlanmamış olsa bile uzun süren işlerde Draft PR erken açılabilir. Böylece her yeni push otomatik doğrulama sürecine girer.

PR başlığı değişikliğin amacını açıkça belirtmelidir.

Örnek:

```
feat: add TTWars village reader
```

PR açıklamasında en az şu bilgiler bulunmalıdır:

- Değişikliğin amacı
- Değişen ana bileşenler
- Test durumu
- Bilinen sınırlamalar

## Merge stratejisi

Küçük ve tek amaçlı feature branch'leri için **Squash Merge** tercih edilir. Böylece `main` geçmişinde her özellik tek ve anlaşılır bir commit olarak görünür.

Örneğin:

```
feature/village-reader
  feat: ...
  test: ...
  fix: ...
  fix: ...
        |
        +--> squash merge
                |
                v
main
  feat: add TTWars village reader
```

## Branch yaşam döngüsü

Bir branch yalnızca üzerinde çalışılan konu için yaşar.

İş tamamlanıp `main`e alındıktan sonra branch silinir.

Uzun süre yaşamış ve birden fazla bağımsız özelliği içeren branch'ler oluşturulmaz.

## CI performansı

Mevcut CI, `main` push'larını ve `main`e açılan PR'ları doğrular. Özellik geliştirirken Draft PR açılması önerilir; böylece feature branch'teki her yeni push PR üzerinden doğrulanabilir.

CI'de paralel ve gereksiz ikinci bir `develop` entegrasyon katmanı tutulmaz.

NuGet bağımlılık önbelleklemesi ileride restore süresi anlamlı ölçüde artarsa ayrıca eklenebilir. Cache eklenirken lock-file stratejisiyle birlikte ele alınmalıdır.

## Branch isimlendirme örnekleri

```
feature/village-management
feature/building-system
feature/resource-management
feature/troop-management
feature/farm-system
feature/oasis-system
feature/hero-system
feature/report-system

fix/server-url-parser
fix/playwright-session

refactor/ttwars-readers

docs/architecture
docs/automation-rules

chore/ci
chore/project-structure
```

## Değişmez kural

`main` = çalışan sürüm.

Bir sonraki özelliğin kodu başlamadan önce son yeşil `main` commit'i temel alınır.

Bir PR'da CI kırılmışsa başka özellik eklenmez; önce kırılan CI düzeltilir.
