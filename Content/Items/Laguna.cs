using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Content.Projectiles.Rapture;

namespace VanillaPlus.Content.Items
{
    public class Laguna : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 88;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.width = 24;
            Item.height = 24;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2f;
            Item.value = Item.sellPrice(gold: 5);
            Item.rare = ItemRarityID.Cyan;
            Item.UseSound = SoundID.Item1;
            Item.channel = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<LagunaProjectile>();
            Item.shootSpeed = 16f;

            Item.ResearchUnlockCount = 1;
        }
    }
}
