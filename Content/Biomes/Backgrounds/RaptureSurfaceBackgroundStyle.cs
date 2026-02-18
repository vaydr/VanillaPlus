using Terraria;
using Terraria.ModLoader;

namespace VanillaPlus.Content.Biomes.Backgrounds;

/// <summary>
/// Surface background style for the Rapture biome.
/// Supports 4 Hallow surface variants based on world ID.
/// Rainbows are drawn by RaptureRainbowSystem in the sky layer.
/// </summary>
public class RaptureSurfaceBackgroundStyle : ModSurfaceBackgroundStyle
{
    // Determine variant based on world ID (0-3)
    private static int GetVariant() => (int)(Main.worldID % 4);

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
    {
        return GetVariant() switch
        {
            0 => BackgroundTextureLoader.GetBackgroundSlot("VanillaPlus/Content/Biomes/Backgrounds/RaptureSurface1_Far"),
            1 => BackgroundTextureLoader.GetBackgroundSlot("VanillaPlus/Content/Biomes/Backgrounds/RaptureSurface2_Far"),
            2 => BackgroundTextureLoader.GetBackgroundSlot("VanillaPlus/Content/Biomes/Backgrounds/RaptureSurface3_Far"),
            3 => BackgroundTextureLoader.GetBackgroundSlot("VanillaPlus/Content/Biomes/Backgrounds/RaptureSurface4_Far"),
            _ => BackgroundTextureLoader.GetBackgroundSlot("VanillaPlus/Content/Biomes/Backgrounds/RaptureSurface1_Far")
        };
    }

    public override int ChooseMiddleTexture()
    {
        return GetVariant() switch
        {
            0 => BackgroundTextureLoader.GetBackgroundSlot("VanillaPlus/Content/Biomes/Backgrounds/RaptureSurface1_Mid"),
            1 => BackgroundTextureLoader.GetBackgroundSlot("VanillaPlus/Content/Biomes/Backgrounds/RaptureSurface2_Mid"),
            2 => BackgroundTextureLoader.GetBackgroundSlot("VanillaPlus/Content/Biomes/Backgrounds/RaptureSurface3_Far"), // Variant 3 only has 2 layers
            3 => BackgroundTextureLoader.GetBackgroundSlot("VanillaPlus/Content/Biomes/Backgrounds/RaptureSurface4_Mid"),
            _ => BackgroundTextureLoader.GetBackgroundSlot("VanillaPlus/Content/Biomes/Backgrounds/RaptureSurface1_Mid")
        };
    }

    public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b)
    {
        return GetVariant() switch
        {
            0 => BackgroundTextureLoader.GetBackgroundSlot("VanillaPlus/Content/Biomes/Backgrounds/RaptureSurface1_Close"),
            1 => BackgroundTextureLoader.GetBackgroundSlot("VanillaPlus/Content/Biomes/Backgrounds/RaptureSurface2_Close"),
            2 => BackgroundTextureLoader.GetBackgroundSlot("VanillaPlus/Content/Biomes/Backgrounds/RaptureSurface3_Close"),
            3 => BackgroundTextureLoader.GetBackgroundSlot("VanillaPlus/Content/Biomes/Backgrounds/RaptureSurface4_Close"),
            _ => BackgroundTextureLoader.GetBackgroundSlot("VanillaPlus/Content/Biomes/Backgrounds/RaptureSurface1_Close")
        };
    }
}
