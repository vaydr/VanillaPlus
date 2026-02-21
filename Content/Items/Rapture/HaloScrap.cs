using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaPlus.Content.Items.Rapture
{
    public class HaloScrap : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 9999;
            Item.value = Item.buyPrice(silver: 20);
            Item.rare = ItemRarityID.LightRed;

            Item.scale = 1.5f;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, position, frame, drawColor, 0f, origin, scale * 1.5f, SpriteEffects.None, 0f);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            scale *= 1.5f;
            return true;
        }
    }
}
