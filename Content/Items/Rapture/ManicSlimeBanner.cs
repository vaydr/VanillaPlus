using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Content.Tiles.Rapture;

namespace VanillaPlus.Content.Items.Rapture
{
    public class ManicSlimeBanner : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<ManicSlimeBannerTile>(), 0);
            Item.width = 10;
            Item.height = 24;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(silver: 10);
        }
    }
}
