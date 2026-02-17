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
		private int _paintType;
		private int _specialFlag;
		private bool _stuck;
		private float _stuckRotation;
		private int _frameCounter;
		private bool _lodging;
		private Vector2 _lodgeVelocity;
		private float _lodgeDistance;

		private const int HangFrames = 15;

		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.Flare);
			Projectile.aiStyle = -1;
			Projectile.light = 0f;
		}

		public override void OnSpawn(Terraria.DataStructures.IEntitySource source)
		{
			_paintType = (int)Projectile.ai[0];
			_specialFlag = (int)Projectile.ai[1];
			Projectile.ai[0] = 0f;
			Projectile.ai[1] = 0f;
		}

		public override void AI()
		{
			Color paintColor = GetPaintColor();

			// Emit colored light
			float lightIntensity = _specialFlag switch
			{
				1 => 1.4f,  // Illuminant
				2 => 0.3f,  // Echo
				4 => 0.2f,  // Shadow
				_ => 0.8f
			};
			Lighting.AddLight(Projectile.Center,
				paintColor.R / 255f * lightIntensity,
				paintColor.G / 255f * lightIntensity,
				paintColor.B / 255f * lightIntensity);

			// Dust particles (skip for Echo coating)
			if (_specialFlag != 2)
				SpawnDust(paintColor);

			// Lodging into tile - move along captured velocity for 2 pixels
			if (_lodging)
			{
				// Stop immediately if we hit liquid
				if (Collision.WetCollision(Projectile.position, Projectile.width, Projectile.height))
				{
					_stuck = true;
					_lodging = false;
					return;
				}

				float remainingDistance = 2f - _lodgeDistance;
				if (remainingDistance <= 0f)
				{
					_stuck = true;
					_lodging = false;
					return;
				}

				// Move at most the remaining distance, never more
				Vector2 direction = Vector2.Normalize(_lodgeVelocity);
				float moveAmount = System.Math.Min(_lodgeVelocity.Length(), remainingDistance);
				Projectile.position += direction * moveAmount;
				_lodgeDistance += moveAmount;
				Projectile.rotation = _stuckRotation;

				if (_lodgeDistance >= 2f)
				{
					_stuck = true;
					_lodging = false;
				}
				return;
			}

			// Stuck behavior
			if (_stuck)
			{
				// Check if the tile we're stuck in still exists
				Vector2 checkPos = Projectile.Center + (_stuckRotation - MathHelper.PiOver2).ToRotationVector2() * 8f;
				if (!Collision.SolidCollision(checkPos, 1, 1))
				{
					// Tile is gone, unstick and resume physics
					_stuck = false;
					Projectile.tileCollide = true;
					Projectile.friendly = true;
					return;
				}

				Projectile.velocity = Vector2.Zero;
				Projectile.rotation = _stuckRotation;
				Projectile.friendly = false;
				return;
			}

			// Physics
			_frameCounter++;
			if (_frameCounter > HangFrames)
				Projectile.velocity.Y += 0.06f;
			if (Projectile.velocity.Y > 18f)
				Projectile.velocity.Y = 18f;

			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			_stuckRotation = Projectile.rotation;

			// Shimmer bounce
			if (Projectile.shimmerWet)
			{
				Projectile.velocity.Y = -System.Math.Abs(Projectile.velocity.Y);
				Projectile.shimmerWet = false;
			}
		}

		private void SpawnDust(Color paintColor)
		{
			if (!_stuck)
			{
				// Trail behind flying flare
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
			else
			{
				// Sparks from back of stuck flare
				float backAngle = _stuckRotation - MathHelper.PiOver2 + MathHelper.Pi;
				Vector2 spawnPos = Projectile.Center + backAngle.ToRotationVector2() * 6f;

				for (int i = 0; i < 2; i++)
				{
					float spread = Main.rand.NextFloat(-0.256f, 0.256f);
					Vector2 dustVel = (backAngle + spread).ToRotationVector2() * Main.rand.NextFloat(3.2f, 4.8f);
					Dust dust = Dust.NewDustPerfect(spawnPos + Main.rand.NextVector2Circular(2f, 2f),
						DustID.RainbowMk2, dustVel, 0, paintColor, 0.7f);
					dust.noGravity = true;
				}
			}
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			_lodging = true;
			_lodgeVelocity = oldVelocity;
			_stuckRotation = oldVelocity.ToRotation() + MathHelper.PiOver2;
			Projectile.tileCollide = false;
			return false;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.AddBuff(BuffID.OnFire, Main.rand.NextBool(3) ? 600 : 300);
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Color paintColor = GetPaintColor();

			// Apply special coating effects
			paintColor = _specialFlag switch
			{
				4 => new Color(30, 30, 40), // Shadow
				3 => new Color(255 - paintColor.R, 255 - paintColor.G, 255 - paintColor.B, 255), // Negative
				2 => paintColor * 0.3f, // Echo
				1 => Color.Lerp(paintColor, Color.White, 0.4f), // Illuminant
				_ => paintColor
			};

			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
			Main.EntitySpriteDraw(
				texture,
				Projectile.Center - Main.screenPosition,
				texture.Bounds,
				paintColor,
				Projectile.rotation,
				texture.Size() / 2f,
				Projectile.scale,
				SpriteEffects.None,
				0
			);

			return false;
		}

		private Color GetPaintColor()
		{
			return FlareGunGlobalItem.GetPaintColor(_paintType);
		}
	}
}
