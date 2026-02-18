using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using VanillaPlus.Common.Systems;

namespace VanillaPlus.Content.Projectiles.Rapture
{
    /// <summary>
    /// Golden Solution projectile - converts tiles to Rapture variants when sprayed.
    /// Used by the Clentaminator with Golden Solution ammo.
    /// </summary>
    public class GoldenSolution : ModProjectile
    {
        public ref float Progress => ref Projectile.ai[0];

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.alpha = 255; // Invisible
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 2;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            // Convert tiles at projectile position (only for the owner, server-side)
            if (Projectile.owner == Main.myPlayer)
            {
                int tileX = (int)(Projectile.position.X + Projectile.width / 2) / 16;
                int tileY = (int)(Projectile.position.Y + Projectile.height / 2) / 16;
                RaptureWorldGeneration.RaptureConvert(tileX, tileY, 2);
            }

            // Limit lifetime
            if (Projectile.timeLeft > 133)
                Projectile.timeLeft = 133;

            // Dust effects for visual spray
            int dustType = ModContent.DustType<Dusts.GoldenSolution>();

            if (Progress > 7f)
            {
                float dustScale = 1f;
                if (Progress == 8f) dustScale = 0.2f;
                else if (Progress == 9f) dustScale = 0.4f;
                else if (Progress == 10f) dustScale = 0.6f;
                else if (Progress == 11f) dustScale = 0.8f;

                Progress += 1f;

                int dust = Dust.NewDust(
                    new Vector2(Projectile.position.X, Projectile.position.Y),
                    Projectile.width,
                    Projectile.height,
                    dustType,
                    Projectile.velocity.X * 0.2f,
                    Projectile.velocity.Y * 0.2f,
                    100
                );

                Main.dust[dust].noGravity = true;
                Main.dust[dust].scale *= 1.75f;
                Main.dust[dust].velocity.X *= 2f;
                Main.dust[dust].velocity.Y *= 2f;
                Main.dust[dust].scale *= dustScale;

                // 2 in 3 chance for baby blue particles (matching Rapture water color)
                if (Main.rand.Next(3) < 2)
                {
                    Main.dust[dust].color = new Color(150, 200, 255);
                }
            }
            else
            {
                Progress += 1f;
            }

            Projectile.rotation += 0.3f * Projectile.direction;
        }
    }
}
