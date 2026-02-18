using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Common;
using VanillaPlus.Content.Items.Rapture;

namespace VanillaPlus.Content.Walls.Rapture
{
    public class BlisstoneWallSafe : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;

            WallID.Sets.Conversion.Stone[Type] = true;

            RaptureIDs.Sets.RaptureWall[Type] = true;

            AddMapEntry(new Color(180, 170, 140));

            DustType = DustID.Gold;
            RegisterItemDrop(ModContent.ItemType<BlisstoneWallItem>());
        }
    }
}
