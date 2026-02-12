using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaPlus.Content.Items
{
	public class LeptonPick : ModItem
	{
		public override string Texture => $"Terraria/Images/Item_{ItemID.TitaniumPickaxe}";

		// Get hue based on game time - constantly cycling rainbow
		private float GetHue() => (Main.GameUpdateCount % 360) / 360f;

		public override void SetDefaults()
		{
			Item.damage = 80;
			Item.DamageType = DamageClass.Melee;
			Item.width = 88;
			Item.height = 88;
			Item.scale = 2f; // Twice as large when swung
			Item.useTime = 1;
			Item.useAnimation = 15;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 5.5f;
			Item.value = Item.buyPrice(gold: 10);
			Item.rare = ItemRarityID.Expert;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.pick = 500; // 500% pickaxe power
			Item.axe = 100; // 500% axe power (displayed value is 5x internal)
			Item.hammer = 500; // 500% hammer power
			Item.tileBoost = 48; // +48 range
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

			// Rainbow light matching pickaxe color
			Color c = Main.hslToRgb(hue, 1f, 0.5f);
			Lighting.AddLight(player.Center, c.R / 255f, c.G / 255f, c.B / 255f);

			// Rainbow dust particles matching current hue
			if (Main.rand.NextBool(2))
			{
				Color dustColor = Main.hslToRgb(hue, 1f, 0.5f);
				Vector2 dustPos = new Vector2(hitbox.X, hitbox.Y) + new Vector2(Main.rand.Next(hitbox.Width), Main.rand.Next(hitbox.Height));
				Dust dust = Dust.NewDustPerfect(dustPos, DustID.RainbowMk2, Vector2.Zero, 0, dustColor, 1.2f);
				dust.noGravity = true;
			}
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
