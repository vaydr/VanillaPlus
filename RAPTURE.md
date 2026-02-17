# Rapture Biome - Comprehensive Development Plan

## Overview

**Rapture** is an alternative to the Hallow biome with an **Angelic/Divine** theme. When the Wall of Flesh is defeated, the world randomly picks between Hallow OR Rapture for the V-stripe generation (similar to how worlds pick Corruption OR Crimson at creation).

- **Theme**: Angelic, divine, heavenly
- **Color Palette**: Gold, white, soft yellows, divine glow
- **Relationship to Hallow**: Same as Crimson to Corruption - mutually exclusive by default, but can be manually spread via Clentaminator

---

## Reference Implementation - CRITICAL

### **[Confection REBAKED](https://github.com/Lion8cake/ConfectionREBAKED)**

This is THE reference implementation. The Confection mod does **almost exactly** what we want to do with Rapture. It is a complete, working Hallow alternative with:

- ✅ Hardmode hook that replaces Hallow generation
- ✅ V-stripe generation with custom tiles
- ✅ Spreading tile mechanics
- ✅ Biome detection and tile counting
- ✅ Enemies, items, souls
- ✅ Clentaminator integration
- ✅ Full multiplayer support

**Key files to study:**
- `ConfectionWorldGeneration.cs` - Hardmode tasks, V-stripe generation
- `Biomes/` - ModBiome implementation
- `Tiles/` - Spreading tile patterns
- `NPCs/` - Biome enemy spawning

**License**: CC - source code can be freely used with or without credit.

**FOLLOW THIS PATTERN CLOSELY** - Don't reinvent the wheel. Adapt their proven approach for Rapture.

---

## Phase 1: Core Infrastructure

### 1.1 World System (`Common/Systems/RaptureWorldSystem.cs`)

```csharp
public class RaptureWorldSystem : ModSystem
{
    public static bool hasRapture; // Does this world have Rapture instead of Hallow?

    public override void SaveWorldData(TagCompound tag) {
        tag["hasRapture"] = hasRapture;
    }

    public override void LoadWorldData(TagCompound tag) {
        hasRapture = tag.GetBool("hasRapture");
    }

    public override void ModifyHardmodeTasks(List<GenPass> list) {
        int hallowIndex = list.FindIndex(g => g.Name.Equals("Hardmode Good"));
        if (hallowIndex != -1) {
            // DEBUG: Always Rapture (change to Main.rand.NextBool() for 50/50)
            hasRapture = true;

            list.Insert(hallowIndex + 1, new PassLegacy("Rapture", RaptureRunner));
            list.RemoveAt(hallowIndex);
        }
    }

    private void RaptureRunner(GenerationProgress progress, GameConfiguration config) {
        progress.Message = "Spreading the divine light...";
        // V-stripe generation logic
    }
}
```

### 1.2 Biome Definition (`Content/Biomes/RaptureBiome.cs`)

```csharp
public class RaptureBiome : ModBiome
{
    public override int Music => MusicID.Hallow; // Placeholder until custom music
    public override SceneEffectPriority Priority => SceneEffectPriority.BiomeMedium;

    public override bool IsBiomeActive(Player player) {
        return ModContent.GetInstance<RaptureTileCount>().raptureCount >= 40;
    }
}
```

### 1.3 Tile Counting System

```csharp
public class RaptureTileCount : ModSystem
{
    public int raptureCount;

    public override void TileCountsAvailable(int[] tileCounts) {
        raptureCount = tileCounts[ModContent.TileType<RaptureGrass>()]
                     + tileCounts[ModContent.TileType<RaptureStone>()]
                     + tileCounts[ModContent.TileType<RaptureSand>()];
    }
}
```

---

## Phase 2: Tiles

### 2.1 Core Tiles to Create

| Tile | Replaces | Spreads To | Notes |
|------|----------|------------|-------|
| `RaptureGrass` | Grass | Dirt, Mud | Golden/white grass |
| `RaptureStone` | Stone | Stone | Divine stone |
| `RaptureSand` | Sand | Sand | Glowing sand |
| `RaptureIce` | Ice | Ice | Crystalline ice |
| `RaptureSandstone` | Sandstone | Sandstone | - |
| `RaptureHardenedSand` | Hardened Sand | Hardened Sand | - |

### 2.2 Tile Spreading

Each tile needs `RandomUpdate()` to spread:

```csharp
public override void RandomUpdate(int i, int j) {
    // Check adjacent tiles
    // Convert convertible tiles to Rapture variants
    // Similar to Hallow spreading logic
}
```

### 2.3 Tile Conversion Table

Register conversions for Clentaminator and natural spread:

```csharp
// In ModSystem
TileLoader.RegisterConversion(TileID.Grass, ModContent.TileType<RaptureGrass>(), BiomeConversionType.Rapture);
TileLoader.RegisterConversion(TileID.Stone, ModContent.TileType<RaptureStone>(), BiomeConversionType.Rapture);
```

---

