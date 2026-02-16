using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Content.Projectiles;

namespace VanillaPlus.Common.GlobalItems
{
	public class FlareGunGlobalItem : GlobalItem
	{
		// Special coating ItemIDs (not in ItemID enum in some versions)
		private const int IlluminantCoatingID = 4668;
		private const int EchoCoatingID = 5344;

		// Paint ItemID to Color mapping
		private static readonly Dictionary<int, Color> PaintColors = new()
		{
			// Standard paints
			{ ItemID.RedPaint, new Color(255, 60, 60) },
			{ ItemID.OrangePaint, new Color(255, 140, 40) },
			{ ItemID.YellowPaint, new Color(255, 255, 60) },
			{ ItemID.LimePaint, new Color(180, 255, 60) },
			{ ItemID.GreenPaint, new Color(60, 255, 60) },
			{ ItemID.TealPaint, new Color(60, 255, 180) },
			{ ItemID.CyanPaint, new Color(60, 255, 255) },
			{ ItemID.SkyBluePaint, new Color(60, 180, 255) },
			{ ItemID.BluePaint, new Color(60, 60, 255) },
			{ ItemID.PurplePaint, new Color(180, 60, 255) },
			{ ItemID.VioletPaint, new Color(255, 60, 255) },
			{ ItemID.PinkPaint, new Color(255, 60, 180) },

			// Deep paints
			{ ItemID.DeepRedPaint, new Color(180, 30, 30) },
			{ ItemID.DeepOrangePaint, new Color(180, 100, 20) },
			{ ItemID.DeepYellowPaint, new Color(180, 180, 30) },
			{ ItemID.DeepLimePaint, new Color(120, 180, 30) },
			{ ItemID.DeepGreenPaint, new Color(30, 180, 30) },
			{ ItemID.DeepTealPaint, new Color(30, 180, 120) },
			{ ItemID.DeepCyanPaint, new Color(30, 180, 180) },
			{ ItemID.DeepSkyBluePaint, new Color(30, 120, 180) },
			{ ItemID.DeepBluePaint, new Color(30, 30, 180) },
			{ ItemID.DeepPurplePaint, new Color(120, 30, 180) },
			{ ItemID.DeepVioletPaint, new Color(180, 30, 180) },
			{ ItemID.DeepPinkPaint, new Color(180, 30, 120) },

			// Neutral paints
			{ ItemID.BlackPaint, new Color(40, 40, 40) },
			{ ItemID.WhitePaint, new Color(255, 255, 255) },
			{ ItemID.GrayPaint, new Color(128, 128, 128) },
			{ ItemID.BrownPaint, new Color(150, 100, 50) },

			// Special paints
			{ ItemID.ShadowPaint, new Color(20, 20, 30) },
			{ ItemID.NegativePaint, new Color(255, 255, 255) }, // Handled specially in projectile

			// Special coatings
			{ IlluminantCoatingID, new Color(255, 255, 200) }, // Bright glow
			{ EchoCoatingID, new Color(200, 200, 255) },       // Ghostly blue-white
		};

		// Special coatings that have unique effects
		private static readonly HashSet<int> SpecialCoatings = new()
		{
			IlluminantCoatingID,
			EchoCoatingID,
		};

		public override bool AppliesToEntity(Item entity, bool lateInstantiation)
		{
			return entity.type == ItemID.FlareGun;
		}

		public override bool AltFunctionUse(Item item, Player player)
		{
			return true;
		}

		public override bool CanUseItem(Item item, Player player)
		{
			// Right-click mode
			if (player.altFunctionUse == 2)
			{
				// Check if player has normal flares
				if (!HasNormalFlares(player))
					return false;

				// Check if player has paint
				if (FindPaintInInventory(player) == null)
					return false;
			}

			return base.CanUseItem(item, player);
		}

		public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source,
			Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			// Only intercept right-click with normal flares
			if (player.altFunctionUse != 2)
				return true;

			// Verify we're shooting a normal flare (not blue flare)
			if (type != ProjectileID.Flare)
				return true;

			Item paintItem = FindPaintInInventory(player);
			if (paintItem == null)
				return true;

			int paintType = paintItem.type;

			// Consume paint using ammo conservation logic
			if (ShouldConsumePaint(player))
			{
				paintItem.stack--;
				if (paintItem.stack <= 0)
					paintItem.TurnToAir();
			}

			// Determine special effect flag for coatings
			int specialFlag = 0;
			if (paintType == IlluminantCoatingID)
				specialFlag = 1;
			else if (paintType == EchoCoatingID)
				specialFlag = 2;
			else if (paintType == ItemID.NegativePaint)
				specialFlag = 3;
			else if (paintType == ItemID.ShadowPaint)
				specialFlag = 4;

			// Spawn painted flare - pass ai0/ai1 directly so OnSpawn can capture them
			Projectile.NewProjectile(
				source,
				position,
				velocity,
				ModContent.ProjectileType<PaintedFlare>(),
				damage,
				knockback,
				player.whoAmI,
				ai0: paintType,      // Paint ItemID for color lookup
				ai1: specialFlag     // Special effect flag
			);

			return false; // Don't fire vanilla flare
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			TooltipLine tip = new(Mod, "PaintedFlare", "Right-click with paint to fire colored flares (normal flares only)");
			tooltips.Add(tip);
		}

		private static bool HasNormalFlares(Player player)
		{
			// Check ammo slots first (54-57)
			for (int i = 54; i < 58; i++)
			{
				if (player.inventory[i].type == ItemID.Flare && player.inventory[i].stack > 0)
					return true;
			}

			// Check main inventory (0-49)
			for (int i = 0; i < 50; i++)
			{
				if (player.inventory[i].type == ItemID.Flare && player.inventory[i].stack > 0)
					return true;
			}

			return false;
		}

		private static Item FindPaintInInventory(Player player)
		{
			// Check ammo slots first (54-57) - same priority as ammo
			for (int i = 54; i < 58; i++)
			{
				if (IsPaintItem(player.inventory[i]))
					return player.inventory[i];
			}

			// Check main inventory (0-49)
			for (int i = 0; i < 50; i++)
			{
				if (IsPaintItem(player.inventory[i]))
					return player.inventory[i];
			}

			return null;
		}

		private static bool IsPaintItem(Item item)
		{
			if (item == null || item.IsAir || item.stack <= 0)
				return false;

			return PaintColors.ContainsKey(item.type) || SpecialCoatings.Contains(item.type);
		}

		private static bool ShouldConsumePaint(Player player)
		{
			// Base consumption chance
			float consumeChance = 1f;

			// Apply ammo conservation effects (same as vanilla ammo)
			if (player.ammoBox)
				consumeChance *= 0.8f;
			if (player.ammoPotion)
				consumeChance *= 0.8f;
			if (player.ammoCost80)
				consumeChance *= 0.8f;
			if (player.ammoCost75)
				consumeChance *= 0.75f;

			return Main.rand.NextFloat() < consumeChance;
		}

		// Public method for projectile to access paint colors
		public static Color GetPaintColor(int paintType)
		{
			if (PaintColors.TryGetValue(paintType, out Color color))
				return color;

			// Default to orange (normal flare color) if unknown
			return new Color(255, 150, 50);
		}
	}
}
