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
            Item.damage = 34;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 15;
            Item.width = 28;
            Item.height = 32;
            Item.useTime = 63;
            Item.useAnimation = 63;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 10f;
            Item.value = Item.sellPrice(gold: 4, silver: 20);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item15;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<RadiantBeam>();
            Item.shootSpeed = 1f;
            Item.ResearchUnlockCount = 1;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int count = Main.rand.Next(3, 5);
            float aimAngle = (Main.MouseWorld - player.Center).ToRotation();
            float halfCone = MathHelper.ToRadians(10f);

            for (int i = 0; i < count; i++)
            {
                float angle = aimAngle + Main.rand.NextFloat(-halfCone, halfCone);

                Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, damage, knockback, player.whoAmI,
                    ai0: Main.rand.NextFloat(), ai1: angle);
            }

            // Particle burst at the book
            Vector2 bookPos = player.Center + new Vector2(player.direction * 12f, -4f);
            for (int i = 0; i < 24; i++)
            {
                float dustAngle = aimAngle + Main.rand.NextFloat(-halfCone, halfCone);
                Vector2 dustVel = dustAngle.ToRotationVector2() * Main.rand.NextFloat(4f, 10f);

                int dustType = DustID.UltraBrightTorch;
                Dust dust = Dust.NewDustDirect(bookPos, 1, 1, dustType, dustVel.X, dustVel.Y, 100, default, 1.6f);
                dust.noGravity = true;
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
