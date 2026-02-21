using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaPlus.Content.Items.Rapture
{
    public class HaloScrap : ModItem
    {
        public override string Texture => $"Terraria/Images/Item_{ItemID.HallowedBar}";

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 9999;
            Item.value = Item.buyPrice(silver: 20);
            Item.rare = ItemRarityID.LightRed;
        }
    }
}
