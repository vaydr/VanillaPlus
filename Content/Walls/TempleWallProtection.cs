using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Content.NPCs;

namespace VanillaPlus.Content.Walls
{
	public class TempleWallProtection : GlobalWall
	{
		public override void KillWall(int i, int j, int type, ref bool fail)
		{
			// Only for Lihzahrd Brick Wall (unsafe version in temple)
			if (type != WallID.LihzahrdBrickUnsafe)
				return;

			// Only if Plantera hasn't been defeated
			if (NPC.downedPlantBoss)
				return;

			// If the wall is actually being destroyed, spawn guardian
			if (!fail)
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
