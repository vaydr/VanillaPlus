using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Common;

namespace VanillaPlus.Content.Walls.Rapture
{
    /// <summary>
    /// BlissGrassWall - the Rapture equivalent of Grass walls.
    /// White grass wall that generates during Rapture spread.
    /// </summary>
    public class BlissGrassWall : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;

            // Mark as grass wall for conversions
            WallID.Sets.Conversion.Grass[Type] = true;

            // Rapture-specific sets
            RaptureIDs.Sets.RaptureWall[Type] = true;

            AddMapEntry(new Color(235, 235, 230));

            DustType = DustID.Marble;
        }
    }
}
