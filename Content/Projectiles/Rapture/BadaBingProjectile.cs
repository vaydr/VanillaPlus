using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaPlus.Content.Projectiles.Rapture
{
    public class BadaBingProjectile : ModProjectile
    {
        private static readonly Color GoldColor = new Color(255, 215, 0);
        private static readonly Color BlueColor = new Color(137, 207, 240);

        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.RocketI}";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 2;
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
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            float cycle = (float)Math.Sin(Main.GameUpdateCount * 0.1f) * 0.5f + 0.5f;

            // Smoke trail
            if (Main.rand.NextBool(2))
            {
                Dust smoke = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height,
                    DustID.Smoke, 0f, 0f, 100, default, 1f);
                smoke.noGravity = true;
                smoke.velocity *= 0.3f;
            }

            // Gold/blue fire trail (Rapture themed)
            Color dustColor = Color.Lerp(GoldColor, BlueColor, cycle);
            Dust fire = Dust.NewDustDirect(
                Projectile.position - Projectile.velocity * 0.5f,
                Projectile.width, Projectile.height,
                DustID.TintableDustLighted, 0f, 0f, 0, dustColor, 1.4f);
            fire.noGravity = true;
            fire.velocity = Projectile.velocity * -0.15f + Main.rand.NextVector2Circular(0.5f, 0.5f);
            fire.fadeIn = 1.3f;

            // Tile lighting
            Vector3 lightColor = Vector3.Lerp(
                new Vector3(1f, 0.84f, 0f),
                new Vector3(0.53f, 0.81f, 0.98f),
                cycle);
            Lighting.AddLight(Projectile.Center, lightColor * 0.6f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 origin = texture.Size() * 0.5f;

            // Draw afterimage trail
            for (int i = Projectile.oldPos.Length - 1; i > 0; i--)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                    continue;

                float progress = 1f - (float)i / Projectile.oldPos.Length;
                float trailCycle = (float)Math.Sin((Main.GameUpdateCount - i * 4) * 0.1f) * 0.5f + 0.5f;
                Color trailColor = Color.Lerp(GoldColor, BlueColor, trailCycle) * (progress * 0.4f);
                trailColor.A = 0;

                Vector2 trailPos = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition;
                Main.EntitySpriteDraw(texture, trailPos, null, trailColor, Projectile.oldRot[i], origin, Projectile.scale, SpriteEffects.None, 0);
            }

            // Draw main sprite with cycling color tint
            float cycle = (float)Math.Sin(Main.GameUpdateCount * 0.1f) * 0.5f + 0.5f;
            Color drawColor = Color.Lerp(GoldColor, BlueColor, cycle);
            drawColor.A = (byte)(255 - Projectile.alpha);

            Main.EntitySpriteDraw(
                texture,
                Projectile.Center - Main.screenPosition,
                null,
                drawColor,
                Projectile.rotation,
                origin,
                Projectile.scale,
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

            // Gold/blue fire burst (Rapture themed)
            for (int i = 0; i < 30; i++)
            {
                float angle = MathHelper.TwoPi * i / 30f;
                Vector2 vel = angle.ToRotationVector2() * Main.rand.NextFloat(2f, 6f);
                float cycle = (float)Math.Sin(i * 0.5f) * 0.5f + 0.5f;
                Color dustColor = Color.Lerp(GoldColor, BlueColor, cycle);

                Dust dust = Dust.NewDustDirect(Projectile.Center, 0, 0,
                    DustID.TintableDustLighted, vel.X, vel.Y, 0, dustColor, 2f);
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
