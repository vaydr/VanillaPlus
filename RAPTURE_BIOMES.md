# Rapture Biome System - Implementation Plan

## Overview
Rapture is an angelic/divine alternative to the Hallow biome. It should interact with other biomes (ice, desert, underground) the same way vanilla Hallow does.

---

## Vanilla Behavior Reference

### Music Priority (researched from Terraria Wiki & community)
The game checks biome conditions in order. First match wins:
1. **Evil biomes (Corruption/Crimson)** - highest priority
2. **Snow/Ice** - higher than Hallow
3. **Hallow** - mid priority
4. **Desert** - lower priority

**Result for overlapping biomes:**
| Combination | Music Plays |
|-------------|-------------|
| Hallow + Snow (surface) | Ice music |
| Hallow + Snow (underground) | Underground Ice music |
| Hallow + Desert (surface) | **Hallow music** (Hallow wins) |
| Hallow + Desert (underground) | **Underground Hallow music** |
| Underground Hallow | Underground Hallow music |

### Water Color Priority
- Hallow's pink water appears in ALL Hallow variants (ice, desert, underground)
- This is achieved via `SceneEffectPriority.BiomeHigh` when in overlay zones
- Without high priority, other biomes (ice, desert) would override the water style

---

## Current Implementation Issues

1. **Music is static** - Always plays `MusicID.TheHallow` regardless of zone
2. **Water being overridden** - Ice biome takes over water color in Rapture Ice areas
3. **No bestiary variants** - Missing IceRapture, DesertRapture biome entries

---

## Solution

### File: `Content/Biomes/RaptureBiome.cs`

#### 1. Smart Music Property
Replace:
```csharp
public override int Music => MusicID.TheHallow;
```
With:
```csharp
public override int Music
{
    get
    {
        bool isUnderground = (double)Main.LocalPlayer.position.Y >= Main.worldSurface * 16.0;

        // Snow takes priority over Hallow (vanilla behavior)
        if (Main.LocalPlayer.ZoneSnow)
            return isUnderground ? MusicID.UndergroundIce : MusicID.Ice;

        // Desert does NOT override Hallow - Hallow/Rapture music plays
        return isUnderground ? MusicID.UndergroundHallow : MusicID.TheHallow;
    }
}
```

#### 2. Dynamic Priority (for water style)
Update existing Priority property:
```csharp
public override SceneEffectPriority Priority
{
    get
    {
        // BiomeHigh ensures Rapture water style wins over ice/desert
        if (Main.LocalPlayer.ZoneSnow)
            return SceneEffectPriority.BiomeHigh;
        if (Main.LocalPlayer.ZoneDesert && !Main.LocalPlayer.ZoneBeach)
            return SceneEffectPriority.BiomeHigh;
        return SceneEffectPriority.BiomeMedium;
    }
}
```

#### 3. Add Variant Biome Classes
Following Confection's pattern, add lightweight classes for bestiary entries:

