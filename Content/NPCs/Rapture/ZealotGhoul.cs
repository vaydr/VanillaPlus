using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Content.Biomes;
namespace VanillaPlus.Content.NPCs.Rapture
{
    public class ZealotGhoul : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 8;

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers
            {
                Velocity = 1f
            });

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 18;
            NPC.height = 40;
            NPC.aiStyle = NPCAIStyleID.Fighter;
            AIType = NPCID.DesertGhoulHallow;
            AnimationType = NPCID.DesertGhoulHallow;

            NPC.damage = 60;
            NPC.defense = 40;
            NPC.lifeMax = 280;
            NPC.knockBackResist = 0.7f; // 30% resist

            NPC.value = Item.buyPrice(silver: 8);

            NPC.HitSound = SoundID.NPCHit37;
            NPC.DeathSound = SoundID.NPCDeath40;

            NPC.npcSlots = 0.5f;

            Banner = NPCID.DesertGhoul;
            BannerItem = Item.BannerToItem(Item.NPCtoBanner(NPCID.DesertGhoul));
            SpawnModBiomes = new int[1] { ModContent.GetInstance<DesertRaptureUndergroundBiome>().Type };
        }

        public override bool PreAI()
        {
            if (Main.rand.NextBool(700))
                SoundEngine.PlaySound(Main.rand.NextBool(2) ? SoundID.Zombie55 : SoundID.Zombie56, NPC.position);

            return true;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            if (Main.expertMode)
            {
                NPC.damage = 120;
                NPC.lifeMax = 560;
                NPC.knockBackResist = 0.65f;
            }
            if (Main.masterMode)
            {
                NPC.damage = 180;
                NPC.lifeMax = 840;
                NPC.knockBackResist = 0.6f;
            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement("Mods.VanillaPlus.Bestiary.ZealotGhoul")
            });
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Light Shard: 1/2 (50%) - same as Dreamer Ghoul
            npcLoot.Add(ItemDropRule.Common(ItemID.LightShard, 2));

            // Ancient Cloth: 1/7 (~14%) - same as Dreamer Ghoul
            npcLoot.Add(ItemDropRule.Common(ItemID.AncientCloth, 7));
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (!Main.hardMode)
                return 0f;

            if (!spawnInfo.Player.InModBiome<DesertRaptureUndergroundBiome>())
                return 0f;

            if ((double)spawnInfo.SpawnTileY <= Main.rockLayer)
                return 0f;

            if (spawnInfo.Water)
                return 0f;

            return 0.4f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(BuffID.Confused, 840); // 14 seconds at 100%
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            int dustCount = NPC.life <= 0 ? 20 : (int)(hit.Damage / (double)NPC.lifeMax * 20.0);
            for (int i = 0; i < dustCount; i++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f);
                if (Main.rand.NextBool(4))
                {
                    Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.GoldFlame);
                    dust.noGravity = true;
                    dust.scale = 1.5f;
                    dust.fadeIn = 1f;
                    dust.velocity *= 3f;
                }
            }
        }
    }
}
