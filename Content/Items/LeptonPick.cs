using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace VanillaPlus.Content.Items
{
	public class LeptonPick : ModItem
	{
		// 0 = Pickaxe, 1 = Axe, 2 = Hammer
		private int _mode = 0;

		public override string Texture => $"Terraria/Images/Item_{ItemID.TitaniumPickaxe}";

		// Get hue based on game time - constantly cycling rainbow
		private float GetHue() => (Main.GameUpdateCount % 360) / 360f;

		private static readonly string[] ModeNames = { "Pickaxe", "Axe", "Hammer" };

		public override void SetDefaults()
		{
			Item.damage = 80;
			Item.DamageType = DamageClass.Melee;
			Item.width = 88;
			Item.height = 88;
			Item.scale = 2f;
			Item.useTime = 1;
			Item.useAnimation = 15;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 5.5f;
			Item.value = Item.buyPrice(gold: 10);
			Item.rare = ItemRarityID.Expert;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.tileBoost = 48;

			// Set initial tool powers based on mode
			UpdateToolPowers();
		}

		private void UpdateToolPowers()
		{
			Item.pick = _mode == 0 ? 500 : 0;
			Item.axe = _mode == 1 ? 100 : 0; // Displayed value is 5x internal
			Item.hammer = _mode == 2 ? 500 : 0;
		}

		public override bool AltFunctionUse(Player player) => true;

		public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse == 2)
			{
				// Right-click: cycle mode
				_mode = (_mode + 1) % 3;
				UpdateToolPowers();
				SoundEngine.PlaySound(SoundID.MenuTick);
				Main.NewText($"Lepton Pick: {ModeNames[_mode]} Mode", Main.DiscoColor);
				return false;
			}
			return true;
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Main.hslToRgb(GetHue(), 1f, 0.7f);
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			foreach (TooltipLine line in tooltips)
			{
				if (line.Mod == "Terraria" && line.Name == "ItemName")
				{
					line.OverrideColor = Main.DiscoColor;
				}
			}

			// Add mode indicator
			int index = tooltips.FindIndex(t => t.Mod == "Terraria" && t.Name == "Knockback");
			if (index == -1)
				index = tooltips.Count;

			tooltips.Insert(index, new TooltipLine(Mod, "Mode", $"Current Mode: {ModeNames[_mode]}")
			{
				OverrideColor = Main.DiscoColor
			});
			tooltips.Insert(index + 1, new TooltipLine(Mod, "ModeHint", "Right-click to cycle modes")
			{
				OverrideColor = Color.Gray
			});
		}

		public override void MeleeEffects(Player player, Rectangle hitbox)
		{
			float hue = GetHue();

			Color c = Main.hslToRgb(hue, 1f, 0.5f);
			Lighting.AddLight(player.Center, c.R / 255f, c.G / 255f, c.B / 255f);

			if (Main.rand.NextBool(2))
			{
				Color dustColor = Main.hslToRgb(hue, 1f, 0.5f);
				Vector2 dustPos = new Vector2(hitbox.X, hitbox.Y) + new Vector2(Main.rand.Next(hitbox.Width), Main.rand.Next(hitbox.Height));
				Dust dust = Dust.NewDustPerfect(dustPos, DustID.RainbowMk2, Vector2.Zero, 0, dustColor, 1.2f);
				dust.noGravity = true;
			}
		}

		public override void SaveData(TagCompound tag)
		{
			tag["mode"] = _mode;
		}

		public override void LoadData(TagCompound tag)
		{
			_mode = tag.GetInt("mode");
			UpdateToolPowers();
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				// Luminite tier
				.AddIngredient(ItemID.SolarFlarePickaxe)
				.AddIngredient(ItemID.NebulaPickaxe)
				.AddIngredient(ItemID.VortexPickaxe)
				.AddIngredient(ItemID.StardustPickaxe)
				.AddIngredient(ItemID.DrillContainmentUnit)
				.AddIngredient(ItemID.LaserDrill)
				// Post-Golem
				.AddIngredient(ItemID.Picksaw)
				.AddIngredient(ItemID.ShroomiteDiggingClaw)
				// Post-Mech
				.AddIngredient(ItemID.PickaxeAxe)
				.AddIngredient(ItemID.ChlorophytePickaxe)
				.AddIngredient(ItemID.SpectrePickaxe)
				// Hardmode ores
				.AddIngredient(ItemID.TitaniumPickaxe)
				.AddIngredient(ItemID.AdamantitePickaxe)
				.AddIngredient(ItemID.OrichalcumPickaxe)
				.AddIngredient(ItemID.MythrilPickaxe)
				.AddIngredient(ItemID.PalladiumPickaxe)
				.AddIngredient(ItemID.CobaltPickaxe)
				// Pre-Hardmode
				.AddIngredient(ItemID.MoltenPickaxe)
				.AddIngredient(ItemID.DeathbringerPickaxe)
				.AddIngredient(ItemID.NightmarePickaxe)
				.AddIngredient(ItemID.ReaverShark)
				.AddIngredient(ItemID.PlatinumPickaxe)
				.AddIngredient(ItemID.BonePickaxe)
				.AddIngredient(ItemID.GoldPickaxe)
				.AddIngredient(ItemID.FossilPickaxe)
				.AddIngredient(ItemID.CactusPickaxe)
				.AddIngredient(ItemID.CnadyCanePickaxe)
				.AddIngredient(ItemID.TungstenPickaxe)
				.AddIngredient(ItemID.SilverPickaxe)
				.AddIngredient(ItemID.LeadPickaxe)
				.AddIngredient(ItemID.IronPickaxe)
				.AddIngredient(ItemID.TinPickaxe)
				.AddIngredient(ItemID.CopperPickaxe)
				// Money
				.AddIngredient(ItemID.PlatinumCoin, 67)
				.AddTile(TileID.LunarCraftingStation)
				.Register();
		}
	}
}
