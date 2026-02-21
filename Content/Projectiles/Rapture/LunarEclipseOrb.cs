using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaPlus.Content.Projectiles.Rapture
{
    public class LunarEclipseOrb : ModProjectile
    {
        // Deep cosmic purple gradient — cycles between these
        private static readonly Color DeepPurple = new Color(80, 20, 160);
        private static readonly Color DarkViolet = new Color(140, 30, 180);
        private static readonly Color CosmicPink = new Color(160, 40, 120);

        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.NebulaArcanumSubshot}";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 14;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 240;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.alpha = 80;
        }

        private Color GetCycleColor(float timeOffset)
        {
            // Three-way gradient cycle: DeepPurple -> DarkViolet -> CosmicPink -> DeepPurple
            float cycle = (float)Math.Sin((Main.GameUpdateCount + timeOffset) * 0.05f) * 0.5f + 0.5f;
            float cycle2 = (float)Math.Sin((Main.GameUpdateCount + timeOffset) * 0.03f + 1.5f) * 0.5f + 0.5f;
            Color mid = Color.Lerp(DeepPurple, DarkViolet, cycle);
            return Color.Lerp(mid, CosmicPink, cycle2);
        }

        public override void AI()
        {
            // Home toward nearest enemy — slow and deliberate
            float homingRange = 600f;
            float homingSpeed = 6f;
            float turnSpeed = 0.08f;
            NPC target = null;
            float closestDist = homingRange;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && npc.CanBeChasedBy())
                {
                    float dist = Vector2.Distance(Projectile.Center, npc.Center);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        target = npc;
                    }
                }
            }

            if (target != null)
            {
                Vector2 desired = Vector2.Normalize(target.Center - Projectile.Center) * homingSpeed;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desired, turnSpeed);
            }

            Projectile.rotation += 0.15f;

            // Prominent cosmic dust trail
            Color tint = GetCycleColor(0f);
            for (int i = 0; i < 2; i++)
            {
                Dust dust = Dust.NewDustPerfect(
                    Projectile.Center + Main.rand.NextVector2Circular(6f, 6f),
                    DustID.RainbowMk2,
                    Projectile.velocity * -0.4f + Main.rand.NextVector2Circular(1.5f, 1.5f),
                    0, tint, 1.3f);
                dust.noGravity = true;
                dust.fadeIn = 1.5f;
            }

            // Starry sparkle particles
            if (Main.rand.NextBool(2))
            {
                Color sparkle = GetCycleColor(Main.rand.NextFloat(100f));
                Dust star = Dust.NewDustPerfect(
                    Projectile.Center + Main.rand.NextVector2Circular(12f, 12f),
                    DustID.TintableDustLighted,
                    Main.rand.NextVector2Circular(2f, 2f),
                    0, sparkle, 0.8f);
                star.noGravity = true;
            }

            // Cosmic glow
            Color lightTint = GetCycleColor(0f);
            Lighting.AddLight(Projectile.Center, lightTint.ToVector3() * 0.6f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 origin = texture.Size() * 0.5f;

            // Draw afterimage trail — prominent, cycling gradient
            for (int i = Projectile.oldPos.Length - 1; i > 0; i--)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                    continue;

                float progress = 1f - (float)i / Projectile.oldPos.Length;
                Color trailColor = GetCycleColor(i * 6f) * (progress * 0.5f);
                trailColor.A = 0;

                Vector2 trailPos = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition;
                float trailScale = 1.2f * (0.5f + 0.5f * progress);
                Main.EntitySpriteDraw(texture, trailPos, null, trailColor, Projectile.oldRot[i], origin, trailScale, SpriteEffects.None, 0);
            }

            // Main sprite — deep purple tinted
            Color mainColor = GetCycleColor(0f) * 0.9f;
            mainColor.A = 80;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null,
                mainColor, Projectile.rotation, origin, 1.2f, SpriteEffects.None, 0);

            // Additive cosmic glow layer
            Color glowColor = GetCycleColor(20f) * 0.35f;
            glowColor.A = 0;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null,
                glowColor, -Projectile.rotation * 0.6f, origin, 1.6f, SpriteEffects.None, 0);

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            // Cosmic explosion burst
            for (int i = 0; i < 20; i++)
            {
                Color tint = GetCycleColor(i * 8f);
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.RainbowMk2,
                    Main.rand.NextVector2Circular(6f, 6f), 0, tint, 1.6f);
                dust.noGravity = true;
                dust.fadeIn = 1.8f;
            }

            // Starry sparkles on death
            for (int i = 0; i < 8; i++)
            {
                Color sparkle = GetCycleColor(i * 12f);
                Dust star = Dust.NewDustPerfect(Projectile.Center, DustID.TintableDustLighted,
                    Main.rand.NextVector2Circular(4f, 4f), 0, sparkle, 1f);
                star.noGravity = true;
            }
        }
    }
}
