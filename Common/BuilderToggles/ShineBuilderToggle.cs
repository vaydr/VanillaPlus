using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Content.Players;

namespace VanillaPlus.Common.BuilderToggles
{
    public class ShineBuilderToggle : BuilderToggle
    {
        public override bool Active()
            => Main.LocalPlayer.GetModPlayer<ReflectiveCollarPlayer>().hasReflectiveCollar;

        public override string DisplayValue()
            => CurrentState == 0 ? "Shine Effect Off" : "Shine Effect On";

        public override bool Draw(SpriteBatch spriteBatch, ref BuilderToggleDrawParams drawParams)
        {
            // Use gold coin texture as placeholder until custom sprite is made
            drawParams.Texture = TextureAssets.Item[ItemID.GoldCoin].Value;

            // Darken when off (state 0)
            if (CurrentState == 0)
                drawParams.Color = new Color(128, 128, 128);

            return true;
        }
    }
}
