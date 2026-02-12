using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Content.NPCs;

namespace VanillaPlus.Content.Tiles
{
	public class TempleProtection : GlobalTile
	{
		public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
		{
			// Only for Lihzahrd Brick
			if (type != TileID.LihzahrdBrick)
				return;

			// Only if Plantera hasn't been defeated
			if (NPC.downedPlantBoss)
				return;

			// If the tile is actually being destroyed (not just damaged), spawn guardian
			if (!fail && !effectOnly)
			{
				SpawnTempleGuardianForPlayer();
			}
		}

		private void SpawnTempleGuardianForPlayer()
		{
			// Find the local player
			Player player = Main.LocalPlayer;

			// Play spawn sound
			SoundEngine.PlaySound(SoundID.Roar, player.Center);

			// Spawn Temple Guardian
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				Vector2 spawnPos = player.Center + Main.rand.NextVector2CircularEdge(400f, 400f);
				int npcIndex = NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (int)spawnPos.X, (int)spawnPos.Y, ModContent.NPCType<TempleGuardian>());

				if (Main.netMode == NetmodeID.Server && npcIndex < Main.maxNPCs)
				{
					NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npcIndex);
				}
			}
		}
	}
}
