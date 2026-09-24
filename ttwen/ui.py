from __future__ import annotations

from PySide6.QtCore import QTimer
from PySide6.QtGui import QFont
from PySide6.QtWidgets import (
    QApplication,
    QCheckBox,
    QComboBox,
    QFormLayout,
    QGroupBox,
    QHBoxLayout,
    QLabel,
    QLineEdit,
    QMainWindow,
    QMessageBox,
    QPushButton,
    QSpinBox,
    QTabWidget,
    QTextEdit,
    QVBoxLayout,
    QWidget,
    QTableWidget,
    QTableWidgetItem,
)

from .adapter import TTWarsAdapter
from .config import BotConfig, save_config
from .models import OasisState
from .oasis import ArmyTemplate, OasisAnalyzer


NEON_QSS = """
QMainWindow, QWidget {
    background: #080b12;
    color: #e8ecf7;
    font-family: Segoe UI;
    font-size: 13px;
}
QTabWidget::pane {
    border: 1px solid #263147;
    border-radius: 12px;
    background: #0c111b;
}
QTabBar::tab {
    background: #0c111b;
    border: 1px solid #263147;
    padding: 10px 18px;
    margin-right: 4px;
    border-radius: 9px 9px 0 0;
}
QTabBar::tab:selected {
    background: #121b2c;
    border-color: #28d7ff;
}
QLineEdit, QSpinBox, QComboBox, QTextEdit, QTableWidget {
    background: #070a10;
    border: 1px solid #2b3954;
    border-radius: 8px;
    padding: 7px;
}
QPushButton {
    background: #101d31;
    border: 1px solid #28d7ff;
    border-radius: 9px;
    padding: 9px 15px;
}
QPushButton:hover {
    background: #15304b;
}
QGroupBox {
    border: 1px solid #263147;
    border-radius: 10px;
    margin-top: 13px;
    padding: 10px;
}
QGroupBox::title {
    subcontrol-origin: margin;
    left: 10px;
    padding: 0 5px;
}
QLabel#title {
    color: #28d7ff;
    font-size: 28px;
    font-weight: 700;
}
QLabel#status {
    color: #63ffb1;
    font-weight: 700;
}
"""


