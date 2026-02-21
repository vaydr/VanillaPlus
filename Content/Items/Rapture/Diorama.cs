using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Content.Projectiles.Rapture;

namespace VanillaPlus.Content.Items.Rapture
{
    public class Diorama : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 43;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.width = 24;
            Item.height = 24;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 3.3f;
            Item.value = Item.sellPrice(gold: 5);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item1;
            Item.channel = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<DioramaProjectile>();
            Item.shootSpeed = 16f;

            Item.ResearchUnlockCount = 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.WoodYoyo)
                .AddIngredient(ModContent.ItemType<RadiantShard>(), 15)
                .AddIngredient(ItemID.SoulofLight, 12)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
