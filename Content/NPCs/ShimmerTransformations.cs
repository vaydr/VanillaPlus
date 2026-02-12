using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Content.Items.Consumables;

namespace VanillaPlus.Content.Items
{
    public class ShimmerTransformations : GlobalItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[ItemID.GoldLadyBug] = ModContent.ItemType<LuckyMemento>();
        }
    }
}
