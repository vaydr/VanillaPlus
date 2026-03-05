using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Content.Projectiles;

namespace VanillaPlus.Content.Items.Tools
{
	public class WoodenShortsword : ModItem
	{
		public override string Texture => $"Terraria/Images/Item_{ItemID.CopperShortsword}";

		public override Color? GetAlpha(Color lightColor)
		{
			return Main.hslToRgb(0.08f, 0.45f, 0.35f);
		}

		public override void SetDefaults()
		{
			Item.damage = 3;
			Item.DamageType = DamageClass.Melee;
			Item.width = 20;
			Item.height = 20;
			Item.useTime = 13;
			Item.useAnimation = 13;
			Item.useStyle = ItemUseStyleID.Rapier;
			Item.noUseGraphic = true;
			Item.noMelee = true;
			Item.knockBack = 3.5f;
			Item.value = Item.sellPrice(copper: 14);
			Item.rare = ItemRarityID.White;
			Item.UseSound = SoundID.Item1;
			Item.shoot = ModContent.ProjectileType<WoodenShortswordStab>();
			Item.shootSpeed = 2.1f;
			Item.ResearchUnlockCount = 1;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddRecipeGroup(RecipeGroupID.Wood, 5)
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}
