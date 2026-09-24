from __future__ import annotations

from dataclasses import dataclass

from .models import ArmyStack, OasisProfit, OasisState
from .simulator import LocalCombatSimulator


@dataclass(frozen=True)
class ArmyTemplate:
    name: str
    unit_key: str
    initial_count: int
    step: int = 5
    max_count: int = 500


class OasisAnalyzer:
    def __init__(self, simulator: LocalCombatSimulator | None = None):
        self.simulator = simulator or LocalCombatSimulator()

    @staticmethod
    def _gross(result) -> int:
        return sum(result.hero_resources.values())

    def evaluate(
        self,
        oasis: OasisState,
        army: list[ArmyStack],
        hero_attack: int = 0,
        round_trip_seconds: int = 0,
    ) -> OasisProfit:
        result = self.simulator.simulate(oasis, army, hero_attack)
        gross = self._gross(result)
        net = gross - result.lost_troop_cost
        return OasisProfit(
            simulation=result,
            gross_resources=gross,
            loss_cost=result.lost_troop_cost,
            net_profit=net,
            round_trip_seconds=round_trip_seconds,
        )

    def find_minimum_profitable(
        self,
        oasis: OasisState,
        template: ArmyTemplate,
        hero_attack: int = 0,
        round_trip_seconds: int = 0,
        required_profit: int = 0,
    ) -> OasisProfit | None:
        count = max(1, template.initial_count)

        while count <= template.max_count:
            candidate = self.evaluate(
                oasis,
                [ArmyStack(template.unit_key, count)],
                hero_attack,
                round_trip_seconds,
            )
            if candidate.simulation.success and candidate.net_profit >= required_profit:
                return candidate
            count += max(1, template.step)

        return None

    def explain(self, result: OasisProfit) -> str:
        sim = result.simulation
        status = "KÂRLI" if result.profitable else "KÂRLI DEĞİL"
        return (
            f"{status} | Brüt: {result.gross_resources} | "
            f"Asker kaybı maliyeti: {result.loss_cost} | "
            f"Net: {result.net_profit} | "
            f"Tur/sa: {result.resources_per_hour:.1f}"
        )
