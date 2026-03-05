using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaPlus.Common.GlobalItems
{
	public class OreMiningGlobalTile : GlobalTile
	{
		public override bool CanKillTile(int i, int j, int type, ref bool blockDamaged)
		{
			int minPick = GetMinPick(type);
			if (minPick <= 0)
				return true;

			Player player = Main.LocalPlayer;
			if (player.HeldItem.pick < minPick)
			{
				blockDamaged = false;
				return false;
			}

			return true;
		}

		public override bool CanReplace(int i, int j, int type, int tileTypeBeingPlaced)
		{
			int minPick = GetMinPick(type);
			if (minPick <= 0)
				return true;

			if (GetBestPickPower(Main.LocalPlayer) < minPick)
				return false;

			return true;
		}

		private static int GetBestPickPower(Player player)
		{
			int best = 0;
			for (int i = 0; i < player.inventory.Length; i++)
			{
				if (player.inventory[i].pick > best)
					best = player.inventory[i].pick;
			}
			return best;
		}

		private static int GetMinPick(int type)
		{
			switch (type)
			{
				case TileID.Iron:
				case TileID.Lead:
				case TileID.Silver:
				case TileID.Tungsten:
					return 25;
				case TileID.Gold:
				case TileID.Platinum:
					return 36;
				default:
					return 0;
			}
		}
	}
}
