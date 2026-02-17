using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Common;

namespace VanillaPlus.Content.Walls.Rapture
{
    /// <summary>
    /// BlissandstoneWall - the Rapture equivalent of Sandstone walls.
    /// Golden sandstone wall that generates during Rapture spread.
    /// </summary>
    public class BlissandstoneWall : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;

            // Mark as sandstone wall for conversions
            WallID.Sets.Conversion.Sandstone[Type] = true;

            // Allow underground desert enemies
            WallID.Sets.AllowsUndergroundDesertEnemiesToSpawn[Type] = true;

            // Rapture-specific sets
            RaptureIDs.Sets.RaptureWall[Type] = true;

            AddMapEntry(new Color(195, 175, 140));

            DustType = DustID.Sand;
        }
    }
}
