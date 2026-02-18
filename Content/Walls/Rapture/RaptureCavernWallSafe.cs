using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Common;
using VanillaPlus.Content.Items.Rapture;

namespace VanillaPlus.Content.Walls.Rapture
{
    public class RaptureCavernWallSafe : ModWall
    {
        public override void SetStaticDefaults()
        {
            WallID.Sets.Conversion.NewWall1[Type] = true;
            Main.wallHouse[Type] = true;
            DustType = DustID.Gold;
            RaptureIDs.Sets.RaptureWall[Type] = true;
            AddMapEntry(new Color(35, 45, 75));
            RegisterItemDrop(ModContent.ItemType<RaptureCavernWallItem>());
        }
    }
}
