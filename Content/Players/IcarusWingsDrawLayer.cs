using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using VanillaPlus.Content.Items.Accessories;

namespace VanillaPlus.Content.Players
{
	public class IcarusWingsDrawLayer : PlayerDrawLayer
	{
		// Get hue based on game time - constantly cycling rainbow
		private float GetHue() => (Main.GameUpdateCount % 360) / 360f;

		public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Wings);

		public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
		{
			// Only draw if player has Icarus Wings equipped
			Player player = drawInfo.drawPlayer;
			int icarusWingSlot = EquipLoader.GetEquipSlot(ModContent.GetInstance<VanillaPlus>(), "IcarusWings", EquipType.Wings);
			return player.wings == icarusWingSlot && !drawInfo.drawPlayer.dead;
		}

		protected override void Draw(ref PlayerDrawSet drawInfo)
		{
			// Apply rainbow color to the wing draw
			Color rainbow = Main.hslToRgb(GetHue(), 1f, 0.7f);

			// Tint the wings by modifying the color of wing draws
			// We need to find and modify the wing draw data
			for (int i = 0; i < drawInfo.DrawDataCache.Count; i++)
			{
				DrawData data = drawInfo.DrawDataCache[i];
				// Check if this is a wing draw (wings use the wing texture)
				if (data.texture == Terraria.GameContent.TextureAssets.Wings[drawInfo.drawPlayer.wings].Value)
				{
					// Replace with rainbow-tinted version
					data.color = rainbow;
					drawInfo.DrawDataCache[i] = data;
				}
			}
		}
	}
}
