using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Content.Players;

namespace VanillaPlus.Common.Systems
{
    public class ShineUISystem : ModSystem
    {
        private static Rectangle buttonRect;
        private static bool wasMouseDown;

        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            if (Main.LocalPlayer == null || !Main.LocalPlayer.active)
                return;

            if (!Main.playerInventory)
                return;

            ReflectiveCollarPlayer modPlayer = Main.LocalPlayer.GetModPlayer<ReflectiveCollarPlayer>();
            if (!modPlayer.hasReflectiveCollar)
                return;

            // Calculate scaled slot size (vanilla uses 56 * inventoryScale)
            int slotSize = (int)(56 * Main.inventoryScale);

            // Toggle button position - 1 slot right of lucky memento gold coin
            int buttonX = 20 + (int)(15.5f * slotSize);
            int buttonY = (int)(258 * Main.inventoryScale) + (int)(Main.inventoryScale * 56 * 4) + slotSize;
            int buttonSize = (int)(22 * Main.inventoryScale);

            buttonRect = new Rectangle(buttonX, buttonY, buttonSize, buttonSize);

            // Draw the toggle button (shine potion icon)
            Texture2D potionTexture = TextureAssets.Item[ItemID.ShinePotion].Value;
            bool isHoveringButton = buttonRect.Contains(Main.mouseX, Main.mouseY);

            // Fully opaque when ON, translucent when OFF
            Color buttonColor;
            if (modPlayer.shineEnabled)
                buttonColor = isHoveringButton ? Color.White : Color.White * 0.9f;
            else
                buttonColor = isHoveringButton ? Color.White * 0.9f : Color.White * 0.5f;

            float scale = (float)buttonSize / potionTexture.Width;
            spriteBatch.Draw(potionTexture, new Vector2(buttonX, buttonY), null, buttonColor, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            // Handle click and hover tooltip
            if (isHoveringButton)
            {
                Main.LocalPlayer.mouseInterface = true;

                // Show tooltip based on current state
                string tooltipText = modPlayer.shineEnabled ? "Shine Effect On" : "Shine Effect Off";
                Main.hoverItemName = tooltipText;

                if (Main.mouseLeft && !wasMouseDown)
                {
                    modPlayer.shineEnabled = !modPlayer.shineEnabled;
                    SoundEngine.PlaySound(SoundID.MenuTick);
                }
            }

            wasMouseDown = Main.mouseLeft;
        }
    }
}
