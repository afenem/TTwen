from __future__ import annotations

from dataclasses import dataclass, field
from typing import Dict, List


@dataclass(frozen=True)
class AnimalProfile:
    key: str
    name: str
    attack: int
    defense_infantry: int
    defense_cavalry: int
    crop: int
    bounty_per_crop: int = 40


@dataclass(frozen=True)
class TroopProfile:
    key: str
    name: str
    attack: int
    defense_infantry: int
    defense_cavalry: int
    crop: int
    cost_total: int
    mounted: bool = False


@dataclass
class ArmyStack:
    unit_key: str
    count: int

    def copy(self) -> "ArmyStack":
        return ArmyStack(self.unit_key, self.count)


@dataclass
class OasisState:
    x: int
    y: int
    animals: Dict[str, int]
    bonus: str = ""
    distance: float | None = None


@dataclass
class SimulationResult:
    success: bool
    attacker_power: float
    defender_power: float
    attacker_loss_ratio: float
    defender_loss_ratio: float
    troop_losses: Dict[str, int] = field(default_factory=dict)
    animal_kills: Dict[str, int] = field(default_factory=dict)
    animal_remaining: Dict[str, int] = field(default_factory=dict)
    lost_troop_cost: int = 0
    hero_resources: Dict[str, int] = field(default_factory=lambda: {
        "wood": 0, "clay": 0, "iron": 0, "crop": 0
    })
    notes: List[str] = field(default_factory=list)


@dataclass
class OasisProfit:
    simulation: SimulationResult
    gross_resources: int
    loss_cost: int
    net_profit: int
    round_trip_seconds: int = 0

    @property
    def profitable(self) -> bool:
        return self.simulation.success and self.net_profit > 0

    @property
    def resources_per_hour(self) -> float:
        if self.round_trip_seconds <= 0:
            return float(self.net_profit)
        return self.net_profit / (self.round_trip_seconds / 3600)
