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

		// Store paint data in instance fields - captured in OnSpawn before AI touches ai[]
		private int _paintType;
		private int _specialFlag;

		public override void SetDefaults()
		{
			// Clone EVERYTHING from vanilla flare
			Projectile.CloneDefaults(ProjectileID.Flare);
			// Use vanilla flare AI for exact same physics
			AIType = ProjectileID.Flare;
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
			// Add colored lighting (vanilla flare adds red/orange light, we override with paint color)
			Color paintColor = GetPaintColor();

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

			// Draw glow layer
			Main.EntitySpriteDraw(
				texture, drawPos, sourceRect,
				paintColor * 0.5f,
				Projectile.rotation, origin,
				Projectile.scale * 1.5f,
				SpriteEffects.None, 0
			);

			// Draw main flare
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
