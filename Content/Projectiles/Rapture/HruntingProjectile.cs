using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Content.Buffs.Rapture;

namespace VanillaPlus.Content.Projectiles.Rapture
{
	public class HruntingProjectile : ModProjectile
	{
		public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.SwordWhip;

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.IsAWhip[Type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.DefaultToWhip();
			Projectile.WhipSettings.Segments = 20;
			Projectile.WhipSettings.RangeMultiplier = 1.9f;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.AddBuff(ModContent.BuffType<HruntingTagDamage>(), 240);
			Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
			Main.player[Projectile.owner].AddBuff(ModContent.BuffType<HruntingBlessing>(), 3 * 60);

			// Arondight-style blue electric dust burst
			for (int i = 0; i < 6; i++)
			{
				Vector2 vel = (i / 6f * MathHelper.TwoPi).ToRotationVector2() * Main.rand.NextFloat(1f, 2.5f);
				Dust dust = Dust.NewDustPerfect(target.Center, DustID.Electric, vel);
				dust.scale = Main.rand.NextFloat(1.2f, 1.8f);
				dust.noGravity = true;
			}
			Lighting.AddLight(target.Center, 0.5f, 0.8f, 1f);
		}

		public override void PostAI()
		{
			// Arondight-style blue electric particles that travel along the whip arc
			List<Vector2> points = new List<Vector2>();
			Projectile.FillWhipControlPoints(Projectile, points);

			Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out int _, out float _);
			float progress = Projectile.ai[0] / timeToFlyOut;

			for (int i = 0; i < points.Count - 1; i++)
			{
				if (Main.rand.NextBool(2))
				{
					float lerp = Main.rand.NextFloat();
					Vector2 pos = Vector2.Lerp(points[i], points[i + 1], lerp);

					// Tangential velocity along the segment direction, scaled by distance from handle
					Vector2 segmentDir = points[i + 1] - points[i];
					float segmentProgress = (float)i / (points.Count - 1);
					Vector2 tangent = segmentDir.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2 * Projectile.spriteDirection);
					Vector2 vel = tangent * segmentProgress * Main.rand.NextFloat(1f, 3f);

					Dust dust = Dust.NewDustPerfect(pos, DustID.Electric, vel);
					dust.scale = 0.4f + Main.rand.NextFloat() * 0.3f * segmentProgress;
					dust.fadeIn = 0.6f + Main.rand.NextFloat() * 0.2f;
					dust.noGravity = true;
				}
			}

			// Brighter burst at the whip tip
			if (points.Count >= 2)
			{
				Vector2 tip = points[points.Count - 1];
				if (Main.rand.NextBool(2))
				{
					Vector2 tipDir = (points[points.Count - 1] - points[points.Count - 2]).SafeNormalize(Vector2.Zero);
					Vector2 tipVel = tipDir.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(1.5f, 3.5f);
					Dust dust = Dust.NewDustPerfect(tip, DustID.Electric, tipVel);
					dust.scale = Main.rand.NextFloat(0.8f, 1.2f);
					dust.noGravity = true;
				}
				Lighting.AddLight(tip, 0.3f, 0.5f, 0.7f);
			}
		}

		private void DrawLine(List<Vector2> list)
		{
			Texture2D texture = TextureAssets.FishingLine.Value;
			Rectangle frame = texture.Frame();
			Vector2 origin = new Vector2(frame.Width / 2, 2);
			Color gold = new Color(255, 215, 0);

			Vector2 pos = list[0];
			for (int i = 0; i < list.Count - 1; i++)
			{
				Vector2 element = list[i];
				Vector2 diff = list[i + 1] - element;

				float rotation = diff.ToRotation() - MathHelper.PiOver2;
				Color color = Lighting.GetColor(element.ToTileCoordinates(), gold);
				Vector2 scale = new Vector2(1, (diff.Length() + 2) / frame.Height);

				Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, SpriteEffects.None, 0);

				pos += diff;
			}
		}

		public override bool PreDraw(ref Color lightColor)
		{
			List<Vector2> list = new List<Vector2>();
			Projectile.FillWhipControlPoints(Projectile, list);

			DrawLine(list);

			SpriteEffects flip = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			Main.instance.LoadProjectile(Type);
			Texture2D texture = TextureAssets.Projectile[Type].Value;

			int numFrames = 5;
			int frameHeight = texture.Height / numFrames;
			int frameWidth = texture.Width;
			Vector2 origin = new Vector2(frameWidth / 2, frameHeight / 2);

			Vector2 pos = list[0];

			for (int i = 0; i < list.Count - 1; i++)
			{
				Rectangle frame;
				float scale = 1;

				if (i == list.Count - 2)
				{
					frame = new Rectangle(0, frameHeight * 4, frameWidth, frameHeight);

					Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out int _, out float _);
					float t = Projectile.ai[0] / timeToFlyOut;
					scale = MathHelper.Lerp(0.5f, 1.5f, Utils.GetLerpValue(0.1f, 0.7f, t, true) * Utils.GetLerpValue(0.9f, 0.7f, t, true));
				}
				else if (i > 10)
				{
					frame = new Rectangle(0, frameHeight * 3, frameWidth, frameHeight);
				}
				else if (i > 5)
				{
					frame = new Rectangle(0, frameHeight * 2, frameWidth, frameHeight);
				}
				else if (i > 0)
				{
					frame = new Rectangle(0, frameHeight * 1, frameWidth, frameHeight);
				}
				else
				{
					frame = new Rectangle(0, 0, frameWidth, frameHeight);
				}

				Vector2 element = list[i];
				Vector2 diff = list[i + 1] - element;

				float rotation = diff.ToRotation() - MathHelper.PiOver2;
				Color color = Lighting.GetColor(element.ToTileCoordinates());

				Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, flip, 0);

				pos += diff;
			}
			return false;
		}
	}
}
