using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaPlus.Content.Tiles.Rapture
{
    /// <summary>
    /// Hedonwood - the Rapture equivalent of wood.
    /// White wood dropped from Hedon Trees growing on Blissgrass.
    /// </summary>
    public class Hedonwood : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;

            AddMapEntry(new Color(230, 225, 220));

            DustType = DustID.BorealWood;

            RegisterItemDrop(ModContent.ItemType<Items.Rapture.Hedonwood>());
        }
    }
}
