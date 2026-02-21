using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Common.Systems;
using VanillaPlus.Content.Projectiles.Rapture;

namespace VanillaPlus.Content.Items
{
    public class LunarEclipse : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 140;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.width = 24;
            Item.height = 24;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 8f;
            Item.crit = 6; // 4% base + 6% = 10% total
            Item.value = Item.sellPrice(gold: 10);
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item1;
            Item.channel = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<LunarEclipseProjectile>();
            Item.shootSpeed = 16f;

            Item.ResearchUnlockCount = 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SpectreBar, 12)
                .AddIngredient(ItemID.ShroomiteBar, 6)
                .AddRecipeGroup(RaptureRecipes.LunarFragmentGroupName, 10)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
