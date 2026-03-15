using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace VanillaPlus.Common
{
    /// <summary>
    /// Shared beam rendering used by ArmedMinaret and RadiantBeam (Tome of Myths).
    /// Phases: 30-tick telegraph, 30-tick fire, 20-tick fade.
    /// </summary>
    public static class RaptureBeamDrawer
    {
        public const int TelegraphDuration = 30;
        public const int FireDuration = 30;
        public const int FadeDuration = 20;
        public const int TotalDuration = TelegraphDuration + FireDuration + FadeDuration;

        public struct BeamPhase
        {
            public float PhaseFade;
            public float FlashMult;
            public float WidthMult;
            public float Breathe;
            public bool IsFire;
            public bool IsFade;
            public int Elapsed;
        }

        public static BeamPhase GetPhase(int elapsed)
        {
            BeamPhase phase;
            phase.Elapsed = elapsed;
            phase.FlashMult = 1f;
            phase.WidthMult = 1f;
            phase.Breathe = 1f;
            phase.IsFire = false;
            phase.IsFade = false;

            if (elapsed < TelegraphDuration)
            {
                float progress = (float)elapsed / TelegraphDuration;
                phase.PhaseFade = 0.2f + progress * 0.3f;
                phase.WidthMult = 0.4f + progress * 0.6f;
            }
            else if (elapsed < TelegraphDuration + FireDuration)
            {
                phase.IsFire = true;
                float fireTick = elapsed - TelegraphDuration;
                if (fireTick < 5f)
                    phase.FlashMult = 1f + (1f - fireTick / 5f) * 0.8f;
                phase.PhaseFade = 1f;
                phase.Breathe = 1f + (float)Math.Sin(Main.GameUpdateCount * 0.3f) * 0.06f;
            }
            else
            {
                phase.IsFade = true;
                float progress = 1f - (float)(elapsed - TelegraphDuration - FireDuration) / FadeDuration;
                phase.PhaseFade = progress;
                phase.WidthMult = progress * progress;
            }

            return phase;
        }

        public static void DrawCircularGlow(Texture2D pixel, Rectangle src, Vector2 pos, float size, Color color)
        {
            Vector2 origin = new Vector2(0.5f, 0.5f);
            int layers = 6;
            Color layerColor = color * (2f / layers);
            for (int r = 0; r < layers; r++)
            {
                float angle = r / (float)layers * MathHelper.PiOver2;
                Main.EntitySpriteDraw(pixel, pos, src, layerColor,
                    angle, origin, new Vector2(size, size),
                    SpriteEffects.None, 0);
            }
        }

        public static void DrawBeam(Vector2 fireOrigin, float beamAngle, float beamLength, float maxWidthBase,
            BeamPhase phase, Func<float, Color> getColor, float sheenSeed = 0f)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            Rectangle src = new Rectangle(0, 0, 1, 1);
            Vector2 pixelOrigin = new Vector2(0.5f, 0.5f);

            Vector2 dir = beamAngle.ToRotationVector2();
            Vector2 perp = new Vector2(-dir.Y, dir.X);
            float rotation = beamAngle;
            float maxWidth = maxWidthBase * phase.Breathe * phase.WidthMult;
            Color baseColor = getColor(0f);

            // ═══ ORIGIN ORB ═══
            float orbPulse = 1f + (float)Math.Sin(Main.GameUpdateCount * 0.2f) * 0.12f;
            float orbBase;
            if (phase.Elapsed < TelegraphDuration)
                orbBase = 6f + (float)phase.Elapsed / TelegraphDuration * 12f;
            else if (phase.IsFire)
                orbBase = 18f;
            else
                orbBase = 18f * phase.WidthMult;
            float orbSize = orbBase * orbPulse;
            Vector2 orbPos = fireOrigin - Main.screenPosition;

            DrawCircularGlow(pixel, src, orbPos, orbSize * 2.5f, baseColor * (phase.PhaseFade * phase.FlashMult * 0.15f));
            DrawCircularGlow(pixel, src, orbPos, orbSize * 1.5f, baseColor * (phase.PhaseFade * phase.FlashMult * 0.3f));
            DrawCircularGlow(pixel, src, orbPos, orbSize * 0.7f,
                Color.Lerp(baseColor, Color.White, 0.7f) * (phase.PhaseFade * phase.FlashMult * 0.6f));

            // ═══ BEAM BODY ═══
            float sheen1 = ((Main.GameUpdateCount * 0.06f + sheenSeed * 3f) % 1.4f) - 0.2f;
            float sheen2 = ((Main.GameUpdateCount * 0.035f + sheenSeed * 5f + 0.7f) % 1.4f) - 0.2f;

            float[] blurOffsets = { -2.5f, -1f, 0f, 1f, 2.5f };
            float[] blurWeights = { 0.12f, 0.3f, 1f, 0.3f, 0.12f };

            float segStep = 3f;
            int segments = Math.Max(1, (int)(beamLength / segStep));

            for (int b = 0; b < blurOffsets.Length; b++)
            {
                Vector2 blurShift = perp * blurOffsets[b];
                float w = blurWeights[b];

                for (int i = 0; i <= segments; i++)
                {
                    float t = (float)i / segments;

                    // Forward taper — wide at origin, narrows toward end
                    float taper = (float)Math.Pow(1f - t, 0.6f);

                    // High-freq ripple + low-freq undulation
                    float noise = 1f
                        + (float)Math.Sin(t * 25f + Main.GameUpdateCount * 0.15f) * 0.08f
                        + (float)Math.Sin(t * 10f + Main.GameUpdateCount * 0.06f) * 0.05f;

                    float segWidth = maxWidth * taper * noise;
                    if (segWidth < 0.3f)
                        continue;

                    Vector2 pos = fireOrigin + dir * (t * beamLength) + blurShift - Main.screenPosition;
                    Color segColor = getColor(t * 2f) * (phase.PhaseFade * w * phase.FlashMult);

                    // Sheen shimmer during fire phase only
                    float sheenDist1 = Math.Abs(t - sheen1);
                    float sheenDist2 = Math.Abs(t - sheen2);
                    float sheen = phase.IsFire
                        ? (Math.Max(0f, 1f - sheenDist1 * 5f) + Math.Max(0f, 1f - sheenDist2 * 7f) * 0.6f)
                        : 0f;

                    // Wide glare
                    Main.EntitySpriteDraw(pixel, pos, src, segColor * (0.12f + sheen * 0.2f),
                        rotation, pixelOrigin, new Vector2(segStep + 1f, segWidth * 2.5f), SpriteEffects.None, 0);

                    // Outer glow
                    Main.EntitySpriteDraw(pixel, pos, src, segColor * (0.25f + sheen * 0.25f),
                        rotation, pixelOrigin, new Vector2(segStep + 1f, segWidth * 1.5f), SpriteEffects.None, 0);

                    // Mid body
                    Main.EntitySpriteDraw(pixel, pos, src, segColor * (0.45f + sheen * 0.2f),
                        rotation, pixelOrigin, new Vector2(segStep + 1f, segWidth), SpriteEffects.None, 0);

                    // Bright core — center blur pass only
                    if (b == 2)
                    {
                        Color coreColor = Color.Lerp(segColor, Color.White * (phase.PhaseFade * phase.FlashMult), 0.65f)
                            * (0.7f + sheen * 0.4f);
                        Main.EntitySpriteDraw(pixel, pos, src, coreColor,
                            rotation, pixelOrigin, new Vector2(segStep + 1f, segWidth * 0.3f), SpriteEffects.None, 0);
                    }
                }
            }

            // ═══ IMPACT GLOW ═══
            if (phase.IsFire || phase.IsFade)
            {
                float impactPulse = 1f + (float)Math.Sin(Main.GameUpdateCount * 0.25f) * 0.15f;
                float impactSize = 14f * phase.Breathe * impactPulse;
                if (phase.IsFade)
                {
                    float fadeProgress = 1f - (float)(phase.Elapsed - TelegraphDuration - FireDuration) / FadeDuration;
                    impactSize *= (fadeProgress > 0.8f) ? 1.4f : fadeProgress;
                }

                Vector2 endPoint = fireOrigin + dir * beamLength - Main.screenPosition;
                DrawCircularGlow(pixel, src, endPoint, impactSize * 2.2f, baseColor * (phase.PhaseFade * phase.FlashMult * 0.12f));
                DrawCircularGlow(pixel, src, endPoint, impactSize * 1.3f, baseColor * (phase.PhaseFade * phase.FlashMult * 0.25f));
                DrawCircularGlow(pixel, src, endPoint, impactSize * 0.6f,
                    Color.Lerp(baseColor, Color.White, 0.6f) * (phase.PhaseFade * phase.FlashMult * 0.5f));
            }

            // ═══ LIGHTING ═══
            if (phase.IsFire)
            {
                Vector3 lightColor = baseColor.ToVector3() * 0.8f;
                float step = 32f;
                int count = (int)(beamLength / step);
                for (int li = 0; li <= count; li++)
                    Lighting.AddLight(fireOrigin + dir * (li * step), lightColor);
            }
        }

        public static void SpawnImpactSparks(Vector2 endPos, float beamAngle, BeamPhase phase)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            if (phase.IsFire)
            {
                for (int i = 0; i < 2; i++)
                {
                    float perpAngle = beamAngle + (Main.rand.NextBool() ? MathHelper.PiOver2 : -MathHelper.PiOver2);
                    Vector2 dustVel = perpAngle.ToRotationVector2() * Main.rand.NextFloat(1.5f, 3.5f);
                    Dust d = Dust.NewDustDirect(endPos, 0, 0, DustID.GoldFlame, dustVel.X, dustVel.Y);
                    d.noGravity = true;
                    d.scale = 1.4f;
                }
            }
            else if (phase.IsFade)
            {
                float fadeProgress = 1f - (float)(phase.Elapsed - TelegraphDuration - FireDuration) / FadeDuration;
                float perpAngle = beamAngle + (Main.rand.NextBool() ? MathHelper.PiOver2 : -MathHelper.PiOver2);
                Vector2 dustVel = perpAngle.ToRotationVector2() * Main.rand.NextFloat(1f, 2.5f);
                Dust d = Dust.NewDustDirect(endPos, 0, 0, DustID.GoldFlame, dustVel.X, dustVel.Y);
                d.noGravity = true;
                d.scale = 1.2f * fadeProgress;
            }
        }

        public static void SpawnExplosionBurst(Vector2 center)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            for (int i = 0; i < 20; i++)
            {
                Vector2 vel = (i / 20f * MathHelper.TwoPi).ToRotationVector2() * Main.rand.NextFloat(3f, 7f);
                Dust d = Dust.NewDustDirect(center - new Vector2(4), 8, 8, DustID.GoldFlame, vel.X, vel.Y);
                d.noGravity = true;
                d.scale = Main.rand.NextFloat(1.8f, 2.8f);
            }

            for (int i = 0; i < 8; i++)
            {
                Vector2 vel = Main.rand.NextVector2CircularEdge(4f, 4f);
                Dust d = Dust.NewDustDirect(center - new Vector2(2), 4, 4, DustID.GoldFlame, vel.X, vel.Y);
                d.noGravity = true;
                d.scale = Main.rand.NextFloat(1f, 1.6f);
                d.color = new Color(255, 255, 220);
            }

            Lighting.AddLight(center, 1.2f, 1f, 0.5f);
        }
    }
}
