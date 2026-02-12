using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaPlus.Content.Projectiles
{
	public class PurpleTerraBeam : ModProjectile
	{
		// Use a white/bright projectile texture that can be tinted any color
		public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.RainbowCrystalExplosion}";

		public override void SetDefaults()
		{
			Projectile.width = 8;
			Projectile.height = 8;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.penetrate = 3;
			Projectile.timeLeft = 300;
			Projectile.aiStyle = -1;
			Projectile.extraUpdates = 2;
			Projectile.tileCollide = true;
			Projectile.ignoreWater = true;
			Projectile.scale = 1.5f;

			// Use local immunity so each projectile can hit independently
			// Without this, only one projectile per swing would deal damage
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 10; // Short cooldown between hits from same projectile
		}

		// ai[0] is set by the sword with its current hue

		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

			float hue = Projectile.ai[0];
			Color c = Main.hslToRgb(hue, 1f, 0.5f);
			Lighting.AddLight(Projectile.Center, c.R / 255f * 0.8f, c.G / 255f * 0.8f, c.B / 255f * 0.8f);

			if (Main.rand.NextBool(2))
			{
				Color dustColor = Main.hslToRgb(hue, 1f, 0.5f);
				Vector2 dustPos = Projectile.Center + Main.rand.NextVector2Circular(4f, 4f);
				Dust dust = Dust.NewDustPerfect(dustPos, DustID.RainbowMk2, Vector2.Zero, 0, dustColor, 1.2f);
				dust.noGravity = true;
				dust.velocity = Projectile.velocity * 0.1f;
			}
		}

		public override bool PreDraw(ref Color lightColor)
		{
			float hue = Projectile.ai[0];
			Color rainbow = Main.hslToRgb(hue, 1f, 0.6f);

			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
			Vector2 drawPos = Projectile.Center - Main.screenPosition;
			Rectangle sourceRect = texture.Bounds;
			Vector2 origin = texture.Size() / 2f;

			// Draw with full rainbow color (no multiplication with texture)
			Main.EntitySpriteDraw(
				texture,
				drawPos,
				sourceRect,
				rainbow,
				Projectile.rotation,
				origin,
				Projectile.scale,
				SpriteEffects.None,
				0
			);

			return false; // Don't draw normally
		}

		public override void OnKill(int timeLeft)
		{
			float hue = Projectile.ai[0];
			Color explosionColor = Main.hslToRgb(hue, 1f, 0.5f);

			// Lunar Flare style sound
			SoundEngine.PlaySound(SoundID.Item89, Projectile.Center);

			// Colorful explosion dust
			for (int i = 0; i < 15; i++)
			{
				Vector2 dustVel = Main.rand.NextVector2Circular(6f, 6f);
				Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.RainbowMk2, dustVel, 0, explosionColor, 2f);
				dust.noGravity = true;
			}

			// Bright light flash
			Lighting.AddLight(Projectile.Center, explosionColor.R / 255f, explosionColor.G / 255f, explosionColor.B / 255f);
		}
	}
}
