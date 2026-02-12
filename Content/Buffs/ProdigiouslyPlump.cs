using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaPlus.Content.Buffs
{
	public class ProdigiouslyPlump : ModBuff
	{
		// Use vanilla Exquisitely Stuffed sprite for now
		public override string Texture => $"Terraria/Images/Buff_{BuffID.WellFed3}";

		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = false;
			Main.debuff[Type] = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			// 4th tier Well Fed - linear extrapolation from Exquisitely Stuffed
			player.statDefense += 5;
			player.GetDamage(DamageClass.Generic) += 0.125f; // +12.5% all damage
			player.GetAttackSpeed(DamageClass.Melee) += 0.125f; // +12.5% melee speed
			player.pickSpeed -= 0.20f; // +20% mining speed (lower = faster)
			player.moveSpeed += 0.50f; // +50% movement speed
			player.GetCritChance(DamageClass.Generic) += 5f; // +5% crit chance
		}
	}
}
