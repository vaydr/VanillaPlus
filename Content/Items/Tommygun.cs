using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaPlus.Content.Items
{
    public class Tommygun : ModItem
    {
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.Minishark);
            Item.damage = 7; // Minishark is 6
            Item.useTime = 7; // Minishark is 8
            Item.useAnimation = 7;
            Item.UseSound = SoundID.Item11; // Chaingun sound
            Item.scale = 1.4f;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(0, 0);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            // Spawn bullets at the muzzle tip of the gun sprite
            // Muzzle is at the right edge of the sprite (spriteWidth/2 from center),
            // shifted back by holdoutOffset.X (-6), all multiplied by Item.scale (1.4)
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            float barrelLength = (tex.Width / 2f - 6f) * Item.scale;

            Vector2 dir = velocity;
            dir.Normalize();
            position += dir * barrelLength;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            scale *= 1.4f;
            return true;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            spriteBatch.Draw(texture, position - new Vector2(0, 4), frame, drawColor, 0f, origin, scale * 1.4f, SpriteEffects.None, 0f);
            return false;
        }
    }
}
