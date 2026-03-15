using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Common;

namespace VanillaPlus.Content.Projectiles.Rapture
{
    public class RadiantBeam : ModProjectile
    {
        private static readonly Color Gold = new Color(255, 215, 80);
        private static readonly Color Warm = new Color(255, 255, 240);
        private static readonly Color SkyBlue = new Color(140, 210, 255);

        // Max beam reach before LaserScan — randomized at spawn via ai[0]
        private float BeamLengthMax
        {
            get
            {
                float r = Projectile.ai[0];
                if (r < 0.25f)
                    return 160f + (r / 0.25f) * 64f;
                if (r < 0.5f)
                    return 224f + ((r - 0.25f) / 0.25f) * 64f;
                return 352f + ((r - 0.5f) / 0.5f) * 64f;
            }
        }

        // Actual beam length after tile collision scan
        private float BeamLength => Projectile.localAI[1];

        private const float MaxWidth = 14f;

        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.RainbowCrystalExplosion}";

        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = RaptureBeamDrawer.TotalDuration;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        private float BeamAngle => Projectile.ai[1];

        private Color GetBeamColor(float offset)
        {
            float t = Projectile.ai[0];
            float cycle = (float)Math.Sin(Main.GameUpdateCount * 0.08f + t * MathHelper.TwoPi + offset) * 0.5f + 0.5f;
            float cycle2 = (float)Math.Sin(Main.GameUpdateCount * 0.05f + t * MathHelper.TwoPi + offset + 1.5f) * 0.5f + 0.5f;
            Color mid = Color.Lerp(Gold, Warm, cycle);
            return Color.Lerp(mid, SkyBlue, cycle2);
        }

        public override void AI()
        {
            // First tick: LaserScan to find beam length (stops at tiles)
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                Vector2 dir = BeamAngle.ToRotationVector2();
                float[] samples = new float[3];
                Collision.LaserScan(Projectile.Center, dir, 0f, BeamLengthMax, samples);
                Projectile.localAI[1] = (samples[0] + samples[1] + samples[2]) / 3f;
            }

            int elapsed = RaptureBeamDrawer.TotalDuration - Projectile.timeLeft;

            if (elapsed == RaptureBeamDrawer.TelegraphDuration)
                SoundEngine.PlaySound(SoundID.Item67, Projectile.Center);

            var phase = RaptureBeamDrawer.GetPhase(elapsed);
            Vector2 beamDir = BeamAngle.ToRotationVector2();
            Vector2 endPos = Projectile.Center + beamDir * BeamLength;

            if (elapsed >= RaptureBeamDrawer.TelegraphDuration)
                RaptureBeamDrawer.SpawnImpactSparks(endPos, BeamAngle, phase);

            if (elapsed == RaptureBeamDrawer.TelegraphDuration + RaptureBeamDrawer.FireDuration)
                RaptureBeamDrawer.SpawnExplosionBurst(endPos);
        }

        public override bool? CanHitNPC(NPC target)
        {
            int elapsed = RaptureBeamDrawer.TotalDuration - Projectile.timeLeft;
            if (elapsed < RaptureBeamDrawer.TelegraphDuration || elapsed >= RaptureBeamDrawer.TelegraphDuration + RaptureBeamDrawer.FireDuration)
                return false;
            return null;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float point = 0f;
            Vector2 start = Projectile.Center;
            Vector2 end = start + BeamAngle.ToRotationVector2() * BeamLength;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 16f, ref point);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch sb = Main.spriteBatch;

            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearClamp,
                DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            int elapsed = RaptureBeamDrawer.TotalDuration - Projectile.timeLeft;
            var phase = RaptureBeamDrawer.GetPhase(elapsed);
            RaptureBeamDrawer.DrawBeam(Projectile.Center, BeamAngle, BeamLength, MaxWidth, phase, GetBeamColor, Projectile.ai[0]);

            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp,
                DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}
