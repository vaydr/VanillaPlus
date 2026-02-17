using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Common;

namespace VanillaPlus.Content.Tiles.Rapture
{
    /// <summary>
    /// HardenedBlissand - the Rapture equivalent of Hardened Pearlsand.
    /// Found in underground desert areas that have been converted to Rapture.
    /// </summary>
    public class HardenedBlissand : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileMergeDirt[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            // Mark as hardened sand type for conversions
            TileID.Sets.Conversion.HardenedSand[Type] = true;
            TileID.Sets.ForAdvancedCollision.ForSandshark[Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = true;

            // Rapture-specific sets
            RaptureIDs.Sets.CanGrowDivineShard[Type] = true;
            RaptureIDs.Sets.RaptureBiomeSight[Type] = true;
            RaptureIDs.Sets.Rapture[Type] = true;
            RaptureIDs.Sets.IsNaturalRaptureTile[Type] = true;

            // Merge with other Rapture tiles
            Main.tileMerge[Type][ModContent.TileType<Blisstone>()] = true;
            Main.tileMerge[Type][ModContent.TileType<Blissand>()] = true;
            Main.tileMerge[Type][ModContent.TileType<Blissandstone>()] = true;

            // Color: Slightly darker golden sand
            AddMapEntry(new Color(210, 195, 160));

            DustType = DustID.Sand;

            // Drop the HardenedBlissand item
            RegisterItemDrop(ModContent.ItemType<Items.Rapture.HardenedBlissand>());
        }

        public override void RandomUpdate(int i, int j)
        {
            if (!WorldGen.AllowedToSpreadInfections)
                return;

            SpreadRapture(i, j);
        }

        /// <summary>
        /// Spread Rapture to adjacent hardened sand tiles.
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

                    // Hardened Sand -> HardenedBlissand
                    if (tile.TileType == TileID.HardenedSand)
                    {
                        newType = Type;
                    }
                    // Hallow Hardened Sand -> HardenedBlissand
                    else if (tile.TileType == TileID.HallowHardenedSand)
                    {
                        newType = Type;
                    }
                    // Corrupt/Crimson Hardened Sand -> HardenedBlissand
                    else if (tile.TileType == TileID.CorruptHardenedSand || tile.TileType == TileID.CrimsonHardenedSand)
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
