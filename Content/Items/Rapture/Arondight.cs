using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Content.Projectiles.Rapture;

namespace VanillaPlus.Content.Items.Rapture
{
	public class Arondight : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 90;
			Item.DamageType = DamageClass.Melee;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 23;
			Item.useAnimation = 23;
			Item.scale = 1.2f;
			Item.noMelee = true;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 4.5f;
			Item.value = Item.sellPrice(gold: 4, silver: 60);
			Item.rare = ItemRarityID.Pink;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<ArondightSlash>();
			Item.ResearchUnlockCount = 1;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Projectile.NewProjectile(source, player.MountedCenter, velocity, type, damage, knockback, player.whoAmI, player.direction * player.gravDir, player.itemAnimationMax);
			return false;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<ExaltedBar>(), 12)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}
	}
}
