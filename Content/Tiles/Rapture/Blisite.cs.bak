using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Common;

namespace VanillaPlus.Content.Tiles.Rapture
{
    /// <summary>
    /// Blisite - the Rapture equivalent of Pearlstone.
    /// A divine white/gold stone that spreads Rapture.
    /// </summary>
    public class Blisite : ModTile
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

            // Mark as stone type for conversions
            TileID.Sets.Conversion.Stone[Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = true;

            // Rapture-specific sets
            RaptureIDs.Sets.CanGrowDivineShard[Type] = true;
            RaptureIDs.Sets.RaptureBiomeSight[Type] = true;
            RaptureIDs.Sets.Rapture[Type] = true;
            RaptureIDs.Sets.IsNaturalRaptureTile[Type] = true;

            // Color: White with gold tint (angelic theme)
            AddMapEntry(new Color(240, 230, 200));

            // Use a tink sound like other stone
            HitSound = SoundID.Tink;

            // Requires pickaxe to mine, same as Pearlstone
            MineResist = 2f;
            MinPick = 65;

            // Use vanilla stone dust for now
            DustType = DustID.Marble;
        }

        public override void RandomUpdate(int i, int j)
        {
            // Spreading logic - spread to adjacent convertible tiles
            if (!WorldGen.AllowedToSpreadInfections)
                return;

            SpreadRapture(i, j);
        }

        /// <summary>
        /// Spread Rapture to adjacent tiles.
        /// </summary>
        private void SpreadRapture(int i, int j)
        {
            // Check all 4 cardinal directions + diagonals
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

                    // Random chance to spread (same rate as vanilla)
                    if (!WorldGen.genRand.NextBool(3))
                        continue;

                    ushort newType = 0;

                    // Stone -> Blisite
                    if (tile.TileType == TileID.Stone)
                    {
                        newType = (ushort)ModContent.TileType<Blisite>();
                    }
                    // Pearlstone -> Blisite
                    else if (tile.TileType == TileID.Pearlstone)
                    {
                        newType = (ushort)ModContent.TileType<Blisite>();
                    }
                    // Ebonstone/Crimstone -> Blisite
                    else if (tile.TileType == TileID.Ebonstone || tile.TileType == TileID.Crimstone)
                    {
                        newType = (ushort)ModContent.TileType<Blisite>();
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