class MainWindow(QMainWindow):
    def __init__(self):
        super().__init__()
        self.setWindowTitle("TTwen • TTWars Control")
        self.resize(1160, 760)

        self.config = BotConfig()
        self.adapter = TTWarsAdapter()
        self.analyzer = OasisAnalyzer()

        self.server_input = QLineEdit()
        self.server_input.setPlaceholderText("https://nor7.ttwars.com")

        self.status = QLabel("HAZIR")
        self.status.setObjectName("status")

        self.connect_button = QPushButton("SUNUCUYA BAĞLAN")
        self.connect_button.clicked.connect(self.connect_server)

        self.log = QTextEdit()
        self.log.setReadOnly(True)

        self.hero_attack = QSpinBox()
        self.hero_attack.setRange(0, 1000000)
        self.hero_attack.setValue(0)

        self.min_profit = QSpinBox()
        self.min_profit.setRange(-10000000, 10000000)

        self.troop_select = QComboBox()
        self.troop_select.addItem("Topuzlu", "club")
        self.troop_select.addItem("Mızrakçı", "spear")
        self.troop_select.addItem("Kılıçlı", "sword")
        self.troop_select.addItem("Lejyoner", "legionnaire")
        self.troop_select.addItem("Imperian", "imperian")
        self.troop_select.addItem("Paladin", "paladin")
        self.troop_select.addItem("Equites Imperatoris", "ec_imperatoris")

        self.troop_start = QSpinBox()
        self.troop_start.setRange(1, 100000)
        self.troop_start.setValue(20)

        self.troop_step = QSpinBox()
        self.troop_step.setRange(1, 100000)
        self.troop_step.setValue(10)

        self.troop_max = QSpinBox()
        self.troop_max.setRange(1, 100000)
        self.troop_max.setValue(500)

        self.round_trip = QSpinBox()
        self.round_trip.setRange(0, 864000)
        self.round_trip.setValue(600)

        self.auto_send = QCheckBox(
            "Kârlı bulunan vahaya otomatik gönder"
        )

        self._build()

    def _build(self):
        root = QWidget()
        layout = QVBoxLayout(root)

        header = QHBoxLayout()
        title = QLabel("TTWEN")
        title.setObjectName("title")
        header.addWidget(title)
        header.addStretch()
        header.addWidget(self.status)
        layout.addLayout(header)

        tabs = QTabWidget()
        tabs.addTab(self._dashboard_tab(), "Dashboard")
        tabs.addTab(self._oasis_tab(), "Vaha Yağması")
        tabs.addTab(self._settings_tab(), "Ayarlar")
        tabs.addTab(self.log, "Log")
        layout.addWidget(tabs)

        self.setCentralWidget(root)

        self.timer = QTimer(self)
        self.timer.timeout.connect(self._heartbeat)
        self.timer.start(20000)

    def _dashboard_tab(self):
        w = QWidget()
        lay = QVBoxLayout(w)

        box = QGroupBox("TTWars Bağlantısı")
        form = QFormLayout(box)
        form.addRow("Sunucu", self.server_input)
        form.addRow(self.connect_button)
        lay.addWidget(box)

        grid = QHBoxLayout()
        for title, value in [
            ("KÖY", "—"),
            ("ASKER", "—"),
            ("VAHA", "—"),
            ("BOT", "BEKLEMEDE"),
        ]:
            card = QGroupBox(title)
            card_layout = QVBoxLayout(card)
            label = QLabel(value)
            label.setStyleSheet(
                "font-size: 22px; font-weight: 700; color: #28d7ff;"
            )
            card_layout.addWidget(label)
            grid.addWidget(card)
        lay.addLayout(grid)

        hint = QLabel(
            "Canlı TTWars bağlantısı açıldığında tarayıcı ayrı bir profil ile "
            "çalışır. İlk giriş tarayıcı üzerinden yapılabilir."
        )
        hint.setWordWrap(True)
        lay.addWidget(hint)
        lay.addStretch()
        return w

    def _oasis_tab(self):
        w = QWidget()
        lay = QVBoxLayout(w)

        controls = QGroupBox("Vaha Yağması Ayarları")
        form = QFormLayout(controls)
        form.addRow("Kahraman saldırı gücü", self.hero_attack)
        form.addRow("Minimum net kâr", self.min_profit)
        form.addRow("Asker tipi", self.troop_select)
        form.addRow("Başlangıç asker", self.troop_start)
        form.addRow("Artış", self.troop_step)
        form.addRow("Maksimum asker", self.troop_max)
        form.addRow("Gidiş + dönüş (sn)", self.round_trip)
        form.addRow(self.auto_send)
        lay.addWidget(controls)

        buttons = QHBoxLayout()
        sample = QPushButton("ÖRNEK VAHAYI HESAPLA")
        sample.clicked.connect(self.simulate_sample)
        scan = QPushButton("HARİTAYI TARA")
        scan.clicked.connect(self.scan_map)
        buttons.addWidget(sample)
        buttons.addWidget(scan)
        buttons.addStretch()
        lay.addLayout(buttons)

        self.oasis_table = QTableWidget(0, 7)
        self.oasis_table.setHorizontalHeaderLabels(
            ["Koordinat", "Hayvanlar", "Savunma", "Gönder", "Brüt", "Kayıp", "Net"]
        )
        lay.addWidget(self.oasis_table)

        return w

    def _settings_tab(self):
        w = QWidget()
        lay = QVBoxLayout(w)

        save_btn = QPushButton("AYARLARI KAYDET")
        save_btn.clicked.connect(self.save_settings)

        note = QTextEdit()
        note.setReadOnly(True)
        note.setPlainText(
            "TTWars'ın sunucuya özel HTML/oyun kuralları standart Travian'dan "
            "farklıysa simulator ve selector ayarları tek merkezden güncellenecek."
        )

        lay.addWidget(save_btn)
        lay.addWidget(note)
        lay.addStretch()
        return w

    def log_line(self, message: str):
        self.log.append(message)

    def connect_server(self):
        try:
            url = self.adapter.start(self.server_input.text())
            self.status.setText("BAĞLI")
            self.log_line(f"Bağlantı açıldı: {url}")
            info = self.adapter.inspect_current_page()
            self.log_line(
                f"Sayfa: {info['title']} | HTML: {info['html_size']} byte"
            )
        except Exception as exc:
            self.status.setText("HATA")
            QMessageBox.critical(self, "TTwen", str(exc))
            self.log_line(f"HATA: {exc}")

    def simulate_sample(self):
        oasis = OasisState(
            x=0,
            y=0,
            animals={"rat": 10, "spider": 5, "boar": 3},
        )
        template = ArmyTemplate(
            name="Seçili asker",
            unit_key=self.troop_select.currentData(),
            initial_count=self.troop_start.value(),
            step=self.troop_step.value(),
            max_count=self.troop_max.value(),
        )
        result = self.analyzer.find_minimum_profitable(
            oasis,
            template,
            hero_attack=self.hero_attack.value(),
            round_trip_seconds=self.round_trip.value(),
            required_profit=self.min_profit.value(),
        )

        self.oasis_table.setRowCount(0)
        if not result:
            self.log_line("Örnek vaha: kârlı sonuç bulunamadı.")
            return

        row = self.oasis_table.rowCount()
        self.oasis_table.insertRow(row)
        values = [
            "0|0",
            ", ".join(f"{k}={v}" for k, v in oasis.animals.items()),
            f"{result.simulation.defender_power:.0f}",
            str(result.simulation.troop_losses.get(template.unit_key, 0)),
            str(result.gross_resources),
            str(result.loss_cost),
            str(result.net_profit),
        ]
        for col, value in enumerate(values):
            self.oasis_table.setItem(row, col, QTableWidgetItem(value))

        self.log_line(self.analyzer.explain(result))
        for note in result.simulation.notes:
            self.log_line(f"• {note}")

    def scan_map(self):
        try:
            data = self.adapter.scan_visible_oases()
            self.log_line(f"Harita taraması: {len(data)} aday bulundu.")
            for item in data[:20]:
                self.log_line(str(item))
        except Exception as exc:
            self.log_line(f"Harita tarama hatası: {exc}")

    def save_settings(self):
        self.config.server_url = self.server_input.text().strip()
        self.config.hero_attack = self.hero_attack.value()
        self.config.minimum_profit = self.min_profit.value()
        self.config.selected_troop = self.troop_select.currentData()
        self.config.troop_start = self.troop_start.value()
        self.config.troop_step = self.troop_step.value()
        self.config.troop_max = self.troop_max.value()
        self.config.round_trip_seconds = self.round_trip.value()
        self.config.auto_send_profitable_oasis = self.auto_send.isChecked()
        save_config(self.config)
        self.log_line("Ayarlar config.json dosyasına kaydedildi.")

    def _heartbeat(self):
        if self.adapter.page:
            self.status.setText("BAĞLI")


def run_app():
    app = QApplication([])
    app.setStyleSheet(NEON_QSS)
    app.setFont(QFont("Segoe UI", 10))
    window = MainWindow()
    window.show()
    app.exec()
