using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Content.Players;

namespace VanillaPlus.Content.Items.Consumables
{
    public class ReflectiveCollar : ModItem
    {
        public override string Texture => $"Terraria/Images/Item_{ItemID.CoinRing}";

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.AegisFruit);
            Item.rare = ItemRarityID.LightPurple;
        }

        public override bool? UseItem(Player player)
        {
            ReflectiveCollarPlayer modPlayer = player.GetModPlayer<ReflectiveCollarPlayer>();

            if (modPlayer.hasReflectiveCollar)
            {
                return null;
            }

            modPlayer.hasReflectiveCollar = true;
            return true;
        }
    }
}
