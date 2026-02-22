using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaPlus.Content.Projectiles.Rapture
{
    public class MinaretBeamExplosion : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.Grenade}";

        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 4;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;

                if (Main.netMode != NetmodeID.Server)
                {
                    // Radial gold flame burst
                    for (int i = 0; i < 20; i++)
                    {
                        Vector2 vel = (i / 20f * MathHelper.TwoPi).ToRotationVector2() * Main.rand.NextFloat(3f, 7f);
                        Dust d = Dust.NewDustDirect(Projectile.Center - new Vector2(4), 8, 8, DustID.GoldFlame, vel.X, vel.Y);
                        d.noGravity = true;
                        d.scale = Main.rand.NextFloat(1.8f, 2.8f);
                    }

                    // Bright white-gold sparks
                    for (int i = 0; i < 8; i++)
                    {
                        Vector2 vel = Main.rand.NextVector2CircularEdge(4f, 4f);
                        Dust d = Dust.NewDustDirect(Projectile.Center - new Vector2(2), 4, 4, DustID.GoldFlame, vel.X, vel.Y);
                        d.noGravity = true;
                        d.scale = Main.rand.NextFloat(1f, 1.6f);
                        d.color = new Color(255, 255, 220);
                    }
                }

                Lighting.AddLight(Projectile.Center, 1.2f, 1f, 0.5f);
            }
        }

        public override bool PreDraw(ref Color lightColor) => false;
    }
}
