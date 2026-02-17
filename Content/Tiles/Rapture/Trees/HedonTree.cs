using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace VanillaPlus.Content.Tiles.Rapture.Trees
{
    /// <summary>
    /// HedonTree - the Rapture equivalent of forest trees.
    /// White tree trunks that grow on Blissgrass.
    /// </summary>
    public class HedonTree : ModTile
    {
        private Asset<Texture2D> treeTopTexture;
        private Asset<Texture2D> treeBranchTexture;

        // Tree top frame size (80x80 per frame, 6 frames total)
        private const int TopFrameWidth = 80;
        private const int TopFrameHeight = 80;
        // Tree branch frame size (40x40 per frame)
        private const int BranchFrameWidth = 40;
        private const int BranchFrameHeight = 40;

        public override void SetStaticDefaults()
        {
            Main.tileAxe[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;

            TileID.Sets.IsATreeTrunk[Type] = true;
            TileID.Sets.IsShakeable[Type] = true;
            TileID.Sets.GetsDestroyedForMeteors[Type] = true;
            TileID.Sets.GetsCheckedForLeaves[Type] = true;
            TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;
            TileID.Sets.PreventsTileReplaceIfOnTopOfIt[Type] = true;

            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(220, 215, 210), name);

            DustType = DustID.BorealWood;
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            // Adjust draw dimensions to match vanilla tree drawing
            width = 20;
            height = 20;
        }

        public override void Load()
        {
            // Load textures
            treeTopTexture = ModContent.Request<Texture2D>("VanillaPlus/Content/Tiles/Rapture/Trees/HedonTree_Tops");
            treeBranchTexture = ModContent.Request<Texture2D>("VanillaPlus/Content/Tiles/Rapture/Trees/HedonTree_Branches");
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            Tile tile = Main.tile[i, j];

            // Drop acorn from treetop
            if (tile.TileFrameX >= 22 && tile.TileFrameY >= 198)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient && Main.rand.NextBool(2))
                {
                    Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, ItemID.Acorn);
                }
            }

            // Drop Hedonwood
            if (!noItem && !effectOnly)
            {
                Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, ModContent.ItemType<Items.Rapture.Hedonwood>());
            }
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            Tile tile = Main.tile[i, j];
            if (i > 5 && j > 5 && i < Main.maxTilesX - 5 && j < Main.maxTilesY - 5 && Main.tile[i, j] != null)
            {
                if (tile.HasTile && Main.tileFrameImportant[Type])
                {
                    WorldGen.CheckTree(i, j);
                }
            }
            return false;
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            // Get bonus wood chance
            int dropItemStack = 1;
            int num = Player.FindClosest(new Vector2(i * 16, j * 16), 16, 16);
            int axe = Main.player[num].inventory[Main.player[num].selectedItem].axe;
            if (WorldGen.genRand.Next(100) < axe || Main.rand.NextBool(3))
            {
                dropItemStack++;
            }
            yield return new Item(ModContent.ItemType<Items.Rapture.Hedonwood>(), dropItemStack);
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            // Convert back to vanilla tree if not on Blissgrass
            WorldGen.GetTreeBottom(i, j, out var x, out var y);
            Tile tileBelow = Main.tile[x, y + 1];
            Tile tileCurrent = Main.tile[x, y];

            if (tileBelow.TileType != ModContent.TileType<Blissgrass>() &&
                tileCurrent.TileType != ModContent.TileType<Blissgrass>() &&
                tileBelow.TileType != Type && tileCurrent.TileType != Type)
            {
                Main.tile[i, j].TileType = TileID.Trees;
            }
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            if (tile == null || !tile.HasTile)
                return;

            short frameX = tile.TileFrameX;
            short frameY = tile.TileFrameY;

            // Only draw for tree top/branch frames
            if (frameY < 198 || frameX < 22)
                return;

            // Get wind sway
            double treeWindCounter = 0;
            try
            {
                var field = typeof(TileDrawing).GetField("_treeWindCounter", BindingFlags.NonPublic | BindingFlags.Instance);
                if (field != null)
                    treeWindCounter = (double)field.GetValue(Main.instance.TilesRenderer);
            }
            catch { }

            bool hasWall = tile.WallType > 0;
            float windCycle = 0f;
            if (!hasWall)
            {
                windCycle = Main.instance.TilesRenderer.GetWindCycle(i, j, treeWindCounter);
            }

            Vector2 unscaledPosition = Main.Camera.UnscaledPosition;
            Vector2 offScreen = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Color lightColor = Lighting.GetColor(i, j);
            if (tile.IsTileFullbright)
                lightColor = Color.White;

            // Determine tree frame variant (0-2 blue, 3-5 gold based on position)
            int treeFrame = WorldGen.GetTreeFrame(tile);
            // Add 3 for gold variant based on tile position
            if ((i + j) % 2 == 0)
            {
                treeFrame += 3;
            }

            float rotationSway = 0.08f;
            float branchSway = 0.06f;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Matrix.Identity);

            switch (frameX)
            {
                case 22: // Tree top
                    {
                        Vector2 drawPos = new Vector2(i * 16 - (int)unscaledPosition.X + 8, j * 16 - (int)unscaledPosition.Y + 16) + offScreen;
                        drawPos.X += windCycle * 2f;
                        drawPos.Y += Math.Abs(windCycle) * 2f;

                        Rectangle sourceRect = new Rectangle(treeFrame * (TopFrameWidth + 2), 0, TopFrameWidth, TopFrameHeight);
                        Vector2 origin = new Vector2(TopFrameWidth / 2, TopFrameHeight);

                        spriteBatch.Draw(treeTopTexture.Value, drawPos, sourceRect, lightColor, windCycle * rotationSway, origin, 1f, SpriteEffects.None, 0f);
                        break;
                    }
                case 44: // Right branch
                    {
                        Vector2 drawPos = new Vector2(i * 16, j * 16) - unscaledPosition.Floor() + offScreen + new Vector2(16f, 12f);
                        if (windCycle > 0f)
                            drawPos.X += windCycle;
                        drawPos.X += Math.Abs(windCycle) * 2f;

                        // Right branch is first column in branch texture
                        int branchVariant = (treeFrame >= 3) ? 1 : 0; // 0 = blue, 1 = gold
                        Rectangle sourceRect = new Rectangle(branchVariant * (BranchFrameWidth * 2 + 4), (treeFrame % 3) * 42, BranchFrameWidth, BranchFrameHeight);
                        Vector2 origin = new Vector2(BranchFrameWidth, 24f);

                        spriteBatch.Draw(treeBranchTexture.Value, drawPos, sourceRect, lightColor, windCycle * branchSway, origin, 1f, SpriteEffects.None, 0f);
                        break;
                    }
                case 66: // Left branch
                    {
                        Vector2 drawPos = new Vector2(i * 16, j * 16) - unscaledPosition.Floor() + offScreen + new Vector2(0f, 18f);
                        if (windCycle < 0f)
                            drawPos.X += windCycle;
                        drawPos.X -= Math.Abs(windCycle) * 2f;

                        // Left branch is second column in branch texture
                        int branchVariant = (treeFrame >= 3) ? 1 : 0;
                        Rectangle sourceRect = new Rectangle(branchVariant * (BranchFrameWidth * 2 + 4) + BranchFrameWidth + 2, (treeFrame % 3) * 42, BranchFrameWidth, BranchFrameHeight);
                        Vector2 origin = new Vector2(0f, 30f);

                        spriteBatch.Draw(treeBranchTexture.Value, drawPos, sourceRect, lightColor, windCycle * branchSway, origin, 1f, SpriteEffects.None, 0f);
                        break;
                    }
            }

            spriteBatch.End();
            spriteBatch.Begin();
        }
    }
}
