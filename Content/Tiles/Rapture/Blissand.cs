using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Common;
using VanillaPlus.Content.Projectiles.Rapture;

namespace VanillaPlus.Content.Tiles.Rapture
{
    /// <summary>
    /// Blissand - the Rapture equivalent of Pearlsand.
    /// Divine white/gold sand that spreads Rapture and falls like normal sand.
    /// </summary>
    public class Blissand : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileBrick[Type] = true;
            Main.tileSand[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;

            // Mark as sand type for conversions
            TileID.Sets.Conversion.Sand[Type] = true;
            TileID.Sets.ForAdvancedCollision.ForSandshark[Type] = true;
            TileID.Sets.CanBeDugByShovel[Type] = true;
            TileID.Sets.Falling[Type] = true;
            TileID.Sets.Suffocate[Type] = true;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = true;

            // Set up falling projectile - uses custom BlissandProjectile to place correct tile
            TileID.Sets.FallingBlockProjectile[Type] = new TileID.Sets.FallingBlockProjectileInfo(
                ModContent.ProjectileType<BlissandProjectile>(), 15);

            // Rapture-specific sets
            RaptureIDs.Sets.RaptureBiomeSight[Type] = true;
            RaptureIDs.Sets.Rapture[Type] = true;
            RaptureIDs.Sets.IsNaturalRaptureTile[Type] = true;

            // Color: Light baby blue sand
            AddMapEntry(new Color(180, 215, 230));

            MineResist = 0.5f;
            DustType = DustID.Sand;

            // Drop the Blissand item
            RegisterItemDrop(ModContent.ItemType<Items.Rapture.Blissand>());
        }

        public override bool HasWalkDust()
        {
            return Main.rand.NextBool(3);
        }

        public override void WalkDust(ref int dustType, ref bool makeDust, ref Color color)
        {
            dustType = DustType;
        }

        public override void RandomUpdate(int i, int j)
        {
            if (!WorldGen.AllowedToSpreadInfections)
                return;

            SpreadRapture(i, j);
        }

        /// <summary>
        /// Spread Rapture to adjacent sand tiles.
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

                    // Sand -> Blissand
                    if (tile.TileType == TileID.Sand)
                    {
                        newType = Type;
                    }
                    // Pearlsand -> Blissand
                    else if (tile.TileType == TileID.Pearlsand)
                    {
                        newType = Type;
                    }
                    // Ebonsand/Crimsand -> Blissand
                    else if (tile.TileType == TileID.Ebonsand || tile.TileType == TileID.Crimsand)
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
