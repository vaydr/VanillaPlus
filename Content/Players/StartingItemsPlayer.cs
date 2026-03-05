using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Content.Items.Tools;

namespace VanillaPlus.Content.Players
{
	public class StartingItemsPlayer : ModPlayer
	{
		public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath)
		{
			if (mediumCoreDeath)
				return [];

			var pickaxe = new Item();
			pickaxe.SetDefaults(ModContent.ItemType<WoodenPickaxe>());

			var shortsword = new Item();
			shortsword.SetDefaults(ModContent.ItemType<WoodenShortsword>());

			var axe = new Item();
			axe.SetDefaults(ModContent.ItemType<WoodenAxe>());

			return [shortsword, pickaxe, axe];
		}

		public override void ModifyStartingInventory(IReadOnlyDictionary<string, List<Item>> itemsByMod, bool mediumCoreDeath)
		{
			if (mediumCoreDeath)
				return;

			if (itemsByMod.TryGetValue("Terraria", out List<Item> vanillaItems))
			{
				vanillaItems.RemoveAll(item =>
					item.type == ItemID.CopperShortsword ||
					item.type == ItemID.CopperPickaxe ||
					item.type == ItemID.CopperAxe);
			}
		}
	}
}
