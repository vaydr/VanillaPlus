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
                Tile tileCurrent = Main.tile[x, y];

                // Check if tree is on Blissgrass or already part of HedonTree
                if (tileBelow.TileType == ModContent.TileType<Blissgrass>() ||
                    tileBelow.TileType == ModContent.TileType<HedonTree>() ||
                    tileCurrent.TileType == ModContent.TileType<Blissgrass>() ||
                    tileCurrent.TileType == ModContent.TileType<HedonTree>())
                {
                    Main.tile[i, j].TileType = (ushort)ModContent.TileType<HedonTree>();
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
            Tile tileBelow = Main.tile[i, j + 1];

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

            return true;
        }
    }
}
