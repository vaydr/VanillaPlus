using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaPlus.Content.Items.Accessories
{
	public class IcarusWings : ModItem
	{
		// Use Angel Wings item sprite
		public override string Texture => $"Terraria/Images/Item_{ItemID.AngelWings}";

		// Get hue based on game time - constantly cycling rainbow
		private float GetHue() => (Main.GameUpdateCount % 360) / 360f;

		public override void SetStaticDefaults()
		{
			// Get the wing slot we registered in VanillaPlus.Load()
			int wingSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Wings);

			// Solar Wings but 2x speed: flyTime 1000, speed 18f, accel 5f
			ArmorIDs.Wing.Sets.Stats[wingSlot] = new WingStats(
				flyTime: 1000,
				flySpeedOverride: 30f,
				accelerationMultiplier: 15f
			);
		}

		public override void SetDefaults()
		{
			Item.width = 22;
			Item.height = 20;
			Item.value = Item.buyPrice(gold: 10);
			Item.rare = ItemRarityID.Expert;
			Item.accessory = true;

			// Assign the wing slot we registered
			Item.wingSlot = (sbyte)EquipLoader.GetEquipSlot(Mod, Name, EquipType.Wings);
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

		public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling,
			ref float ascentWhenRising, ref float maxCanAscendMultiplier,
			ref float maxAscentMultiplier, ref float constantAscend)
		{
			// Solar Wings values doubled
			ascentWhenFalling = 1.7f;    // 0.85 * 2
			ascentWhenRising = 0.3f;     // 0.15 * 2
			maxCanAscendMultiplier = 2f; // 1 * 2
			maxAscentMultiplier = 6f;    // 3 * 2
			constantAscend = 0.27f;      // 0.135 * 2
		}

		public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
		{
			speed = 30f;          // 9 * 2
			acceleration *= 2f;   // Double acceleration
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			float hue = GetHue();

			// Rainbow light matching current hue (bright)
			Color c = Main.hslToRgb(hue, 1f, 0.5f);
			Lighting.AddLight(player.Center, c.R / 255f * 2f, c.G / 255f * 2f, c.B / 255f * 2f);

			// Rainbow dust particles
			if (Main.rand.NextBool(3))
			{
				Color dustColor = Main.hslToRgb(hue, 1f, 0.5f);
				Vector2 dustPos = player.Center + Main.rand.NextVector2Circular(20f, 20f);
				Dust dust = Dust.NewDustPerfect(dustPos, DustID.RainbowMk2, Vector2.Zero, 0, dustColor, 1.5f);
				dust.noGravity = true;
			}
		}

	}
}
