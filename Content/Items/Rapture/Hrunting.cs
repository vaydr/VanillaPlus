using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Content.Projectiles.Rapture;

namespace VanillaPlus.Content.Items.Rapture
{
	public class Hrunting : ModItem
	{

		public override void SetDefaults()
		{
			Item.DefaultToWhip(ModContent.ProjectileType<HruntingProjectile>(), 57, 2, 4);
			Item.rare = ItemRarityID.Pink;
			Item.value = Item.sellPrice(gold: 4, silver: 60);
		}

		public override bool MeleePrefix() => true;

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<ExaltedBar>(), 12)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}
	}
}
