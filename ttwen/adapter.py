from __future__ import annotations

import re
from typing import Any

from playwright.sync_api import BrowserContext, Page, sync_playwright


class TTWarsAdapter:
    def __init__(self):
        self.playwright = None
        self.context: BrowserContext | None = None
        self.page: Page | None = None

    @staticmethod
    def normalize_server_url(value: str) -> str:
        value = value.strip()
        if not value:
            raise ValueError("TTWars sunucu adresi boş.")
        if not re.match(r"^https?://", value, re.I):
            value = "https://" + value
        return value.rstrip("/")

    def start(self, server_url: str) -> str:
        url = self.normalize_server_url(server_url)
        self.playwright = sync_playwright().start()
        self.context = self.playwright.chromium.launch_persistent_context(
            user_data_dir=".ttwen-browser",
            headless=False,
            viewport={"width": 1440, "height": 900},
        )
        self.page = (
            self.context.pages[0]
            if self.context.pages
            else self.context.new_page()
        )
        self.page.goto(url, wait_until="domcontentloaded")
        return self.page.url

    def stop(self) -> None:
        if self.context:
            self.context.close()
        self.context = None
        self.page = None
        if self.playwright:
            self.playwright.stop()
        self.playwright = None

    def inspect_current_page(self) -> dict[str, Any]:
        if not self.page:
            raise RuntimeError("Tarayıcı başlatılmadı.")
        return {
            "url": self.page.url,
            "title": self.page.title(),
            "html_size": len(self.page.content()),
        }

    def read_village_summary(self) -> dict[str, Any]:
        if not self.page:
            raise RuntimeError("Tarayıcı başlatılmadı.")
        text = self.page.locator("body").inner_text()
        numbers = [
            int(x.replace(".", "").replace(",", ""))
            for x in re.findall(r"(?<!\d)\d{1,9}(?!\d)", text)
        ]
        return {
            "url": self.page.url,
            "numbers": numbers[:100],
            "raw_text_preview": text[:4000],
        }

    def scan_visible_oases(self) -> list[dict[str, Any]]:
        if not self.page:
            raise RuntimeError("Tarayıcı başlatılmadı.")

        return self.page.evaluate(
            """() => {
                const nodes = [...document.querySelectorAll(
                    '[data-x][data-y], [title], a[href*="karte"], a[href*="z="]'
                )];

                const animalWords = [
                    'fare','rat','örümcek','spider','yılan','snake','yarasa','bat',
                    'domuz','boar','kurt','wolf','ayı','bear','timsah','crocodile',
                    'kaplan','tiger','fil','elephant'
                ];

                function number(v) {
                    if (v == null) return null;
                    const m = String(v).match(/-?\\d+/);
                    return m ? Number(m[0]) : null;
                }

                return nodes.map(el => {
                    const text = [
                        el.innerText || '',
                        el.getAttribute('title') || '',
                        el.getAttribute('data-title') || ''
                    ].join(' ').trim();

                    const lower = text.toLowerCase();
                    const animal = animalWords.find(
                        w => lower.includes(w)
                    ) || null;

                    if (!animal) return null;

                    const x = number(el.getAttribute('data-x'));
                    const y = number(el.getAttribute('data-y'));
                    const href = el.getAttribute('href') || '';

                    return {x, y, animal_hint: animal, text, href};
                }).filter(Boolean);
            }"""
        )
