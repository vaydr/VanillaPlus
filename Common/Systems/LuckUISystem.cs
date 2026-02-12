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

            // Draw the toggle button (lucky coin icon)
            Texture2D coinTexture = TextureAssets.Item[ItemID.LuckyCoin].Value;
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

                // Gradient: full green at >= 0.5, full red at <= -0.5, white at 0
                Color luckColor;
                if (luck >= 0.6f)
                    luckColor = Color.Green;
                else if (luck <= -0.6f)
                    luckColor = Color.Red;
                else if (luck > 0)
                    luckColor = Color.Lerp(Color.White, Color.Green, luck / 0.6f);
                else if (luck < 0)
                    luckColor = Color.Lerp(Color.White, Color.Red, -luck / 0.6f);
                else
                    luckColor = Color.White;

                // Position above defense indicator
                Vector2 defensePos = AccessorySlotLoader.DefenseIconPosition;
                // Center the luck indicator above the defense indicator; not safe but tested and it works.
                int luckX = (int)defensePos.X - (int)(4f * slotSize); //left by 4 slots
                int luckY = (int)defensePos.Y - (int)(1f * slotSize);

                // Draw luck text same style as defense number (luck * 100 to avoid leading "0.")
                string luckText = (luck * 100).ToString("0");

                // Shift horizontally based on digit count to keep centered
                if (luckText.Length == 1)
                    luckX += (int)(0.25f * slotSize); // 1 digit: shift right
                else if (luckText.Length == 2)
                    luckX += (int)(0.1f * slotSize); // 2 digits: shift right
                else if (luckText.Length >= 3)
                    luckX -= (int)(0.2f * slotSize); // 3+ digits: shift left
                // 2 digits: no shift

                Vector2 textPos = new Vector2(luckX, luckY);

                ChatManager.DrawColorCodedStringWithShadow(
                    spriteBatch,
                    FontAssets.MouseText.Value,
                    luckText,
                    textPos,
                    luckColor,
                    0f,
                    Vector2.Zero,
                    Vector2.One
                );

                // Check for hover on luck text
                Vector2 textSize = FontAssets.MouseText.Value.MeasureString(luckText);
                Rectangle luckRect = new Rectangle(
                    (int)luckX,
                    (int)luckY,
                    (int)textSize.X,
                    (int)textSize.Y
                );

                if (luckRect.Contains(Main.mouseX, Main.mouseY))
                {
                    Main.LocalPlayer.mouseInterface = true;
                    Main.hoverItemName = $"{luck * 100:0} Centiluck";
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
