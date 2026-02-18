using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Common;
using VanillaPlus.Content.Items.Rapture;

namespace VanillaPlus.Content.Walls.Rapture
{
    public class RaptureCrystallineWallSafe : ModWall
    {
        public override void SetStaticDefaults()
        {
            WallID.Sets.Conversion.NewWall2[Type] = true;
            Main.wallHouse[Type] = true;
            DustType = DustID.Gold;
            RaptureIDs.Sets.RaptureWall[Type] = true;
            AddMapEntry(new Color(55, 65, 85));
            RegisterItemDrop(ModContent.ItemType<RaptureCrystallineWallItem>());
        }
    }
}
