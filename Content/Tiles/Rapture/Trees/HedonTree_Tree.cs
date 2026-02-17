using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaPlus.Content.Tiles.Rapture.Trees
{
    /// <summary>
    /// ModTree class for HedonTree - defines tree properties and textures.
    /// Has two foliage variants: light blue (frames 0-2) and gold (frames 3-5).
    /// </summary>
    public class HedonTree_Tree : ModTree
    {
        public override TreePaintingSettings TreeShaderSettings => new TreePaintingSettings
        {
            UseSpecialGroups = true,
            SpecialGroupMinimalHueValue = 0.0f,
            SpecialGroupMaximumHueValue = 0.25f,
            SpecialGroupMinimumSaturationValue = 0.0f,
            SpecialGroupMaximumSaturationValue = 0.5f
        };

        public override void SetStaticDefaults()
        {
            // Trees grow on Blissgrass and on HedonTree tiles (so conversion doesn't break them)
            GrowsOnTileId = new int[] {
                ModContent.TileType<Blissgrass>(),
                ModContent.TileType<HedonTree>()
            };
        }

        public override int CreateDust()
        {
            return DustID.BorealWood;
        }

        public override bool Shake(int x, int y, ref bool createLeaves)
        {
            createLeaves = true;
            return false;
        }

        public override Asset<Texture2D> GetTexture()
        {
            return ModContent.Request<Texture2D>("VanillaPlus/Content/Tiles/Rapture/Trees/HedonTree");
        }

        public override Asset<Texture2D> GetBranchTextures()
        {
            return ModContent.Request<Texture2D>("VanillaPlus/Content/Tiles/Rapture/Trees/HedonTree_Branches");
        }

        public override Asset<Texture2D> GetTopTextures()
        {
            return ModContent.Request<Texture2D>("VanillaPlus/Content/Tiles/Rapture/Trees/HedonTree_Tops");
        }

        [System.Obsolete]
        public override void SetTreeFoliageSettings(Tile tile, ref int xoffset, ref int treeFrame, ref int floorY, ref int topTextureFrameWidth, ref int topTextureFrameHeight)
        {
            // Texture has 6 frames: 0-2 are blue, 3-5 are gold
            // Use tile position to deterministically select variant (roughly 50/50)
            // treeFrame comes in as 0, 1, or 2 - add 3 for gold variant
            if ((tile.TileFrameX / 22 + tile.TileFrameY / 22) % 2 == 0)
            {
                treeFrame += 3; // Use gold variant (frames 3-5)
            }
            // else keep blue variant (frames 0-2)
        }

        public override int DropWood()
        {
            return ModContent.ItemType<Items.Rapture.Hedonwood>();
        }

        public override int TreeLeaf()
        {
            // Use vanilla forest tree leaves for now
            return GoreID.TreeLeaf_Normal;
        }
    }
}
