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

            // Fixed icon position relative to defense indicator
            Vector2 defensePos = AccessorySlotLoader.DefenseIconPosition;
            Texture2D iconTexture = _luckIcon.Value;

            // Icon center position (fixed)
            float iconCenterX = defensePos.X - (3.6f * slotSize);
            float iconCenterY = defensePos.Y - (0.7f * slotSize);

            // Draw icon at fixed position
            Vector2 iconPos = new Vector2(
                iconCenterX - iconTexture.Width / 2f,
                iconCenterY - iconTexture.Height / 2f - (0.1f * slotSize)
            );
            spriteBatch.Draw(iconTexture, iconPos, null, Color.White * 0.8f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            // Draw text centered on icon
            string luckText = (luck * 100).ToString("0");
            Vector2 textSize = FontAssets.MouseText.Value.MeasureString(luckText);
            Vector2 textPos = new Vector2(
                iconCenterX - textSize.X / 2f,
                iconCenterY - textSize.Y / 2f
            );

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

            // Hover detection based on icon bounds
            Rectangle luckRect = new Rectangle(
                (int)(iconCenterX - iconTexture.Width / 2f),
                (int)(iconCenterY - iconTexture.Height / 2f),
                iconTexture.Width,
                iconTexture.Height
            );

            if (luckRect.Contains(Main.mouseX, Main.mouseY))
            {
                Main.LocalPlayer.mouseInterface = true;
                Main.hoverItemName = luckText + " Centiluck";
            }
        }
    }
}
