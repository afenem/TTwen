# Savaş simülatörü

## Ayrı proje

Savaş hesabının UI veya HTML'e bağlı olmaması için simulator ayrı tutulur.

## Planlanan bileşenler

CombatModels
CombatRuleSet
AttackPowerCalculator
DefensePowerCalculator
LossCalculator
HeroLootCalculator
ProfitCalculator

İlk implementasyon LocalCombatSimulator'dır.

## Kalibrasyon

İlk formül kesin TTWars formülü değildir. Gerçek raporlarla saldırı, savunma, kayıp, hayvan ölümü ve kaynak sonucu karşılaştırılarak kalibre edilir.

## Test

Kazanç, kayıp, tek hayvan, çoklu hayvan, piyade, süvari ve kahramanlı saldırı senaryoları test edilir.