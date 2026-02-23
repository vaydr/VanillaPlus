using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaPlus.Content.Buffs.Rapture
{
	public class HruntingTagDamage : ModBuff
	{
		public override string Texture => $"Terraria/Images/Buff_{BuffID.SwordWhipNPCDebuff}";

		public override void SetStaticDefaults()
		{
			BuffID.Sets.IsATagBuff[Type] = true;
		}
	}
}
