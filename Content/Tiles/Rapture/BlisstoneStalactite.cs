using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Common;

namespace VanillaPlus.Content.Tiles.Rapture
{
    public class BlisstoneStalactite : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileObsidianKill[Type] = true;
            Main.tileBlockLight[Type] = true;

            TileID.Sets.DrawFlipMode[Type] = 1;
            TileID.Sets.BreakableWhenPlacing[Type] = true;

            RaptureIDs.Sets.Rapture[Type] = true;
            RaptureIDs.Sets.RaptureBiomeSight[Type] = true;

            DustType = DustID.Pearlsand;
            AddMapEntry(new Color(240, 230, 200));
            HitSound = SoundID.Dig;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            RaptureStalactiteHelper.CheckTight(i, j);
            return false;
        }
    }
}
