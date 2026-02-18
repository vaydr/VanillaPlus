using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using VanillaPlus.Content.Biomes;
using VanillaPlus.Content.Items.Rapture;

namespace VanillaPlus.Common
{
    /// <summary>
    /// Global NPC hooks for Rapture biome integration.
    /// Handles shop modifications for NPCs like the Steampunker.
    /// </summary>
    public class RaptureGlobalNPC : GlobalNPC
    {
        // Custom conditions for Rapture biome
        public static Condition InRapture = new Condition(
            "Mods.VanillaPlus.Conditions.InRapture",
            () => Main.LocalPlayer.InModBiome<RaptureBiome>()
        );

        public static Condition NotInRapture = new Condition(
            "Mods.VanillaPlus.Conditions.NotInRapture",
            () => !Main.LocalPlayer.InModBiome<RaptureBiome>()
        );

        public override void ModifyShop(NPCShop shop)
        {
            if (shop.NpcType == NPCID.Steampunker)
            {
                // Sell Golden Solution when player is in Rapture biome (not in graveyard)
                shop.InsertAfter(
                    ItemID.BlueSolution,
                    ModContent.ItemType<GoldenSolution>(),
                    Condition.Hardmode,
                    InRapture,
                    Condition.NotInGraveyard
                );

                // Also sell in Hallow graveyard as fallback (like Confection does)
                shop.InsertAfter(
                    ItemID.BlueSolution,
                    ModContent.ItemType<GoldenSolution>(),
                    Condition.Hardmode,
                    Condition.InHallow,
                    Condition.InGraveyard
                );

                // In Rapture graveyard, sell Blue Solution instead (consistency with Confection)
                shop.InsertAfter(
                    ItemID.BlueSolution,
                    ItemID.BlueSolution,
                    Condition.Hardmode,
                    InRapture,
                    Condition.InGraveyard
                );

                // Disable Green Solution in Rapture biome (Rapture replaces Hallow's purity function)
                if (shop.TryGetEntry(ItemID.GreenSolution, out NPCShop.Entry greenEntry))
                {
                    greenEntry.AddCondition(NotInRapture);
                }
            }
        }
    }
}
