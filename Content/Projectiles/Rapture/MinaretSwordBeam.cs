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

            // Dust trail
            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height,
                    DustID.GoldFlame, 0f, 0f, 100, default, 0.8f);
                dust.noGravity = true;
                dust.velocity *= 0.3f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 origin = texture.Size() * 0.5f;

            // Fade between yellow and blue-tinted white
            float cycle = (Main.GameUpdateCount + Projectile.identity * 7) % 60 / 60f;
            Color yellow = new Color(255, 220, 100);
            Color blueWhite = new Color(200, 220, 255);
            Color drawColor = Color.Lerp(yellow, blueWhite, (float)System.Math.Sin(cycle * MathHelper.TwoPi) * 0.5f + 0.5f);
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
