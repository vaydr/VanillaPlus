using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaPlus.Content.Items.Rapture
{
    /// <summary>
    /// RadiantShard - crafting material from the Rapture biome.
    /// Equivalent to Crystal Shards from the Hallow.
    /// </summary>
    public class RadiantShard : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.maxStack = 9999;
            Item.value = Item.sellPrice(silver: 16);
            Item.rare = ItemRarityID.Green;
        }
    }
}
