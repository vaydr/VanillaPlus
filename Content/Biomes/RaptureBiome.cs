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
        /// Use Hallow music as placeholder until custom music is created.
        /// </summary>
        public override int Music => MusicID.TheHallow;

        /// <summary>
        /// Custom bright gold water for the Rapture biome.
        /// </summary>
        public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("VanillaPlus/RaptureWaterStyle");

        /// <summary>
        /// Priority level - same as Hallow (BiomeMedium).
        /// Desert overlay takes higher priority.
        /// </summary>
        public override SceneEffectPriority Priority
        {
            get
            {
                if (Main.LocalPlayer.ZoneDesert && !Main.LocalPlayer.ZoneBeach)
                {
                    return SceneEffectPriority.BiomeHigh;
                }
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
    /// Underground Rapture biome variant.
    /// Uses different music/backgrounds when underground.
    /// </summary>
    public class RaptureUndergroundBiome : ModBiome
    {
        public override int Music => MusicID.UndergroundHallow; // Placeholder

        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeLow;

        public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("VanillaPlus/RaptureWaterStyle");

        public override string BestiaryIcon => "VanillaPlus/Content/Biomes/RaptureUndergroundBiomeIcon";

        public override string BackgroundPath => "VanillaPlus/Content/Biomes/RaptureUndergroundBiomeBackground";

        public override bool IsBiomeActive(Player player)
        {
            var tileCount = ModContent.GetInstance<RaptureTileCount>();
            if (tileCount == null)
                return false;

            // Underground Rapture - need tiles and be underground
            return tileCount.RaptureBlockCount >= RaptureTileCount.RaptureThreshold
                && (player.ZoneDirtLayerHeight || player.ZoneRockLayerHeight);
        }
    }
}
