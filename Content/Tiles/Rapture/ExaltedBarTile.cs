using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using VanillaPlus.Content.Items.Rapture;

namespace VanillaPlus.Content.Tiles.Rapture
{
    public class ExaltedBarTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileShine[Type] = 1100;
            Main.tileSolid[Type] = true;
            Main.tileSolidTop[Type] = true;
            Main.tileFrameImportant[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            RegisterItemDrop(ModContent.ItemType<ExaltedBar>());
            AddMapEntry(new Color(140, 210, 240), Language.GetText("MapObject.MetalBar"));
        }
    }
}
