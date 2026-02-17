using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Common;

namespace VanillaPlus.Content.Tiles.Rapture
{
    /// <summary>
    /// Blissand - the Rapture equivalent of Pearlsand.
    /// Divine white/gold sand that spreads Rapture and falls like normal sand.
    /// </summary>
    public class Blissand : ModTile
    {
        public override string Texture => "Terraria/Images/Tiles_53"; // Use vanilla sand texture
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

            // Set up falling projectile (uses vanilla sand projectile behavior)
            // TODO: Create custom BlissandProjectile
            TileID.Sets.FallingBlockProjectile[Type] = new TileID.Sets.FallingBlockProjectileInfo(ProjectileID.SandBallFalling, 15);

            // Rapture-specific sets
            RaptureIDs.Sets.RaptureBiomeSight[Type] = true;
            RaptureIDs.Sets.Rapture[Type] = true;
            RaptureIDs.Sets.IsNaturalRaptureTile[Type] = true;

            // Color: Light golden sand
            AddMapEntry(new Color(230, 215, 180));

            MineResist = 0.5f;
            DustType = DustID.Sand;

            // Drop the Blissand item
            RegisterItemDrop(ModContent.ItemType<Items.Rapture.Blissand>());
        }

        // Gold tint color for Rapture theme
        private static readonly Color RaptureTint = Main.hslToRgb(0.12f, 0.6f, 0.75f);

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            Texture2D texture = TextureAssets.Tile[TileID.Sand].Value;
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 position = new Vector2(i * 16, j * 16) - Main.screenPosition + zero;
            Rectangle frame = new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16);
            Color color = new Color(Lighting.GetColor(i, j).ToVector4() * RaptureTint.ToVector4());

            spriteBatch.Draw(texture, position, frame, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            return false;
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
