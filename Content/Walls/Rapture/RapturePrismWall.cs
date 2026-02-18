using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Common;

namespace VanillaPlus.Content.Walls.Rapture
{
    public class RapturePrismWall : ModWall
    {
        public override void SetStaticDefaults()
        {
            WallID.Sets.Conversion.NewWall3[Type] = true;
            Main.wallHouse[Type] = false;
            DustType = DustID.Gold;
            RaptureIDs.Sets.RaptureWall[Type] = true;
            AddMapEntry(new Color(85, 75, 45));
        }
    }
}
