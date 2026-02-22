using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaPlus.Content.Projectiles
{
    public class YellowSnowballProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.SnowBallFriendly);
            AIType = ProjectileID.SnowBallFriendly;
        }

        public override void PostAI()
        {
            Projectile.rotation = 0f;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item51, Projectile.Center);

            // Snowball impact puff — short-lived, no gravity, quick fade
            for (int i = 0; i < 7; i++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(2f, 2f);
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Snow, speed.X, speed.Y);
                d.color = new Color(255, 210, 30);
                d.noGravity = true;
                d.scale = Main.rand.NextFloat(0.6f, 1f);
                d.fadeIn = 0f;
                d.alpha = 100;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Ichor, 600);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Ichor, 600);
        }
    }
}
