using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaPlus.Content.Projectiles.Rapture
{
    public class MinaretSwordBeam : ModProjectile
    {
        private static readonly Color BananaYellow = new Color(255, 230, 80);
        private static readonly Color BabyBlue = new Color(137, 207, 240);

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = 3;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 180;
            Projectile.light = 0.4f;
            Projectile.alpha = 80;
        }

        public override void AI()
        {
            // Rotate to face movement direction
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

            // Color-cycling particle trail
            float cycle = (float)Math.Sin(Main.GameUpdateCount * 0.06f) * 0.5f + 0.5f;
            Color dustColor = Color.Lerp(BananaYellow, BabyBlue, cycle);
            for (int i = 0; i < 2; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height,
                    DustID.TintableDustLighted, 0f, 0f, 100, dustColor, 1.1f);
                dust.noGravity = true;
                dust.velocity = Projectile.velocity * -0.15f + Main.rand.NextVector2Circular(0.5f, 0.5f);
                dust.fadeIn = 1.3f;
            }
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
                float trailCycle = (float)Math.Sin((Main.GameUpdateCount - i * 4) * 0.06f) * 0.5f + 0.5f;
                Color trailColor = Color.Lerp(BananaYellow, BabyBlue, trailCycle) * (progress * 0.3f);
                trailColor.A = 0;

                Vector2 trailPos = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition;
                Main.EntitySpriteDraw(texture, trailPos, null, trailColor, Projectile.oldRot[i], origin, Projectile.scale, SpriteEffects.None, 0);
            }

            // Draw main sprite with cycling color
            float cycle = (float)Math.Sin(Main.GameUpdateCount * 0.06f) * 0.5f + 0.5f;
            Color drawColor = Color.Lerp(BananaYellow, BabyBlue, cycle);
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
    }
}
