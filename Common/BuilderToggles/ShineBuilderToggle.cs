using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using VanillaPlus.Content.Players;

namespace VanillaPlus.Common.BuilderToggles
{
    public class ShineBuilderToggle : BuilderToggle
    {
        public override string Texture => "VanillaPlus/Common/BuilderToggles/ShineToggle";

        public override bool Active()
            => Main.LocalPlayer.GetModPlayer<ReflectiveTapePlayer>().hasReflectiveTape;

        public override string DisplayValue()
            => CurrentState == 0 ? "Shine Effect Off" : "Shine Effect On";

        public override bool Draw(SpriteBatch spriteBatch, ref BuilderToggleDrawParams drawParams)
        {
            // Darken when off (state 0)
            if (CurrentState == 0)
                drawParams.Color = new Color(128, 128, 128);

            return true;
        }
    }
}
