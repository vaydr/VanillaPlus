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
			// Corruption enemies
			NPCID.EaterofSouls,
			NPCID.BigEater,
			NPCID.LittleEater,
			NPCID.DevourerHead,
			NPCID.DevourerBody,
			NPCID.DevourerTail,
			NPCID.CorruptBunny,
			NPCID.CorruptGoldfish,
			NPCID.CorruptPenguin,
			NPCID.CorruptSlime,
			NPCID.Slimeling,
			NPCID.Slimer,
			NPCID.SeekerHead,
			NPCID.SeekerBody,
			NPCID.SeekerTail,
			NPCID.Corruptor,
			NPCID.Clinger,
			NPCID.CursedHammer,
			NPCID.DarkMummy,
			NPCID.DesertGhoulCorruption,

			// Crimson enemies
			NPCID.FaceMonster,
			NPCID.Crimera,
			NPCID.BloodCrawler,
			NPCID.BloodCrawlerWall,
			NPCID.CrimsonBunny,
			NPCID.CrimsonGoldfish,
			NPCID.CrimsonPenguin,
			NPCID.Herpling,
			NPCID.Crimslime,
			NPCID.IchorSticker,
			NPCID.FloatyGross,
			NPCID.CrimsonAxe,
			NPCID.BigMimicCorruption,
			NPCID.BigMimicCrimson,
			NPCID.DesertGhoulCrimson,

			// Evil bosses
			NPCID.EaterofWorldsHead,
			NPCID.EaterofWorldsBody,
			NPCID.EaterofWorldsTail,
			NPCID.BrainofCthulhu,
			NPCID.Creeper,
		};

		public override void ResetEffects()
		{
			evilDamageReduction = false;
		}

		public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
		{
			if (evilDamageReduction && IsEvilOrBoss(npc))
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
				if (npc.active && IsEvilOrBoss(npc))
				{
					modifiers.FinalDamage *= 0.85f;
					return;
				}
			}
		}

		private static bool IsEvilOrBoss(NPC npc)
		{
			if (npc.boss)
				return true;

			return EvilNPCs.Contains(npc.type);
		}
	}
}
