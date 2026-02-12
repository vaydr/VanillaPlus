using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Content.Buffs;

namespace VanillaPlus.Content.Items
{
	public class Gabagool : ModItem
	{
		// Use vanilla Bacon sprite
		public override string Texture => $"Terraria/Images/Item_{ItemID.Bacon}";

		public override void SetStaticDefaults()
		{
			// Register animation so only one frame displays (Bacon texture has 3 frames)
			// Use a large but safe value instead of int.MaxValue to avoid potential overflow issues
			Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(9999999, 3));
		}

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
