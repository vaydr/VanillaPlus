using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaPlus.Content.Items.Rapture
{
    /// <summary>
    /// Golden Solution - Clentaminator ammo that spreads the Rapture biome.
    /// Sold by the Steampunker when in the Rapture biome.
    /// </summary>
    public class GoldenSolution : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults()
        {
            // The projectile offset trick: Clentaminator adds ProjectileID.PureSpray to the ammo's shoot value
            Item.shoot = ModContent.ProjectileType<Projectiles.Rapture.GoldenSolution>() - ProjectileID.PureSpray;
            Item.ammo = AmmoID.Solution;
            Item.width = 10;
            Item.height = 12;
            Item.value = Item.buyPrice(silver: 15);
            Item.rare = ItemRarityID.Orange;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
        }
    }
}
