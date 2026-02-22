using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaPlus.Content.Items
{
    public class YellowSnowball : ModItem
    {
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.Snowball);
            Item.shoot = ModContent.ProjectileType<Projectiles.YellowSnowballProjectile>();
            Item.ammo = Item.type;
        }

        public override void AddRecipes()
        {
            CreateRecipe(25)
                .AddIngredient(ItemID.Snowball, 25)
                .AddIngredient(ItemID.Ichor)
                .Register();
        }
    }
}
