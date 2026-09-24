from __future__ import annotations

import math

from .models import ArmyStack, OasisState, SimulationResult
from .rules import ANIMALS, TROOPS


class LocalCombatSimulator:
    """
    Deterministic, configurable Travian-style approximation.

    This class is isolated so TTWars combat-report calibration can replace
    the formula later without changing the rest of the application.
    """

    def __init__(self, power_exponent: float = 1.5):
        self.power_exponent = power_exponent

    @staticmethod
    def _army_power(stacks: list[ArmyStack]) -> tuple[float, float, float]:
        total = 0.0
        infantry = 0.0
        cavalry = 0.0
        for stack in stacks:
            profile = TROOPS.get(stack.unit_key)
            if not profile or stack.count <= 0:
                continue
            value = profile.attack * stack.count
            total += value
            if profile.mounted:
                cavalry += value
            else:
                infantry += value
        return total, infantry, cavalry

    @staticmethod
    def _animal_defense(oasis: OasisState, infantry_weight: float, cavalry_weight: float) -> float:
        total_weight = infantry_weight + cavalry_weight
        if total_weight <= 0:
            infantry_weight = cavalry_weight = 0.5
            total_weight = 1.0
        iw = infantry_weight / total_weight
        cw = cavalry_weight / total_weight
        defense = 0.0
        for key, count in oasis.animals.items():
            profile = ANIMALS.get(key)
            if profile:
                defense += count * (
                    profile.defense_infantry * iw
                    + profile.defense_cavalry * cw
                )
        return defense

    def simulate(
        self,
        oasis: OasisState,
        army: list[ArmyStack],
        hero_attack: int = 0,
    ) -> SimulationResult:
        total_attack, infantry_attack, cavalry_attack = self._army_power(army)
        attacker_power = total_attack + max(0, hero_attack)
        defender_power = self._animal_defense(
            oasis, infantry_attack, cavalry_attack
        )

        if attacker_power <= 0:
            return SimulationResult(
                success=False,
                attacker_power=0,
                defender_power=defender_power,
                attacker_loss_ratio=1.0,
                defender_loss_ratio=0.0,
                animal_remaining=dict(oasis.animals),
                notes=["Saldırı gücü 0."]
            )

        ratio = attacker_power / max(defender_power, 1.0)

        if ratio >= 1:
            attacker_loss = min(1.0, (1.0 / ratio) ** self.power_exponent)
            defender_loss = 1.0
            success = True
        else:
            attacker_loss = 1.0
            defender_loss = min(1.0, ratio ** self.power_exponent)
            success = False

        troop_losses: dict[str, int] = {}
        lost_cost = 0
        for stack in army:
            profile = TROOPS.get(stack.unit_key)
            if not profile:
                continue
            losses = min(stack.count, math.ceil(stack.count * attacker_loss))
            troop_losses[stack.unit_key] = losses
            lost_cost += losses * profile.cost_total

        animal_kills: dict[str, int] = {}
        animal_remaining = dict(oasis.animals)
        if success:
            animal_kills = dict(oasis.animals)
            animal_remaining = {k: 0 for k in oasis.animals}
        else:
            for key, count in oasis.animals.items():
                kills = min(count, math.floor(count * defender_loss))
                animal_kills[key] = kills
                animal_remaining[key] = max(0, count - kills)

        resources = {
            "wood": 0,
            "clay": 0,
            "iron": 0,
            "crop": 0,
        }
        for key, kills in animal_kills.items():
            profile = ANIMALS.get(key)
            if not profile:
                continue
            amount = profile.crop * profile.bounty_per_crop * kills
            for resource in resources:
                resources[resource] += amount

        return SimulationResult(
            success=success,
            attacker_power=attacker_power,
            defender_power=defender_power,
            attacker_loss_ratio=attacker_loss,
            defender_loss_ratio=defender_loss,
            troop_losses=troop_losses,
            animal_kills=animal_kills,
            animal_remaining=animal_remaining,
            lost_troop_cost=lost_cost,
            hero_resources=resources,
            notes=[
                "Sonuç yerel simülatörde tahmin edilmiştir.",
                "Gerçek TTWars savaş raporlarıyla kalibrasyon yapılmalıdır.",
            ],
        )
