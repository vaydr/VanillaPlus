using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Content.Buffs;

namespace VanillaPlus.Content.Items
{
	public class LepTonic : ModItem
	{
		// Use Lesser Luck Potion sprite
		public override string Texture => $"Terraria/Images/Item_{ItemID.LuckPotionLesser}";

		// Get hue based on game time - constantly cycling rainbow
		private float GetHue() => (Main.GameUpdateCount % 360) / 360f;

		public override Color? GetAlpha(Color lightColor)
		{
			// Return cycling rainbow color for item sprite
			return Main.hslToRgb(GetHue(), 1f, 0.7f);
		}

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 26;
			Item.consumable = true;
			Item.useStyle = ItemUseStyleID.DrinkLiquid;
			Item.useTime = 17;
			Item.useAnimation = 17;
			Item.UseSound = SoundID.Item3;
			Item.maxStack = 9999;
			Item.value = Item.buyPrice(silver: 50);
			Item.rare = ItemRarityID.Orange;
		}

		public override bool? UseItem(Player player)
		{
			int duration = 172800; // 48 minutes

			// Combat buffs
			player.AddBuff(BuffID.Ironskin, duration);
			player.AddBuff(BuffID.Regeneration, duration);
			player.AddBuff(BuffID.Swiftness, duration);
			player.AddBuff(BuffID.Rage, duration);
			player.AddBuff(BuffID.Wrath, duration);
			player.AddBuff(BuffID.Endurance, duration);
			player.AddBuff(BuffID.Lifeforce, duration);
			player.AddBuff(BuffID.Inferno, duration);
			player.AddBuff(BuffID.Thorns, duration);
			player.AddBuff(BuffID.Titan, duration);
			player.AddBuff(BuffID.Archery, duration);

			// Magic buffs
			player.AddBuff(BuffID.MagicPower, duration);
			player.AddBuff(BuffID.ManaRegeneration, duration);
			player.AddBuff(BuffID.Clairvoyance, duration);

			// Summoner buffs
			player.AddBuff(BuffID.Summoning, duration);
			player.AddBuff(BuffID.Bewitched, duration);

			// Utility buffs
			player.AddBuff(BuffID.Mining, duration);
			player.AddBuff(BuffID.Builder, duration);
			player.AddBuff(BuffID.Lucky, duration);
			player.AddBuff(BuffID.Gills, duration);
			player.AddBuff(BuffID.Heartreach, duration);
			player.AddBuff(BuffID.Fishing, duration);
			player.AddBuff(BuffID.NightOwl, duration);
			player.AddBuff(BuffID.Sharpened, duration);
			player.AddBuff(BuffID.AmmoReservation, duration);
			player.AddBuff(BuffID.AmmoBox, duration);

			// Healing/regen buffs
			player.AddBuff(BuffID.RapidHealing, duration);
			player.AddBuff(BuffID.SugarRush, duration);

			// Happiness buffs
			player.AddBuff(BuffID.Sunflower, duration); // Happy!
			player.AddBuff(BuffID.Calm, duration); // High Spirits equivalent

			// War Table buffs
			player.AddBuff(BuffID.WarTable, duration); // Strategist

			// Custom buff
			player.AddBuff(ModContent.BuffType<ProdigiouslyPlump>(), duration);

			return true;
		}
	}
}
