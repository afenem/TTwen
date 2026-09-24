# ADR-002: Browser automation

Karar:
Playwright .NET.

Playwright yalnızca Infrastructure katmanında bulunur. Application selector bilmez. Domain HTML bilmez. UI DOM bilmez.

Amaç:
TTWars HTML değiştiğinde değişikliğin mümkün olduğunca tek sınırda tutulması.