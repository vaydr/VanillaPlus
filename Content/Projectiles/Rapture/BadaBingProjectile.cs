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
            Projectile.width = 14;
            Projectile.height = 14;
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
            // Point in direction of travel, offset 90 degrees CCW
            Projectile.rotation = Projectile.velocity.ToRotation();

            // Rocket fire particles (vanilla rocket style)
            if (Main.rand.NextBool(2))
            {
                Dust smoke = Dust.NewDustDirect(
                    Projectile.position - Projectile.velocity * 0.5f,
                    Projectile.width, Projectile.height,
                    DustID.Smoke, 0f, 0f, 100, default, 1.2f);
                smoke.noGravity = true;
                smoke.velocity *= 0.3f;
            }

            // Rocket fire trail
            for (int i = 0; i < 2; i++)
            {
                Dust fire = Dust.NewDustDirect(
                    Projectile.position - Projectile.velocity * 0.5f,
                    Projectile.width, Projectile.height,
                    DustID.Torch, 0f, 0f, 100, default, 1.4f);
                fire.noGravity = true;
                fire.velocity = Projectile.velocity * -0.3f + Main.rand.NextVector2Circular(1f, 1f);
            }

            // Warm orange tile lighting
            Lighting.AddLight(Projectile.Center, new Vector3(1f, 0.6f, 0.2f) * 0.6f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = _ammoTexture != null && _ammoTexture.IsLoaded
                ? _ammoTexture.Value
                : TextureAssets.Projectile[Type].Value;
            Vector2 origin = texture.Size() * 0.5f;

            // Draw afterimage trail
            for (int i = Projectile.oldPos.Length - 1; i > 0; i--)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                    continue;

                float progress = 1f - (float)i / Projectile.oldPos.Length;
                Color trailColor = lightColor * (progress * 0.35f);
                trailColor.A = 0;

                Vector2 trailPos = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition;
                Main.EntitySpriteDraw(texture, trailPos, null, trailColor, Projectile.oldRot[i], origin, DrawScale * (0.7f + 0.3f * progress), SpriteEffects.None, 0);
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
                SpriteEffects.None,
                0
            );

            return false;
        }

        public override void OnKill(int timeLeft)
        {
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
