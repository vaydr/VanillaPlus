using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Common;
using VanillaPlus.Content.Biomes;
using VanillaPlus.Content.Items.Rapture;
using VanillaPlus.Content.Projectiles.Rapture;

namespace VanillaPlus.Content.NPCs.Rapture
{
    public class ArmedMinaret : ModNPC
    {
        public static Asset<Texture2D> SegmentTexture;

        private static readonly Color BananaYellow = new Color(255, 230, 80);
        private static readonly Color BabyBlue = new Color(137, 207, 240);

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 4;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;

            if (!Main.dedServ)
            {
                SegmentTexture = ModContent.Request<Texture2D>("VanillaPlus/Content/NPCs/Rapture/ArmedMinaretSegment", AssetRequestMode.AsyncLoad);
            }
        }

        public override void SetDefaults()
        {
            NPC.width = 30;
            NPC.height = 30;
            NPC.aiStyle = -1;
            AIType = -1;

            NPC.damage = 80;
            NPC.defense = 45;
            NPC.lifeMax = 400;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;

            NPC.value = Item.buyPrice(gold: 1);

            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath6;

            Banner = NPC.type;
            BannerItem = ModContent.ItemType<ArmedMinaretBanner>();
            SpawnModBiomes = new int[1] { ModContent.GetInstance<RaptureUndergroundBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement("Mods.VanillaPlus.Bestiary.ArmedMinaret")
            });
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Vine: 1/4 (same as Man Eater)
            npcLoot.Add(ItemDropRule.Common(ItemID.Vine, 4));
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

