using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Common;

namespace VanillaPlus.Content.Walls.Rapture
{
    /// <summary>
    /// BlisstoneWall - the Rapture equivalent of Ebonstone/Crimstone walls.
    /// White stone wall that generates during Rapture spread.
    /// </summary>
    public class BlisstoneWall : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;

            // Mark as stone wall for conversions
            WallID.Sets.Conversion.Stone[Type] = true;

            // Rapture-specific sets
            RaptureIDs.Sets.RaptureWall[Type] = true;

            AddMapEntry(new Color(140, 130, 100));

            DustType = DustID.Gold;
        }
    }
}
