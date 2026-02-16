using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using VanillaPlus.Content.Players;

namespace VanillaPlus.Common.BuilderToggles
{
    public class LuckBuilderToggle : BuilderToggle
    {
        public override bool Active()
            => Main.LocalPlayer.GetModPlayer<LuckyMementoPlayer>().hasLuckyMemento;

        public override string DisplayValue()
            => CurrentState == 0 ? "Luck Indicator Off" : "Luck Indicator On";

        public override bool Draw(SpriteBatch spriteBatch, ref BuilderToggleDrawParams drawParams)
        {
            // Darken when off (state 0)
            if (CurrentState == 0)
                drawParams.Color = new Color(128, 128, 128);

            return true; // Let vanilla draw with our modified params
        }
    }
}
