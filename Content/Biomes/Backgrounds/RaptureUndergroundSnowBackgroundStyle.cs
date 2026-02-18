using Terraria.ModLoader;

namespace VanillaPlus.Content.Biomes.Backgrounds;

/// <summary>
/// Underground ice background style for the Rapture biome.
/// Uses Hallow ice backgrounds: 203, 204, 205, 206
/// </summary>
public class RaptureUndergroundSnowBackgroundStyle : ModUndergroundBackgroundStyle
{
    public override void FillTextureArray(int[] textureSlots)
    {
        textureSlots[0] = BackgroundTextureLoader.GetBackgroundSlot("VanillaPlus/Content/Biomes/Backgrounds/RaptureIce0");
        textureSlots[1] = BackgroundTextureLoader.GetBackgroundSlot("VanillaPlus/Content/Biomes/Backgrounds/RaptureIce1");
        textureSlots[2] = BackgroundTextureLoader.GetBackgroundSlot("VanillaPlus/Content/Biomes/Backgrounds/RaptureIce2");
        textureSlots[3] = BackgroundTextureLoader.GetBackgroundSlot("VanillaPlus/Content/Biomes/Backgrounds/RaptureIce3");
    }
}
