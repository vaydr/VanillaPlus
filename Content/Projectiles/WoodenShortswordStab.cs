using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaPlus.Content.Projectiles
{
	public class WoodenShortswordStab : ModProjectile
	{
		public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.CopperShortswordStab}";

		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.CopperShortswordStab);
			AIType = ProjectileID.CopperShortswordStab;
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Main.hslToRgb(0.08f, 0.45f, 0.35f);
		}
	}
}
