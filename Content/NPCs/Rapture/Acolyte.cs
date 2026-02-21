using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Content.Biomes;

namespace VanillaPlus.Content.NPCs.Rapture
{
    public class Acolyte : ModNPC
    {
        public override string Texture => $"Terraria/Images/NPC_{NPCID.Paladin}";

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.Paladin];
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.Paladin);
            AIType = NPCID.Paladin;
            AnimationType = NPCID.Paladin;

            NPC.npcSlots = 1f;
            NPC.scale = 1f;
            NPC.damage = 50;
            NPC.defense = 25;
            NPC.lifeMax = 2500;
            NPC.value = Item.buyPrice(gold: 1);

            Banner = NPC.type;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<RaptureBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement("Mods.VanillaPlus.Bestiary.Acolyte")
            });
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

            int dustCount = NPC.life <= 0 ? 25 : (int)(hit.Damage / (double)NPC.lifeMax * 10.0);
            for (int i = 0; i < dustCount; i++)
            {
                Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height,
                    DustID.GoldFlame, hit.HitDirection * 2f, -1f);
                dust.noGravity = true;
                dust.scale = 1.1f;
            }
        }
    }
}
