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

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.myPlayer != Projectile.owner)
                return;

            int bubbleCount = Main.rand.Next(1, 3); // 1-2 bubbles
            int bubbleDamage = (int)(Projectile.damage * 0.75f);

            for (int i = 0; i < bubbleCount; i++)
            {
                // Aim bubbles at the hit target with some spread
                Vector2 toTarget = target.Center - Projectile.Center;
                if (toTarget != Vector2.Zero)
                    toTarget = Vector2.Normalize(toTarget) * Main.rand.NextFloat(6f, 10f);
                toTarget += new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f));

                // Bubble Gun bubble (ID 409) — visible, moves, pops on hit
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    toTarget,
                    409,
                    bubbleDamage,
                    Projectile.knockBack,
                    Projectile.owner
                );
            }
        }
    }
}
