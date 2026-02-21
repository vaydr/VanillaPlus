using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Content.Projectiles.Rapture;

namespace VanillaPlus.Content.Items.Rapture
{
    public class BadaBoom : ModItem
    {
        private static Asset<Texture2D> _loadedTexture;

        public override void SetStaticDefaults()
        {
            _loadedTexture = ModContent.Request<Texture2D>(Texture + "Loaded");
        }

        public override void SetDefaults()
        {
            Item.damage = 65;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 50;
            Item.height = 20;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 7f;
            Item.value = Item.sellPrice(gold: 5);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item11;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<BadaBingProjectile>();
            Item.shootSpeed = 8f;
            Item.useAmmo = ModContent.ItemType<BadaBing>();
            Item.autoReuse = true;

            Item.ResearchUnlockCount = 1;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10f, 0f);
        }

        private static bool PlayerHasBadaBing(Player player)
        {
            int ammoType = ModContent.ItemType<BadaBing>();
            for (int i = 0; i < 58; i++)
            {
                if (player.inventory[i].type == ammoType && player.inventory[i].stack > 0)
                    return true;
            }
            return false;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (PlayerHasBadaBing(Main.LocalPlayer) && _loadedTexture != null && _loadedTexture.IsLoaded)
            {
                Texture2D tex = _loadedTexture.Value;
                spriteBatch.Draw(tex, position, null, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HallowedBar, 12)
                .AddIngredient(ModContent.ItemType<RadiantShard>(), 15)
                .AddIngredient(ItemID.SoulofLight, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
