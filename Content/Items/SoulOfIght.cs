using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaPlus.Content.Items
{
	public class SoulOfIght : ModItem
	{
		// Use vanilla Soul of Fright sprite
		public override string Texture => $"Terraria/Images/Item_{ItemID.SoulofFright}";

		public override void SetStaticDefaults()
		{
			// Register animation (Soul of Fright has 4 frames)
			Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 4));

			// Makes the item float up and down in world like other souls
			ItemID.Sets.ItemNoGravity[Item.type] = true;
		}

		public override void SetDefaults()
		{
			Item.width = 18;
			Item.height = 18;
			Item.maxStack = 9999;
			Item.value = Item.sellPrice(silver: 40);
			Item.rare = ItemRarityID.LightPurple;
		}

	}
}
