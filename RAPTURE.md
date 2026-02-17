# Rapture Biome - Comprehensive Development Plan

## Overview

**Rapture** is an alternative to the Hallow biome with an **Angelic/Divine** theme. When the Wall of Flesh is defeated, the world randomly picks between Hallow OR Rapture for the V-stripe generation (similar to how worlds pick Corruption OR Crimson at creation).

- **Theme**: Angelic, divine, heavenly
- **Color Palette**: Gold, white, baby blue, soft yellows
- **Color Translation**: Pinks/blues → Whites/golds
- **Relationship to Hallow**: Same as Crimson to Corruption - mutually exclusive by default, but can be manually spread via Clentaminator

---

## Art Direction

### Sprite Strategy
**Re-use and recolor vanilla sprites wherever possible.** Custom sprites come later.

### Color Mapping (Hallow → Rapture)
| Hallow Color | Rapture Color |
|--------------|---------------|
| Pink | White |
| Blue | Gold |
| Purple | Baby Blue |
| Pastel Rainbow | White/Gold/Baby Blue |

### Temporary Placeholders
- Use Hallow backgrounds until custom ones are made
- Use Hallow music until custom music
- Recolor vanilla tile sprites for initial implementation

---

## Reference Implementation - CRITICAL

### **Confection REBAKED** (reference copy: `../ConfectionREBAKED_Reference/`)

> **NOTE**: The reference folder is located ONE LEVEL UP in `ModSources/ConfectionREBAKED_Reference/` to prevent it from being compiled with VanillaPlus.

This is THE reference implementation. The Confection mod does **almost exactly** what we want to do with Rapture. It is a complete, working Hallow alternative with:

- ✅ Hardmode hook that replaces Hallow generation
- ✅ V-stripe generation with custom tiles
- ✅ Spreading tile mechanics
- ✅ Biome detection and tile counting
- ✅ Enemies, items, souls
- ✅ Clentaminator integration
- ✅ Full multiplayer support

**Key files to study:**
- `../ConfectionREBAKED_Reference/Common/Systems/ConfectionWorldGeneration.cs` - Hardmode tasks, V-stripe generation
- `../ConfectionREBAKED_Reference/Biomes/` - ModBiome implementation
- `../ConfectionREBAKED_Reference/Tiles/` - Spreading tile patterns
- `../ConfectionREBAKED_Reference/NPCs/` - Biome enemy spawning

**License**: CC - source code can be freely used with or without credit.

**FOLLOW THIS PATTERN CLOSELY** - Don't reinvent the wheel. Adapt their proven approach for Rapture.

---

## Naming Convention

All Rapture equivalents follow this naming pattern:

| Hallow Name | Rapture Name |
|-------------|--------------|
| Pearlstone | **Blisite** |
| Pearlsand | **Blissite** |
| Hardened Pearlsand | Hardened Blissand |
| Pearlsandstone | Blissandstone |
| Hallowed Grass | **Blissgrass** (white) |
| Pearlwood | **Hedonwood** |
| Crystal Shard | **Divine Shard** |
| Blue Solution | **Golden Solution** |
| Soul of Light | **Soul of Divinity** |

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
        // V-stripe generation logic - follow Confection pattern
    }
}
```

### 1.2 Biome Definition (`Content/Biomes/RaptureBiome.cs`)

```csharp
public class RaptureBiome : ModBiome
{
    public override int Music => MusicID.Hallow; // PLACEHOLDER - use Hallow music for now
    public override SceneEffectPriority Priority => SceneEffectPriority.BiomeMedium;

    // PLACEHOLDER - use Hallow background for now
    public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => null; // Falls back to vanilla

    public override bool IsBiomeActive(Player player) {
        return ModContent.GetInstance<RaptureTileCount>().raptureCount >= 40;
    }
}
```

### 1.3 Tile Counting System (`Common/Systems/RaptureTileCount.cs`)

```csharp
public class RaptureTileCount : ModSystem
{
    public int raptureCount;

