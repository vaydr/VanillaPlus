using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaPlus.Content.Buffs
{
	public class LesserShine : ModBuff
	{
		public override string Texture => $"Terraria/Images/Buff_{BuffID.Shine}";

		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = false;
			Main.debuff[Type] = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			// 0.5x normal shine light (normal shine is roughly 0.8f white)
			Lighting.AddLight(player.Center, 0.4f, 0.4f, 0.4f);

			// Remove this buff if any stronger shine is active
			if (player.HasBuff(BuffID.Shine) ||
				player.HasBuff(ModContent.BuffType<GreaterShine>()) ||
				player.HasBuff(ModContent.BuffType<SuperShine>()))
			{
				player.DelBuff(buffIndex);
				buffIndex--;
			}
		}
	}
}
