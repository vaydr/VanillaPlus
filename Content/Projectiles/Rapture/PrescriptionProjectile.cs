using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaPlus.Content.Projectiles.Rapture
{
    public class PrescriptionProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Type] = -1f; // Infinite
            ProjectileID.Sets.YoyosMaximumRange[Type] = 416f; // 26 tiles
            ProjectileID.Sets.YoyosTopSpeed[Type] = 15f;
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
            // Venom-like particles
            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height,
                    DustID.Venom, 0f, 0f, 100, default, 0.8f);
                dust.noGravity = true;
                dust.velocity *= 0.3f;
            }
        }

        public override void PostAI()
        {
            if (Main.myPlayer != Projectile.owner)
                return;

            // Venom rain — spawn acid droplets frequently below the yoyo
            Projectile.localAI[1]++;
            if (Projectile.localAI[1] >= 5f) // every ~0.08 seconds
            {
                Projectile.localAI[1] = 0f;

                Vector2 spawnPos = Projectile.Center + new Vector2(Main.rand.NextFloat(-2.5f, 2.5f), 18f);
                Vector2 vel = new Vector2(Main.rand.NextFloat(-0.3f, 0.3f), Main.rand.NextFloat(4.8f, 7.2f));

                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    spawnPos,
                    vel,
                    ModContent.ProjectileType<VenomRainDrop>(),
                    (int)(Projectile.damage * 0.3f),
                    0f,
                    Projectile.owner,
                    ai0: Main.rand.NextFloat()
                );
            }
        }
    }
}
