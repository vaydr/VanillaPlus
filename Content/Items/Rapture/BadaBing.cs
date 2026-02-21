using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Content.Projectiles.Rapture;

namespace VanillaPlus.Content.Items.Rapture
{
    public class BadaBing : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 15;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 14;
            Item.height = 14;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.ammo = Item.type;
            Item.shoot = ModContent.ProjectileType<BadaBingProjectile>();
            Item.knockBack = 3f;
            Item.value = Item.buyPrice(copper: 50);
            Item.rare = ItemRarityID.LightRed;

            Item.ResearchUnlockCount = 99;
        }

        public override void AddRecipes()
        {
            CreateRecipe(25)
                .AddIngredient(ModContent.ItemType<RadiantShard>())
                .AddIngredient(ItemID.RocketI, 25)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
