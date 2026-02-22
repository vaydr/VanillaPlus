using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Content.Biomes;
using VanillaPlus.Content.Items.Rapture;
using RaptureTiles = VanillaPlus.Content.Tiles.Rapture;

namespace VanillaPlus.Content.NPCs.Rapture
{
    public class BrightMummy : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 16;

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers
            {
                Velocity = 0.5f
            });

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 18;
            NPC.height = 40;
            NPC.aiStyle = NPCAIStyleID.Fighter;
            AIType = NPCID.LightMummy;
            AnimationType = NPCID.LightMummy;

            NPC.damage = 55;
            NPC.defense = 18;
            NPC.lifeMax = 200;
            NPC.knockBackResist = 0.55f;

            NPC.value = 700f;

            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath6;

            Banner = NPC.type;
            BannerItem = ModContent.ItemType<BrightMummyBanner>();
            SpawnModBiomes = new int[2] {
                ModContent.GetInstance<DesertRaptureSurfaceBiome>().Type,
                ModContent.GetInstance<DesertRaptureUndergroundBiome>().Type
            };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement("Mods.VanillaPlus.Bestiary.BrightMummy")
            });
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Light Shard: 1/10 (10%)
            npcLoot.Add(ItemDropRule.Common(ItemID.LightShard, 10));

            // Mummy vanity set: 1/75 each (1.33%)
            npcLoot.Add(ItemDropRule.Common(ItemID.MummyMask, 75));
            npcLoot.Add(ItemDropRule.Common(ItemID.MummyShirt, 75));
            npcLoot.Add(ItemDropRule.Common(ItemID.MummyPants, 75));

            // Trifold Map: 1/100 (1%)
            npcLoot.Add(ItemDropRule.StatusImmunityItem(ItemID.TrifoldMap, 100));
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            // Match vanilla Light Mummy: hardmode + spawns on Pearlsand (→ Blissand), cavern layer only
            if (Main.hardMode && spawnInfo.SpawnTileType == ModContent.TileType<RaptureTiles.Blissand>()
                && (double)spawnInfo.SpawnTileY > Main.rockLayer)
                return 0.5f;

            return 0f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (Main.rand.NextBool(14))
            {
                int duration = 300; // Classic: 5 seconds
                if (Main.masterMode)
                    duration = 750; // Master: 12.5 seconds
                else if (Main.expertMode)
                    duration = 600; // Expert: 10 seconds

                target.AddBuff(BuffID.Confused, duration);
            }
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
