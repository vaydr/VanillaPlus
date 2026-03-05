using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaPlus.Content.Projectiles.Rapture
{
	public class ArondightSlash : ModProjectile
	{
		public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.Excalibur;

		public override void SetStaticDefaults()
		{
			Main.projFrames[Type] = 4;
		}

		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.aiStyle = -1;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.penetrate = 3;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.localNPCHitCooldown = -1;
			Projectile.ownerHitCheck = true;
			Projectile.ownerHitCheckDistance = 300f;
			Projectile.usesOwnerMeleeHitCD = true;
			Projectile.stopsDealingDamageAfterPenetrateHits = true;
		}

		public override void AI()
		{
			Projectile.localAI[0] += 1f;
			Player player = Main.player[Projectile.owner];
			float progress = Projectile.localAI[0] / Projectile.ai[1];
			float direction = Projectile.ai[0];
			float baseAngle = Projectile.velocity.ToRotation();

			Projectile.rotation = (float)Math.PI * direction * progress + baseAngle + direction * (float)Math.PI + player.fullRotation;
			Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) - Projectile.velocity;
			Projectile.scale = 1.3f + progress * 0.6f;

			// Sky blue dust sweeping along the swing arc
			float arcSpeed = (float)Math.PI / Projectile.ai[1];
			for (int d = 0; d < 1; d++)
			{
				float dist = 40f + Main.rand.NextFloat() * 50f;
				Vector2 dustPos = Projectile.Center + Projectile.rotation.ToRotationVector2() * dist * Projectile.scale;
				Vector2 tangent = (Projectile.rotation + direction * MathHelper.PiOver2).ToRotationVector2();
				Vector2 dustVel = tangent * dist * arcSpeed * 0.4f;
				Dust dust = Dust.NewDustPerfect(dustPos, DustID.Electric, dustVel);
				dust.scale = 0.4f + Main.rand.NextFloat() * 0.2f;
				dust.fadeIn = 0.6f + Main.rand.NextFloat() * 0.2f;
				dust.noGravity = true;
			}

			if (Projectile.localAI[0] >= Projectile.ai[1])
				Projectile.Kill();

			if (!Projectile.noEnchantmentVisuals)
				UpdateEnchantmentVisuals();
		}

		public override void CutTiles()
		{
			Vector2 start = (Projectile.rotation - (float)Math.PI / 4f).ToRotationVector2() * 60f * Projectile.scale;
			Vector2 end = (Projectile.rotation + (float)Math.PI / 4f).ToRotationVector2() * 60f * Projectile.scale;
			Utils.PlotTileLine(Projectile.Center + start, Projectile.Center + end, 60f * Projectile.scale, DelegateMethods.CutTiles);
		}

		public override bool? CanCutTiles() => true;

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			float coneLength = 94f * Projectile.scale;
			float rotOffset = (float)Math.PI * 2f / 25f * Projectile.ai[0];
			float maxAngle = (float)Math.PI / 4f;
			float rot = Projectile.rotation + rotOffset;

			if (targetHitbox.IntersectsConeSlowMoreAccurate(Projectile.Center, coneLength, rot, maxAngle))
				return true;

			float remap = Utils.Remap(Projectile.localAI[0], Projectile.ai[1] * 0.3f, Projectile.ai[1] * 0.5f, 1f, 0f);
			if (remap > 0f)
			{
				float coneRot = rot - (float)Math.PI / 4f * Projectile.ai[0] * remap;
				if (targetHitbox.IntersectsConeSlowMoreAccurate(Projectile.Center, coneLength, coneRot, maxAngle))
					return true;
			}

			return false;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			hit.HitDirection = Main.player[Projectile.owner].Center.X < target.Center.X ? 1 : -1;

			// Blue flash of light on hit
			for (int i = 0; i < 6; i++)
			{
				Vector2 vel = (i / 6f * MathHelper.TwoPi).ToRotationVector2() * Main.rand.NextFloat(1f, 2.5f);
				Dust dust = Dust.NewDustPerfect(target.Center, DustID.Electric, vel);
				dust.scale = Main.rand.NextFloat(1.2f, 1.8f);
				dust.noGravity = true;
			}
			Lighting.AddLight(target.Center, 0.5f, 0.8f, 1f);
		}

		private void UpdateEnchantmentVisuals()
		{
			if (Projectile.npcProj)
				return;

			for (float num = -(float)Math.PI / 4f; num <= (float)Math.PI / 4f; num += (float)Math.PI / 2f)
			{
				Rectangle r = Utils.CenteredRectangle(
					Projectile.Center + (Projectile.rotation + num).ToRotationVector2() * 70f * Projectile.scale,
					new Vector2(60f * Projectile.scale, 60f * Projectile.scale));
				Projectile.EmitEnchantmentVisualsAt(r.TopLeft(), r.Width, r.Height);
			}
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Vector2 drawPos = Projectile.Center - Main.screenPosition;
			Asset<Texture2D> asset = TextureAssets.Projectile[Projectile.type];
			Rectangle frame = asset.Frame(1, 4);
			Vector2 origin = frame.Size() / 2f;
			float scale = Projectile.scale * 1.1f;
			SpriteEffects effects = Projectile.ai[0] >= 0f ? SpriteEffects.None : SpriteEffects.FlipVertically;

			float progress = Projectile.localAI[0] / Projectile.ai[1];
			float opacity = Utils.Remap(progress, 0f, 0.6f, 0f, 1f) * Utils.Remap(progress, 0.6f, 1f, 1f, 0f);

			// Lighting
			Color ambient = Lighting.GetColor(Projectile.Center.ToTileCoordinates());
			float brightness = ambient.ToVector3().Length() / (float)Math.Sqrt(3.0);
			brightness = Utils.Remap(brightness, 0f, 1f, 0.2f, 1f);

			// Sky blue color theme
			Color skyBlue = new Color(137, 207, 240);
			Color deepBlue = new Color(70, 130, 180);
			Color iceBlue = new Color(200, 230, 255);

			// Main slash arc
			Main.spriteBatch.Draw(asset.Value, drawPos, (Rectangle?)frame, skyBlue * brightness * opacity, Projectile.rotation + Projectile.ai[0] * ((float)Math.PI / 4f) * -1f * (1f - progress), origin, scale, effects, 0f);

			// Layered glow
			Color glowColor = Color.White * opacity * 0.5f;
			glowColor.A = (byte)(glowColor.A * (1f - brightness));
			Color dimGlow = glowColor * brightness * 0.5f;
			dimGlow.G = (byte)(dimGlow.G * brightness);
			dimGlow.B = (byte)(dimGlow.R * (0.25f + brightness * 0.75f));
			Main.spriteBatch.Draw(asset.Value, drawPos, (Rectangle?)frame, dimGlow * 0.15f, Projectile.rotation + Projectile.ai[0] * 0.01f, origin, scale, effects, 0f);
			Main.spriteBatch.Draw(asset.Value, drawPos, (Rectangle?)frame, iceBlue * brightness * opacity * 0.3f, Projectile.rotation, origin, scale, effects, 0f);
			Main.spriteBatch.Draw(asset.Value, drawPos, (Rectangle?)frame, deepBlue * brightness * opacity * 0.5f, Projectile.rotation, origin, scale * 0.975f, effects, 0f);

			// White glow frames
			Main.spriteBatch.Draw(asset.Value, drawPos, (Rectangle?)asset.Frame(1, 4, 0, 3), Color.White * 0.6f * opacity, Projectile.rotation + Projectile.ai[0] * 0.01f, origin, scale, effects, 0f);
			Main.spriteBatch.Draw(asset.Value, drawPos, (Rectangle?)asset.Frame(1, 4, 0, 3), Color.White * 0.5f * opacity, Projectile.rotation + Projectile.ai[0] * -0.05f, origin, scale * 0.8f, effects, 0f);
			Main.spriteBatch.Draw(asset.Value, drawPos, (Rectangle?)asset.Frame(1, 4, 0, 3), Color.White * 0.4f * opacity, Projectile.rotation + Projectile.ai[0] * -0.1f, origin, scale * 0.6f, effects, 0f);

			// Star sparkles along the arc
			for (float i = 0f; i < 8f; i += 1f)
			{
				float sparkleRot = Projectile.rotation + Projectile.ai[0] * i * ((float)Math.PI * -2f) * 0.025f + Utils.Remap(progress, 0f, 1f, 0f, (float)Math.PI / 4f) * Projectile.ai[0];
				Vector2 sparklePos = drawPos + sparkleRot.ToRotationVector2() * ((float)asset.Width() * 0.5f - 6f) * scale;
				float sparkleOpacity = i / 9f;
				DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None, sparklePos, new Color(255, 255, 255, 0) * opacity * sparkleOpacity, iceBlue, progress, 0f, 0.5f, 0.5f, 1f, sparkleRot, new Vector2(0f, Utils.Remap(progress, 0f, 1f, 3f, 0f)) * scale, Vector2.One * scale);
			}

			// Center sparkle
			Vector2 centerSparklePos = drawPos + (Projectile.rotation + Utils.Remap(progress, 0f, 1f, 0f, (float)Math.PI / 4f) * Projectile.ai[0]).ToRotationVector2() * ((float)asset.Width() * 0.5f - 4f) * scale;
			DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None, centerSparklePos, new Color(255, 255, 255, 0) * opacity * 0.5f, iceBlue, progress, 0f, 0.5f, 0.5f, 1f, 0f, new Vector2(2f, Utils.Remap(progress, 0f, 1f, 4f, 1f)) * scale, Vector2.One * scale);

			return false;
		}

		private static void DrawPrettyStarSparkle(float opacity, SpriteEffects dir, Vector2 drawpos, Color drawColor, Color shineColor, float flareCounter, float fadeInStart, float fadeInEnd, float fadeOutStart, float fadeOutEnd, float rotation, Vector2 scale, Vector2 fatness)
		{
			Texture2D value = TextureAssets.Extra[98].Value;
			Color color = shineColor * opacity * 0.5f;
			color.A = 0;
			Vector2 origin = value.Size() / 2f;
			Color color2 = drawColor * 0.5f;
			float num = Utils.GetLerpValue(fadeInStart, fadeInEnd, flareCounter, clamped: true) * Utils.GetLerpValue(fadeOutEnd, fadeOutStart, flareCounter, clamped: true);
			Vector2 vector = new Vector2(fatness.X * 0.5f, scale.X) * num;
			Vector2 vector2 = new Vector2(fatness.Y * 0.5f, scale.Y) * num;
			color *= num;
			color2 *= num;
			Main.EntitySpriteDraw(value, drawpos, null, color, (float)Math.PI / 2f + rotation, origin, vector, dir);
			Main.EntitySpriteDraw(value, drawpos, null, color, 0f + rotation, origin, vector2, dir);
			Main.EntitySpriteDraw(value, drawpos, null, color2, (float)Math.PI / 2f + rotation, origin, vector * 0.6f, dir);
			Main.EntitySpriteDraw(value, drawpos, null, color2, 0f + rotation, origin, vector2 * 0.6f, dir);
		}
	}
}
