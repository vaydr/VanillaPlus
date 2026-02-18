using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace VanillaPlus.Content.Biomes
{
    /// <summary>
    /// Custom water style for the Rapture biome - white/baby blue water.
    /// </summary>
    public class RaptureWaterStyle : ModWaterStyle
    {
        public override int ChooseWaterfallStyle()
        {
            return ModContent.Find<ModWaterfallStyle>("VanillaPlus/RaptureWaterfallStyle").Slot;
        }

        public override int GetSplashDust()
        {
            return Terraria.ID.DustID.Cloud;
        }

        public override int GetDropletGore()
        {
            return Terraria.ID.GoreID.WaterDrip;
        }

        public override void LightColorMultiplier(ref float r, ref float g, ref float b)
        {
            // Soft baby blue tint for light passing through water
            r = 0.9f;
            g = 0.95f;
            b = 1f;
        }

        public override Color BiomeHairColor()
        {
            // Baby blue hair color for mermaids
            return new Color(220, 235, 255);
        }

        public override byte GetRainVariant()
        {
            return (byte)Main.rand.Next(3);
        }
    }
}
