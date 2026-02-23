using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaPlus.Common.GlobalItems
{
	public class SpectreScraperGlobalItem : GlobalItem
	{
		public override bool AppliesToEntity(Item entity, bool lateInstantiation)
		{
			return entity.type == ItemID.SpectrePaintScraper;
		}

		public override bool? UseItem(Item item, Player player)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
				return null;

			int x = Player.tileTargetX;
			int y = Player.tileTargetY;

			if (!WorldGen.InWorld(x, y, 1))
				return null;

			Tile tile = Main.tile[x, y];
			if (!tile.HasTile)
				return null;

			int mossItem = GetMossItem(tile.TileType);
			if (mossItem > 0)
			{
				Item.NewItem(player.GetSource_TileInteraction(x, y), x * 16, y * 16, 16, 16, mossItem);
			}

			return null;
		}

		private static int GetMossItem(int tileType)
		{
			return tileType switch
			{
				TileID.GreenMoss => ItemID.GreenMoss,
				TileID.BrownMoss => ItemID.BrownMoss,
				TileID.RedMoss => ItemID.RedMoss,
				TileID.BlueMoss => ItemID.BlueMoss,
				TileID.PurpleMoss => ItemID.PurpleMoss,
				TileID.LavaMoss => ItemID.LavaMoss,
				TileID.KryptonMoss => ItemID.KryptonMoss,
				TileID.XenonMoss => ItemID.XenonMoss,
				TileID.ArgonMoss => ItemID.ArgonMoss,
				TileID.VioletMoss => ItemID.VioletMoss,
				TileID.RainbowMoss => ItemID.RainbowMoss,
				_ => 0
			};
		}
	}
}
