using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Content.Biomes;
using VanillaPlus.Content.Items.Rapture;

namespace VanillaPlus.Content.NPCs.Rapture
{
    public class Cherub : ModNPC
    {
        public override string Texture => $"Terraria/Images/NPC_{NPCID.Slimer}";

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.Slimer];
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.Slimer);
            AIType = NPCID.Slimer;
            AnimationType = NPCID.Slimer;

            Banner = NPC.type;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<RaptureBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement("Mods.VanillaPlus.Bestiary.Cherub")
            });
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<HaloScrap>(), 1, 1, 3));
        }

        // Spawn logic handled by RaptureGlobalNPC.EditSpawnPool (Pixie-equivalent rate)

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            int dustCount = NPC.life <= 0 ? 20 : (int)(hit.Damage / (double)NPC.lifeMax * 10.0);
            for (int i = 0; i < dustCount; i++)
            {
                Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height,
                    DustID.GoldFlame, hit.HitDirection * 2f, -1f);
                dust.noGravity = true;
                dust.scale = 1.2f;
            }
        }
    }
}