```csharp
/// <summary>
/// Ice Rapture surface variant for bestiary.
/// </summary>
public class IceRaptureSurfaceBiome : ModBiome
{
    public override string BestiaryIcon => "VanillaPlus/Content/Biomes/IceRaptureBiomeIcon";
    public override string BackgroundPath => "VanillaPlus/Content/Biomes/IceRaptureBiomeBackground";

    public override bool IsBiomeActive(Player player)
    {
        return RaptureBiome.InRaptureBiome(player)
            && player.ZoneSnow
            && player.ZoneOverworldHeight;
    }
}

/// <summary>
/// Ice Rapture underground variant for bestiary.
/// </summary>
public class IceRaptureUndergroundBiome : ModBiome
{
    public override string BestiaryIcon => "VanillaPlus/Content/Biomes/IceRaptureUndergroundBiomeIcon";
    public override string BackgroundPath => "VanillaPlus/Content/Biomes/IceRaptureUndergroundBiomeBackground";

    public override bool IsBiomeActive(Player player)
    {
        return RaptureBiome.InRaptureBiome(player)
            && player.ZoneSnow
            && (player.ZoneDirtLayerHeight || player.ZoneRockLayerHeight);
    }
}

/// <summary>
/// Desert Rapture surface variant for bestiary.
/// </summary>
public class DesertRaptureSurfaceBiome : ModBiome
{
    public override string BestiaryIcon => "VanillaPlus/Content/Biomes/DesertRaptureBiomeIcon";
    public override string BackgroundPath => "VanillaPlus/Content/Biomes/DesertRaptureBiomeBackground";

    public override bool IsBiomeActive(Player player)
    {
        return RaptureBiome.InRaptureBiome(player)
            && player.ZoneDesert
            && player.ZoneOverworldHeight;
    }
}

/// <summary>
/// Desert Rapture underground variant for bestiary.
/// </summary>
public class DesertRaptureUndergroundBiome : ModBiome
{
    public override string BestiaryIcon => "VanillaPlus/Content/Biomes/DesertRaptureUndergroundBiomeIcon";
    public override string BackgroundPath => "VanillaPlus/Content/Biomes/DesertRaptureUndergroundBiomeBackground";

    public override bool IsBiomeActive(Player player)
    {
        return RaptureBiome.InRaptureBiome(player)
            && player.ZoneDesert
            && (player.ZoneDirtLayerHeight || player.ZoneRockLayerHeight);
    }
}
```

---

## Complete Biome Class List

After implementation, Rapture will have 6 biome classes (like Confection):

| Class | Purpose |
|-------|---------|
| `RaptureBiome` | Main surface biome, handles music/water/priority |
| `RaptureUndergroundBiome` | Underground bestiary entry |
| `IceRaptureSurfaceBiome` | Ice surface bestiary entry |
| `IceRaptureUndergroundBiome` | Ice underground bestiary entry |
| `DesertRaptureSurfaceBiome` | Desert surface bestiary entry |
| `DesertRaptureUndergroundBiome` | Desert underground bestiary entry |

---

## Tile Counting (Already Implemented)

`RaptureTileCount.cs` already tracks:
- `RaptureBlockCount` - total Rapture tiles
- `SnowRaptureCount` - GoldenIce tiles (for ice variant)
- `DesertRaptureCount` - Blissand/Blissandstone/HardenedBlissand (for desert variant)

And adds to vanilla scene metrics:
- `Main.SceneMetrics.SnowTileCount += SnowRaptureCount`
- `Main.SceneMetrics.SandTileCount += DesertRaptureCount`

This ensures `ZoneSnow` and `ZoneDesert` activate properly in Rapture areas.

---

## Verification Checklist

After implementation, test:

| Location | Expected Music | Expected Water |
|----------|----------------|----------------|
| Rapture surface | Hallow | Rapture (gold/cream) |
| Rapture underground | Underground Hallow | Rapture |
| Rapture + Ice surface | **Ice** | Rapture |
| Rapture + Ice underground | **Underground Ice** | Rapture |
| Rapture + Desert surface | **Hallow** | Rapture |
| Rapture + Desert underground | **Underground Hallow** | Rapture |

---

## Assets Needed

For bestiary entries, create placeholder or actual icons:
- `Content/Biomes/IceRaptureBiomeIcon.png`
- `Content/Biomes/IceRaptureBiomeBackground.png`
- `Content/Biomes/IceRaptureUndergroundBiomeIcon.png`
- `Content/Biomes/IceRaptureUndergroundBiomeBackground.png`
- `Content/Biomes/DesertRaptureBiomeIcon.png`
- `Content/Biomes/DesertRaptureBiomeBackground.png`
- `Content/Biomes/DesertRaptureUndergroundBiomeIcon.png`
- `Content/Biomes/DesertRaptureUndergroundBiomeBackground.png`

---

## Reference: Confection's Implementation

Confection uses the same pattern:
- Main `ConfectionBiome` with smart Music/Priority
- 6 lightweight variant classes for bestiary
- `ConfectionBiomeTileCount` tracking variant-specific tiles
- Water style via `ModContent.Find<ModWaterStyle>("TheConfectionRebirth/CreamWaterStyle")`
