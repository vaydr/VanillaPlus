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
    /// BlissIce - the Rapture equivalent of Hallowed Ice.
    /// Divine golden-tinted ice that spreads Rapture in snow biomes.
    /// </summary>
    public class BlissIce : ModTile
    {
        public override string Texture => "Terraria/Images/Tiles_161"; // Use vanilla ice texture
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

        // Gold tint color for Rapture theme
        private static readonly Color RaptureTint = Main.hslToRgb(0.12f, 0.6f, 0.75f);

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            Texture2D texture = TextureAssets.Tile[TileID.IceBlock].Value;
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
