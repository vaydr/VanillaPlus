using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Content.NPCs;

namespace VanillaPlus.Content.Players
{
	public class TempleGuardianPlayer : ModPlayer
	{
		private int spawnCooldown = 0;

		public override void PostUpdate()
		{
			// Decrease cooldown
			if (spawnCooldown > 0)
				spawnCooldown--;

			// Only check if Plantera hasn't been defeated
			if (NPC.downedPlantBoss)
				return;

			// Can't spawn if on cooldown
			if (spawnCooldown > 0)
				return;

			// Check 1: Proximity to actual temple door (within 3 blocks)
			if (CheckNearTempleDoor())
			{
				SpawnTempleGuardian();
				spawnCooldown = 60;
				return;
			}

			// Check 2: Attempting to mine temple blocks/walls
			if (Player.itemAnimation > 0)
			{
				Item item = Player.HeldItem;
				if (item.pick > 0 || item.hammer > 0)
				{
					int tileX = Player.tileTargetX;
					int tileY = Player.tileTargetY;

					if (tileX >= 0 && tileX < Main.maxTilesX && tileY >= 0 && tileY < Main.maxTilesY)
					{
						Tile tile = Main.tile[tileX, tileY];
						bool isTempleTile = tile.HasTile && tile.TileType == TileID.LihzahrdBrick;
						bool isTempleWall = tile.WallType == WallID.LihzahrdBrickUnsafe;

						if (isTempleTile || isTempleWall)
						{
							SpawnTempleGuardian();
							spawnCooldown = 60;
						}
					}
				}
			}
		}

		private bool CheckNearTempleDoor()
		{
			int playerTileX = (int)(Player.Center.X / 16);
			int playerTileY = (int)(Player.Center.Y / 16);
			int range = 3;

			for (int x = playerTileX - range; x <= playerTileX + range; x++)
			{
				for (int y = playerTileY - range; y <= playerTileY + range; y++)
				{
					if (x < 0 || x >= Main.maxTilesX || y < 0 || y >= Main.maxTilesY)
						continue;

					Tile tile = Main.tile[x, y];
					// Check for Lihzahrd Door: TileID.ClosedDoor (10) with style 11
					if (tile.HasTile && tile.TileType == TileID.ClosedDoor)
					{
						int doorStyle = tile.TileFrameY / 54;
						if (doorStyle == 11) // Lihzahrd door style
							return true;
					}
				}
			}
			return false;
		}

		private void SpawnTempleGuardian()
		{
			// Play the same sound as Dungeon Guardian spawn (Skeletron roar)
			SoundEngine.PlaySound(SoundID.Roar, Player.Center);

			// Spawn the Temple Guardian near the player (similar to Dungeon Guardian behavior)
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				// Spawn from a random direction around the player
				Vector2 spawnPos = Player.Center + Main.rand.NextVector2CircularEdge(400f, 400f);
				int npcIndex = NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (int)spawnPos.X, (int)spawnPos.Y, ModContent.NPCType<TempleGuardian>());

				if (Main.netMode == NetmodeID.Server && npcIndex < Main.maxNPCs)
				{
					NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npcIndex);
				}
			}
		}
	}
}
