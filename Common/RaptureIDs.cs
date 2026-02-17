using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaPlus.Common
{
    /// <summary>
    /// Contains tile/wall sets and IDs for the Rapture biome.
    /// Follows the same pattern as Confection's ConfectionIDs.
    /// </summary>
    public class RaptureIDs
    {
        public static class Sets
        {
            /// <summary>
            /// Whether a tile is a Rapture biome tile (used for biome detection and wall generation).
            /// </summary>
            public static bool[] Rapture = TileID.Sets.Factory.CreateNamedSet("Rapture")
                .Description("Whether a tile is a Rapture biome tile")
                .RegisterBoolSet();

            /// <summary>
            /// Whether a tile is a natural Rapture tile (stone, grass, sand, ice equivalents).
            /// </summary>
            public static bool[] IsNaturalRaptureTile = TileID.Sets.Factory.CreateNamedSet("IsNaturalRaptureTile")
                .Description("Whether a tile is a natural Rapture tile like Blisite, Blissgrass, etc.")
                .RegisterBoolSet();

            /// <summary>
            /// Whether a tile can grow Divine Shards (crystal shards equivalent).
            /// </summary>
            public static bool[] CanGrowDivineShard = TileID.Sets.Factory.CreateNamedSet("CanGrowDivineShard")
                .Description("Whether a tile can grow Divine Shards")
                .RegisterBoolSet();

            /// <summary>
            /// Whether a tile has Rapture biome sight coloring.
            /// </summary>
            public static bool[] RaptureBiomeSight = TileID.Sets.Factory.CreateNamedSet("RaptureBiomeSight")
                .Description("Whether a tile has Rapture biome sight coloring")
                .RegisterBoolSet();

            /// <summary>
            /// Used to add extra tiles to Rapture worldgen conversion.
            /// Set to the tile type that it should convert TO.
            /// </summary>
            public static int[] ConvertsToRapture = TileID.Sets.Factory.CreateNamedSet("ConvertsToRapture")
                .Description("Used to add extra tiles to Rapture worldgen conversion")
                .RegisterIntSet();

            /// <summary>
            /// Whether a wall is a natural Rapture wall.
            /// </summary>
            public static bool[] IsNaturalRaptureWall = WallID.Sets.Factory.CreateNamedSet("IsNaturalRaptureWall")
                .Description("Whether a wall is a natural Rapture wall")
                .RegisterBoolSet();

            /// <summary>
            /// Whether a wall is a Rapture biome wall (used for biome detection).
            /// </summary>
            public static bool[] RaptureWall = WallID.Sets.Factory.CreateNamedSet("RaptureWall")
                .Description("Whether a wall is a Rapture biome wall")
                .RegisterBoolSet();
        }
    }
}
