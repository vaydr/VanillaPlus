using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ModLoader;
using VanillaPlus.Content.Biomes;

namespace VanillaPlus.Common.Systems;

/// <summary>
/// Draws rainbows in the sky for the Rapture biome.
/// Hooks into DrawSurfaceBG to draw rainbows at the correct layer (with the far background).
/// </summary>
public class RaptureRainbowSystem : ModSystem
{
    public override void Load()
    {
        On_Main.DrawSurfaceBG += DrawRainbowsHook;
    }

    public override void Unload()
    {
        On_Main.DrawSurfaceBG -= DrawRainbowsHook;
    }

    private void DrawRainbowsHook(On_Main.orig_DrawSurfaceBG orig, Main self)
    {
        // Draw rainbows FIRST so they appear BEHIND the backgrounds
        if (ShouldDrawRainbows())
        {
            DrawRainbows();
        }

        // Then draw the normal backgrounds on top
        orig(self);
    }

    private bool ShouldDrawRainbows()
    {
        if (!Main.dayTime || Main.gameMenu)
            return false;

        if ((double)Main.screenPosition.Y >= Main.worldSurface * 16.0 + 16.0)
            return false;

        if (!RaptureBiome.InRaptureBiome(Main.LocalPlayer))
            return false;

        // Don't draw in desert
        if (Main.LocalPlayer.ZoneDesert)
            return false;

        return true;
    }

    private void DrawRainbows()
    {
        var rainbow1 = ModContent.Request<Texture2D>("VanillaPlus/Content/Biomes/Backgrounds/RaptureRainbow1", AssetRequestMode.ImmediateLoad);
        var rainbow2 = ModContent.Request<Texture2D>("VanillaPlus/Content/Biomes/Backgrounds/RaptureRainbow2", AssetRequestMode.ImmediateLoad);

        if (rainbow1 == null || rainbow2 == null || !rainbow1.IsLoaded || !rainbow2.IsLoaded)
            return;

        // Calculate alpha based on time of day (strongest at midday)
        double timeOfDay = Main.time;
        double midday = 27000.0;
        double timeFactor = 1.0 - Math.Abs(timeOfDay - midday) / midday;
        float alpha = (float)(timeFactor * 0.5f); // Max 50% opacity

        if (alpha <= 0.05f)
            return;

        Color color = Main.ColorOfTheSkies * alpha;

        // Calculate position - rainbows in far sky with slow parallax
        float bgParallax = 0.05f;
        int bgStartX = (int)(0.0 - Math.IEEERemainder(Main.screenPosition.X * bgParallax, rainbow1.Width()) - (rainbow1.Width() / 2));

        // Position high in the sky
        int bgTopY = (int)(Main.screenHeight * 0.08f);

        // Draw rainbow 1 (larger, more faded)
        Main.spriteBatch.Draw(
            rainbow1.Value,
            new Vector2(bgStartX + Main.screenWidth / 2, bgTopY),
            null,
            color * 0.35f,
            0f,
            new Vector2(rainbow1.Width() / 2f, 0),
            1.3f,
            SpriteEffects.None,
            0f
        );

        // Draw rainbow 2 (smaller, slightly offset)
        Main.spriteBatch.Draw(
            rainbow2.Value,
            new Vector2(bgStartX + Main.screenWidth / 2 + 150, bgTopY + 30),
            null,
            color * 0.4f,
            0f,
            new Vector2(rainbow2.Width() / 2f, 0),
            1.1f,
            SpriteEffects.None,
            0f
        );
    }
}
