using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Content.Tiles.Rapture;

namespace VanillaPlus.Common
{
    /// <summary>
    /// Helper methods for Rapture stalactite/stalagmite placement and validation.
    /// Follows the same pattern as Confection's ConfectionWorldGeneration stalactite code.
    ///
    /// Frame layout (each cell 18x18):
    ///   FrameY 0:  Large stalactite top (hangs from ceiling)
    ///   FrameY 18: Large stalactite bottom
    ///   FrameY 36: Large stalagmite top (grows from floor)
    ///   FrameY 54: Large stalagmite bottom
    ///   FrameY 72: Small stalactite (single tile, ceiling)
    ///   FrameY 90: Small stalagmite (single tile, floor)
    ///
    ///   FrameX 0/18/36: Three visual variants
    /// </summary>
    public static class RaptureStalactiteHelper
    {
        // Stalactite style IDs
        private const int STYLE_ICE = 0;
        private const int STYLE_STONE = 1;
        private const int STYLE_VANILLA = 2;

        /// <summary>
        /// Place a stalactite/stalagmite at the given position, checking for liquid.
        /// </summary>
        public static void PlaceTight(int x, int y)
        {
            if (Main.tile[x, y].LiquidType == LiquidID.Shimmer)
                return;

            PlaceUncheckedStalactite(x, y, WorldGen.genRand.NextBool(2), WorldGen.genRand.Next(3));

            if (Main.tile[x, y].TileType == ModContent.TileType<GoldenIceStalactite>() ||
                Main.tile[x, y].TileType == ModContent.TileType<BlisstoneStalactite>())
            {
                CheckTight(x, y);
            }
        }

        /// <summary>
        /// Place a stalactite without checking liquid. Determines type based on adjacent solid tile.
        /// </summary>
        public static void PlaceUncheckedStalactite(int x, int y, bool preferSmall, int variation)
        {
            ushort type;
            variation = Utils.Clamp(variation, 0, 2);

            // Try stalactite (hanging from ceiling - solid tile above)
            if (WorldGen.SolidTile(x, y - 1) && !Main.tile[x, y].HasTile && !Main.tile[x, y + 1].HasTile)
            {
                // GoldenIce stalactite
                if (Main.tile[x, y - 1].TileType == ModContent.TileType<GoldenIce>())
                {
                    type = (ushort)ModContent.TileType<GoldenIceStalactite>();
                    if (preferSmall)
                    {
                        int frameX = variation * 18;
                        Tile tile = Main.tile[x, y];
                        tile.TileType = type;
                        tile.HasTile = true;
                        tile.TileFrameX = (short)frameX;
                        tile.TileFrameY = 72;
                    }
                    else
                    {
                        int frameX = variation * 18;
                        Tile tile = Main.tile[x, y];
                        tile.TileType = type;
                        tile.HasTile = true;
                        tile.TileFrameX = (short)frameX;
                        tile.TileFrameY = 0;
                        Tile tile2 = Main.tile[x, y + 1];
                        tile2.TileType = type;
                        tile2.HasTile = true;
                        tile2.TileFrameX = (short)frameX;
                        tile2.TileFrameY = 18;
                    }
                }

                // Blisstone/Blissandstone/HardenedBlissand stalactite
                if (Main.tile[x, y - 1].TileType == ModContent.TileType<Blisstone>() ||
                    Main.tile[x, y - 1].TileType == ModContent.TileType<Blissandstone>() ||
                    Main.tile[x, y - 1].TileType == ModContent.TileType<HardenedBlissand>())
                {
                    type = (ushort)ModContent.TileType<BlisstoneStalactite>();
                    if (preferSmall)
                    {
                        int frameX = variation * 18;
                        Tile tile = Main.tile[x, y];
                        tile.TileType = type;
                        tile.HasTile = true;
                        tile.TileFrameX = (short)frameX;
                        tile.TileFrameY = 72;
                    }
                    else
                    {
                        int frameX = variation * 18;
                        Tile tile = Main.tile[x, y];
                        tile.TileType = type;
                        tile.HasTile = true;
                        tile.TileFrameX = (short)frameX;
                        tile.TileFrameY = 0;
                        Tile tile2 = Main.tile[x, y + 1];
                        tile2.TileType = type;
                        tile2.HasTile = true;
                        tile2.TileFrameX = (short)frameX;
                        tile2.TileFrameY = 18;
                    }
                }
            }
            // Try stalagmite (growing from floor - solid tile below)
            else if (WorldGen.SolidTile(x, y + 1) && !Main.tile[x, y].HasTile && !Main.tile[x, y - 1].HasTile)
            {
                // Blisstone/Blissandstone/HardenedBlissand stalagmite
                if (Main.tile[x, y + 1].TileType == ModContent.TileType<Blisstone>() ||
                    Main.tile[x, y + 1].TileType == ModContent.TileType<Blissandstone>() ||
                    Main.tile[x, y + 1].TileType == ModContent.TileType<HardenedBlissand>())
                {
                    type = (ushort)ModContent.TileType<BlisstoneStalactite>();
                    if (preferSmall)
                    {
                        int frameX = variation * 18;
                        Tile tile = Main.tile[x, y];
                        tile.TileType = type;
                        tile.HasTile = true;
                        tile.TileFrameX = (short)frameX;
                        tile.TileFrameY = 90;
                    }
                    else
                    {
                        int frameX = variation * 18;
                        Tile tile = Main.tile[x, y - 1];
                        tile.TileType = type;
                        tile.HasTile = true;
                        tile.TileFrameX = (short)frameX;
                        tile.TileFrameY = 36;
                        Tile tile2 = Main.tile[x, y];
                        tile2.TileType = type;
                        tile2.HasTile = true;
                        tile2.TileFrameX = (short)frameX;
                        tile2.TileFrameY = 54;
                    }
                }
            }
        }

