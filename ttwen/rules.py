from .models import AnimalProfile, TroopProfile

ANIMALS = {
    "rat": AnimalProfile("rat", "Fare", 10, 25, 20, 1),
    "spider": AnimalProfile("spider", "Örümcek", 20, 35, 40, 1),
    "snake": AnimalProfile("snake", "Yılan", 60, 40, 60, 1),
    "bat": AnimalProfile("bat", "Yarasa", 80, 66, 50, 1),
    "boar": AnimalProfile("boar", "Yaban Domuzu", 50, 70, 33, 2),
    "wolf": AnimalProfile("wolf", "Kurt", 100, 80, 70, 2),
    "bear": AnimalProfile("bear", "Ayı", 250, 140, 200, 3),
    "crocodile": AnimalProfile("crocodile", "Timsah", 450, 380, 240, 3),
    "tiger": AnimalProfile("tiger", "Kaplan", 200, 170, 250, 3),
    "elephant": AnimalProfile("elephant", "Fil", 600, 440, 520, 5),
}

# Common baseline troop data. TTWars custom values can replace these.
TROOPS = {
    "club": TroopProfile("club", "Topuzlu", 40, 20, 5, 1, 250, False),
    "spear": TroopProfile("spear", "Mızrakçı", 10, 35, 60, 1, 340, False),
    "sword": TroopProfile("sword", "Kılıçlı", 65, 35, 20, 1, 535, False),
    "legionnaire": TroopProfile("legionnaire", "Lejyoner", 40, 35, 50, 1, 320, False),
    "imperian": TroopProfile("imperian", "Imperian", 70, 40, 25, 1, 650, False),
    "paladin": TroopProfile("paladin", "Paladin", 55, 65, 50, 2, 810, True),
    "ec_imperatoris": TroopProfile("ec_imperatoris", "Equites Imperatoris", 120, 65, 50, 2, 1190, True),
}

RESOURCE_NAMES = ("wood", "clay", "iron", "crop")
