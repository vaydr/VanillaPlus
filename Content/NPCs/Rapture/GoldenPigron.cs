using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Content.Biomes;

namespace VanillaPlus.Content.NPCs.Rapture
{
    public class GoldenPigron : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 14;

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers
            {
                Position = new(10f, 5f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = -12f
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 44;
            NPC.height = 36;
            NPC.aiStyle = NPCAIStyleID.DemonEye;
            AIType = NPCID.PigronHallow;
            AnimationType = NPCID.PigronHallow;

            NPC.damage = 70;
            NPC.defense = 16;
            NPC.lifeMax = 210;
            NPC.knockBackResist = 0.3f;
            NPC.noGravity = true;

            NPC.value = 800f;

            NPC.HitSound = SoundID.NPCHit27;
            NPC.DeathSound = SoundID.NPCDeath30;

            Banner = NPCID.PigronHallow;
            BannerItem = Item.BannerToItem(Item.NPCtoBanner(NPCID.PigronHallow));
            SpawnModBiomes = new int[1] { ModContent.GetInstance<IceRaptureUndergroundBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement("Mods.VanillaPlus.Bestiary.GoldenPigron")
            });
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Food(ItemID.Bacon, 15, 1, 1));
            npcLoot.Add(new ItemDropWithConditionRule(ItemID.KitePigron, 25, 1, 1, new Conditions.WindyEnoughForKiteDrops(), 1));
            npcLoot.Add(ItemDropRule.Common(ItemID.PigronMinecart, 100, 1, 1));
            npcLoot.Add(new ItemDropWithConditionRule(ItemID.HamBat, 10, 1, 1, new Conditions.DontStarveIsUp(), 1));
            npcLoot.Add(new ItemDropWithConditionRule(ItemID.HamBat, 25, 1, 1, new Conditions.DontStarveIsNotUp(), 1));
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (!Main.hardMode)
                return 0f;

            if (!spawnInfo.Player.InModBiome<IceRaptureUndergroundBiome>())
                return 0f;

            if ((double)spawnInfo.SpawnTileY <= Main.rockLayer)
                return 0f;

            if (spawnInfo.Water)
                return 0f;

            return 0.15f;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            if (NPC.life > 0)
            {
                for (int i = 0; (double)i < hit.Damage / (double)NPC.lifeMax * 50.0; i++)
                {
                    int dustID = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Cloud, 0f, 0f, 0, default(Color), 1.5f);
                    Dust dust = Main.dust[dustID];
                    dust.velocity *= 1.5f;
                    dust.noGravity = true;
                }
            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    int dustID = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Cloud, 0f, 0f, 0, default(Color), 1.5f);
                    Dust dust = Main.dust[dustID];
                    dust.velocity *= 2f;
                    dust.noGravity = true;
                }
                for (int i = 0; i < 4; i++)
                {
                    int type = 11 + i;
                    if (type > 13)
                    {
                        type = Main.rand.Next(11, 14);
                    }
                    int goreID = Gore.NewGore(NPC.GetSource_Death(), new Vector2(NPC.position.X, NPC.position.Y + (float)(NPC.height / 2) - 10f), new Vector2((float)hit.HitDirection, 0f), type, NPC.scale);
                    Gore gore = Main.gore[goreID];
                    gore.velocity *= 0.3f;
                }
            }
        }
    }
}
