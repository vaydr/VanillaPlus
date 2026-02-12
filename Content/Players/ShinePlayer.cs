using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Content.Buffs;

namespace VanillaPlus.Content.Players
{
	public class ShinePlayer : ModPlayer
	{
		public override void PostUpdateBuffs()
		{
			// If player has Greater or Super Shine, remove vanilla Shine buff
			if (Player.HasBuff(ModContent.BuffType<GreaterShine>()) ||
				Player.HasBuff(ModContent.BuffType<SuperShine>()))
			{
				Player.ClearBuff(BuffID.Shine);
			}

			// If player has vanilla Shine, remove Lesser Shine (vanilla is stronger)
			if (Player.HasBuff(BuffID.Shine))
			{
				Player.ClearBuff(ModContent.BuffType<LesserShine>());
			}
		}
	}
}
