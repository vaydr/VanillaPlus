# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

VanillaPlus is a Terraria tModLoader mod that adds the **Rapture** biome - an angelic/divine alternative to the Hallow. When the Wall of Flesh is defeated, worlds randomly choose between Hallow OR Rapture for the V-stripe generation (similar to Corruption vs Crimson). It also adds many other smaller Vanilla-like features.

**Requirements**: .NET 8 or higher

## Build Commands

```bash
# Build the mod (run from ModSources directory or use tModLoader's in-game build)
dotnet build VanillaPlus.csproj

# tModLoader typically handles building via the in-game Mod Sources menu
# Launch Terraria with tModLoader, go to Workshop > Develop Mods > Build
```

## Architecture

### Directory Structure

- **Common/** - Core systems and infrastructure
  - `Systems/` - World systems (`RaptureWorldSystem.cs`, `RaptureTileCount.cs`)
  - `UI/` - IL hooks for world creation menu integration
  - `RaptureIDs.cs` - Central tile/wall set registry for biome detection
  - `RaptureGlobalTile.cs` - Global tile hooks for conversions

- **Content/** - Game content organized by type
  - `Biomes/` - ModBiome definitions with surface/underground/ice/desert variants
  - `Tiles/Rapture/` - Custom tiles (Blissgrass, Blisstone, Blissand, etc.)
  - `Walls/Rapture/` - Corresponding wall variants
  - `Items/`, `Buffs/`, `NPCs/`, `Projectiles/`, `Players/`

- **Assets/** - UI icons and visual resources
- **Localization/** - String translations (`en-US_Mods.VanillaPlus.hjson`)

### Key Systems

**Biome Generation Pipeline:**
1. `RaptureWorldSystem.OnModifyHardmodeTasks()` hooks into hardmode generation
2. `RaptureGERunner()` generates V-stripe by converting vanilla tiles to Rapture equivalents
3. Uses `RaptureIDs.cs` tile sets for efficient bulk conversions

**Biome Detection:**
1. `RaptureTileCount` tracks nearby Rapture tiles via `TileCountsAvailable()`
2. `RaptureBiome.IsBiomeActive()` checks threshold (125+ tiles)
3. Triggers music/water/background switching

**World Creation UI:**
- `RaptureSelectionMenu.cs` uses IL manipulation to inject biome selection buttons
- `RaptureWorldIconEdit.cs` modifies world list icons for Rapture worlds

### Reference Implementation

The **Confection REBAKED** mod (located at `../ConfectionREBAKED_Reference/`) serves as the primary reference for implementation patterns. It's a working Hallow alternative that demonstrates:
- Hardmode hooks and V-stripe generation
- Spreading tile mechanics
- Biome detection and tile counting
- Clentaminator integration

The reference folder is excluded from compilation via the .csproj file.

### Naming Conventions

Rapture tiles follow thematic naming:
| Hallow | Rapture |
|--------|---------|
| Pearlstone | Blisstone |
| Pearlsand | Blissand |
| Hallowed Grass | Blissgrass |
| Pearlwood | Hedonwood |
| Hallowed Ice | GoldenIce |
| Soul of Light | Soul of Divinity |

### Color Palette

- **Gold, white, baby blue, soft yellows** (angelic/divine theme)
- Pink → White, Blue → Gold, Purple → Baby Blue

## Key Files

- `VanillaPlus.cs` - Main mod class, loads IL hooks
- `Common/Systems/RaptureWorldSystem.cs` - World generation, save/load, multiplayer sync
- `Common/RaptureIDs.cs` - Tile/wall set definitions for biome mechanics
- `Content/Biomes/RaptureBiome.cs` - Main biome with music/water/priority logic
- `RAPTURE.md` - Comprehensive development plan with implementation details
- `RAPTURE_BIOMES.md` - Biome variant system documentation

## Reference Implementations

Look in ../Confection_REBAKED and ../Calamity_Reference every time you're asked to make a new feature; these mods correctly implement things and should be used for patterns.
