using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaPlus.Content.Projectiles.Rapture
{
    public class LunarEclipseProjectile : ModProjectile
    {
        private static readonly Color DeepPurple = new Color(120, 40, 200);

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Type] = -1f; // Infinite
            ProjectileID.Sets.YoyosMaximumRange[Type] = 384f; // 24 tiles
            ProjectileID.Sets.YoyosTopSpeed[Type] = 17f;
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

        public override void AI()
        {
            // Purple particles + glow
            Lighting.AddLight(Projectile.Center, 0.47f, 0.16f, 0.78f);

            if (Main.rand.NextBool(2))
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height,
                    DustID.RainbowMk2, 0f, 0f, 0, DeepPurple, 1f);
                dust.noGravity = true;
                dust.velocity *= 0.5f;
            }

            // Spawn a homing cosmic orb every 0.5-1 second
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.localAI[1]++;
                if (Projectile.localAI[1] >= 45f)
                {
                    Projectile.localAI[1] = Main.rand.Next(-8, 8);
                    SpawnOrb();
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.myPlayer == Projectile.owner)
                SpawnOrb();
        }

        private void SpawnOrb()
        {
            // Launch from yoyo center with a random initial direction
            Vector2 vel = Main.rand.NextVector2CircularEdge(6f, 6f);

            Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                Projectile.Center,
                vel,
                ModContent.ProjectileType<LunarEclipseOrb>(),
                (int)(Projectile.damage * 0.6f),
                Projectile.knockBack * 0.5f,
                Projectile.owner
            );
        }
    }
}
