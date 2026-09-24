# TTwen

TTWars için C#/.NET tabanlı, ayrıntılı Windows masaüstü otomasyon ve yönetim uygulaması.

## Teknoloji
- C#
- .NET 10 LTS
- WinUI 3 / Windows App SDK 2.5.1
- Playwright .NET
- xUnit
- System.Text.Json

## Ana modüller
Dashboard, TTWars Sunucu, Köyler, Köy Kurma, Binalar, Kaynak Yönetimi, Askerler, Farm, Vaha Yağması, Natar, Kahraman, Savaş Simülatörü, Raporlar, Pazar/Transfer, İşlem Merkezi, Loglar ve Ayarlar.

## Dokümantasyon
Her önemli class, method, property ve sabitin:
- ne olduğu,
- neden bulunduğu,
- hangi katmana ait olduğu,
- hangi varsayıma dayandığı

kod içinde açıklanır. Mimari ve iş akışları docs/ altında ayrıca belgelenir.

## Mimari
UI → Application → Domain/Core
Infrastructure dış sistemlerle iletişim kurar.
Simulator savaş ve ekonomi hesaplarını UI/HTML'den bağımsız yürütür.

Python prototipi nihai yapıdan çıkarılmıştır. Canlı TTWars selector'ları ve gerçek savaş kuralları gerçek HTML/raporlarla kalibre edilecektir.
