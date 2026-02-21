using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaPlus.Content.Projectiles.Rapture
{
    public class LagunaProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Type] = -1f; // Infinite
            ProjectileID.Sets.YoyosMaximumRange[Type] = 352f; // 22 tiles
            ProjectileID.Sets.YoyosTopSpeed[Type] = 16f;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = ProjAIStyleID.Yoyo;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.penetrate = -1;
        }

        public override void PostAI()
        {
            if (Main.myPlayer != Projectile.owner)
                return;

            // Spawn a homing bubble every 20 ticks (~3x per second)
            Projectile.localAI[1] += 1f;
            if (Projectile.localAI[1] >= 20f)
            {
                Projectile.localAI[1] = 0f;

                float angle = Main.rand.NextFloat(MathHelper.TwoPi);
                Vector2 vel = angle.ToRotationVector2() * Main.rand.NextFloat(3f, 6f);
                int bubbleDamage = (int)(Projectile.damage * 0.6f);

                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    vel,
                    ProjectileID.FlaironBubble,
                    bubbleDamage,
                    Projectile.knockBack,
                    Projectile.owner
                );
            }
        }
    }
}
