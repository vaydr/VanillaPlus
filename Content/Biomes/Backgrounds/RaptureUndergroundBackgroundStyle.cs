using Terraria.ModLoader;

namespace VanillaPlus.Content.Biomes.Backgrounds;

/// <summary>
/// Underground/Cavern background style for the Rapture biome.
/// Uses Hallow underground (295) and cavern backgrounds (196, 197, 198, 199).
/// Slot 0-3: Cavern layers (196-199)
/// Slot 4: Underground layer (295)
/// </summary>
public class RaptureUndergroundBackgroundStyle : ModUndergroundBackgroundStyle
{
    public override void FillTextureArray(int[] textureSlots)
    {
        // Cavern backgrounds (196-199)
        textureSlots[0] = BackgroundTextureLoader.GetBackgroundSlot("VanillaPlus/Content/Biomes/Backgrounds/RaptureCavern0");
        textureSlots[1] = BackgroundTextureLoader.GetBackgroundSlot("VanillaPlus/Content/Biomes/Backgrounds/RaptureCavern1");
        textureSlots[2] = BackgroundTextureLoader.GetBackgroundSlot("VanillaPlus/Content/Biomes/Backgrounds/RaptureCavern2");
        textureSlots[3] = BackgroundTextureLoader.GetBackgroundSlot("VanillaPlus/Content/Biomes/Backgrounds/RaptureCavern3");
        // Underground layer (295)
        textureSlots[4] = BackgroundTextureLoader.GetBackgroundSlot("VanillaPlus/Content/Biomes/Backgrounds/RaptureUG_295");
    }
}
