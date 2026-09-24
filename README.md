# TTwen

TTWars odaklı, Windows üzerinde çalışan otomasyon ve analiz botu.

## Modüller

- TTWars sunucu adresini normalize etme
- Playwright tabanlı gerçek tarayıcı oturumu
- Köy/kaynak özetini okuma altyapısı
- Harita/vaha tarama altyapısı
- Vaha yağması analizi
- Hayvan savunma gücü hesabı
- Hayvan başına kahraman heybesine gelen hammadde hesabı
- Asker kaybı ve kayıp kaynak maliyeti hesabı
- Net kâr / tur süresi hesabı
- Minimum kârlı asker gönderme önerisi
- T3.x benzeri ayarlanabilir savaş motoru
- Neon/dark PySide6 arayüzü
- Windows EXE derleme betiği

## Vaha yağması

Temel ödül hesabında her öldürülen doğa biriminin tahıl tüketim değeri başına 40'ar Odun/Tuğla/Demir/Tahıl ödülü kabul edilir. Örneğin 2 tahıl tüketimli bir hayvan öldürülürse 80/80/80/80 kaynak üretilir.

TTWars'ın kendi kuralları standart Travian kurallarından farklıysa ttwen/rules.py içindeki değerler ve simülasyon katsayıları tek noktadan değiştirilebilir.

## Savaş simülatörü

Yerel simülatör saldırı gücü ile doğa birliklerinin savunmasını karşılaştırır ve kayıpları tahmin eder.

Sonuçlar:
- başarı / başarısızlık
- öldürülen hayvanlar
- asker kaybı
- kayıp asker kaynak maliyeti
- kazanılan kahraman kaynakları
- net kaynak kârı
- tur başına kârlılık

olarak döner.

Bu motor başlangıçta ayarlanabilir bir T3.x benzeri hesap kullanır. Gerçek TTWars savaş raporları geldikçe parametreleri TTWars sonuçlarına göre kalibre etmek için tasarlanmıştır.

## Çalıştırma

PowerShell:

py -m venv .venv
.\.venv\Scripts\Activate.ps1
pip install -r requirements.txt
python main.py

## EXE

build_exe.ps1 çalıştırılarak PyInstaller ile dist\TTwen.exe oluşturulur.

## Durum

İlk iskelet; analiz/simülasyon motoru ve arayüz hazırdır. Canlı TTWars gönderiminde kullanılacak kesin form alanları ve sayfa seçicileri, gerçek TTWars HTML yapısı üzerinden kalibre edilmelidir.
