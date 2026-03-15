using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaPlus.Content.Projectiles.Rapture
{
    public class RadiantBeam : ModProjectile
    {
        private static readonly Color Gold = new Color(255, 215, 80);
        private static readonly Color Warm = new Color(255, 255, 240);
        private static readonly Color SkyBlue = new Color(140, 210, 255);

        // 50% chance 18-22 tiles, 25% chance 14-18 tiles, 25% chance 10-14 tiles
        private float BeamLengthActual
        {
            get
            {
                float r = Projectile.ai[0];
                if (r < 0.25f)
                    return 160f + (r / 0.25f) * 64f;       // 10-14 tiles
                if (r < 0.5f)
                    return 224f + ((r - 0.25f) / 0.25f) * 64f; // 14-18 tiles
                return 352f + ((r - 0.5f) / 0.5f) * 64f;      // 22-26 tiles
            }
        }
        private const float MaxWidth = 14f;

        // Phase timing
        private const int TotalLife = 80;
        private const int FadeInDuration = 25;
        private const int FadeOutDuration = 15;

        private float GetPhaseFade()
        {
            int t = Projectile.timeLeft;
            int fireTick = TotalLife - FadeInDuration;
            if (t > fireTick)
                return (float)(TotalLife - t) / FadeInDuration;
            if (t > FadeOutDuration)
            {
                int ticksSinceFire = fireTick - t;
                if (ticksSinceFire < 5)
                    return 1f + (1f - ticksSinceFire / 5f) * 0.5f;
                return 1f;
            }
            return (float)t / FadeOutDuration;
        }

        private float GetWidthMult()
        {
            int t = Projectile.timeLeft;
            int fireTick = TotalLife - FadeInDuration;
            if (t > fireTick)
            {
                float progress = (float)(TotalLife - t) / FadeInDuration;
                return 0.3f + progress * 0.7f;
            }
            if (t > FadeOutDuration)
                return 1f;
            float fadeProgress = (float)t / FadeOutDuration;
            return fadeProgress * fadeProgress;
        }

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
            // Play fire sound when fade-in completes (burst moment)
            if (Projectile.timeLeft == TotalLife - FadeInDuration)
            {
                SoundEngine.PlaySound(SoundID.Item67, Projectile.Center);
            }

            float fade = GetPhaseFade();
            Vector2 dir = BeamAngle.ToRotationVector2();
            Color c = GetBeamColor(0f);

            for (float d = 0; d < BeamLengthActual; d += 48f)
            {
                Lighting.AddLight(Projectile.Center + dir * d, c.ToVector3() * 0.5f * fade);
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            // Only deal damage after the fade-in burst
            if (Projectile.timeLeft > TotalLife - FadeInDuration)
                return false;
            return null;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float point = 0f;
            Vector2 start = Projectile.Center;
            Vector2 end = start + BeamAngle.ToRotationVector2() * BeamLengthActual;
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
            float fade = GetPhaseFade();
            float widthMult = GetWidthMult();

            Vector2 dir = BeamAngle.ToRotationVector2();
            Vector2 perp = new Vector2(-dir.Y, dir.X);
            float rotation = BeamAngle;

            // Two sheens traveling at different speeds for texture
            float sheen1 = ((Main.GameUpdateCount * 0.05f + Projectile.ai[0] * 3f) % 1.6f) - 0.3f;
            float sheen2 = ((Main.GameUpdateCount * 0.03f + Projectile.ai[0] * 5f + 0.7f) % 1.6f) - 0.3f;

            // Blur offsets perpendicular to beam direction
            float[] blurOffsets = { -3f, -1.5f, 0f, 1.5f, 3f };
            float[] blurWeights = { 0.15f, 0.35f, 1f, 0.35f, 0.15f };

            float segStep = 3f;
            int segments = (int)(BeamLengthActual / segStep);

            for (int b = 0; b < blurOffsets.Length; b++)
            {
                Vector2 blurShift = perp * blurOffsets[b];
                float w = blurWeights[b];

                for (int i = 0; i <= segments; i++)
                {
                    float t = (float)i / segments;

                    // Diamond profile
                    float diamondWidth = (float)Math.Sin(t * MathHelper.Pi) * MaxWidth * widthMult;
                    if (diamondWidth < 0.5f)
                        continue;

                    Vector2 pos = Projectile.Center + dir * (t * BeamLengthActual) + blurShift - Main.screenPosition;

                    Color baseColor = GetBeamColor(t * 2f) * fade * w;

                    // Double sheen for shimmer texture
                    float sheenDist1 = Math.Abs(t - sheen1);
                    float sheenDist2 = Math.Abs(t - sheen2);
                    float sheen = Math.Max(0f, 1f - sheenDist1 * 5f) + Math.Max(0f, 1f - sheenDist2 * 7f) * 0.6f;

                    // Wide glare
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

                    // Bright core (only on center pass)
                    if (b == 2)
                    {
                        Color coreColor = baseColor * (0.8f + sheen * 0.4f);
                        Main.EntitySpriteDraw(pixel, pos, src, coreColor,
                            rotation, new Vector2(0.5f, 0.5f),
                            new Vector2(segStep + 1f, diamondWidth * 0.35f),
                            SpriteEffects.None, 0);
                    }
                }
            }
        }
    }
}
