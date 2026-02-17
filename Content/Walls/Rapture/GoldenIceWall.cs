using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Common;

namespace VanillaPlus.Content.Walls.Rapture
{
    /// <summary>
    /// GoldenIceWall - the Rapture equivalent of Ice walls.
    /// Golden ice wall that generates during Rapture spread.
    /// </summary>
    public class GoldenIceWall : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;

            // Mark as ice wall for conversions
            WallID.Sets.Conversion.Ice[Type] = true;

            // Rapture-specific sets
            RaptureIDs.Sets.RaptureWall[Type] = true;

            AddMapEntry(new Color(240, 225, 190));

            DustType = DustID.Ice;
        }
    }
}
