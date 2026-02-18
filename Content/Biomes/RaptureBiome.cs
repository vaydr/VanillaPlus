using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Common.Systems;

namespace VanillaPlus.Content.Biomes
{
    /// <summary>
    /// The Rapture biome - an angelic/divine alternative to the Hallow.
    /// Activated when player is near enough Rapture tiles.
    /// </summary>
    public class RaptureBiome : ModBiome
    {
        /// <summary>
        /// Smart music selection based on zone and depth.
        /// Matches vanilla Hallow behavior: Snow > Hallow > Desert.
        /// </summary>
        public override int Music
        {
            get
            {
                bool isUnderground = (double)Main.LocalPlayer.position.Y >= Main.worldSurface * 16.0;

                // Snow takes priority over Hallow (vanilla behavior)
                // MusicID.Snow = surface snow, MusicID.Ice = underground snow
                if (Main.LocalPlayer.ZoneSnow)
                    return isUnderground ? MusicID.Ice : MusicID.Snow;

                // Desert does NOT override Hallow - Hallow/Rapture music plays
                return isUnderground ? MusicID.UndergroundHallow : MusicID.TheHallow;
            }
        }

        /// <summary>
        /// Custom bright gold water for the Rapture biome.
        /// </summary>
        public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("VanillaPlus/RaptureWaterStyle");

        /// <summary>
        /// Priority level - BiomeHigh for snow/desert overlays to ensure water style wins.
        /// </summary>
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

        /// <summary>
        /// Use Hallow background as placeholder until custom backgrounds are created.
        /// Returns null to fall back to vanilla handling.
        /// </summary>
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => null;

        /// <summary>
        /// Map background path - use Hallow's for now.
        /// </summary>
        public override string MapBackground => "Terraria/Images/MapBG1";

        /// <summary>
        /// Bestiary background icon path.
        /// </summary>
        public override string BestiaryIcon => "VanillaPlus/Content/Biomes/RaptureBiomeIcon";

        /// <summary>
        /// Background path for the biome (used in bestiary, etc.).
        /// </summary>
        public override string BackgroundPath => "VanillaPlus/Content/Biomes/RaptureBiomeBackground";

        /// <summary>
        /// Check if the Rapture biome should be active for this player.
        /// Requires at least 125 Rapture tiles nearby and the player to be in a valid zone.
        /// </summary>
        public override bool IsBiomeActive(Player player)
        {
            return InRaptureBiome(player);
        }

        /// <summary>
        /// Static method to check if player is in Rapture biome.
        /// Can be called from anywhere without needing a ModBiome instance.
        /// </summary>
        public static bool InRaptureBiome(Player player)
        {
            var tileCount = ModContent.GetInstance<RaptureTileCount>();
            if (tileCount == null)
                return false;

            // Need at least 125 tiles and be in a valid zone
            return tileCount.RaptureBlockCount >= RaptureTileCount.RaptureThreshold
                && (player.ZoneOverworldHeight || player.ZoneDirtLayerHeight ||
                    player.ZoneRockLayerHeight || player.ZoneSkyHeight);
        }
    }

    /// <summary>
    /// Underground Rapture biome variant for bestiary.
    /// </summary>
    public class RaptureUndergroundBiome : ModBiome
    {
        public override string BestiaryIcon => "VanillaPlus/Content/Biomes/RaptureUndergroundBiomeIcon";
        public override string BackgroundPath => "VanillaPlus/Content/Biomes/RaptureUndergroundBiomeBackground";

        public override bool IsBiomeActive(Player player)
        {
            return RaptureBiome.InRaptureBiome(player)
                && !player.ZoneSnow
                && !player.ZoneDesert
                && (player.ZoneDirtLayerHeight || player.ZoneRockLayerHeight);
        }
    }

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
}
