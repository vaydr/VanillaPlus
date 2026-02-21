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

            // Occasionally spawn a shadow fireball while spinning (~every 2 seconds)
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.localAI[1]++;
                if (Projectile.localAI[1] >= 60)
                {
                    Projectile.localAI[1] = 0;
                    SpawnShadowFireball();
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Spawn a shadow fireball on every hit
            if (Main.myPlayer == Projectile.owner)
                SpawnShadowFireball();
        }

        private void SpawnShadowFireball()
        {
            // Find nearest enemy to aim at
            Vector2 velocity = new Vector2(Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-4f, 4f));
            float closestDist = 600f;
            NPC closestNPC = null;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && npc.CanBeChasedBy())
                {
                    float dist = Vector2.Distance(Projectile.Center, npc.Center);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        closestNPC = npc;
                    }
                }
            }

            if (closestNPC != null)
            {
                velocity = Vector2.Normalize(closestNPC.Center - Projectile.Center) * 8f;
            }

            // Projectile 468 = CultistBossFireBallClone (homing shadow fireball)
            int idx = Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                Projectile.Center,
                velocity,
                468,
                Projectile.damage,
                Projectile.knockBack,
                Projectile.owner
            );
            // Vanilla projectile 468 is hostile by default — flip it to friendly
            if (idx >= 0 && idx < Main.maxProjectiles)
            {
                Main.projectile[idx].friendly = true;
                Main.projectile[idx].hostile = false;
            }
        }
    }
}
