using System.Collections.Generic;
using Terraria;
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
