from __future__ import annotations

import json
from dataclasses import asdict, dataclass, field
from pathlib import Path


@dataclass
class BotConfig:
    server_url: str = ""
    hero_attack: int = 0
    minimum_profit: int = 0
    round_trip_seconds: int = 0
    poll_seconds: int = 20
    selected_troop: str = "club"
    troop_start: int = 20
    troop_step: int = 10
    troop_max: int = 500
    auto_send_profitable_oasis: bool = False
    max_concurrent_actions: int = 1
    hero_resource_capacity: int = 10000
    metadata: dict = field(default_factory=dict)


def load_config(path: str = "config.json") -> BotConfig:
    p = Path(path)
    if not p.exists():
        return BotConfig()
    data = json.loads(p.read_text(encoding="utf-8"))
    return BotConfig(**data)


def save_config(config: BotConfig, path: str = "config.json") -> None:
    Path(path).write_text(
        json.dumps(asdict(config), ensure_ascii=False, indent=2),
        encoding="utf-8",
    )
