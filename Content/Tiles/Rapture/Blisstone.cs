using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Common;

namespace VanillaPlus.Content.Tiles.Rapture
{
    /// <summary>
    /// Blisstone - the Rapture equivalent of Pearlstone.
    /// A divine white/gold stone that spreads Rapture.
    /// </summary>
    public class Blisstone : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileMergeDirt[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileStone[Type] = true;
            Main.tileShine2[Type] = true;
            Main.tileShine[Type] = 9000;
            Main.tileBrick[Type] = true;
            Main.tileBlockLight[Type] = true;

            TileID.Sets.Conversion.Stone[Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = true;

            RaptureIDs.Sets.CanGrowRadiantShard[Type] = true;
            RaptureIDs.Sets.RaptureBiomeSight[Type] = true;
            RaptureIDs.Sets.Rapture[Type] = true;
            RaptureIDs.Sets.IsNaturalRaptureTile[Type] = true;

            AddMapEntry(new Color(240, 230, 200));
            HitSound = SoundID.Tink;
            MineResist = 2f;
            MinPick = 65;
            DustType = DustID.Pearlsand;

            // Drop the Blisstone item
            RegisterItemDrop(ModContent.ItemType<Items.Rapture.Blisstone>());
        }

        public override void RandomUpdate(int i, int j)
        {
            if (!WorldGen.AllowedToSpreadInfections)
                return;

            SpreadRapture(i, j);
            GenerateStalactites(i, j);
        }

        /// <summary>
        /// Generate Blisstone stalactites/stalagmites adjacent to this block.
        /// Stalactites hang below, stalagmites grow above. Similar to pearlstone behavior.
        /// </summary>
        private void GenerateStalactites(int i, int j)
        {
            if (!Main.tile[i, j].HasUnactuatedTile)
                return;

            if (!Main.rand.NextBool(10))
                return;

            // Count existing stalactites in a 7-wide window
            int count = 0;
            for (int x = i - 3; x < i + 4; x++)
            {
                for (int dy = -3; dy <= 3; dy++)
                {
                    if (WorldGen.InWorld(x, j + dy, 1) &&
                        Main.tile[x, j + dy].TileType == ModContent.TileType<BlisstoneStalactite>() &&
                        Main.tile[x, j + dy].HasTile)
                    {
                        count++;
                    }
                }
            }

            if (count >= 2)
                return;

            // Try stalactite below (ceiling)
            if (!Main.tile[i, j + 1].HasTile && !Main.tile[i, j + 2].HasTile)
            {
                RaptureStalactiteHelper.PlaceTight(i, j + 1);
                WorldGen.SquareTileFrame(i, j + 1);
                if (Main.netMode == NetmodeID.Server && Main.tile[i, j + 1].HasTile)
                    NetMessage.SendTileSquare(-1, i, j + 1, 1, 2);
            }
            // Try stalagmite above (floor)
            else if (!Main.tile[i, j - 1].HasTile && !Main.tile[i, j - 2].HasTile)
            {
                RaptureStalactiteHelper.PlaceTight(i, j - 1);
                WorldGen.SquareTileFrame(i, j - 1);
                if (Main.netMode == NetmodeID.Server && Main.tile[i, j - 1].HasTile)
                    NetMessage.SendTileSquare(-1, i, j - 1, 1, 2);
            }
        }

        private void SpreadRapture(int i, int j)
        {
            for (int x = i - 1; x <= i + 1; x++)
            {
                for (int y = j - 1; y <= j + 1; y++)
                {
                    if (x == i && y == j) continue;
                    if (!WorldGen.InWorld(x, y, 1)) continue;

                    Tile tile = Main.tile[x, y];
                    if (!tile.HasTile) continue;
                    if (!WorldGen.genRand.NextBool(3)) continue;

                    ushort newType = 0;

                    if (tile.TileType == TileID.Stone || tile.TileType == TileID.Pearlstone ||
                        tile.TileType == TileID.Ebonstone || tile.TileType == TileID.Crimstone ||
                        tile.TileType == TileID.GreenMoss || tile.TileType == TileID.BrownMoss ||
                        tile.TileType == TileID.RedMoss || tile.TileType == TileID.BlueMoss ||
                        tile.TileType == TileID.PurpleMoss || tile.TileType == TileID.LavaMoss ||
                        tile.TileType == TileID.KryptonMoss || tile.TileType == TileID.XenonMoss ||
                        tile.TileType == TileID.ArgonMoss || tile.TileType == TileID.VioletMoss ||
                        tile.TileType == TileID.RainbowMoss)
                    {
                        newType = (ushort)ModContent.TileType<Blisstone>();
                    }

                    if (newType != 0)
                    {
                        tile.TileType = newType;
                        WorldGen.SquareTileFrame(x, y);
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendTileSquare(-1, x, y, 1);
                    }
                }
            }
        }
    }
}
