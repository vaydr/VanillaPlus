using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Common;

namespace VanillaPlus.Content.Walls.Rapture
{
    /// <summary>
    /// HardenedBlissandWall - the Rapture equivalent of Hardened Sand walls.
    /// Golden hardened sand wall that generates during Rapture spread.
    /// </summary>
    public class HardenedBlissandWall : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;

            // Mark as hardened sand wall for conversions
            WallID.Sets.Conversion.HardenedSand[Type] = true;

            // Allow underground desert enemies
            WallID.Sets.AllowsUndergroundDesertEnemiesToSpawn[Type] = true;

            // Rapture-specific sets
            RaptureIDs.Sets.RaptureWall[Type] = true;

            AddMapEntry(new Color(160, 195, 210));

            DustType = DustID.Sand;
        }
    }
}
