using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaPlus.Content.Buffs
{
	public class SuperShine : ModBuff
	{
		public override string Texture => $"Terraria/Images/Buff_{BuffID.Shine}";

		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = false;
			Main.debuff[Type] = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			// 4x normal shine light
			Lighting.AddLight(player.Center, 5f, 5f, 5f);

			// Remove all weaker shines (strongest buff, removes everything else)
			player.ClearBuff(ModContent.BuffType<LesserShine>());
			player.ClearBuff(ModContent.BuffType<GreaterShine>());
		}
	}
}
