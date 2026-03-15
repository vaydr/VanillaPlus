using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Common;

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
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                RaptureBeamDrawer.SpawnExplosionBurst(Projectile.Center);
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.immuneTime = 40;
            target.hurtCooldowns[ImmunityCooldownID.Bosses] = 40;
        }

        public override bool PreDraw(ref Color lightColor) => false;
    }
}
