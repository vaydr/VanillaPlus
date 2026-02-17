using System;
using Terraria;
using Terraria.ModLoader;
using VanillaPlus.Content.Tiles.Rapture;

namespace VanillaPlus.Common.Systems
{
    /// <summary>
    /// Counts Rapture tiles for biome detection.
    /// Similar to how vanilla counts Hallow tiles for the Hallow biome.
    /// </summary>
    public class RaptureTileCount : ModSystem
    {
        /// <summary>
        /// Total count of Rapture tiles near the player.
        /// Used for biome detection threshold (typically 125 for biome activation).
        /// </summary>
        public int RaptureBlockCount { get; private set; }

        /// <summary>
        /// Snow-type Rapture tiles (for snow biome overlay).
        /// </summary>
        public int SnowRaptureCount { get; private set; }

        /// <summary>
        /// Desert-type Rapture tiles (for desert biome overlay).
        /// </summary>
        public int DesertRaptureCount { get; private set; }

        /// <summary>
        /// Threshold for the Rapture biome to be active.
        /// Same as Hallow's threshold of 125.
        /// </summary>
        public const int RaptureThreshold = 125;

        public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
        {
            // Count ice-type tiles for snow biome overlay
            SnowRaptureCount = tileCounts[ModContent.TileType<GoldenIce>()];

            // Count desert-type tiles for desert biome overlay
            DesertRaptureCount = tileCounts[ModContent.TileType<Blissand>()]
                + tileCounts[ModContent.TileType<Blissandstone>()]
                + tileCounts[ModContent.TileType<HardenedBlissand>()];

            // Total Rapture tile count for biome detection
            RaptureBlockCount = tileCounts[ModContent.TileType<Blisstone>()]
                + tileCounts[ModContent.TileType<Blissgrass>()]
                + tileCounts[ModContent.TileType<Blissand>()]
                + tileCounts[ModContent.TileType<GoldenIce>()]
                + tileCounts[ModContent.TileType<HardenedBlissand>()]
                + tileCounts[ModContent.TileType<Blissandstone>()];

            // Rapture tiles counter evil/blood tile counts (same behavior as Hallow)
            // This prevents corruption/crimson music from playing in Rapture
            Main.SceneMetrics.EvilTileCount -= RaptureBlockCount;
            if (Main.SceneMetrics.EvilTileCount < 0)
                Main.SceneMetrics.EvilTileCount = 0;

            Main.SceneMetrics.BloodTileCount -= RaptureBlockCount;
            if (Main.SceneMetrics.BloodTileCount < 0)
                Main.SceneMetrics.BloodTileCount = 0;

            // Add to snow/sand counts for sub-biome detection
            Main.SceneMetrics.SnowTileCount += SnowRaptureCount;
            Main.SceneMetrics.SandTileCount += DesertRaptureCount;
        }
    }
}
