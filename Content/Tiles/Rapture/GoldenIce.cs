using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Common;

namespace VanillaPlus.Content.Tiles.Rapture
{
    /// <summary>
    /// GoldenIce - the Rapture equivalent of Hallowed Ice.
    /// Divine golden-tinted ice that spreads Rapture in snow biomes.
    /// </summary>
    public class GoldenIce : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileShine2[Type] = true;
            Main.tileBrick[Type] = true;
            Main.tileBlockLight[Type] = true;

            // Mark as ice type for conversions
            TileID.Sets.Conversion.Ice[Type] = true;
            TileID.Sets.IceSkateSlippery[Type] = true;
            TileID.Sets.Ices[Type] = true;
            TileID.Sets.IcesSlush[Type] = true;
            TileID.Sets.IcesSnow[Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = true;

            // Rapture-specific sets
            RaptureIDs.Sets.CanGrowRadiantShard[Type] = true;
            RaptureIDs.Sets.RaptureBiomeSight[Type] = true;
            RaptureIDs.Sets.Rapture[Type] = true;
            RaptureIDs.Sets.IsNaturalRaptureTile[Type] = true;

            // Merge with other ice types, Rapture tiles, and stalactites
            Main.tileMerge[Type][ModContent.TileType<Blisstone>()] = true;
            Main.tileMerge[Type][ModContent.TileType<GoldenIceStalactite>()] = true;
            Main.tileMerge[Type][ModContent.TileType<BlisstoneStalactite>()] = true;
            Main.tileMerge[Type][TileID.IceBlock] = true;
            Main.tileMerge[Type][TileID.SnowBlock] = true;
            Main.tileMerge[Type][TileID.FleshIce] = true;
            Main.tileMerge[Type][TileID.CorruptIce] = true;
            Main.tileMerge[Type][TileID.HallowedIce] = true;

            // Reverse merging
            Main.tileMerge[TileID.IceBlock][Type] = true;
            Main.tileMerge[TileID.SnowBlock][Type] = true;
            Main.tileMerge[TileID.FleshIce][Type] = true;
            Main.tileMerge[TileID.CorruptIce][Type] = true;
            Main.tileMerge[TileID.HallowedIce][Type] = true;

            // Color: Golden-white ice
            AddMapEntry(new Color(240, 225, 190));

            HitSound = SoundID.Item50; // Ice break sound
            DustType = DustID.Ice;

            // Drop the GoldenIce item
            RegisterItemDrop(ModContent.ItemType<Items.Rapture.GoldenIce>());
        }

        public override void RandomUpdate(int i, int j)
        {
            if (!WorldGen.AllowedToSpreadInfections)
                return;

            SpreadRapture(i, j);
            GenerateStalactites(i, j);
        }

        /// <summary>
        /// Generate GoldenIce stalactites below this ice block.
        /// Follows the same pattern as Confection's BlueIce.RandomUpdate.
        /// </summary>
        private void GenerateStalactites(int i, int j)
        {
            if (!Main.tile[i, j].HasUnactuatedTile)
                return;

            if (!Main.rand.NextBool(10))
                return;

            if (Main.tile[i, j + 1].HasTile || Main.tile[i, j + 2].HasTile)
                return;

            // Count existing stalactites in a 7-wide window to prevent overcrowding
            int count = 0;
            for (int x = i - 3; x < i + 4; x++)
            {
                for (int dy = 0; dy <= 3; dy++)
                {
                    if (WorldGen.InWorld(x, j + dy, 1) &&
                        Main.tile[x, j + dy].TileType == ModContent.TileType<GoldenIceStalactite>() &&
                        Main.tile[x, j + dy].HasTile)
                    {
                        count++;
                    }
                }
            }

            if (count < 2)
            {
                RaptureStalactiteHelper.PlaceTight(i, j + 1);
                WorldGen.SquareTileFrame(i, j + 1);
                if (Main.netMode == NetmodeID.Server && Main.tile[i, j + 1].HasTile)
                    NetMessage.SendTileSquare(-1, i, j + 1, 1, 2);
            }
        }

        /// <summary>
        /// Spread Rapture to adjacent ice tiles.
        /// </summary>
        private void SpreadRapture(int i, int j)
        {
            for (int x = i - 1; x <= i + 1; x++)
            {
                for (int y = j - 1; y <= j + 1; y++)
                {
                    if (x == i && y == j)
                        continue;

                    if (!WorldGen.InWorld(x, y, 1))
                        continue;

                    Tile tile = Main.tile[x, y];
                    if (!tile.HasTile)
                        continue;

                    if (!WorldGen.genRand.NextBool(3))
                        continue;

                    ushort newType = 0;

                    // Ice -> GoldenIce
                    if (tile.TileType == TileID.IceBlock)
                    {
                        newType = Type;
                    }
                    // Hallowed Ice -> GoldenIce
                    else if (tile.TileType == TileID.HallowedIce)
                    {
                        newType = Type;
                    }
                    // Corrupt/Crimson Ice -> GoldenIce
                    else if (tile.TileType == TileID.CorruptIce || tile.TileType == TileID.FleshIce)
                    {
                        newType = Type;
                    }

                    if (newType != 0)
                    {
                        tile.TileType = newType;
                        WorldGen.SquareTileFrame(x, y);

                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendTileSquare(-1, x, y, 1);
                        }
                    }
                }
            }
        }
    }
}
