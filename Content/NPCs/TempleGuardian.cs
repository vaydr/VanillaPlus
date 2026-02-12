using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaPlus.Content.NPCs
{
	public class TempleGuardian : ModNPC
	{
		// Use Golem Head texture
		public override string Texture => $"Terraria/Images/NPC_{NPCID.GolemHead}";

		public override void SetStaticDefaults()
		{
			// Use same frame count as Golem Head so texture is sliced correctly
			Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.GolemHead];
		}

		public override void SetDefaults()
		{
			// Exact same stats as Dungeon Guardian
			NPC.width = 80;
			NPC.height = 102;
			NPC.damage = 9999;
			NPC.defense = 9999;
			NPC.lifeMax = 9999;
			NPC.HitSound = SoundID.NPCHit4; // Skeletron hit sound
			NPC.DeathSound = SoundID.NPCDeath14; // Skeletron death sound
			NPC.knockBackResist = 0f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.npcSlots = 6f;
			NPC.boss = false;
			NPC.lavaImmune = true;
			NPC.trapImmune = true;

			// Use Dungeon Guardian AI (same aiStyle 11, but no hands)
			NPC.aiStyle = 11;
			AIType = NPCID.DungeonGuardian;
		}

		public override void AI()
		{
			// Spin the sprite constantly (Dungeon Guardian style)
			NPC.rotation += 0.5f;
		}

		public override void FindFrame(int frameHeight)
		{
			// Lock to first frame only - no animation, just spinning
			NPC.frame.Y = 0;
		}

		public override bool CheckDead()
		{
			// Play death sound
			Terraria.Audio.SoundEngine.PlaySound(SoundID.NPCDeath14, NPC.Center);
			return true;
		}

		// No drops
		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			// Empty - no drops
		}
	}
}
