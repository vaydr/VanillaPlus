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

			return base.IsArmorSet(head, body, legs);
		}

		public override void UpdateArmorSet(Player player, string set)
		{
			if (set == "VanillaPlus:Silver" || set == "VanillaPlus:Tungsten")
			{
				player.setBonus = "15% reduced damage from evil biome enemies and bosses";
				player.GetModPlayer<ArmorBonusPlayer>().evilDamageReduction = true;
			}
		}
	}
}
