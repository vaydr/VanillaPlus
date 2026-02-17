using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Content.Players;

namespace VanillaPlus.Content.Items.Consumables
{
    public class ReflectiveTape : ModItem
    {

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
            ReflectiveTapePlayer modPlayer = player.GetModPlayer<ReflectiveTapePlayer>();

            if (modPlayer.hasReflectiveTape)
            {
                return null;
            }

            modPlayer.hasReflectiveTape = true;
            return true;
        }
    }
}
