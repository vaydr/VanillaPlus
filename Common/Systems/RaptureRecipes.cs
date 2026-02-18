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
            CrystalShardRecipeGroup = new RecipeGroup(
                () => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.CrystalShard)}",
                ItemID.CrystalShard,
                ModContent.ItemType<RadiantShard>()
            );
            RecipeGroup.RegisterGroup(Lang.GetItemNameValue(ItemID.CrystalShard), CrystalShardRecipeGroup);
        }

        public override void PostAddRecipes()
        {
            // Iterate through all recipes and replace Crystal Shard with the RecipeGroup
            for (int i = 0; i < Recipe.numRecipes; i++)
            {
                Recipe recipe = Main.recipe[i];

                // Skip Crystal Block and Shifting Pearlsands Dye (Hallow-specific items)
                if (recipe.HasResult(ItemID.CrystalBlock) || recipe.HasResult(ItemID.ShiftingPearlSandsDye))
                    continue;

                // Replace Crystal Shard ingredient with RecipeGroup
                if (recipe.TryGetIngredient(ItemID.CrystalShard, out var crystalShard))
                {
                    recipe.AddRecipeGroup(Lang.GetItemNameValue(ItemID.CrystalShard), crystalShard.stack);
                    recipe.RemoveIngredient(crystalShard);
                }
            }
        }
    }
}
