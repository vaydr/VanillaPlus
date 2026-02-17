using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Common;

namespace VanillaPlus.Content.Tiles.Rapture
{
    /// <summary>
    /// Blissgrass - the Rapture equivalent of Hallowed Grass.
    /// White/gold grass that spreads Rapture on dirt.
    /// </summary>
    public class Blissgrass : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileBrick[Type] = true;
            Main.tileShine[Type] = 9000;
            Main.tileLighted[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            // Mark as grass type for conversions
            TileID.Sets.Conversion.MergesWithDirtInASpecialWay[Type] = true;
            TileID.Sets.Conversion.Grass[Type] = true;
            TileID.Sets.ForcedDirtMerging[Type] = true;
            TileID.Sets.CanBeDugByShovel[Type] = true;
            TileID.Sets.ResetsHalfBrickPlacementAttempt[Type] = true;
            TileID.Sets.DoesntPlaceWithTileReplacement[Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;
            TileID.Sets.SpreadOverground[Type] = true;
            TileID.Sets.SpreadUnderground[Type] = true;
            TileID.Sets.Grass[Type] = true;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = true;
            TileID.Sets.GrassSpecial[Type] = true;

            // Rapture-specific sets
            RaptureIDs.Sets.RaptureBiomeSight[Type] = true;
            RaptureIDs.Sets.Rapture[Type] = true;
            RaptureIDs.Sets.IsNaturalRaptureTile[Type] = true;

            // Merges with Blisstone
            Main.tileMerge[Type][ModContent.TileType<Blisstone>()] = true;

            // Color: White with slight gold tint
            AddMapEntry(new Color(245, 240, 220));

            // When mined, drop dirt
            RegisterItemDrop(ItemID.DirtBlock);

            // Use marble dust for the white theme
            DustType = DustID.Marble;
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            // When grass is broken but tile remains, convert to dirt
            if (fail && !effectOnly)
            {
                Main.tile[i, j].TileType = TileID.Dirt;
            }
        }

        public override void RandomUpdate(int i, int j)
        {
            // Only spread on surface/underground
            if (!WorldGen.AllowedToSpreadInfections)
                return;

            // Spread grass to adjacent dirt
            SpreadBlissgrass(i, j);

            // TODO: Grow Rapture plants/vines
        }

        /// <summary>
        /// Spread Blissgrass to adjacent dirt tiles.
        /// </summary>
        private void SpreadBlissgrass(int i, int j)
        {
            int minI = i - 1;
            int maxI = i + 2;
            int minJ = j - 1;
            int maxJ = j + 2;

            for (int x = minI; x < maxI; x++)
            {
                for (int y = minJ; y < maxJ; y++)
                {
                    if (x == i && y == j)
                        continue;

                    if (!WorldGen.InWorld(x, y, 1))
                        continue;

                    Tile tile = Main.tile[x, y];
                    if (!tile.HasTile)
                        continue;

                    // Random chance
                    if (!WorldGen.genRand.NextBool(4))
                        continue;

                    ushort newType = 0;

                    // Dirt -> Blissgrass
                    if (tile.TileType == TileID.Dirt)
                    {
                        // Only convert if exposed to air (has empty space adjacent)
                        if (HasAirExposure(x, y))
                        {
                            newType = Type;
                        }
                    }
                    // Grass variants -> Blissgrass
                    else if (tile.TileType == TileID.Grass ||
                             tile.TileType == TileID.CorruptGrass ||
                             tile.TileType == TileID.CrimsonGrass ||
                             tile.TileType == TileID.HallowedGrass)
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

        /// <summary>
        /// Check if a tile has at least one adjacent empty space (for grass growth).
        /// </summary>
        private bool HasAirExposure(int i, int j)
        {
            for (int x = i - 1; x <= i + 1; x++)
            {
                for (int y = j - 1; y <= j + 1; y++)
                {
                    if (x == i && y == j)
                        continue;

                    if (!WorldGen.InWorld(x, y, 1))
                        continue;

                    if (!Main.tile[x, y].HasTile)
                        return true;
                }
            }
            return false;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            // Subtle golden glow
            r = 0.05f;
            g = 0.04f;
            b = 0.02f;
        }
    }
}
