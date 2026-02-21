using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Content.Items.Rapture;
using VanillaPlus.Content.Projectiles.Rapture;

namespace VanillaPlus.Content.Items
{
    public class BadaBoom : ModItem
    {
        private static Asset<Texture2D> _loadedTexture;
        private static Asset<Texture2D> _defaultTexture;

        public override void SetStaticDefaults()
        {
            _loadedTexture = ModContent.Request<Texture2D>(Texture + "Loaded");
            _defaultTexture = ModContent.Request<Texture2D>(Texture);
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
            Item.shootSpeed = 12f;
            Item.useAmmo = ModContent.ItemType<BadaBing>();
            Item.autoReuse = true;

            Item.scale = 2f;

            Item.ResearchUnlockCount = 1;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10f, 0f);
        }

        public override void HoldItem(Player player)
        {
            // Swap held weapon texture to loaded version while firing
            if (player.itemAnimation > 0 && _loadedTexture != null && _loadedTexture.IsLoaded)
            {
                TextureAssets.Item[Type] = _loadedTexture;
            }
            else if (_defaultTexture != null && _defaultTexture.IsLoaded)
            {
                TextureAssets.Item[Type] = _defaultTexture;
            }
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            // Always show unloaded sprite in inventory
            if (_defaultTexture != null && _defaultTexture.IsLoaded)
            {
                spriteBatch.Draw(_defaultTexture.Value, position, null, drawColor, 0f, origin, scale * 1.5f, SpriteEffects.None, 0f);
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
