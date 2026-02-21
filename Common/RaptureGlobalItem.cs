using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Content.Items.Rapture;

namespace VanillaPlus.Common
{
    /// <summary>
    /// Global item hooks for Rapture biome integration.
    /// Handles boss bag loot modifications (Hallowed Bar → Exalted Bar swap).
    /// </summary>
    public class RaptureGlobalItem : GlobalItem
    {
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            // Swap Hallowed Bar → Exalted Bar in mech boss treasure bags
            if (item.type == ItemID.DestroyerBossBag
                || item.type == ItemID.TwinsBossBag
                || item.type == ItemID.SkeletronPrimeBossBag)
            {
                // Remove vanilla Hallowed Bar drops from the bag
                itemLoot.RemoveWhere(
                    rule => rule is CommonDrop common && common.itemId == ItemID.HallowedBar
                );

                // Add conditional drops: Exalted Bar in Rapture, Hallowed Bar in Hallow
                LeadingConditionRule raptureCondition = new LeadingConditionRule(new RaptureDropRule());
                raptureCondition.OnSuccess(ItemDropRule.Common(ModContent.ItemType<ExaltedBar>(), 1, 15, 30));
                itemLoot.Add(raptureCondition);

                LeadingConditionRule hallowCondition = new LeadingConditionRule(new HallowDropRule());
                hallowCondition.OnSuccess(ItemDropRule.Common(ItemID.HallowedBar, 1, 15, 30));
                itemLoot.Add(hallowCondition);
            }
        }
    }
}