    public override void TileCountsAvailable(int[] tileCounts) {
        raptureCount = tileCounts[ModContent.TileType<Blissgrass>()]
                     + tileCounts[ModContent.TileType<Blisite>()]
                     + tileCounts[ModContent.TileType<Blissand>()]
                     + tileCounts[ModContent.TileType<BlissIce>()]
                     + tileCounts[ModContent.TileType<HardenedBlissand>()]
                     + tileCounts[ModContent.TileType<Blissandstone>()];
    }
}
```

---

## Phase 2: Tiles

### 2.1 Complete Tile List

| Rapture Tile | Vanilla Equivalent | Spreads | Sprite Source |
|--------------|-------------------|---------|---------------|
| **Blissgrass** | Hallowed Grass (109) | Yes | Recolor grass → white |
| **Blissite** | Pearlstone (117) | Yes | Recolor pearlstone |
| **Blissand** | Pearlsand (116) | Yes | Recolor pearlsand |
| **BlissIce** | Hallowed Ice (164) | Yes | Recolor hallowed ice |
| **HardenedBlissand** | Hardened Pearlsand (402) | Yes | Recolor |
| **Blissandstone** | Pearlsandstone (403) | Yes | Recolor |
| **Hedonwood** | Pearlwood (Tree) | No | White trunk, gold/blue leaves |
| **DivineShard** | Crystal Shard (129) | No | Yellow/white/blue variants |

### 2.2 Trees - Hedonwood

**Trunk**: White (recolor Pearlwood trunk)
**Leaves**: Randomly picks between:
- Gold leaves
- Baby blue leaves

Implementation: Custom tree tile with random leaf color selection on placement.

### 2.3 Crystal Shards - Divine Shards

Replace pink/blue crystal shards with:
| Hallow Shard | Divine Shard |
|--------------|--------------|
| Pink Crystal | **Yellow Divine Shard** |
| Blue Crystal | **White Divine Shard** |
| (new) | **Baby Blue Divine Shard** |

Shards grow on Blisstone in underground Rapture.

### 2.4 Tile Spreading

Each spreading tile needs `RandomUpdate()`:

```csharp
public override void RandomUpdate(int i, int j) {
    // Check 4 adjacent tiles
    // If adjacent tile is convertible (dirt, stone, sand, ice, etc.)
    // Convert to Rapture variant
    // Same spread rate as Hallow
}
```

### 2.5 Tile Sets Registration

```csharp
// Mark tiles as "Rapture" type (similar to TileID.Sets.Hallow)
// Create custom TileID.Sets.Rapture array
public static bool[] Rapture = TileID.Sets.Factory.CreateBoolSet(false,
    ModContent.TileType<Blissgrass>(),
    ModContent.TileType<Blisstone>(),
    ModContent.TileType<Blissand>(),
    // ... etc
);
```

---

## Phase 3: Clentaminator - Golden Solution

### 3.1 Golden Solution Item (`Content/Items/GoldenSolution.cs`)

| Property | Value |
|----------|-------|
| Ammo Type | AmmoID.Solution |
| Rarity | LightPurple |
| Value | Same as Blue Solution |
| Sold By | Steampunker (when in Rapture world) |

### 3.2 Solution Projectile

When sprayed:
- Converts Corruption/Crimson tiles → Rapture equivalents
- Converts Hallow tiles → Rapture equivalents
- Converts pure tiles → Rapture equivalents

### 3.3 NPC Shop Integration

```csharp
// GlobalNPC or Steampunker hook
if (RaptureWorldSystem.hasRapture) {
    // Sell Golden Solution instead of Blue Solution
    // OR sell both
}
```

---

## Phase 4: World Generation (V-Stripe)

### 4.1 V-Stripe Algorithm

Follow Confection's `ConfectionWorldGeneration.cs` pattern:

1. Find starting position (similar to Hallow)
2. Run diagonal stripe generation
3. Convert tiles along path:
   - Stone → Blisstone
   - Grass/Dirt → Blissgrass
   - Sand → Blissand
   - Ice → BlissIce
   - etc.

### 4.2 Conversion Mapping

```csharp
private static Dictionary<int, int> TileConversions = new() {
    { TileID.Stone, ModContent.TileType<Blisstone>() },
    { TileID.Grass, ModContent.TileType<Blissgrass>() },
    { TileID.Sand, ModContent.TileType<Blissand>() },
    { TileID.IceBlock, ModContent.TileType<BlissIce>() },
    { TileID.HardenedSand, ModContent.TileType<HardenedBlissand>() },
    { TileID.Sandstone, ModContent.TileType<Blissandstone>() },
    // Also convert Hallow tiles if encountered
    { TileID.Pearlstone, ModContent.TileType<Blisstone>() },
    { TileID.Pearlsand, ModContent.TileType<Blissand>() },
    { TileID.HallowedGrass, ModContent.TileType<Blissgrass>() },
    { TileID.HallowedIce, ModContent.TileType<BlissIce>() },
};
```

---

## Phase 5: Enemies

### 5.1 Surface Enemies

| Enemy | HP | Damage | Hallow Equivalent | Notes |
|-------|-----|--------|-------------------|-------|
| `Seraph` | 200 | 50 | Pixie | Flying angel, glows gold |
| `GoldenSlime` | 150 | 40 | Hallowed Slime | Divine slime |
| `Lightbringer` | 180 | 45 | Gastropod | Shoots golden projectiles |

### 5.2 Underground Enemies

| Enemy | HP | Damage | Hallow Equivalent | Notes |
|-------|-----|--------|-------------------|-------|
| `Cherub` | 300 | 60 | Illuminant | Illuminant equivalent |
| `Ascendant` | 250 | 55 | Chaos Elemental | Teleporting enemy, drops Rod of Ascension |

---

## Phase 6: Items & Materials

### 6.1 Souls

| Item | Drop Source | Notes |
|------|-------------|-------|
| **Soul of Divinity** | Underground Rapture enemies | Soul of Light equivalent |

### 6.2 Key Weapon

| Item | Drop Source | Notes |
|------|-------------|-------|
| **Rod of Ascension** | Ascendant (rare) | Rod of Discord equivalent |

### 6.3 Crafting Materials

| Item | Source | Notes |
|------|--------|-------|
| **Divine Shard** | Grows on Blisstone | Crystal Shard equivalent |
| **Hedonwood** | Trees | Pearlwood equivalent |

---

## Phase 7: Fishing & Crates

| Item | Type | Hallow Equivalent |
|------|------|-------------------|
| `BlissCrate` | Crate | Hallowed Crate |
| `DivineCrate` | Hardmode Crate | Divine Crate |
| `PrismFish` | Quest Fish | Prismite equivalent |

---

## Phase 8: Backgrounds & Visuals (LATER)

### Current: Use Hallow Placeholders
- Surface background: **Hallow background** (temporary)
- Underground background: **Hallow underground** (temporary)
- Music: **Hallow music** (temporary)
- Water: Default (temporary)

### Future Custom Assets (sprite later)
- Golden clouds, divine rays
- Glowing white/gold crystals underground
- Angelic choir music
- Golden-tinted water

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
│   │       ├── Blissgrass.cs          # White grass
│   │       ├── Blisstone.cs           # Pearlstone equivalent
│   │       ├── Blissand.cs            # Pearlsand equivalent
│   │       ├── BlissIce.cs            # Hallowed ice equivalent
│   │       ├── HardenedBlissand.cs
│   │       ├── Blissandstone.cs
│   │       ├── Hedonwood.cs           # Tree - white trunk, gold/blue leaves
│   │       └── DivineShard.cs         # Yellow/white/blue crystal shards
│   ├── NPCs/
│   │   └── Rapture/
│   │       ├── Seraph.cs              # Pixie equivalent
│   │       ├── GoldenSlime.cs
│   │       ├── Lightbringer.cs        # Gastropod equivalent
│   │       ├── Cherub.cs              # Illuminant equivalent
│   │       └── Ascendant.cs           # Chaos Elemental equivalent
│   └── Items/
│       └── Rapture/
│           ├── SoulOfDivinity.cs      # Soul of Light equivalent
│           ├── GoldenSolution.cs      # Clentaminator solution
│           ├── DivineShard.cs         # Crafting material
│           └── RodOfAscension.cs      # Rod of Discord equivalent
└── RAPTURE.md                          # This file
```

