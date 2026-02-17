using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaPlus.Content.Items.Rapture
{
    public class HardenedBlissand : ModItem
    {
        public override string Texture => "Terraria/Images/Item_3271"; // Vanilla hardened sand item

        private static readonly Color RaptureTint = Main.hslToRgb(0.12f, 0.6f, 0.75f);

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = TextureAssets.Item[ItemID.HardenedSand].Value;
            spriteBatch.Draw(texture, position, frame, new Color(RaptureTint.ToVector4() * drawColor.ToVector4()), 0f, origin, scale, SpriteEffects.None, 0f);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = TextureAssets.Item[ItemID.HardenedSand].Value;
            Vector2 position = Item.position - Main.screenPosition + new Vector2(Item.width / 2, Item.height / 2);
            spriteBatch.Draw(texture, position, null, new Color(RaptureTint.ToVector4() * lightColor.ToVector4()), rotation, texture.Size() / 2, scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.maxStack = Item.CommonMaxStack;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.Rapture.HardenedBlissand>();
        }
    }
}
