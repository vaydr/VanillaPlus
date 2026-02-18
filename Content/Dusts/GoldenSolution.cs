using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace VanillaPlus.Content.Dusts
{
    /// <summary>
    /// Golden Solution dust - visual effect for the Rapture Clentaminator spray.
    /// Emits a warm golden light matching the Rapture palette.
    /// </summary>
    public class GoldenSolution : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.noLight = false;
        }

        public override bool Update(Dust dust)
        {
            // Calculate light intensity based on dust scale
            float scale = dust.scale * 0.1f;
            if (scale > 1f)
                scale = 1f;

            // Emit warm golden light (Rapture palette: gold/white/baby blue)
            Lighting.AddLight(
                (int)(dust.position.X / 16f),
                (int)(dust.position.Y / 16f),
                scale * 1.0f,   // Red - full
                scale * 0.85f,  // Green - warm gold
                scale * 0.4f    // Blue - minimal for gold tint
            );

            return true; // Use default dust behavior
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            // Use dust.color if it was set (for baby blue variant), otherwise default golden
            if (dust.color != default)
            {
                return new Color(dust.color.R, dust.color.G, dust.color.B, 0);
            }
            // Default: bright golden-white color
            return new Color(255, 230, 150, 0);
        }
    }
}
