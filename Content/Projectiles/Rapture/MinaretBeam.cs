using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaPlus.Content.Projectiles.Rapture
{
    public class MinaretBeam : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.RainbowCrystalExplosion}";

        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override bool ShouldUpdatePosition() => false;

        // ai[0] = beam angle, ai[1] = beam length, ai[2] = NPC index
        public override void AI()
        {
            int npcIndex = (int)Projectile.ai[2];
            if (npcIndex < 0 || npcIndex >= Main.maxNPCs || !Main.npc[npcIndex].active)
            {
                Projectile.Kill();
                return;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float point = 0f;
            Vector2 start = Projectile.Center;
            Vector2 end = start + Projectile.ai[0].ToRotationVector2() * Projectile.ai[1];
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 22f, ref point);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Invisible - all visuals drawn by the NPC
            return false;
        }
    }
}
