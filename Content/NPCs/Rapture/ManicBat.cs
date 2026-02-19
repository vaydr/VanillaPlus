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
    public class ManicBat : ModNPC
    {
        private static readonly Color BananaYellow = new Color(255, 230, 80);
        private static readonly Color BabyBlue = new Color(137, 207, 240);

        private const float SpeedMultiplier = 2f;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 4;

            NPCID.Sets.TrailCacheLength[Type] = 8;
            NPCID.Sets.TrailingMode[Type] = 1;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 26;
            NPC.height = 22;
            NPC.aiStyle = NPCAIStyleID.Bat;
            AIType = NPCID.GiantBat;
            AnimationType = NPCID.GiantBat;

            NPC.damage = 38;
            NPC.defense = 30;
            NPC.lifeMax = 100;
            NPC.knockBackResist = 0.6f;
            NPC.noGravity = true;

            NPC.value = Item.buyPrice(silver: 5);

            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath4;

            NPC.alpha = 100;

            Banner = NPC.type;
            BannerItem = ModContent.ItemType<ManicBatBanner>();
            SpawnModBiomes = new int[1] { ModContent.GetInstance<RaptureUndergroundBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement("Mods.VanillaPlus.Bestiary.ManicBat")
            });
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Nazar: 1/100 (same as Illuminant Bat)
            npcLoot.Add(ItemDropRule.Common(ItemID.Nazar, 100));
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (!Main.hardMode)
                return 0f;

            if (!spawnInfo.Player.InModBiome<RaptureUndergroundBiome>())
                return 0f;

            if (spawnInfo.Water)
                return 0f;

            return 0.4f;
        }

        public override bool PreAI()
        {
            NPC.velocity /= SpeedMultiplier;
            return true;
        }

        public override void PostAI()
        {
            NPC.velocity *= SpeedMultiplier;

            float cycle = (float)Math.Sin(Main.GameUpdateCount * 0.06f) * 0.5f + 0.5f;
            Color lightColor = Color.Lerp(BananaYellow, BabyBlue, cycle);
            Lighting.AddLight(NPC.Center, lightColor.ToVector3() * 0.8f);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

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
            float cycle = (float)Math.Sin(Main.GameUpdateCount * 0.06f) * 0.5f + 0.5f;
            Color tint = Color.Lerp(BananaYellow, BabyBlue, cycle);

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

            Color? alpha = GetAlpha(drawColor);
            Vector2 drawPos = NPC.Center - screenPos;
            spriteBatch.Draw(texture, drawPos, NPC.frame, alpha ?? drawColor, NPC.rotation, origin, NPC.scale, effects, 0f);

            return false;
        }
    }
}