---

## Development Order

### Phase 1 - Core (DO FIRST)
1. `RaptureWorldSystem.cs` - World flag + hardmode hook
2. `RaptureBiome.cs` - Basic biome detection
3. `RaptureTileCount.cs` - Tile counting

### Phase 2 - Basic Tiles
4. `Blissgrass.cs` - White grass, spreading
5. `Blisstone.cs` - Stone, spreading
6. `Blissand.cs` - Sand, spreading

### Phase 3 - World Gen
7. V-stripe generation in `RaptureRunner`
8. Test WOF kill → Rapture stripe appears

### Phase 4 - More Tiles
9. `BlissIce.cs`, `HardenedBlissand.cs`, `Blissandstone.cs`
10. `Hedonwood.cs` - Trees
11. `DivineShard.cs` - Crystal shards

### Phase 5 - Clentaminator
12. `GoldenSolution.cs` - Item + projectile
13. Steampunker shop integration

### Phase 6 - Enemies
14. 2-3 surface enemies
15. 2 underground enemies

### Phase 7+ - Everything Else
16. Souls, items, weapons
17. Fishing, crates
18. Custom sprites, music, backgrounds

---

## Testing Checklist

- [ ] WOF kill generates Rapture V-stripe (not Hallow)
- [ ] Blissgrass/Blisstone/Blissand spread correctly
- [ ] Standing on 40+ Rapture tiles triggers biome
- [ ] Hallow music plays in Rapture (placeholder)
- [ ] Hedonwood trees generate with white trunk, random gold/blue leaves
- [ ] Divine Shards grow on Blisstone underground
- [ ] Golden Solution converts tiles to Rapture
- [ ] Steampunker sells Golden Solution in Rapture worlds
- [ ] Enemies spawn in Rapture biome
- [ ] Souls of Divinity drop from underground enemies
- [ ] World saves/loads Rapture state correctly
- [ ] Multiplayer sync works

