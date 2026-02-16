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
		// Store paint data in instance fields - captured in OnSpawn before AI touches ai[]
		private int _paintType;
		private int _specialFlag;

		private bool _stuck;
		private float _stuckRotation;
		private int _frameCounter;
		private bool _passingThrough;
		private float _distanceAfterHit;

		// CONFIGURABLE: Frames before gravity starts (set to 0 for immediate gravity)
		private const int HangFrames = 15;

		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.Flare);
			// CRITICAL: Disable vanilla AI and automatic light - otherwise orange effects still happen
			Projectile.aiStyle = -1;
			Projectile.light = 0f;
		}

		public override void OnSpawn(Terraria.DataStructures.IEntitySource source)
		{
			// Capture paint data immediately on spawn, before AI runs
			_paintType = (int)Projectile.ai[0];
			_specialFlag = (int)Projectile.ai[1];

			// Clear ai[] so vanilla flare AI starts fresh
			Projectile.ai[0] = 0f;
			Projectile.ai[1] = 0f;
		}

		public override void AI()
		{
			Color paintColor = GetPaintColor();

			// Lighting
			float lightIntensity = 0.8f;
			if (_specialFlag == 1) lightIntensity = 1.4f;
			else if (_specialFlag == 4) lightIntensity = 0.2f;
			else if (_specialFlag == 2) lightIntensity = 0.3f;

			Lighting.AddLight(Projectile.Center,
				paintColor.R / 255f * lightIntensity,
				paintColor.G / 255f * lightIntensity,
				paintColor.B / 255f * lightIntensity);

			// Trailing dust behind the flare
			if (_specialFlag != 2)
			{
				if (!_stuck)
				{
					// Trail behind the flare (opposite of velocity) - spawn 2 smaller particles per frame
					for (int i = 0; i < 2; i++)
					{
						Vector2 trailPos = Projectile.Center - Vector2.Normalize(Projectile.velocity) * 8f;
						trailPos += Main.rand.NextVector2Circular(3f, 3f);
						Dust dust = Dust.NewDustPerfect(trailPos, DustID.RainbowMk2,
							-Projectile.velocity * 0.05f + Main.rand.NextVector2Circular(0.5f, 0.5f),
							0, paintColor, 0.7f);
						dust.noGravity = true;
					}
				}
				else if (_stuck)
				{
					// Sparks shooting out from the back of the stuck flare - 2 smaller particles per frame
					// _stuckRotation points the tip direction, so back is opposite
					float backAngle = _stuckRotation - MathHelper.PiOver2 + MathHelper.Pi;
					Vector2 backDir = backAngle.ToRotationVector2();
					Vector2 spawnPos = Projectile.Center + backDir * 6f;

					for (int i = 0; i < 2; i++)
					{
						// Shoot particles outward from back with some spread
						float spread = Main.rand.NextFloat(-0.256f, 0.256f);
						Vector2 dustVel = (backAngle + spread).ToRotationVector2() * Main.rand.NextFloat(3.2f, 4.8f);

						Dust dust = Dust.NewDustPerfect(spawnPos + Main.rand.NextVector2Circular(2f, 2f), DustID.RainbowMk2, dustVel, 0, paintColor, 0.7f);
						dust.noGravity = true;
					}
				}
			}

			if (_stuck)
			{
				Projectile.velocity = Vector2.Zero;
				Projectile.rotation = _stuckRotation;
				Projectile.friendly = false; // Stop dealing damage when stuck
				return;
			}

			// Track distance after first tile hit, re-enable collision after 16 pixels
			if (_passingThrough)
			{
				_distanceAfterHit += Projectile.velocity.Length();
				if (_distanceAfterHit >= 16f)
					Projectile.tileCollide = true;
			}

			_frameCounter++;

			// Physics - gravity only starts after HangFrames
			if (_frameCounter > HangFrames)
				Projectile.velocity.Y += 0.06f;
			if (Projectile.velocity.Y > 18f)
				Projectile.velocity.Y = 18f;
			Projectile.velocity.X *= 1f;
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			_stuckRotation = Projectile.rotation; // Save for when we stick

			// Shimmer bounce
			if (Projectile.shimmerWet)
			{
				Projectile.velocity.Y = -System.Math.Abs(Projectile.velocity.Y);
				Projectile.shimmerWet = false;
			}
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			// First hit: pass through, disable collision, track distance in AI
			if (!_passingThrough)
			{
				_passingThrough = true;
				Projectile.tileCollide = false;
				return false;
			}

			// Second hit: stop
			_stuck = true;
			Projectile.velocity = Vector2.Zero;
			return false;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.AddBuff(BuffID.OnFire, Main.rand.NextBool(3) ? 600 : 300);
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Color paintColor = GetPaintColor();

			// Apply special effects
			if (_specialFlag == 4) // Shadow paint
				paintColor = new Color(30, 30, 40);
			else if (_specialFlag == 3) // Negative paint
				paintColor = new Color(255 - paintColor.R, 255 - paintColor.G, 255 - paintColor.B, 255);
			else if (_specialFlag == 2) // Echo coating - semi-transparent
				paintColor = paintColor * 0.3f;
			else if (_specialFlag == 1) // Illuminant - full brightness
				paintColor = Color.Lerp(paintColor, Color.White, 0.4f);

			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
			Vector2 drawPos = Projectile.Center - Main.screenPosition;
			Rectangle sourceRect = texture.Bounds;
			Vector2 origin = texture.Size() / 2f;

			// Draw flare with paint color
			Main.EntitySpriteDraw(
				texture, drawPos, sourceRect,
				paintColor,
				Projectile.rotation, origin,
				Projectile.scale,
				SpriteEffects.None, 0
			);

			return false;
		}

		private Color GetPaintColor()
		{
			return FlareGunGlobalItem.GetPaintColor(_paintType);
		}
	}
}
