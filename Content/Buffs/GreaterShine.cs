using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaPlus.Content.Buffs
{
	public class GreaterShine : ModBuff
	{
		public override string Texture => $"Terraria/Images/Buff_{BuffID.Shine}";

		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = false;
			Main.debuff[Type] = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			// 2x normal shine light
			Lighting.AddLight(player.Center, 1.4f, 1.4f, 1.4f);

			// Remove weaker shines
			player.ClearBuff(ModContent.BuffType<LesserShine>());

			// Remove this buff if Super Shine is active
			if (player.HasBuff(ModContent.BuffType<SuperShine>()))
			{
				player.DelBuff(buffIndex);
				buffIndex--;
			}
		}
	}
}
