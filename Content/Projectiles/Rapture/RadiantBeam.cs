using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaPlus.Content.Projectiles.Rapture
{
    public class RadiantBeam : ModProjectile
    {
        private static readonly Color Gold = new Color(255, 215, 80);
        private static readonly Color Warm = new Color(255, 255, 240);
        private static readonly Color SkyBlue = new Color(140, 210, 255);

        private const float BeamLength = 300f;
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
            Projectile.timeLeft = 80;
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
            float fade = Projectile.timeLeft / 80f;
            Vector2 dir = BeamAngle.ToRotationVector2();
            Color c = GetBeamColor(0f);

            for (float d = 0; d < BeamLength; d += 48f)
            {
                Lighting.AddLight(Projectile.Center + dir * d, c.ToVector3() * 0.5f * fade);
            }
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

            // Switch to additive blending for proper light beam look
            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearClamp,
                DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            DrawBeam();

            // Restore default blending
            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp,
                DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        private void DrawBeam()
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            Rectangle src = new Rectangle(0, 0, 1, 1);
            float fade = Projectile.timeLeft / 80f;

            Vector2 dir = BeamAngle.ToRotationVector2();
            float rotation = BeamAngle;

            // Two sheens traveling at different speeds for texture
            float sheen1 = ((Main.GameUpdateCount * 0.05f + Projectile.ai[0] * 3f) % 1.6f) - 0.3f;
            float sheen2 = ((Main.GameUpdateCount * 0.03f + Projectile.ai[0] * 5f + 0.7f) % 1.6f) - 0.3f;

            float segStep = 3f;
            int segments = (int)(BeamLength / segStep);

            for (int i = 0; i <= segments; i++)
            {
                float t = (float)i / segments;

                // Diamond profile
                float diamondWidth = (float)Math.Sin(t * MathHelper.Pi) * MaxWidth;
                if (diamondWidth < 0.5f)
                    continue;

                Vector2 pos = Projectile.Center + dir * (t * BeamLength) - Main.screenPosition;

                Color baseColor = GetBeamColor(t * 2f) * fade;

                // Double sheen for shimmer texture
                float sheenDist1 = Math.Abs(t - sheen1);
                float sheenDist2 = Math.Abs(t - sheen2);
                float sheen = Math.Max(0f, 1f - sheenDist1 * 5f) + Math.Max(0f, 1f - sheenDist2 * 7f) * 0.6f;

                // Wide glare — additive makes this bloom naturally
                Color glareColor = baseColor * (0.15f + sheen * 0.25f);
                Main.EntitySpriteDraw(pixel, pos, src, glareColor,
                    rotation, new Vector2(0.5f, 0.5f),
                    new Vector2(segStep + 1f, diamondWidth * 2.2f),
                    SpriteEffects.None, 0);

                // Outer glow
                Color glowColor = baseColor * (0.3f + sheen * 0.3f);
                Main.EntitySpriteDraw(pixel, pos, src, glowColor,
                    rotation, new Vector2(0.5f, 0.5f),
                    new Vector2(segStep + 1f, diamondWidth * 1.4f),
                    SpriteEffects.None, 0);

                // Mid body
                Color midColor = baseColor * (0.5f + sheen * 0.25f);
                Main.EntitySpriteDraw(pixel, pos, src, midColor,
                    rotation, new Vector2(0.5f, 0.5f),
                    new Vector2(segStep + 1f, diamondWidth),
                    SpriteEffects.None, 0);

                // Bright core
                Color coreColor = baseColor * (0.8f + sheen * 0.4f);
                Main.EntitySpriteDraw(pixel, pos, src, coreColor,
                    rotation, new Vector2(0.5f, 0.5f),
                    new Vector2(segStep + 1f, diamondWidth * 0.35f),
                    SpriteEffects.None, 0);
            }
        }
    }
}
