using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using VanillaPlus.Content.Items.Rapture;

namespace VanillaPlus.Common.Systems
{
    /// <summary>
    /// Handles recipe group registration for Rapture items.
    /// Makes Radiant Shards work as alternatives to Crystal Shards in all vanilla recipes.
    /// </summary>
    public class RaptureRecipes : ModSystem
    {
        public static RecipeGroup CrystalShardRecipeGroup;

        public override void Unload()
        {
            CrystalShardRecipeGroup = null;
        }

        public override void AddRecipeGroups()
        {
            // Register Radiant Shard as alternative to Crystal Shard
            // This makes Radiant Shards usable in ALL vanilla Crystal Shard recipes:
            // - Phasesabers, Crystal Storm, Crystal Bullets, Crystal Darts
            // - Chik, Magical Harp, Rainbow Rod, Greater Healing Potion, Super Mana Potion
            CrystalShardRecipeGroup = new RecipeGroup(
                () => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.CrystalShard)}",
                ItemID.CrystalShard,
                ModContent.ItemType<RadiantShard>()
            );
            RecipeGroup.RegisterGroup(Lang.GetItemNameValue(ItemID.CrystalShard), CrystalShardRecipeGroup);
        }
    }
}
