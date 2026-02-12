using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Content.Players;

namespace VanillaPlus.Content.Items.Consumables
{
    public class LuckyMemento : ModItem
    {
        public override string Texture => $"Terraria/Images/Item_{ItemID.LuckyCoin}";

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.AegisFruit);
            Item.rare = ItemRarityID.LightRed;
        }

        public override bool CanUseItem(Player player)
        {
            return !player.GetModPlayer<LuckyMementoPlayer>().hasLuckyMemento;
        }

        public override bool? UseItem(Player player)
        {
            LuckyMementoPlayer modPlayer = player.GetModPlayer<LuckyMementoPlayer>();

            if (modPlayer.hasLuckyMemento)
            {
                return null;
            }

            modPlayer.hasLuckyMemento = true;
            return true;
        }
    }
}
