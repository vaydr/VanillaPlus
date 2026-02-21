using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Content.Projectiles.Rapture;

namespace VanillaPlus.Content.Items.Rapture
{
    public class TomeOfMyths : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 28;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 15;
            Item.width = 28;
            Item.height = 32;
            Item.useTime = 42;
            Item.useAnimation = 42;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 10f;
            Item.value = Item.sellPrice(gold: 4, silver: 20);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item117;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<RadiantBeam>();
            Item.shootSpeed = 1f;
            Item.ResearchUnlockCount = 1;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int count = Main.rand.Next(2, 6);
            float aimAngle = (Main.MouseWorld - player.Center).ToRotation();
            float halfCone = MathHelper.ToRadians(17.5f);

            for (int i = 0; i < count; i++)
            {
                float angle = aimAngle + Main.rand.NextFloat(-halfCone, halfCone);

                Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, damage, knockback, player.whoAmI,
                    ai0: Main.rand.NextFloat(), ai1: angle);
            }

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SpellTome, 1)
                .AddIngredient(ModContent.ItemType<RadiantShard>(), 20)
                .AddIngredient(ItemID.SoulofLight, 15)
                .AddTile(TileID.Bookcases)
                .Register();
        }
    }
}
