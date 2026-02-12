using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;
using Terraria.UI.Chat;
using VanillaPlus.Content.Players;

namespace VanillaPlus.Common.Systems
{
    public class LuckUISystem : ModSystem
    {
        private static Rectangle buttonRect;
        private static bool wasMouseDown;

        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            if (Main.LocalPlayer == null || !Main.LocalPlayer.active)
                return;

            if (!Main.playerInventory)
                return;

            LuckyMementoPlayer modPlayer = Main.LocalPlayer.GetModPlayer<LuckyMementoPlayer>();
            if (!modPlayer.hasLuckyMemento)
                return;

            // Calculate scaled slot size (vanilla uses 56 * inventoryScale)
            int slotSize = (int)(56 * Main.inventoryScale);

            // Toggle button position - next to journey mode icon row
            int buttonX = 20 + (int)(14.5f * slotSize); // Left 3 from previous
            int buttonY = (int)(258 * Main.inventoryScale) + (int)(Main.inventoryScale * 56 * 4) + slotSize; // Down 1
            int buttonSize = (int)(22 * Main.inventoryScale);

            buttonRect = new Rectangle(buttonX, buttonY, buttonSize, buttonSize);

            // Draw the toggle button (gold coin icon)
            Texture2D coinTexture = TextureAssets.Item[ItemID.GoldCoin].Value;
            bool isHoveringButton = buttonRect.Contains(Main.mouseX, Main.mouseY);

            // Fully opaque when ON, translucent when OFF
            Color buttonColor;
            if (modPlayer.showLuckDisplay)
                buttonColor = isHoveringButton ? Color.White : Color.White * 0.9f;
            else
                buttonColor = isHoveringButton ? Color.White * 0.9f : Color.White * 0.5f;

            float scale = (float)buttonSize / coinTexture.Width;
            spriteBatch.Draw(coinTexture, new Vector2(buttonX, buttonY), null, buttonColor, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            // Handle click and hover tooltip
            if (isHoveringButton)
            {
                Main.LocalPlayer.mouseInterface = true;

                // Show tooltip based on current state
                string tooltipText = modPlayer.showLuckDisplay ? "Luck Indicator On" : "Luck Indicator Off";
                Main.hoverItemName = tooltipText;

                if (Main.mouseLeft && !wasMouseDown)
                {
                    modPlayer.showLuckDisplay = !modPlayer.showLuckDisplay;
                    SoundEngine.PlaySound(SoundID.MenuTick);
                }
            }

            wasMouseDown = Main.mouseLeft;

            // Draw luck indicator if toggled on
            if (modPlayer.showLuckDisplay)
            {
                float luck = Main.LocalPlayer.luck;

                Color circleColor;
                if (luck > 0)
                    circleColor = Color.LimeGreen;
                else if (luck < 0)
                    circleColor = Color.OrangeRed;
                else
                    circleColor = Color.Gold;

                // Position directly above the defense indicator using the actual defense icon position
                Vector2 defensePos = AccessorySlotLoader.DefenseIconPosition;
                int luckX = (int)defensePos.X; // Raw defense X
                int luckY = (int)defensePos.Y; // Raw defense Y - see where this actually is

                // Draw circle background (doubled size)
                int circleSize = (int)(56 * Main.inventoryScale);
                DrawCircle(spriteBatch, new Vector2(luckX, luckY), circleSize / 2, circleColor * 0.8f);

                // Draw luck value text centered in circle
                string luckText = luck.ToString("0.0");
                Vector2 textSize = FontAssets.MouseText.Value.MeasureString(luckText) * 0.6f * Main.inventoryScale;
                Vector2 textPos = new Vector2(luckX - textSize.X / 2, luckY - textSize.Y / 2);

                ChatManager.DrawColorCodedStringWithShadow(
                    spriteBatch,
                    FontAssets.MouseText.Value,
                    luckText,
                    textPos,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    Vector2.One * 0.6f * Main.inventoryScale
                );

                // Check for hover on luck indicator
                Rectangle luckRect = new Rectangle(
                    (int)(luckX - circleSize / 2),
                    (int)(luckY - circleSize / 2),
                    circleSize,
                    circleSize
                );

                if (luckRect.Contains(Main.mouseX, Main.mouseY))
                {
                    Main.LocalPlayer.mouseInterface = true;
                    Main.hoverItemName = $"{luck:0.00} Luck";
                }
            }
        }

        private void DrawCircle(SpriteBatch spriteBatch, Vector2 center, int radius, Color color)
        {
            // Use a simple filled rectangle as placeholder (will be sprited later)
            // For now, draw using the MagicPixel texture
            Texture2D pixel = TextureAssets.MagicPixel.Value;

            // Draw a square as placeholder for the circle
            Rectangle rect = new Rectangle(
                (int)(center.X - radius),
                (int)(center.Y - radius),
                radius * 2,
                radius * 2
            );

            spriteBatch.Draw(pixel, rect, color);
        }
    }
}
