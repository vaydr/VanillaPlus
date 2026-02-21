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
        public static RecipeGroup HallowedBarRecipeGroup;
        public static RecipeGroup LunarFragmentRecipeGroup;
        public const string LunarFragmentGroupName = "VanillaPlus:AnyLunarFragment";

        public override void Unload()
        {
            CrystalShardRecipeGroup = null;
            HallowedBarRecipeGroup = null;
            LunarFragmentRecipeGroup = null;
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

            // Register Exalted Bar as alternative to Hallowed Bar
            HallowedBarRecipeGroup = new RecipeGroup(
                () => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.HallowedBar)}",
                ItemID.HallowedBar,
                ModContent.ItemType<ExaltedBar>()
            );
            RecipeGroup.RegisterGroup(Lang.GetItemNameValue(ItemID.HallowedBar), HallowedBarRecipeGroup);

            // Any Lunar Fragment (Solar, Vortex, Nebula, Stardust)
            LunarFragmentRecipeGroup = new RecipeGroup(
                () => $"{Language.GetTextValue("LegacyMisc.37")} Lunar Fragment",
                ItemID.FragmentSolar,
                ItemID.FragmentVortex,
                ItemID.FragmentNebula,
                ItemID.FragmentStardust
            );
            RecipeGroup.RegisterGroup(LunarFragmentGroupName, LunarFragmentRecipeGroup);
        }

        public override void PostAddRecipes()
        {
            // Iterate through all recipes and replace Crystal Shard with the RecipeGroup
            for (int i = 0; i < Recipe.numRecipes; i++)
            {
                Recipe recipe = Main.recipe[i];

                // Skip Hallow-specific items and Chik (Chik uses Crystal Shards only; Tempo uses Radiant Shards)
                if (recipe.HasResult(ItemID.CrystalBlock) || recipe.HasResult(ItemID.ShiftingPearlSandsDye)
                    || recipe.HasResult(ItemID.Chik))
                    continue;

                // Replace Crystal Shard ingredient with RecipeGroup
                if (recipe.TryGetIngredient(ItemID.CrystalShard, out var crystalShard))
                {
                    recipe.AddRecipeGroup(Lang.GetItemNameValue(ItemID.CrystalShard), crystalShard.stack);
                    recipe.RemoveIngredient(crystalShard);
                }

                // Replace Hallowed Bar ingredient with RecipeGroup
                if (recipe.TryGetIngredient(ItemID.HallowedBar, out var hallowedBar))
                {
                    recipe.AddRecipeGroup(Lang.GetItemNameValue(ItemID.HallowedBar), hallowedBar.stack);
                    recipe.RemoveIngredient(hallowedBar);
                }
            }
        }
    }
}
