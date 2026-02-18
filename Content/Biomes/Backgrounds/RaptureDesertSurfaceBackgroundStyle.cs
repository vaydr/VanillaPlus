using Terraria.ModLoader;

namespace VanillaPlus.Content.Biomes.Backgrounds;

/// <summary>
/// Desert surface background style for the Rapture biome.
/// Uses Hallow desert (dunes) backgrounds: 311, 312, 313
/// </summary>
public class RaptureDesertSurfaceBackgroundStyle : ModSurfaceBackgroundStyle
{
    public override void ModifyFarFades(float[] fades, float transitionSpeed)
    {
        for (int i = 0; i < fades.Length; i++)
        {
            if (i == Slot)
            {
                fades[i] += transitionSpeed;
                if (fades[i] > 1f) fades[i] = 1f;
            }
            else
            {
                fades[i] -= transitionSpeed;
                if (fades[i] < 0f) fades[i] = 0f;
            }
        }
    }

    public override int ChooseFarTexture()
        => BackgroundTextureLoader.GetBackgroundSlot("VanillaPlus/Content/Biomes/Backgrounds/RaptureDesert_Far");

    public override int ChooseMiddleTexture()
        => BackgroundTextureLoader.GetBackgroundSlot("VanillaPlus/Content/Biomes/Backgrounds/RaptureDesert_Mid");

    public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b)
        => BackgroundTextureLoader.GetBackgroundSlot("VanillaPlus/Content/Biomes/Backgrounds/RaptureDesert_Close");
}
