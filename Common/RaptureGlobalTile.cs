using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Content.Tiles.Rapture;
using VanillaPlus.Content.Tiles.Rapture.Trees;

namespace VanillaPlus.Common
{
    /// <summary>
    /// GlobalTile for Rapture biome tile conversions.
    /// Handles dynamic tree conversion and biome sight.
    /// </summary>
    public class RaptureGlobalTile : GlobalTile
    {
        public override void NearbyEffects(int i, int j, int type, bool closer)
        {
            // Convert vanilla trees on Blissgrass to HedonTree
            if (type == TileID.Trees)
            {
                WorldGen.GetTreeBottom(i, j, out var x, out var y);
                Tile tileBelow = Main.tile[x, y + 1];

                // Only check the tile directly below the tree base - this is the grass
                if (tileBelow.TileType == ModContent.TileType<Blissgrass>())
                {
                    ushort hedonTreeType = (ushort)ModContent.TileType<HedonTree>();
                    if (Main.tile[i, j].TileType != hedonTreeType)
                    {
                        Main.tile[i, j].TileType = hedonTreeType;

                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendTileSquare(-1, i, j, 1);
                        }
                    }
                }
            }
        }

        public override bool? IsTileBiomeSightable(int i, int j, int type, ref Color sightColor)
        {
            // Rapture tiles show as gold/white with biome sight
            if (RaptureIDs.Sets.RaptureBiomeSight[type])
            {
                sightColor = new Color(240, 230, 200); // Gold/cream color
                return true;
            }
            return null;
        }

        public override bool TileFrame(int i, int j, int type, ref bool resetFrame, ref bool noBreak)
        {
            Tile tile = Main.tile[i, j];

            // Convert vines growing from Blissgrass to vanilla Hallow vines (until we have custom vines)
            if (TileID.Sets.IsVine[type])
            {
                Tile tileAbove = Main.tile[i, j - 1];
                int aboveTileType = tileAbove.HasUnactuatedTile && !tileAbove.BottomSlope ? tileAbove.TileType : -1;

                // If vine is attached to Blissgrass, convert to Hallow vine
                if (aboveTileType == ModContent.TileType<Blissgrass>() && type != TileID.HallowedVines)
                {
                    tile.TileType = TileID.HallowedVines;
                    WorldGen.SquareTileFrame(i, j);
                    return true;
                }
            }

            // Convert vanilla stalactites adjacent to Rapture tiles
            if (type == TileID.Stalactite)
            {
                ConvertVanillaStalactite(i, j);
            }

            return true;
        }

        /// <summary>
        /// Convert a vanilla stalactite to Rapture variant if its parent tile is a Rapture tile.
        /// Called during TileFrame when the parent block changes (e.g., spreading).
        /// </summary>
        private void ConvertVanillaStalactite(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            int parentType = -1;

            // Determine parent tile based on frame orientation
            if (tile.TileFrameY == 72)
            {
                // Small stalactite - parent above
                parentType = Main.tile[i, j - 1].TileType;
            }
            else if (tile.TileFrameY == 90)
            {
                // Small stalagmite - parent below
                parentType = Main.tile[i, j + 1].TileType;
            }
            else if (tile.TileFrameY >= 36)
            {
                // Large stalagmite - find top tile, parent below bottom
                int topY = tile.TileFrameY == 54 ? j - 1 : j;
                parentType = Main.tile[i, topY + 2].TileType;
            }
            else
            {
                // Large stalactite - find top tile, parent above top
                int topY = tile.TileFrameY == 18 ? j - 1 : j;
                parentType = Main.tile[i, topY - 1].TileType;
            }

            ushort newTileType = 0;
            if (parentType == ModContent.TileType<GoldenIce>())
            {
                newTileType = (ushort)ModContent.TileType<GoldenIceStalactite>();
            }
            else if (parentType == ModContent.TileType<Blisstone>() ||
                     parentType == ModContent.TileType<Blissandstone>() ||
                     parentType == ModContent.TileType<HardenedBlissand>())
            {
                newTileType = (ushort)ModContent.TileType<BlisstoneStalactite>();
            }

            if (newTileType == 0)
                return;

            // Convert this tile and its partner (if large stalactite/stalagmite)
            int frameX = WorldGen.genRand.Next(3) * 18;

            if (tile.TileFrameY == 72 || tile.TileFrameY == 90)
            {
                // Single-tile stalactite/stalagmite
                tile.TileType = newTileType;
                tile.TileFrameX = (short)frameX;
            }
            else if (tile.TileFrameY >= 36)
            {
                // Large stalagmite
                int topY = tile.TileFrameY == 54 ? j - 1 : j;
                Main.tile[i, topY].TileType = newTileType;
                Main.tile[i, topY].TileFrameX = (short)frameX;
                Main.tile[i, topY + 1].TileType = newTileType;
                Main.tile[i, topY + 1].TileFrameX = (short)frameX;
            }
            else
            {
                // Large stalactite
                int topY = tile.TileFrameY == 18 ? j - 1 : j;
                Main.tile[i, topY].TileType = newTileType;
                Main.tile[i, topY].TileFrameX = (short)frameX;
                Main.tile[i, topY + 1].TileType = newTileType;
                Main.tile[i, topY + 1].TileFrameX = (short)frameX;
            }

            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendTileSquare(-1, i, j, 1, 2);
        }

        public override void RandomUpdate(int i, int j, int type)
        {
            // Only tiles that can grow Radiant Shards
            if (!RaptureIDs.Sets.CanGrowRadiantShard[type])
                return;

            // Only underground
            if (j <= Main.rockLayer)
                return;

            // Check 8-tile radius for existing shards (prevent overcrowding)
            for (int nx = i - 4; nx <= i + 4; nx++)
            {
                for (int ny = j - 4; ny <= j + 4; ny++)
                {
                    if (!WorldGen.InWorld(nx, ny)) continue;
                    if (Main.tile[nx, ny].TileType == ModContent.TileType<RadiantShard>())
                        return;
                }
            }

            // 2.5% chance to spawn on adjacent empty tile (matching vanilla crystal shard rate)
            if (!WorldGen.genRand.NextBool(40))
                return;

            Tile tile = Main.tile[i, j];
            if (tile.IsHalfBlock || tile.Slope != SlopeType.Solid)
                return;

            // Try adjacent positions (in random order to avoid bias)
            int[] dx = { 1, -1, 0, 0 };
            int[] dy = { 0, 0, 1, -1 };

            // Shuffle directions
            for (int s = 3; s > 0; s--)
            {
                int r = WorldGen.genRand.Next(s + 1);
                (dx[s], dx[r]) = (dx[r], dx[s]);
                (dy[s], dy[r]) = (dy[r], dy[s]);
            }

            for (int d = 0; d < 4; d++)
            {
                int nx = i + dx[d];
                int ny = j + dy[d];

                if (!WorldGen.InWorld(nx, ny, 1)) continue;

                // Use the helper method that properly sets TileFrameX/Y for orientation
                if (RadiantShard.PlaceShardWithFrame(nx, ny, (ushort)ModContent.TileType<RadiantShard>()))
                    break;
            }
        }
    }
}
