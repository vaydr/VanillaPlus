using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Content.Walls.Rapture;

namespace VanillaPlus.Content.Items.Rapture
{
    public class BlisstoneWallItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<BlisstoneWallSafe>());
            Item.width = 24;
            Item.height = 24;
            Item.value = 0;
        }

        public override void AddRecipes()
        {
            CreateRecipe(4)
                .AddIngredient(ModContent.ItemType<Blisstone>(), 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
