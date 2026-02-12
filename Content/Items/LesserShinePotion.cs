using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Content.Buffs;

namespace VanillaPlus.Content.Items
{
	public class LesserShinePotion : ModItem
	{
		public override string Texture => $"Terraria/Images/Item_{ItemID.ShinePotion}";

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 26;
			Item.consumable = true;
			Item.useStyle = ItemUseStyleID.DrinkLiquid;
			Item.useTime = 17;
			Item.useAnimation = 17;
			Item.UseSound = SoundID.Item3;
			Item.maxStack = 9999;
			Item.value = Item.buyPrice(silver: 1);
			Item.rare = ItemRarityID.Blue;
			Item.buffType = ModContent.BuffType<LesserShine>();
			Item.buffTime = 18000; // 5 minutes
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.BottledWater)
				.AddIngredient(ItemID.Gel)
				.AddTile(TileID.Bottles)
				.Register();
		}
	}
}
