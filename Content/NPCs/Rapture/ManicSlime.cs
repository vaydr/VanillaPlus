using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Content.Biomes;
using VanillaPlus.Content.Items.Rapture;

namespace VanillaPlus.Content.NPCs.Rapture
{
    public class ManicSlime : ModNPC
    {
        // Color cycling endpoints
        private static readonly Color BananaYellow = new Color(255, 230, 80);
        private static readonly Color BabyBlue = new Color(137, 207, 240);

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 2;

            // Trail afterimages
            NPCID.Sets.TrailCacheLength[Type] = 8;
            NPCID.Sets.TrailingMode[Type] = 1;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 24;
            NPC.height = 18;
            NPC.aiStyle = NPCAIStyleID.Slime;
            AIType = NPCID.IlluminantSlime;
            AnimationType = NPCID.IlluminantSlime;

            NPC.damage = 80;
            NPC.defense = 30;
            NPC.lifeMax = 190;
            NPC.knockBackResist = 0.8f; // 20% resist = 0.8 multiplier

            NPC.value = Item.buyPrice(silver: 5);

            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath6;

            NPC.alpha = 100;
            NPC.noGravity = false;

            Banner = NPC.type;
            BannerItem = ModContent.ItemType<ManicSlimeBanner>();
            SpawnModBiomes = new int[1] { ModContent.GetInstance<RaptureUndergroundBiome>().Type };
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            if (Main.expertMode)
            {
                NPC.damage = 160;
                NPC.lifeMax = 380;
                NPC.knockBackResist = 0.75f; // 25% resist
            }
            if (Main.masterMode)
            {
                NPC.damage = 240;
                NPC.lifeMax = 570;
                NPC.knockBackResist = 0.7f; // 30% resist
            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement("Mods.VanillaPlus.Bestiary.ManicSlime")
            });
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Gel: 1-2, always (same as Illuminant Slime)
            npcLoot.Add(ItemDropRule.Common(ItemID.Gel, 1, 1, 2));

            // Slime Staff: 1/10000 (same as Illuminant Slime)
            npcLoot.Add(ItemDropRule.Common(ItemID.SlimeStaff, 10000));

            // Apple Pie: same as Illuminant Slime
            npcLoot.Add(ItemDropRule.Common(ItemID.ApplePie, 15));
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (!Main.hardMode)
                return 0f;

            if (!spawnInfo.Player.InModBiome<RaptureBiome>())
                return 0f;

            // Underground only
            if ((double)spawnInfo.SpawnTileY <= Main.rockLayer)
                return 0f;

            if (spawnInfo.Water)
                return 0f;

            return 0.4f;
        }

        private const float SpeedMultiplier = 2f;

        public override bool PreAI()
        {
            // Undo horizontal scaling so the AI sees "normal" speed for its internal logic
            NPC.velocity.X /= SpeedMultiplier;
            return true;
        }

        public override void PostAI()
        {
            // Reapply horizontal scaling — the AI targeted normal speeds,
            // so the actual movement is 2x faster without compounding
            NPC.velocity.X *= SpeedMultiplier;

            // Emit cycling light
            float cycle = (float)Math.Sin(Main.GameUpdateCount * 0.06f) * 0.5f + 0.5f;
            Color lightColor = Color.Lerp(BananaYellow, BabyBlue, cycle);
            Lighting.AddLight(NPC.Center, lightColor.ToVector3() * 0.8f);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            // Dust on hit - cycling colored particles
            int dustCount = NPC.life <= 0 ? 30 : (int)(hit.Damage / (double)NPC.lifeMax * 10.0);
            for (int i = 0; i < dustCount; i++)
            {
                Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.TintableDustLighted, hit.HitDirection * 2f, -2f);
                float cycle = (float)Math.Sin((Main.GameUpdateCount + i * 10) * 0.06f) * 0.5f + 0.5f;
                dust.color = Color.Lerp(BananaYellow, BabyBlue, cycle);
                dust.noGravity = true;
                dust.scale = 1.2f;
            }
        }

        public override Color? GetAlpha(Color drawColor)
        {
            // Calculate the cycling color
            float cycle = (float)Math.Sin(Main.GameUpdateCount * 0.06f) * 0.5f + 0.5f;
            Color tint = Color.Lerp(BananaYellow, BabyBlue, cycle);

            // Apply alpha for semi-transparency (like illuminant slime)
            float alphaFactor = (255 - NPC.alpha) / 255f;
            return new Color(
                (int)(tint.R * alphaFactor),
                (int)(tint.G * alphaFactor),
                (int)(tint.B * alphaFactor),
                (int)(180 * alphaFactor)
            );
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[Type].Value;
            Vector2 origin = NPC.frame.Size() / 2f;
            SpriteEffects effects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            // Draw afterimage trail
            for (int i = NPC.oldPos.Length - 1; i > 0; i--)
            {
                if (NPC.oldPos[i] == Vector2.Zero)
                    continue;

                float progress = 1f - (float)i / NPC.oldPos.Length;
                float trailCycle = (float)Math.Sin((Main.GameUpdateCount - i * 4) * 0.06f) * 0.5f + 0.5f;
                Color trailColor = Color.Lerp(BananaYellow, BabyBlue, trailCycle) * (progress * 0.3f);
                trailColor.A = 0;

                Vector2 trailPos = NPC.oldPos[i] + NPC.Size / 2f - screenPos;
                spriteBatch.Draw(texture, trailPos, NPC.frame, trailColor, NPC.rotation, origin, NPC.scale, effects, 0f);
            }

            // Draw main sprite with cycling color
            Color? alpha = GetAlpha(drawColor);
            Vector2 drawPos = NPC.Center - screenPos;
            spriteBatch.Draw(texture, drawPos, NPC.frame, alpha ?? drawColor, NPC.rotation, origin, NPC.scale, effects, 0f);

            return false; // Skip default drawing
        }
    }
}
