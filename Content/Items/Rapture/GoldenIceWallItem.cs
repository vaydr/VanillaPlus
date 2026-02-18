using Terraria;
using Terraria.ModLoader;
using VanillaPlus.Content.Walls.Rapture;

namespace VanillaPlus.Content.Items.Rapture
{
    public class GoldenIceWallItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<GoldenIceWallSafe>());
            Item.width = 24;
            Item.height = 24;
            Item.value = 0;
        }

        // No recipe - uncraftable, only obtained by mining Golden Ice Walls
    }
}
