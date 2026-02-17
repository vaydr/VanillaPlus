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
    /// Blisstone - the Rapture equivalent of Pearlstone.
    /// A divine white/gold stone that spreads Rapture.
    /// </summary>
    public class Blisstone : ModTile
    {
        public override string Texture => "Terraria/Images/Tiles_1"; // Use vanilla stone texture
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

            RaptureIDs.Sets.CanGrowDivineShard[Type] = true;
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

        // Gold tint color for Rapture theme
        private static readonly Color RaptureTint = Main.hslToRgb(0.12f, 0.6f, 0.75f);

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            Texture2D texture = TextureAssets.Tile[TileID.Stone].Value;
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 position = new Vector2(i * 16, j * 16) - Main.screenPosition + zero;
            Rectangle frame = new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16);
            Color color = new Color(Lighting.GetColor(i, j).ToVector4() * RaptureTint.ToVector4());

            spriteBatch.Draw(texture, position, frame, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            return false;
        }

        public override void RandomUpdate(int i, int j)
        {
            if (!WorldGen.AllowedToSpreadInfections)
                return;

            SpreadRapture(i, j);
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
                        tile.TileType == TileID.Ebonstone || tile.TileType == TileID.Crimstone)
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
