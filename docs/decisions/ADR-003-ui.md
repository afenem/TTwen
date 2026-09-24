# ADR-003: UI

Karar:
WinUI 3 + NavigationView + sayfa bazlı MVVM.

MainWindow uygulama kabuğudur.
Page modül görünümüdür.
ViewModel sunum durumunu taşır.
Application service iş akışını yönetir.

Amaç:
Çok sayıda modülü tek MainWindow class'ına sıkıştırmamak.