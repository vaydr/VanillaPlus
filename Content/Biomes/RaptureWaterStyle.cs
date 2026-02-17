using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace VanillaPlus.Content.Biomes
{
    /// <summary>
    /// Custom water style for the Rapture biome - bright gold water.
    /// </summary>
    public class RaptureWaterStyle : ModWaterStyle
    {
        public override int ChooseWaterfallStyle()
        {
            return ModContent.Find<ModWaterfallStyle>("VanillaPlus/RaptureWaterfallStyle").Slot;
        }

        public override int GetSplashDust()
        {
            // Use vanilla gold/yellow dust for now
            return Terraria.ID.DustID.GoldCoin;
        }

        public override int GetDropletGore()
        {
            // Use vanilla water droplet for now
            return Terraria.ID.GoreID.WaterDripCorrupt; // Yellowish droplet
        }

        public override void LightColorMultiplier(ref float r, ref float g, ref float b)
        {
            // Bright banana yellow tint for light passing through water
            r = 1f;
            g = 1f;
            b = 0.6f;
        }

        public override Color BiomeHairColor()
        {
            // Bright banana yellow hair color for mermaids
            return new Color(255, 255, 150);
        }

        public override byte GetRainVariant()
        {
            return (byte)Main.rand.Next(3);
        }
    }
}
