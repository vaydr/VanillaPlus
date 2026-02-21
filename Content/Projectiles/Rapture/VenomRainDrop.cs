using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaPlus.Content.Projectiles.Rapture
{
    public class VenomRainDrop : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.RainFriendly}";

        private Color GetAcidColor()
        {
            float shift = Projectile.ai[0];
            return new Color(
                (int)(90 + 60 * shift),
                (int)(190 + 50 * shift),
                (int)(20 + 30 * shift)
            );
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 40;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Venom, 180);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            Color color = GetAcidColor();
            Vector2 pos = Projectile.Center - Main.screenPosition;
            Vector2 streakOrigin = new Vector2(0.5f, 0f);
            float vel = Projectile.velocity.Length();

            // Main body streak — slightly thinner
            float bodyLen = vel * 5.4f;
            Main.EntitySpriteDraw(pixel, pos, new Rectangle(0, 0, 1, 1), color,
                Projectile.rotation, streakOrigin, new Vector2(2f, bodyLen), SpriteEffects.None, 0);

            // Brighter core
            Color coreColor = color * 1.3f;
            coreColor.A = 200;
            Main.EntitySpriteDraw(pixel, pos, new Rectangle(0, 0, 1, 1), coreColor,
                Projectile.rotation, streakOrigin, new Vector2(1f, vel * 4.2f), SpriteEffects.None, 0);

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            Color color = GetAcidColor();
            for (int i = 0; i < 3; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Venom,
                    Main.rand.NextVector2Circular(2f, 1f), 100, default, 0.5f);
                dust.noGravity = true;
            }
        }
    }
}
