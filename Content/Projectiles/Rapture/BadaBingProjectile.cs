using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaPlus.Content.Projectiles.Rapture
{
    public class BadaBingProjectile : ModProjectile
    {
        private const float DrawScale = 2.4f;

        private static Asset<Texture2D> _ammoTexture;

        public override string Texture => "VanillaPlus/Content/Items/BadaBing";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            _ammoTexture = ModContent.Request<Texture2D>("VanillaPlus/Content/Items/BadaBing");
        }

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 26;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            // Tail position: back end of the rocket sprite
            Vector2 velDir = Projectile.velocity.SafeNormalize(Vector2.Zero);
            Vector2 tailPos = Projectile.Center - velDir * 18f;

            // === Rocket exhaust effect ===

            // 1) Bright inner flame core (white-hot center)
            for (int i = 0; i < 2; i++)
            {
                Dust core = Dust.NewDustPerfect(
                    tailPos + Main.rand.NextVector2Circular(2f, 2f),
                    DustID.WhiteTorch,
                    -velDir * Main.rand.NextFloat(3f, 6f) + Main.rand.NextVector2Circular(0.8f, 0.8f),
                    0, default, 1.3f);
                core.noGravity = true;
                core.fadeIn = 0.8f;
            }

            // 2) Mid flame (orange fire, wider spread)
            for (int i = 0; i < 3; i++)
            {
                Dust flame = Dust.NewDustPerfect(
                    tailPos + Main.rand.NextVector2Circular(4f, 4f),
                    DustID.Torch,
                    -velDir * Main.rand.NextFloat(2f, 5f) + Main.rand.NextVector2Circular(1.5f, 1.5f),
                    50, default, Main.rand.NextFloat(1.4f, 2f));
                flame.noGravity = true;
            }

            // 3) Outer flame (dimmer, larger, more spread)
            if (Main.rand.NextBool(2))
            {
                Dust outer = Dust.NewDustPerfect(
                    tailPos + Main.rand.NextVector2Circular(5f, 5f),
                    DustID.Torch,
                    -velDir * Main.rand.NextFloat(1f, 3f) + Main.rand.NextVector2Circular(2.5f, 2.5f),
                    100, default, Main.rand.NextFloat(1.8f, 2.5f));
                outer.noGravity = true;
                outer.fadeIn = 1.8f;
            }

            // 4) Smoke trail (drifts behind, slight gravity)
            if (Main.rand.NextBool(3))
            {
                Dust smoke = Dust.NewDustPerfect(
                    tailPos + Main.rand.NextVector2Circular(6f, 6f),
                    DustID.Smoke,
                    -velDir * Main.rand.NextFloat(0.5f, 2f) + Main.rand.NextVector2Circular(1f, 1f),
                    120, default, Main.rand.NextFloat(1.5f, 2.2f));
                smoke.noGravity = false;
                smoke.velocity.Y -= 0.3f;
            }

            // 5) Tiny ember sparks that scatter
            if (Main.rand.NextBool(4))
            {
                Dust ember = Dust.NewDustPerfect(
                    tailPos + Main.rand.NextVector2Circular(3f, 3f),
                    DustID.Torch,
                    -velDir * Main.rand.NextFloat(4f, 8f) + Main.rand.NextVector2Circular(3f, 3f),
                    0, default, 0.7f);
                ember.noGravity = false;
            }

            // Warm lighting at exhaust point
            Lighting.AddLight(tailPos, new Vector3(1f, 0.7f, 0.3f) * 0.8f);
            Lighting.AddLight(Projectile.Center, new Vector3(1f, 0.5f, 0.15f) * 0.4f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = _ammoTexture != null && _ammoTexture.IsLoaded
                ? _ammoTexture.Value
                : TextureAssets.Projectile[Type].Value;
            Vector2 origin = texture.Size() * 0.5f;
            SpriteEffects flip = Projectile.velocity.X < 0f ? SpriteEffects.FlipVertically : SpriteEffects.None;

            // Draw afterimage trail
            for (int i = Projectile.oldPos.Length - 1; i > 0; i--)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                    continue;

                float progress = 1f - (float)i / Projectile.oldPos.Length;
                Color trailColor = lightColor * (progress * 0.35f);
                trailColor.A = 0;

                Vector2 trailPos = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition;
                Main.EntitySpriteDraw(texture, trailPos, null, trailColor, Projectile.oldRot[i], origin, DrawScale * (0.7f + 0.3f * progress), flip, 0);
            }

            // Draw main sprite with normal lighting (no color cycling)
            Main.EntitySpriteDraw(
                texture,
                Projectile.Center - Main.screenPosition,
                null,
                lightColor,
                Projectile.rotation,
                origin,
                DrawScale,
                flip,
                0
            );

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            // AoE explosion - 80% damage to nearby enemies
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero,
                    ModContent.ProjectileType<BadaBingExplosion>(), (int)(Projectile.damage * 0.8f),
                    Projectile.knockBack * 0.5f, Projectile.owner);
            }

            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

            // Smoke burst
            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height,
                    DustID.Smoke, 0f, 0f, 100, default, 2f);
                dust.velocity *= 1.4f;
            }

            // Fire burst
            for (int i = 0; i < 30; i++)
            {
                float angle = MathHelper.TwoPi * i / 30f;
                Vector2 vel = angle.ToRotationVector2() * Main.rand.NextFloat(2f, 6f);

                Dust dust = Dust.NewDustDirect(Projectile.Center, 0, 0,
                    DustID.Torch, vel.X, vel.Y, 100, default, 2f);
                dust.noGravity = true;
            }

            // Smoke gores
            for (int i = 0; i < 3; i++)
            {
                Gore.NewGore(
                    Projectile.GetSource_Death(),
                    Projectile.Center,
                    new Vector2(Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f)),
                    Main.rand.Next(61, 64),
                    0.8f);
            }
        }
    }
}