---

## Notes

- **DEBUG MODE**: Currently hardcoded to always generate Rapture. Change to 50/50 when development is complete.
- **SPRITES**: Use recolored vanilla sprites initially. Custom sprites come later.
- **BACKGROUNDS**: Use Hallow backgrounds as placeholder.
- Study the local `../ConfectionREBAKED_Reference/` folder for all implementation patterns
- The Hallow uses tiles: 109 (grass), 117 (stone), 116 (sand), 164 (ice), 402 (hardened sand), 403 (sandstone)

---

## Future: World Creation UI

### Holy Biome Selection (like Evil Biome selection)

Just as vanilla Terraria allows choosing between **Corruption / Crimson / Random** during world creation, VanillaPlus will add a similar selection for the holy biome:

| Option | Behavior |
|--------|----------|
| **Random** | 50/50 chance of Hallow or Rapture on WOF kill |
| **Hallow** | Always generates Hallow (vanilla behavior) |
| **Rapture** | Always generates Rapture |

### Implementation

This requires hooking into the world creation UI:
- Add UI elements to world creation screen
- Store selection in world data
- Check selection in `ModifyHardmodeTasks` instead of random roll

### Reference
- Study how vanilla handles Corruption/Crimson selection
- May require IL editing or UI hooks
- Confection REBAKED may have implementation patterns for this

### Priority
**LOW** - Implement after core biome is working. Random 50/50 is sufficient for initial release.