        /// <summary>
        /// Validate stalactite attachment. Destroys tiles that lost their support.
        /// </summary>
        public static void CheckTight(int x, int j)
        {
            if (Main.tile[x, j] == null)
                return;

            int num = j;

            // Small stalactite (single tile, hanging from ceiling)
            if (Main.tile[x, num].TileFrameY == 72)
            {
                bool destroy = false;
                if (!WorldGen.SolidTile(x, num - 1))
                    destroy = true;

                if (!destroy && !UpdateStalactiteStyle(x, num))
                    destroy = true;

                if (destroy)
                {
                    WorldGen.destroyObject = true;
                    if (Main.tile[x, num].TileType == Main.tile[x, j].TileType)
                        WorldGen.KillTile(x, num);
                    WorldGen.destroyObject = false;
                }
                return;
            }

            // Small stalagmite (single tile, growing from floor)
            if (Main.tile[x, num].TileFrameY == 90)
            {
                bool destroy = false;
                if (!WorldGen.SolidTile(x, num + 1))
                    destroy = true;

                if (!destroy && !UpdateStalactiteStyle(x, num))
                    destroy = true;

                if (destroy)
                {
                    WorldGen.destroyObject = true;
                    if (Main.tile[x, num].TileType == Main.tile[x, j].TileType)
                        WorldGen.KillTile(x, num);
                    WorldGen.destroyObject = false;
                }
                return;
            }

            // Large stalagmite (two tiles, growing from floor)
            if (Main.tile[x, num].TileFrameY >= 36)
            {
                if (Main.tile[x, num].TileFrameY == 54)
                    num--;

                bool destroy = false;
                if (!WorldGen.SolidTile(x, num + 2))
                    destroy = true;
                if (Main.tile[x, num + 1].TileType != Main.tile[x, num].TileType)
                    destroy = true;
                if (Main.tile[x, num + 1].TileFrameX != Main.tile[x, num].TileFrameX)
                    destroy = true;

                if (!destroy && !UpdateStalactiteStyle(x, num))
                    destroy = true;

                if (destroy)
                {
                    WorldGen.destroyObject = true;
                    if (Main.tile[x, num].TileType == Main.tile[x, j].TileType)
                        WorldGen.KillTile(x, num);
                    if (Main.tile[x, num + 1].TileType == Main.tile[x, j].TileType)
                        WorldGen.KillTile(x, num + 1);
                    WorldGen.destroyObject = false;
                }
                return;
            }

            // Large stalactite (two tiles, hanging from ceiling)
            if (Main.tile[x, num].TileFrameY == 18)
                num--;

            {
                bool destroy = false;
                if (!WorldGen.SolidTile(x, num - 1))
                    destroy = true;
                if (Main.tile[x, num + 1].TileType != Main.tile[x, num].TileType)
                    destroy = true;
                if (Main.tile[x, num + 1].TileFrameX != Main.tile[x, num].TileFrameX)
                    destroy = true;

                if (!destroy && !UpdateStalactiteStyle(x, num))
                    destroy = true;

                if (destroy)
                {
                    WorldGen.destroyObject = true;
                    if (Main.tile[x, num].TileType == Main.tile[x, j].TileType)
                        WorldGen.KillTile(x, num);
                    if (Main.tile[x, num + 1].TileType == Main.tile[x, j].TileType)
                        WorldGen.KillTile(x, num + 1);
                    WorldGen.destroyObject = false;
                }
            }
        }

