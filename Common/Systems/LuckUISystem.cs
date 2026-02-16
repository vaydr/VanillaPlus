using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;
using Terraria.UI.Chat;
using VanillaPlus.Common.BuilderToggles;
using VanillaPlus.Content.Players;

namespace VanillaPlus.Common.Systems
{
    public class LuckUISystem : ModSystem
    {
        private static Asset<Texture2D> _luckIcon;

        public override void Load()
        {
            _luckIcon = ModContent.Request<Texture2D>("VanillaPlus/Common/Systems/LuckIndicator");
        }

        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            if (Main.LocalPlayer == null || !Main.LocalPlayer.active)
                return;

            if (!Main.playerInventory)
                return;

            LuckyMementoPlayer modPlayer = Main.LocalPlayer.GetModPlayer<LuckyMementoPlayer>();
            if (!modPlayer.hasLuckyMemento)
                return;

            // Check if luck toggle is enabled via BuilderToggle
            var toggle = ModContent.GetInstance<LuckBuilderToggle>();
            if (toggle.CurrentState == 0)
                return;

            // Calculate scaled slot size (vanilla uses 56 * inventoryScale)
            int slotSize = (int)(56 * Main.inventoryScale);

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
            Vector2 textSize = FontAssets.MouseText.Value.MeasureString(luckText);

            // Draw clover icon behind the number
            Texture2D iconTexture = _luckIcon.Value;
            Vector2 iconPos = new Vector2(
                luckX + textSize.X / 2f - iconTexture.Width / 2f,
                luckY + textSize.Y / 2f - iconTexture.Height / 2f - (0.1f * slotSize)
            );
            spriteBatch.Draw(iconTexture, iconPos, null, Color.White * 0.8f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

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
            Rectangle luckRect = new Rectangle(
                (int)luckX,
                (int)luckY,
                (int)textSize.X,
                (int)textSize.Y
            );

            if (luckRect.Contains(Main.mouseX, Main.mouseY))
            {
                Main.LocalPlayer.mouseInterface = true;
                Main.hoverItemName = luckText + " Centiluck";
            }
        }
    }
}
