using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Content.Buffs;

namespace VanillaPlus.Content.Items.Consumables
{
	public class Gabagool : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 22;
			Item.height = 14;
			Item.consumable = true;
			Item.useStyle = ItemUseStyleID.EatFood;
			Item.useTime = 17;
			Item.useAnimation = 17;
			Item.UseSound = SoundID.Item2;
			Item.maxStack = 9999;
			Item.value = Item.buyPrice(silver: 1);
			Item.rare = ItemRarityID.Blue;
			Item.buffType = ModContent.BuffType<ProdigiouslyPlump>();
			Item.buffTime = 172800; // 48 minutes
		}
	}
}