            return 0.3f;
        }

        public override void AI()
        {
            // First tick: find nearest solid tile as anchor
            if (NPC.ai[0] == 0f && NPC.ai[1] == 0f)
            {
                int centerX = (int)(NPC.Center.X / 16f);
                int centerY = (int)(NPC.Center.Y / 16f);
                float closestDistSq = float.MaxValue;
                bool found = false;

                for (int x = centerX - 30; x <= centerX + 30; x++)
                {
                    for (int y = centerY - 30; y <= centerY + 30; y++)
                    {
                        if (x < 0 || x >= Main.maxTilesX || y < 0 || y >= Main.maxTilesY)
                            continue;

                        Tile tile = Main.tile[x, y];
                        if (!tile.HasTile || !Main.tileSolid[tile.TileType])
                            continue;

                        float distSq = (x - centerX) * (x - centerX) + (y - centerY) * (y - centerY);
                        if (distSq < closestDistSq)
                        {
                            closestDistSq = distSq;
                            NPC.ai[0] = x;
                            NPC.ai[1] = y;
                            found = true;
                        }
                    }
                }

                if (!found)
                {
                    NPC.life = -1;
                    NPC.HitEffect(0, 10.0);
                    NPC.active = false;
                    return;
                }

                NPC.netUpdate = true;
            }

            // Validate anchor tile bounds
            if (NPC.ai[0] < 0f || NPC.ai[0] >= Main.maxTilesX || NPC.ai[1] < 0f || NPC.ai[1] >= Main.maxTilesY)
            {
                NPC.life = -1;
                NPC.HitEffect(0, 10.0);
                NPC.active = false;
                return;
            }

            // Die if anchor tile is destroyed
            if (!Main.tile[(int)NPC.ai[0], (int)NPC.ai[1]].HasTile)
            {
                NPC.life = -1;
                NPC.HitEffect(0, 10.0);
                NPC.active = false;
                return;
            }

            // Target nearest player
            NPC.TargetClosest();

            // Movement — accelerate toward a point 3 blocks from the player along the anchor→player line
            float acceleration = 0.08f;
            float maxVelocity = 2.5f;
            float minDistance = 450f;

            Vector2 anchorPosition = new Vector2(NPC.ai[0] * 16f + 8f, NPC.ai[1] * 16f + 8f);
            Vector2 anchorToPlayer = Main.player[NPC.target].Center - anchorPosition;
            float playerDist = anchorToPlayer.Length();
            Vector2 playerDir = playerDist > 0.01f ? anchorToPlayer / playerDist : Vector2.UnitX;

            // Target point: 48px (3 blocks) before the player on the anchor→player line, clamped to chain
            Vector2 targetPos = Main.player[NPC.target].Center - playerDir * 16f;
            Vector2 anchorToTarget = targetPos - anchorPosition;
            float targetDist = anchorToTarget.Length();
            if (targetDist > minDistance)
                targetPos = anchorPosition + anchorToTarget * (minDistance / targetDist);

            // Uniform damping — no directional weirdness
            NPC.velocity *= 0.96f;

            // Accelerate toward target
            Vector2 toTarget = targetPos - NPC.Center;
            float distToTarget = toTarget.Length();
            if (distToTarget > 1f)
                NPC.velocity += toTarget / distToTarget * acceleration;

            // Clamp velocity
            float speed = NPC.velocity.Length();
            if (speed > maxVelocity)
                NPC.velocity *= maxVelocity / speed;

            // Rotation: head aligned with chain direction (pointing away from anchor)
            Vector2 anchorPos = new Vector2(NPC.ai[0] * 16f + 8f, NPC.ai[1] * 16f + 8f);
            NPC.rotation = (NPC.Center - anchorPos).ToRotation() + MathHelper.PiOver2;

            // Emit cycling light from head
            float glowCycle = (float)Math.Sin(Main.GameUpdateCount * 0.06f) * 0.5f + 0.5f;
            Color lightColor = Color.Lerp(BananaYellow, BabyBlue, glowCycle);
            Lighting.AddLight(NPC.Center, lightColor.ToVector3());

            // Holy laser turret attack cycle (scales with health: 4s at full → 2s at low)
            // 0 to cooldown: Cooldown | +0 to +30: Telegraph | +30 to +60: Fire | +60 to +80: Fade | +80: Reset
            if (NPC.localAI[3] == 0f)
            {
                float healthRatio = (float)NPC.life / NPC.lifeMax;
                NPC.ai[2] = (int)MathHelper.Lerp(10f, 160f, healthRatio);
            }

            NPC.localAI[3] += 1f;

            if (NPC.localAI[3] == NPC.ai[2] && NPC.HasValidTarget)
            {
                Player target = Main.player[NPC.target];
                Vector2 aimDir = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
                Vector2 spawnOrigin = NPC.Center + aimDir * 16f;

                // Only fire if there's a clear line of sight from spawn point to player
                if (!Collision.CanHitLine(spawnOrigin, 0, 0, target.Center, 0, 0))
                {
                    NPC.localAI[3] = 0f;
                }
                else
                {
                    // Telegraph start: lock aim angle, beam length, and fire origin
                    NPC.localAI[1] = (target.Center - NPC.Center).ToRotation();
                    float[] samples = new float[3];
                    Collision.LaserScan(NPC.Center, aimDir, 0f, 560f, samples);
                    NPC.localAI[2] = (samples[0] + samples[1] + samples[2]) / 3f;
                    NPC.ai[3] = spawnOrigin.X;
                    NPC.localAI[0] = spawnOrigin.Y;
                    SoundEngine.PlaySound(SoundID.Item15, NPC.Center);
                    NPC.netUpdate = true;
                }
            }

            if (NPC.localAI[3] == NPC.ai[2] + 30f && NPC.HasValidTarget)
            {
                // Fire start: play laser sound and spawn invisible damage projectile at locked origin
                Vector2 fireOrigin = new Vector2(NPC.ai[3], NPC.localAI[0]);
                SoundEngine.PlaySound(SoundID.Item67, fireOrigin);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(
                        NPC.GetSource_FromAI(),
                        fireOrigin,
                        Vector2.Zero,
                        ModContent.ProjectileType<MinaretBeam>(),
                        NPC.damage / 4,
                        0f,
                        Main.myPlayer,
                        ai0: NPC.localAI[1],
                        ai1: NPC.localAI[2],
                        ai2: NPC.whoAmI
                    );
                }
            }

            // Spawn damaging explosion at beam endpoint when fire ends
            if (NPC.localAI[3] == NPC.ai[2] + 60f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Vector2 fireOrigin = new Vector2(NPC.ai[3], NPC.localAI[0]);
                Vector2 dir = NPC.localAI[1].ToRotationVector2();
                Vector2 endPos = fireOrigin + dir * NPC.localAI[2];

                Projectile.NewProjectile(
                    NPC.GetSource_FromAI(),
                    endPos,
                    Vector2.Zero,
                    ModContent.ProjectileType<MinaretBeamExplosion>(),
                    NPC.damage / 3,
                    2f,
                    Main.myPlayer
                );
            }

            // Impact sparks during fire/fade
            if (NPC.localAI[3] >= NPC.ai[2] + RaptureBeamDrawer.TelegraphDuration &&
                NPC.localAI[3] < NPC.ai[2] + RaptureBeamDrawer.TotalDuration &&
                Main.netMode != NetmodeID.Server)
            {
                int elapsed = (int)(NPC.localAI[3] - NPC.ai[2]);
                Vector2 fireOrigin = new Vector2(NPC.ai[3], NPC.localAI[0]);
                Vector2 endPos = fireOrigin + NPC.localAI[1].ToRotationVector2() * NPC.localAI[2];
                var phase = RaptureBeamDrawer.GetPhase(elapsed);
                RaptureBeamDrawer.SpawnImpactSparks(endPos, NPC.localAI[1], phase);
            }

            if (NPC.localAI[3] >= NPC.ai[2] + 80f)
            {
                NPC.localAI[3] = 0f;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            // Lock to frame 0 (crystal head)
            NPC.frame.Y = 0;
        }

        public override Color? GetAlpha(Color drawColor)
        {
            float cycle = (float)Math.Sin(Main.GameUpdateCount * 0.06f) * 0.5f + 0.5f;
            Color baseTint = Color.Lerp(BananaYellow, BabyBlue, cycle);

            // Charge-up glow: ramp 0→1 during telegraph (60-89), fade back over ~0.25s (15 ticks) after fire
            float chargeIntensity = 0f;
            float timer = NPC.localAI[3];
            float cd = NPC.ai[2];
            if (timer >= cd && timer < cd + 30f)
                chargeIntensity = (timer - cd) / 30f;
            else if (timer >= cd + 30f && timer < cd + 45f)
                chargeIntensity = 1f - (timer - (cd + 30f)) / 15f;

            float mult = MathHelper.Lerp(0.4f, 1f, chargeIntensity);
            Color tint = Color.Lerp(baseTint * mult, Color.White, chargeIntensity * 0.4f);
            tint.A = (byte)MathHelper.Lerp(180f, 255f, chargeIntensity);
            return tint;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            // Draw chain segments from NPC head to anchor tile
            Texture2D chain = SegmentTexture.Value;
            Vector2 center = NPC.Center;
            float drawPositionX = NPC.ai[0] * 16f + 8f - center.X;
            float drawPositionY = NPC.ai[1] * 16f + 8f - center.Y;
            float rotation = (float)Math.Atan2(drawPositionY, drawPositionX) - MathHelper.PiOver2;
            bool draw = true;

            while (draw)
            {
                float totalDrawDistance = (float)Math.Sqrt(drawPositionX * drawPositionX + drawPositionY * drawPositionY);
                if (totalDrawDistance < 16f)
                {
                    draw = false;
                }
                else
                {
                    totalDrawDistance = 16f / totalDrawDistance;
                    drawPositionX *= totalDrawDistance;
                    drawPositionY *= totalDrawDistance;
                    center.X += drawPositionX;
                    center.Y += drawPositionY;
                    drawPositionX = NPC.ai[0] * 16f + 8f - center.X;
                    drawPositionY = NPC.ai[1] * 16f + 8f - center.Y;
                    float segCycle = (float)Math.Sin(Main.GameUpdateCount * 0.06f) * 0.5f + 0.5f;
                    Color segTint = Color.Lerp(BananaYellow, BabyBlue, segCycle);

                    // Charge-up glow on chain segments (same as head)
                    float segCharge = 0f;
                    float segTimer = NPC.localAI[3];
                    float segCd = NPC.ai[2];
                    if (segTimer >= segCd && segTimer < segCd + 30f)
                        segCharge = (segTimer - segCd) / 30f;
                    else if (segTimer >= segCd + 30f && segTimer < segCd + 45f)
                        segCharge = 1f - (segTimer - (segCd + 30f)) / 15f;

                    float segMult = MathHelper.Lerp(0.4f, 1f, segCharge);
                    Color color = Color.Lerp(segTint * segMult, Color.White, segCharge * 0.4f);
                    color.A = (byte)MathHelper.Lerp(180f, 255f, segCharge);
                    Lighting.AddLight(center, segTint.ToVector3() * MathHelper.Lerp(1f, 1.5f, segCharge));
                    spriteBatch.Draw(chain, new Vector2(center.X - screenPos.X, center.Y - screenPos.Y),
                        new Rectangle(0, 0, chain.Width, chain.Height), color, rotation,
                        new Vector2(chain.Width * 0.5f, chain.Height * 0.5f), 1f, SpriteEffects.None, 0f);
                }
            }

            // Draw head with cycling glow tint
            Texture2D texture = TextureAssets.Npc[Type].Value;
            Vector2 origin = NPC.frame.Size() / 2f;
            Color? alpha = GetAlpha(drawColor);
            spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, alpha ?? drawColor, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);

            // Draw beam during telegraph/fire/fade phases
            if (NPC.localAI[3] >= NPC.ai[2] && NPC.localAI[3] < NPC.ai[2] + 80f)
            {
                DrawBeam(spriteBatch, screenPos);
            }

            return false;
        }

        private Color GetBeamColor(float offset = 0f)
        {
            float cycle = (float)Math.Sin(Main.GameUpdateCount * 0.08f + offset) * 0.5f + 0.5f;
            float cycle2 = (float)Math.Sin(Main.GameUpdateCount * 0.05f + offset + 1.5f) * 0.5f + 0.5f;
            Color mid = Color.Lerp(BananaYellow, new Color(255, 255, 240), cycle);
            return Color.Lerp(mid, BabyBlue, cycle2);
        }

        private void DrawBeam(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            int elapsed = (int)(NPC.localAI[3] - NPC.ai[2]);
            var phase = RaptureBeamDrawer.GetPhase(elapsed);
            Vector2 fireOrigin = new Vector2(NPC.ai[3], NPC.localAI[0]);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearClamp,
                DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            RaptureBeamDrawer.DrawBeam(fireOrigin, NPC.localAI[1], NPC.localAI[2], 8f, phase, GetBeamColor);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp,
                DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
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
                dust.scale = 1.3f;
            }
        }
    }
}
