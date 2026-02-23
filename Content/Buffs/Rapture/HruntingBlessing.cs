using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaPlus.Content.Buffs.Rapture
{
	public class HruntingBlessing : ModBuff
	{
		public override string Texture => $"Terraria/Images/Buff_{BuffID.SwordWhipPlayerBuff}";

		public override void SetStaticDefaults()
		{
			Main.buffNoSave[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetAttackSpeed(DamageClass.Melee) += 0.25f;
		}
	}
}
