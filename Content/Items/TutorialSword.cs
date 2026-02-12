using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Content.Projectiles;

namespace VanillaPlus.Content.Items
{
	public class TutorialSword : ModItem
	{
		// Get hue based on game time - constantly cycling rainbow
		private float GetHue() => (Main.GameUpdateCount % 360) / 360f;

		public override void SetDefaults()
		{
			Item.damage = 10000;
			Item.DamageType = DamageClass.Melee;
			Item.width = 120;
			Item.height = 120;
			Item.scale = 3f;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 6;
			Item.value = Item.buyPrice(silver: 1);
			Item.rare = ItemRarityID.Expert;
			Item.UseSound = SoundID.Item105; // Star Wrath swing sound
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<PurpleTerraBeam>();
			Item.shootSpeed = 7.2f; // 60% of original 12f
		}

		public override Color? GetAlpha(Color lightColor)
		{
			// Return cycling rainbow color for item sprite
			return Main.hslToRgb(GetHue(), 1f, 0.7f);
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			foreach (TooltipLine line in tooltips)
			{
				if (line.Mod == "Terraria" && line.Name == "ItemName")
				{
					line.OverrideColor = Main.DiscoColor; // Rainbow expert color
				}
			}
		}

		public override void MeleeEffects(Player player, Rectangle hitbox)
		{
			float hue = GetHue();

			// Rainbow light matching sword color
			Color c = Main.hslToRgb(hue, 1f, 0.5f);
			Lighting.AddLight(player.Center, c.R / 255f, c.G / 255f, c.B / 255f);

			// Rainbow dust particles
			if (Main.rand.NextBool(2))
			{
				Dust dust = Dust.NewDustDirect(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.RainbowTorch);
				dust.noGravity = true;
				dust.scale = 1.5f;
				dust.velocity *= 0.5f;
			}
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source,
			Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			float spread = MathHelper.ToRadians(10f); // 10 degree total spread
			float startAngle = -spread / 2;
			float currentHue = GetHue(); // Get sword's current hue

			for (int i = 0; i < 10; i++)
			{
				float angle = startAngle + (spread / 9) * i;
				Vector2 newVelocity = velocity.RotatedBy(angle);
				Projectile proj = Projectile.NewProjectileDirect(source, position, newVelocity, type, damage, knockback, player.whoAmI);
				proj.ai[0] = currentHue; // Pass sword's hue to projectile
			}
			return false; // Don't fire default projectile
		}

	}
}
