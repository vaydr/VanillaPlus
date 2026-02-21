using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using VanillaPlus.Content.Biomes;
using VanillaPlus.Content.Items.Rapture;
using Tiles = VanillaPlus.Content.Tiles.Rapture;

namespace VanillaPlus.Common
{
    /// <summary>
    /// Global NPC hooks for Rapture biome integration.
    /// Handles shop modifications and critter/enemy spawning.
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

        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            if (!spawnInfo.Player.InModBiome<RaptureBiome>())
                return;

            // Remove vanilla ghoul - Zealot Ghoul replaces it in Rapture
            pool.Remove(NPCID.DesertGhoul);

            // Remove vanilla Light Mummy - Bright Mummy replaces it in Rapture
            pool.Remove(NPCID.LightMummy);

            // Remove vanilla Hallow Pigron - Golden Pigron replaces it in Rapture
            pool.Remove(NPCID.PigronHallow);

            // Remove vanilla Cave Bat and Giant Bat - Manic Bat replaces them in Rapture
            pool.Remove(NPCID.CaveBat);
            pool.Remove(NPCID.GiantBat);

            int tileType = spawnInfo.SpawnTileType;
            bool isRaptureTile = tileType == ModContent.TileType<Tiles.Blissgrass>()
                || tileType == ModContent.TileType<Tiles.Blisstone>()
                || tileType == ModContent.TileType<Tiles.Blissand>()
                || tileType == ModContent.TileType<Tiles.GoldenIce>()
                || tileType == ModContent.TileType<Tiles.HardenedBlissand>()
                || tileType == ModContent.TileType<Tiles.Blissandstone>();

            if (!isRaptureTile)
                return;

            bool surface = (double)spawnInfo.SpawnTileY <= Main.worldSurface;
            bool underground = (double)spawnInfo.SpawnTileY > Main.rockLayer;

            // Lightning Bug - surface, night, same as vanilla Hallow behavior
            if (surface && !Main.dayTime && !spawnInfo.Water)
            {
                if (!NPC.TooWindyForButterflies && Main.rand.NextBool(NPC.fireFlyFriendly))
                {
                    pool[NPCID.LightningBug] = 1f;
                }
            }

            // Fairies - underground Rapture, any time
            if (underground && !spawnInfo.Water)
            {
                if (Main.rand.NextBool(4))
                {
                    int fairy = Main.rand.NextFromList(
                        NPCID.FairyCritterBlue,
                        NPCID.FairyCritterGreen,
                        NPCID.FairyCritterPink
                    );
                    pool[fairy] = 0.5f;
                }
            }

            // Rainbow Slime - same conditions as vanilla Hallow: surface, raining, only one at a time
            if (surface && Main.hardMode && Main.cloudAlpha > 0f && !NPC.AnyNPCs(NPCID.RainbowSlime))
            {
                pool[NPCID.RainbowSlime] = 0.05f;
            }

            // Prismatic Lacewing - post-Plantera, surface, night (7:30 PM to 12:00 AM)
            if (surface && !Main.dayTime && NPC.downedPlantBoss && !spawnInfo.Water)
            {
                // Vanilla spawns between 7:30 PM and midnight
                // In Terraria, night time goes from 0 to 32400 (9 hours)
                // 7:30 PM = time 0, midnight = time 16200
                if (Main.time < 16200.0 && !NPC.AnyNPCs(NPCID.EmpressButterfly))
                {
                    pool[NPCID.EmpressButterfly] = 0.05f;
                }
            }
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == NPCID.DukeFishron)
            {
                // Add Laguna to Duke Fishron's weapon drop pool (1/5 -> 1/6)
                foreach (var rule in npcLoot.Get(false))
                {
                    if (rule is OneFromOptionsNotScaledWithLuckDropRule notScaled
                        && notScaled.dropIds.Contains(ItemID.Flairon))
                    {
                        var newIds = notScaled.dropIds.ToList();
                        newIds.Add(ModContent.ItemType<Laguna>());
                        notScaled.dropIds = newIds.ToArray();
                        break;
                    }
                }
            }
        }

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
