using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaPlus.Content.Items.Tools
{
	public class WoodenAxe : ModItem
	{
		public override string Texture => $"Terraria/Images/Item_{ItemID.CopperAxe}";

		public override Color? GetAlpha(Color lightColor)
		{
			return Main.hslToRgb(0.08f, 0.45f, 0.35f);
		}

		public override void SetDefaults()
		{
			Item.damage = 2;
			Item.DamageType = DamageClass.Melee;
			Item.width = 20;
			Item.height = 20;
			Item.useTime = 22;
			Item.useAnimation = 22;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 4f;
			Item.value = Item.sellPrice(copper: 18);
			Item.rare = ItemRarityID.White;
			Item.UseSound = SoundID.Item1;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.axe = 4; // Displayed as 20%
			Item.tileBoost = -1;
			Item.ResearchUnlockCount = 1;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddRecipeGroup(RecipeGroupID.Wood, 9)
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}
