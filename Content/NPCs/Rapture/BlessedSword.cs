using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Content.Biomes;

namespace VanillaPlus.Content.NPCs.Rapture
{
    public class BlessedSword : ModNPC
    {
        // Uses custom sprite: Arondight with gold border

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.EnchantedSword];
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.EnchantedSword);
            AIType = NPCID.EnchantedSword;
            AnimationType = NPCID.EnchantedSword;

            // CloneDefaults copies already-scaled stats, so reset to base Normal values
            // to avoid double difficulty scaling
            NPC.lifeMax = 200;
            NPC.damage = 80;
            NPC.defense = 20;

            NPC.value = Item.buyPrice(silver: 5);
            NPC.scale = 1.5f;

            Banner = NPC.type;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<RaptureUndergroundBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement("Mods.VanillaPlus.Bestiary.BlessedSword")
            });
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (!Main.hardMode)
                return 0f;

            if (!spawnInfo.Player.InModBiome<RaptureUndergroundBiome>())
                return 0f;

            if ((double)spawnInfo.SpawnTileY <= Main.rockLayer)
                return 0f;

            if (spawnInfo.Water)
                return 0f;

            return 0.25f;
        }

        private static Texture2D _glowTexture;

        private static Texture2D GenerateGlowTexture(Texture2D baseTexture)
        {
            Color[] pixels = new Color[baseTexture.Width * baseTexture.Height];
            baseTexture.GetData(pixels);

            Color[] glowPixels = new Color[pixels.Length];
            for (int i = 0; i < pixels.Length; i++)
            {
                // Semi-transparent pixels (0 < A < 255) are the glow outline
                if (pixels[i].A > 0 && pixels[i].A < 255)
                    glowPixels[i] = pixels[i];
            }

            Texture2D glowTexture = new Texture2D(Main.graphics.GraphicsDevice, baseTexture.Width, baseTexture.Height);
            glowTexture.SetData(glowPixels);
            return glowTexture;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            _glowTexture ??= GenerateGlowTexture(TextureAssets.Npc[Type].Value);

            Texture2D texture = TextureAssets.Npc[Type].Value;
            Rectangle frame = NPC.frame;
            Vector2 origin = frame.Size() / 2f;
            Vector2 drawPos = NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY);
            SpriteEffects effects = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            // Draw NPC sprite
            spriteBatch.Draw(texture, drawPos, frame, NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, effects, 0f);

            // Draw glow overlay at the exact same position
            float pulse = (float)Math.Sin(Main.GameUpdateCount * 0.05f) * 0.3f + 0.5f;
            Color glowColor = Color.White * pulse;
            glowColor.A = 0;
            spriteBatch.Draw(_glowTexture, drawPos, frame, glowColor, NPC.rotation, origin, NPC.scale, effects, 0f);

            return false;
        }

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
