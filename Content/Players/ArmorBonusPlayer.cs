using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaPlus.Content.Players
{
	public class ArmorBonusPlayer : ModPlayer
	{
		public bool evilDamageReduction;

		private static readonly HashSet<int> EvilNPCs = new()
		{
			// Corruption enemies - prehardmode
			NPCID.EaterofSouls,
			NPCID.BigEater,
			NPCID.LittleEater,
			NPCID.DevourerHead,
			NPCID.DevourerBody,
			NPCID.DevourerTail,
			NPCID.CorruptSlime,
			NPCID.Slimeling,
			NPCID.Slimer,

			// Corruption enemies - hardmode
			NPCID.SeekerHead,
			NPCID.SeekerBody,
			NPCID.SeekerTail,
			NPCID.Corruptor,
			NPCID.Clinger,
			NPCID.CursedHammer,
			NPCID.DarkMummy,
			NPCID.DesertGhoulCorruption,
			NPCID.BigMimicCorruption,

			// Corruption critters
			NPCID.CorruptBunny,
			NPCID.CorruptGoldfish,
			NPCID.CorruptPenguin,

			// Crimson enemies - prehardmode
			NPCID.FaceMonster,
			NPCID.Crimera,
			NPCID.BloodCrawler,
			NPCID.BloodCrawlerWall,
			NPCID.BloodFeeder,

			// Crimson enemies - hardmode
			NPCID.Herpling,
			NPCID.Crimslime,
			NPCID.IchorSticker,
			NPCID.FloatyGross,
			NPCID.CrimsonAxe,
			NPCID.DesertGhoulCrimson,
			NPCID.BigMimicCrimson,
			NPCID.BloodMummy,
			NPCID.BloodJelly,

			// Crimson critters
			NPCID.CrimsonBunny,
			NPCID.CrimsonGoldfish,
			NPCID.CrimsonPenguin,

			// Eater of Worlds (Corruption boss)
			NPCID.EaterofWorldsHead,
			NPCID.EaterofWorldsBody,
			NPCID.EaterofWorldsTail,
			NPCID.VileSpit,

			// Brain of Cthulhu (Crimson boss)
			NPCID.BrainofCthulhu,
			NPCID.Creeper,
		};

		public override void ResetEffects()
		{
			evilDamageReduction = false;
		}

		public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
		{
			if (evilDamageReduction && IsEvilNPC(npc))
			{
				modifiers.FinalDamage *= 0.85f;
			}
		}

		public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
		{
			if (!evilDamageReduction)
				return;

			// Find the source NPC for hostile projectiles
			if (proj.npcProj || proj.trap)
				return;

			// Check if any active evil/boss NPC is the likely source
			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC npc = Main.npc[i];
				if (npc.active && IsEvilNPC(npc))
				{
					modifiers.FinalDamage *= 0.85f;
					return;
				}
			}
		}

		private static bool IsEvilNPC(NPC npc)
		{
			return EvilNPCs.Contains(npc.type);
		}
	}
}
