using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Common;

namespace VanillaPlus.Content.Tiles.Rapture
{
    /// <summary>
    /// Blissandstone - the Rapture equivalent of Pearlsandstone/Hallowed Sandstone.
    /// Found in underground desert areas that have been converted to Rapture.
    /// </summary>
    public class Blissandstone : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileMergeDirt[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            // Mark as sandstone type for conversions
            TileID.Sets.Conversion.Sandstone[Type] = true;
            TileID.Sets.ForAdvancedCollision.ForSandshark[Type] = true;
            TileID.Sets.isDesertBiomeSand[Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = true;

            // Rapture-specific sets
            RaptureIDs.Sets.CanGrowRadiantShard[Type] = true;
            RaptureIDs.Sets.RaptureBiomeSight[Type] = true;
            RaptureIDs.Sets.Rapture[Type] = true;
            RaptureIDs.Sets.IsNaturalRaptureTile[Type] = true;

            // Merge with other Rapture tiles
            Main.tileMerge[Type][ModContent.TileType<Blisstone>()] = true;
            Main.tileMerge[Type][ModContent.TileType<Blissand>()] = true;
            Main.tileMerge[Type][ModContent.TileType<HardenedBlissand>()] = true;

            // Color: Baby blue sandstone
            AddMapEntry(new Color(140, 175, 195));

            DustType = DustID.Sand;

            // Drop the Blissandstone item
            RegisterItemDrop(ModContent.ItemType<Items.Rapture.Blissandstone>());
        }

        public override void RandomUpdate(int i, int j)
        {
            if (!WorldGen.AllowedToSpreadInfections)
                return;

            SpreadRapture(i, j);
        }

        /// <summary>
        /// Spread Rapture to adjacent sandstone tiles.
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

                    // Sandstone -> Blissandstone
                    if (tile.TileType == TileID.Sandstone)
                    {
                        newType = Type;
                    }
                    // Hallow Sandstone -> Blissandstone
                    else if (tile.TileType == TileID.HallowSandstone)
                    {
                        newType = Type;
                    }
                    // Corrupt/Crimson Sandstone -> Blissandstone
                    else if (tile.TileType == TileID.CorruptSandstone || tile.TileType == TileID.CrimsonSandstone)
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
