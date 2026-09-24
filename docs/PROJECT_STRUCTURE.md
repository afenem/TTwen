# Proje klasör yapısı

Klasörler dosya depolamak için değil, sorumluluk sınırlarını görünür yapmak için kullanılır.

src = üretim kodu.
tests = otomatik testler.
docs = mimari ve teknik açıklamalar.

TTwen.Domain = oyun kavramları. HTML bilmez.
TTwen.Core = temel abstraction'lar.
TTwen.Application = iş akışları.
TTwen.Infrastructure = Playwright, TTWars ve dış sistemler.
TTwen.Simulator = savaş ve ekonomi hesapları.
TTwen.App = WinUI 3 arayüzü.

Kural: Bir dosya başka klasöre yalnızca sorumluluğu değiştiği için taşınır. Sırf düzenli görünsün diye gereksiz klasör oluşturulmaz.