## Phase 3: World Generation (V-Stripe)

### 3.1 V-Stripe Algorithm

The V-stripe generation needs to:
1. Pick a starting X position near world center
2. Generate two diagonal lines from near-surface down to underworld
3. One line goes left-down, one goes right-down (or vice versa)
4. Convert all tiles along the path to Rapture variants
5. Spread outward from the line with decreasing intensity

### 3.2 Key WorldGen Methods

- `WorldGen.GERunner(int i, int j, double speedX, double speedY, int type, bool addTile)` - Core stripe generation
- `WorldGen.Convert(int i, int j, int type, int size)` - Convert tiles in an area
- Custom `RaptureRunner` - Wrapper that uses above with Rapture tiles

---

## Phase 4: Enemies

### 4.1 Surface Enemies

| Enemy | HP | Damage | Notes |
|-------|-----|--------|-------|
| `Seraph` | 200 | 50 | Flying angel enemy |
| `GoldenSlime` | 150 | 40 | Divine slime |
| `HeavenlyGastropod` | 180 | 45 | Pixie equivalent |

### 4.2 Underground Enemies

| Enemy | HP | Damage | Notes |
|-------|-----|--------|-------|
| `FallenAngel` | 300 | 60 | Illuminant equivalent |
| `DivineElemental` | 250 | 55 | Chaos Elemental equiv |

---

## Phase 5: Items & Materials

### 5.1 Souls

| Item | Drop Source | Use |
|------|-------------|-----|
| `SoulOfDivinity` | Underground Rapture enemies | Crafting (Soul of Light equiv) |

### 5.2 Weapons

| Weapon | Type | Damage | Notes |
|--------|------|--------|-------|
| `HolyBlade` | Melee | 60 | Glowing sword |
| `AngelicBow` | Ranged | 45 | Shoots homing arrows |
| `DivineScepter` | Magic | 50 | Shoots golden bolts |

### 5.3 Armor

| Set | Defense | Set Bonus |
|-----|---------|-----------|
| `Rapture Armor` | 35 total | +15% all damage, emit light |

---

## Phase 6: Fishing & Crates

| Item | Type | Contents |
|------|------|----------|
| `RaptureCrate` | Crate | Rapture materials, potions |
| `DivineCrate` | Hardmode Crate | Better materials |
| `GoldenKoi` | Fish | Crafting material |

---

## Phase 7: Misc Features

### 7.1 Backgrounds
- Surface background: Golden clouds, divine rays
- Underground background: Glowing crystals, divine architecture

### 7.2 Music
- Custom Rapture theme (peaceful, angelic choir)

### 7.3 Water Style
- Golden/white tinted water

### 7.4 Particles
- Floating golden sparkles
- Light rays

### 7.5 NPC Preferences
- Some NPCs prefer Rapture biome

---

## File Structure

```
VanillaPlus/
├── Common/
│   └── Systems/
│       ├── RaptureWorldSystem.cs      # World data + hardmode hook
│       └── RaptureTileCount.cs        # Tile counting
├── Content/
│   ├── Biomes/
│   │   └── RaptureBiome.cs            # ModBiome definition
│   ├── Tiles/
│   │   └── Rapture/
│   │       ├── RaptureGrass.cs
│   │       ├── RaptureStone.cs
│   │       ├── RaptureSand.cs
│   │       └── ...
│   ├── NPCs/
│   │   └── Rapture/
│   │       ├── Seraph.cs
│   │       ├── GoldenSlime.cs
│   │       └── ...
│   └── Items/
│       └── Rapture/
│           ├── SoulOfDivinity.cs
│           ├── Weapons/
│           └── Armor/
└── RAPTURE.md                          # This file
```

---

## Development Order

1. **Phase 1** - Core infrastructure (world system, biome, tile counting)
2. **Phase 2** - Basic tiles (grass, stone, sand) with spreading
3. **Phase 3** - V-stripe generation on WOF kill
4. **Phase 4** - 2-3 basic enemies
5. **Phase 5** - Soul drops and basic weapons
6. **Phase 6+** - Everything else (armor, fishing, polish)

---

## Testing Checklist

- [ ] WOF kill generates Rapture V-stripe (not Hallow)
- [ ] Rapture tiles spread to adjacent convertible tiles
- [ ] Standing on 40+ Rapture tiles triggers biome
- [ ] Biome music/background changes in Rapture
- [ ] Enemies spawn in Rapture biome
- [ ] Souls drop from underground Rapture enemies
- [ ] Clentaminator solutions work with Rapture
- [ ] World saves/loads Rapture state correctly
- [ ] Multiplayer sync works

---

## Notes

- **DEBUG MODE**: Currently hardcoded to always generate Rapture. Change to `Main.rand.NextBool()` for 50/50 when development is complete.
- Study [Confection REBAKED source](https://github.com/Lion8cake/ConfectionREBAKED) for implementation patterns
- The Hallow uses tiles: 109 (grass), 117 (stone), 116 (sand), 164 (ice), etc.
