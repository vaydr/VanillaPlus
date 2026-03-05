using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Content.Players;

namespace VanillaPlus.Common.GlobalItems
{
	public class SilverTungstenArmorGlobalItem : GlobalItem
	{
		public override string IsArmorSet(Item head, Item body, Item legs)
		{
			if (head.type == ItemID.SilverHelmet && body.type == ItemID.SilverChainmail && legs.type == ItemID.SilverGreaves)
				return "VanillaPlus:Silver";

			if (head.type == ItemID.TungstenHelmet && body.type == ItemID.TungstenChainmail && legs.type == ItemID.TungstenGreaves)
				return "VanillaPlus:Tungsten";

			if (head.type == ItemID.GoldHelmet && body.type == ItemID.GoldChainmail && legs.type == ItemID.GoldGreaves)
				return "VanillaPlus:Gold";

			return base.IsArmorSet(head, body, legs);
		}

		public override void UpdateArmorSet(Player player, string set)
		{
			if (set == "VanillaPlus:Silver" || set == "VanillaPlus:Tungsten")
			{
				// Vanilla already grants +3 defense, just add the evil DR
				player.setBonus = "+3 defense\n15% reduced damage from evil biome enemies and bosses";
				player.GetModPlayer<ArmorBonusPlayer>().evilDamageReduction = true;
			}
			else if (set == "VanillaPlus:Gold")
			{
				// Vanilla gives +3, add +1 more to match Platinum's +4
				player.setBonus = "+4 defense";
				player.statDefense += 1;
			}
		}
	}
}
