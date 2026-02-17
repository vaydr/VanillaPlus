using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Common;

namespace VanillaPlus.Content.Tiles.Rapture
{
    /// <summary>
    /// BlissIce - the Rapture equivalent of Hallowed Ice.
    /// Divine golden-tinted ice that spreads Rapture in snow biomes.
    /// </summary>
    public class BlissIce : ModTile
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
            RaptureIDs.Sets.CanGrowDivineShard[Type] = true;
            RaptureIDs.Sets.RaptureBiomeSight[Type] = true;
            RaptureIDs.Sets.Rapture[Type] = true;
            RaptureIDs.Sets.IsNaturalRaptureTile[Type] = true;

            // Merge with other ice types and Rapture tiles
            Main.tileMerge[Type][ModContent.TileType<Blisstone>()] = true;
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

            // Drop the BlissIce item
            RegisterItemDrop(ModContent.ItemType<Items.Rapture.BlissIce>());
        }

        public override void RandomUpdate(int i, int j)
        {
            if (!WorldGen.AllowedToSpreadInfections)
                return;

            SpreadRapture(i, j);

            // TODO: Generate ice stalactites/stalagmites
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

                    // Ice -> BlissIce
                    if (tile.TileType == TileID.IceBlock)
                    {
                        newType = Type;
                    }
                    // Hallowed Ice -> BlissIce
                    else if (tile.TileType == TileID.HallowedIce)
                    {
                        newType = Type;
                    }
                    // Corrupt/Crimson Ice -> BlissIce
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
