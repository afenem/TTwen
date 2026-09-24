# TTwen mimarisi

## Akış

UI → Application → Domain/Core

Infrastructure dış sistemlerle veri alışverişi yapar.
Simulator Application tarafından interface üzerinden kullanılır.

## Vaha örneği

UI "vahaları tara" komutu verir.
Application tarama iş akışını başlatır.
Infrastructure TTWars haritasını okur.
Reader/Mapper ham veriyi Oasis modeline dönüştürür.
Simulator savaş sonucunu tahmin eder.
OasisProfitService ekonomik sonucu çıkarır.
UI tablo ve ayrıntı panellerini günceller.

## Neden?

TTWars HTML'i değiştiğinde mümkün olduğunca sadece web sınırını değiştirmek.

## Sürekli çalışma

Kullanıcı kapatana kadar görev motoru çalışır. Kullanıcı talebi doğrultusunda rastgele anti-detection gecikmesi, kendini silme, gizli mod veya otomatik kapanma tasarımı yoktur.