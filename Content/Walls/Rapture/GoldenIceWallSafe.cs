using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Common;
using VanillaPlus.Content.Items.Rapture;

namespace VanillaPlus.Content.Walls.Rapture
{
    public class GoldenIceWallSafe : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;

            WallID.Sets.Conversion.Ice[Type] = true;

            RaptureIDs.Sets.RaptureWall[Type] = true;

            AddMapEntry(new Color(200, 180, 130));

            DustType = DustID.GoldCoin;
            RegisterItemDrop(ModContent.ItemType<GoldenIceWallItem>());
        }
    }
}
