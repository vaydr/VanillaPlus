using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Content.Biomes;
using VanillaPlus.Content.Items.Rapture;

namespace VanillaPlus.Content.NPCs.Rapture
{
    public class Seraph : ModNPC
    {
        public override string Texture => $"Terraria/Images/NPC_{NPCID.Harpy}";

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.Harpy];
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.Harpy);
            AIType = NPCID.Harpy;
            AnimationType = NPCID.Harpy;

            NPC.damage = 60;
            NPC.defense = 24;
            NPC.lifeMax = 400;
            NPC.value = Item.buyPrice(silver: 10);

            Banner = NPC.type;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<RaptureBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement("Mods.VanillaPlus.Bestiary.Seraph")
            });
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GildedFeather>()));
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (!Main.hardMode)
                return 0f;

            if (!spawnInfo.Player.InModBiome<RaptureBiome>())
                return 0f;

            if (spawnInfo.Water)
                return 0f;

            return 0.3f;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            int dustCount = NPC.life <= 0 ? 30 : (int)(hit.Damage / (double)NPC.lifeMax * 10.0);
            for (int i = 0; i < dustCount; i++)
            {
                Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height,
                    DustID.GoldFlame, hit.HitDirection * 2f, -1f);
                dust.noGravity = true;
                dust.scale = 1.3f;
            }
        }
    }
}