        /// <summary>
        /// Update stalactite tile type to match the parent block it's attached to.
        /// If the parent changed (e.g., from spreading), the stalactite converts.
        /// </summary>
        public static bool UpdateStalactiteStyle(int x, int j)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return true;

            if (Main.tile[x, j] == null)
                return true;

            GetStalactiteStyle(x, j, out int currentStyle, out bool fail);
            if (fail)
                return false;

            GetDesiredStalactiteStyle(x, j, out bool fail2, out int desiredStyle, out int height, out int y);
            if (fail2)
                return false;

            if (currentStyle != desiredStyle)
            {
                int frameX = WorldGen.genRand.Next(3) * 18;
                ushort newType;
                if (desiredStyle == STYLE_ICE)
                    newType = (ushort)ModContent.TileType<GoldenIceStalactite>();
                else if (desiredStyle == STYLE_STONE)
                    newType = (ushort)ModContent.TileType<BlisstoneStalactite>();
                else
                    newType = TileID.Stalactite;

                for (int i = y; i < y + height; i++)
                {
                    Main.tile[x, i].TileFrameX = (short)frameX;
                    Main.tile[x, i].TileType = newType;
                }

                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendTileSquare(-1, x, y, 1, 2);
            }

            return true;
        }

        private static void GetStalactiteStyle(int x, int y, out int type, out bool fail)
        {
            type = 0;
            fail = false;

            if (Main.tile[x, y].TileType == ModContent.TileType<GoldenIceStalactite>())
                type = STYLE_ICE;
            else if (Main.tile[x, y].TileType == ModContent.TileType<BlisstoneStalactite>())
                type = STYLE_STONE;
            else if (Main.tile[x, y].TileType == TileID.Stalactite)
                type = STYLE_VANILLA;
            else
                fail = true;
        }

        private static void GetDesiredStalactiteStyle(int x, int j, out bool fail, out int desiredStyle, out int height, out int y)
        {
            fail = false;
            desiredStyle = 0;
            height = 1;
            y = j;

            // Determine parent tile based on frame orientation
            int parentTileType;
            if (Main.tile[x, y].TileFrameY == 72)
            {
                // Small stalactite - parent is above
                parentTileType = Main.tile[x, y - 1].TileType;
            }
            else if (Main.tile[x, y].TileFrameY == 90)
            {
                // Small stalagmite - parent is below
                parentTileType = Main.tile[x, y + 1].TileType;
            }
            else if (Main.tile[x, y].TileFrameY >= 36)
            {
                // Large stalagmite - parent is below bottom tile
                if (Main.tile[x, y].TileFrameY == 54)
                    y--;
                height = 2;
                parentTileType = Main.tile[x, y + 2].TileType;
            }
            else
            {
                // Large stalactite - parent is above top tile
                if (Main.tile[x, y].TileFrameY == 18)
                    y--;
                height = 2;
                parentTileType = Main.tile[x, y - 1].TileType;
            }

            // Determine desired style from parent tile type
            if (parentTileType == ModContent.TileType<Blisstone>() ||
                parentTileType == ModContent.TileType<Blissandstone>() ||
                parentTileType == ModContent.TileType<HardenedBlissand>())
            {
                desiredStyle = STYLE_STONE;
            }
            else if (parentTileType == ModContent.TileType<GoldenIce>())
            {
                desiredStyle = STYLE_ICE;
            }
            else
            {
                // Not a Rapture parent tile - fall back to vanilla
                desiredStyle = STYLE_VANILLA;
            }
        }
    }
}
