using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Common;

namespace VanillaPlus.Content.Walls.Rapture
{
    public class RaptureShardWall : ModWall
    {
        public override void SetStaticDefaults()
        {
            WallID.Sets.Conversion.NewWall4[Type] = true;
            Main.wallHouse[Type] = false;
            DustType = DustID.Gold;
            RaptureIDs.Sets.RaptureWall[Type] = true;
            AddMapEntry(new Color(65, 55, 45));
        }
    }
}
