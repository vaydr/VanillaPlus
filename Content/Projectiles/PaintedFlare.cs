using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Common.GlobalItems;

namespace VanillaPlus.Content.Projectiles
{
	public class PaintedFlare : ModProjectile
	{
		// Use a white/bright texture that can be tinted any color
		public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.RainbowCrystalExplosion}";

		// Store paint data in instance fields (ai[] gets overwritten by vanilla AI)
		private int _paintType;
		private int _specialFlag;
		private bool _initialized;

		public override void SetDefaults()
		{
			Projectile.width = 6;
			Projectile.height = 6;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 36000; // 10 minutes like vanilla flare
			Projectile.scale = 0.8f;
			Projectile.tileCollide = true;
		}

		public override void AI()
		{
			// Capture paint data on first frame before anything can overwrite it
			if (!_initialized)
			{
				_paintType = (int)Projectile.ai[0];
				_specialFlag = (int)Projectile.ai[1];
				_initialized = true;
			}

			// Simple flare physics - gravity and drag
			Projectile.velocity.Y += 0.15f; // Gravity
			if (Projectile.velocity.Y > 16f)
				Projectile.velocity.Y = 16f;

			Projectile.velocity.X *= 0.99f; // Air drag

			// Rotation based on velocity
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

			// Get paint color for lighting
			Color paintColor = GetPaintColor();

			// Add colored lighting
			float lightIntensity = 0.8f;

			if (_specialFlag == 1) // Illuminant - extra bright
				lightIntensity = 1.4f;
			else if (_specialFlag == 4) // Shadow - very dim
				lightIntensity = 0.2f;
			else if (_specialFlag == 2) // Echo - dim
				lightIntensity = 0.3f;

			Lighting.AddLight(
				Projectile.Center,
				paintColor.R / 255f * lightIntensity,
				paintColor.G / 255f * lightIntensity,
				paintColor.B / 255f * lightIntensity
			);

			// Colored dust trail
			if (_specialFlag != 2 && Main.rand.NextBool(2)) // No dust for echo
			{
				Color dustColor = _specialFlag == 4 ? new Color(30, 30, 40) : paintColor;

				Dust dust = Dust.NewDustPerfect(
					Projectile.Center + Main.rand.NextVector2Circular(4f, 4f),
					DustID.RainbowMk2,
					Projectile.velocity * 0.1f,
					0,
					dustColor,
					1.2f
				);
				dust.noGravity = true;
			}
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			// Stick to tiles like vanilla flare
			Projectile.velocity = Vector2.Zero;
			return false; // Don't die
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Color paintColor = GetPaintColor();

			// Apply special effects
			if (_specialFlag == 4) // Shadow paint
			{
				paintColor = new Color(30, 30, 40);
			}
			else if (_specialFlag == 3) // Negative paint - bright white with inverted feel
			{
				paintColor = new Color(
					255 - paintColor.R,
					255 - paintColor.G,
					255 - paintColor.B,
					255
				);
			}
			else if (_specialFlag == 2) // Echo coating - semi-transparent
			{
				paintColor = paintColor * 0.3f;
			}
			else if (_specialFlag == 1) // Illuminant - full brightness
			{
				paintColor = Color.Lerp(paintColor, Color.White, 0.4f);
			}

			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
			Vector2 drawPos = Projectile.Center - Main.screenPosition;
			Rectangle sourceRect = texture.Bounds;
			Vector2 origin = texture.Size() / 2f;

			// Draw a glow layer first (slightly larger, more transparent)
			Main.EntitySpriteDraw(
				texture,
				drawPos,
				sourceRect,
				paintColor * 0.5f,
				Projectile.rotation,
				origin,
				Projectile.scale * 1.5f,
				SpriteEffects.None,
				0
			);

			// Draw the main flare
			Main.EntitySpriteDraw(
				texture,
				drawPos,
				sourceRect,
				paintColor,
				Projectile.rotation,
				origin,
				Projectile.scale,
				SpriteEffects.None,
				0
			);

			return false; // Don't draw vanilla
		}

		public override void OnKill(int timeLeft)
		{
			Color paintColor = GetPaintColor();

			// Apply special effect colors
			if (_specialFlag == 4) // Shadow
				paintColor = new Color(30, 30, 40);

			// Echo coating has minimal death effect
			if (_specialFlag == 2)
			{
				for (int i = 0; i < 3; i++)
				{
					Dust dust = Dust.NewDustPerfect(
						Projectile.Center + Main.rand.NextVector2Circular(8f, 8f),
						DustID.RainbowMk2,
						Main.rand.NextVector2Circular(2f, 2f),
						0,
						paintColor * 0.3f,
						0.8f
					);
					dust.noGravity = true;
				}
				return;
			}

			// Colored explosion dust - use RainbowMk2 for proper coloring
			for (int i = 0; i < 15; i++)
			{
				Vector2 dustVel = Main.rand.NextVector2Circular(5f, 5f);
				Dust dust = Dust.NewDustPerfect(
					Projectile.Center,
					DustID.RainbowMk2,
					dustVel,
					0,
					paintColor,
					1.8f
				);
				dust.noGravity = true;
			}

			// Light flash
			float flashIntensity = _specialFlag == 1 ? 1.5f : 1f; // Illuminant is brighter
			Lighting.AddLight(
				Projectile.Center,
				paintColor.R / 255f * flashIntensity,
				paintColor.G / 255f * flashIntensity,
				paintColor.B / 255f * flashIntensity
			);
		}

		private Color GetPaintColor()
		{
			return FlareGunGlobalItem.GetPaintColor(_paintType);
		}
	}
}
