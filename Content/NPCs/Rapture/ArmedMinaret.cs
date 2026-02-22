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

            // Movement parameters — smoother, tighter player tracking
            float acceleration = 0.06f;
            float minDistance = 300f;
            float maxVelocity = 3.5f;

            // Subtle periodic distance extension cycle
            NPC.ai[2] += 1f;
            if (NPC.ai[2] > 300f)
            {
                minDistance *= 1.15f;

                if (NPC.ai[2] > 400f)
                    NPC.ai[2] = 0f;
            }

            // Dampen velocity for smoother motion
            NPC.velocity *= 0.97f;

            // Calculate target position: anchor + clamped direction toward player
            Vector2 anchorPosition = new Vector2(NPC.ai[0] * 16f + 8f, NPC.ai[1] * 16f + 8f);
            Vector2 distanceVector = Main.player[NPC.target].Center - anchorPosition;
            float distanceMagnitude = distanceVector.Length();
            if (distanceMagnitude > minDistance)
            {
                distanceVector *= minDistance / distanceMagnitude;
            }

            // Accelerate toward target point
            Vector2 targetPos = anchorPosition + distanceVector;
            if (NPC.position.X < targetPos.X)
            {
                NPC.velocity.X += acceleration;
                if (NPC.velocity.X < 0f && distanceVector.X > 0f)
                    NPC.velocity.X += acceleration * 1.5f;
            }
            else if (NPC.position.X > targetPos.X)
            {
                NPC.velocity.X -= acceleration;
                if (NPC.velocity.X > 0f && distanceVector.X < 0f)
                    NPC.velocity.X -= acceleration * 1.5f;
            }

            if (NPC.position.Y < targetPos.Y)
            {
                NPC.velocity.Y += acceleration;
                if (NPC.velocity.Y < 0f && distanceVector.Y > 0f)
                    NPC.velocity.Y += acceleration * 1.5f;
            }
            else if (NPC.position.Y > targetPos.Y)
            {
                NPC.velocity.Y -= acceleration;
                if (NPC.velocity.Y > 0f && distanceVector.Y < 0f)
                    NPC.velocity.Y -= acceleration * 1.5f;
            }

            // Clamp velocity
            NPC.velocity = Vector2.Clamp(NPC.velocity, new Vector2(-maxVelocity), new Vector2(maxVelocity));

            // Rotation: head aligned with chain direction (pointing away from anchor)
            Vector2 anchorPos = new Vector2(NPC.ai[0] * 16f + 8f, NPC.ai[1] * 16f + 8f);
            NPC.rotation = (NPC.Center - anchorPos).ToRotation() + MathHelper.PiOver2;

            // Collision bounce-back
            if (NPC.collideX)
            {
                NPC.netUpdate = true;
                NPC.velocity.X = NPC.oldVelocity.X * -0.7f;
                if (NPC.velocity.X > 0f && NPC.velocity.X < 2f)
                    NPC.velocity.X = 2f;
                if (NPC.velocity.X < 0f && NPC.velocity.X > -2f)
                    NPC.velocity.X = -2f;
            }
            if (NPC.collideY)
            {
                NPC.netUpdate = true;
                NPC.velocity.Y = NPC.oldVelocity.Y * -0.7f;
                if (NPC.velocity.Y > 0f && NPC.velocity.Y < 2f)
                    NPC.velocity.Y = 2f;
                if (NPC.velocity.Y < 0f && NPC.velocity.Y > -2f)
                    NPC.velocity.Y = -2f;
            }

            // Emit cycling light from head
            float glowCycle = (float)Math.Sin(Main.GameUpdateCount * 0.06f) * 0.5f + 0.5f;
            Color lightColor = Color.Lerp(BananaYellow, BabyBlue, glowCycle);
            Lighting.AddLight(NPC.Center, lightColor.ToVector3());

            // Holy laser turret attack cycle (~3 seconds total)
            // 0-59: Cooldown | 60-89: Telegraph | 90-119: Fire | 120-139: Fade | 140: Reset
            NPC.localAI[3] += 1f;

            if (NPC.localAI[3] == 60f && NPC.HasValidTarget)
            {
                // Telegraph start: lock aim angle, beam length, and fire origin
                Player target = Main.player[NPC.target];
                NPC.localAI[1] = (target.Center - NPC.Center).ToRotation();
                float[] samples = new float[3];
                Vector2 aimDir = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
                Collision.LaserScan(NPC.Center, aimDir, 0f, 2000f, samples);
                NPC.localAI[2] = (samples[0] + samples[1] + samples[2]) / 3f;
                NPC.ai[3] = NPC.Center.X;
                NPC.localAI[0] = NPC.Center.Y;
                SoundEngine.PlaySound(SoundID.Item15, NPC.Center);
                NPC.netUpdate = true;
            }

            if (NPC.localAI[3] == 90f && NPC.HasValidTarget)
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
                        NPC.damage / 2,
                        0f,
                        Main.myPlayer,
                        ai0: NPC.localAI[1],
                        ai1: NPC.localAI[2],
                        ai2: NPC.whoAmI
                    );
                }
            }

            // Spawn damaging explosion at beam endpoint when fire ends
            if (NPC.localAI[3] == 120f && Main.netMode != NetmodeID.MultiplayerClient)
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
            if (NPC.localAI[3] >= 90f && NPC.localAI[3] < 140f && Main.netMode != NetmodeID.Server)
            {
                Vector2 fireOrigin = new Vector2(NPC.ai[3], NPC.localAI[0]);
                Vector2 dir = NPC.localAI[1].ToRotationVector2();
                Vector2 endPos = fireOrigin + dir * NPC.localAI[2];
                float beamAngle = NPC.localAI[1];

                if (NPC.localAI[3] < 120f)
                {
                    // Fire phase: continuous perpendicular sparks
                    for (int i = 0; i < 2; i++)
                    {
                        float perpAngle = beamAngle + (Main.rand.NextBool() ? MathHelper.PiOver2 : -MathHelper.PiOver2);
                        Vector2 dustVel = perpAngle.ToRotationVector2() * Main.rand.NextFloat(1.5f, 3.5f);
                        Dust d = Dust.NewDustDirect(endPos, 0, 0, DustID.GoldFlame, dustVel.X, dustVel.Y);
                        d.noGravity = true;
                        d.scale = 1.4f;
                    }
                }
                else if (NPC.localAI[3] > 120f)
                {
                    // Fade phase: diminishing sparks
                    float fadeProgress = 1f - (NPC.localAI[3] - 120f) / 20f;
                    float perpAngle = beamAngle + (Main.rand.NextBool() ? MathHelper.PiOver2 : -MathHelper.PiOver2);
                    Vector2 dustVel = perpAngle.ToRotationVector2() * Main.rand.NextFloat(1f, 2.5f);
                    Dust d = Dust.NewDustDirect(endPos, 0, 0, DustID.GoldFlame, dustVel.X, dustVel.Y);
                    d.noGravity = true;
                    d.scale = 1.2f * fadeProgress;
                }
            }

            if (NPC.localAI[3] >= 140f)
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
            Color tint = Color.Lerp(BananaYellow, BabyBlue, cycle) * 0.4f;
            tint.A = 180;
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
                    Color color = segTint * 0.4f;
                    color.A = 180;
                    Lighting.AddLight(center, segTint.ToVector3());
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
            if (NPC.localAI[3] >= 60f && NPC.localAI[3] < 140f)
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

        // Draws a circular glow by stacking rotated squares under additive blending.
        // Multiple overlapping rotated squares approximate a circle with natural radial falloff.
        private void DrawCircularGlow(Texture2D pixel, Rectangle src, Vector2 pos, float size, Color color)
        {
            Vector2 origin = new Vector2(0.5f, 0.5f);
            int layers = 6;
            Color layerColor = color * (2f / layers);
            for (int r = 0; r < layers; r++)
            {
                float angle = r / (float)layers * MathHelper.PiOver2;
                Main.EntitySpriteDraw(pixel, pos, src, layerColor,
                    angle, origin, new Vector2(size, size),
                    SpriteEffects.None, 0);
            }
        }

        private void DrawBeam(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            float timer = NPC.localAI[3];
            float beamAngle = NPC.localAI[1];
            float beamLength = NPC.localAI[2];
            Vector2 fireOrigin = new Vector2(NPC.ai[3], NPC.localAI[0]);

            // Phase parameters
            float phaseFade;
            float flashMult = 1f;
            bool isFire = false;
            bool isFade = false;
            float widthMult = 1f;

            if (timer < 90f)
            {
                // Telegraph phase (60-89): beam materializes, grows
                float progress = (timer - 60f) / 30f;
                phaseFade = 0.2f + progress * 0.3f;
                widthMult = 0.4f + progress * 0.6f;
            }
            else if (timer < 120f)
            {
                // Fire phase (90-119): full brightness with initial flash
                isFire = true;
                float fireTick = timer - 90f;
                if (fireTick < 5f)
                    flashMult = 1f + (1f - fireTick / 5f) * 0.8f;
                phaseFade = 1f;
            }
            else
            {
                // Fade phase (120-139): collapse inward
                isFade = true;
                float progress = 1f - (timer - 120f) / 20f;
                phaseFade = progress;
                widthMult = progress * progress;
            }

            // Width breathing during fire
            float breathe = 1f;
            if (isFire)
                breathe = 1f + (float)Math.Sin(Main.GameUpdateCount * 0.3f) * 0.06f;

            // Switch to additive blending
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearClamp,
                DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D pixel = TextureAssets.MagicPixel.Value;
            Rectangle src = new Rectangle(0, 0, 1, 1);
            Vector2 dir = beamAngle.ToRotationVector2();
            Vector2 perp = new Vector2(-dir.Y, dir.X);
            float rotation = beamAngle;
            float maxWidth = 8f * breathe * widthMult;
            Vector2 pixelOrigin = new Vector2(0.5f, 0.5f);
            Color baseColor = GetBeamColor();

            // ═══ ORIGIN ORB ═══
            // Pulsing concentric circular glow at the fire source
            float orbPulse = 1f + (float)Math.Sin(Main.GameUpdateCount * 0.2f) * 0.12f;
            float orbBase;
            if (timer < 90f)
                orbBase = 6f + (timer - 60f) / 30f * 12f;
            else if (timer < 120f)
                orbBase = 18f;
            else
                orbBase = 18f * widthMult;
            float orbSize = orbBase * orbPulse;
            Vector2 orbPos = fireOrigin - Main.screenPosition;

            // Outer glow ring
            DrawCircularGlow(pixel, src, orbPos, orbSize * 2.5f, baseColor * (phaseFade * flashMult * 0.15f));
            // Mid glow ring
            DrawCircularGlow(pixel, src, orbPos, orbSize * 1.5f, baseColor * (phaseFade * flashMult * 0.3f));
            // Bright core
            DrawCircularGlow(pixel, src, orbPos, orbSize * 0.7f,
                Color.Lerp(baseColor, Color.White, 0.7f) * (phaseFade * flashMult * 0.6f));

            // ═══ BEAM BODY ═══
            // Traveling sheens at different speeds
            float sheen1 = ((Main.GameUpdateCount * 0.06f) % 1.4f) - 0.2f;
            float sheen2 = ((Main.GameUpdateCount * 0.035f + 0.7f) % 1.4f) - 0.2f;

            // Gaussian blur passes perpendicular to beam
            float[] blurOffsets = { -2.5f, -1f, 0f, 1f, 2.5f };
            float[] blurWeights = { 0.12f, 0.3f, 1f, 0.3f, 0.12f };

            float segStep = 3f;
            int segments = Math.Max(1, (int)(beamLength / segStep));

            for (int b = 0; b < blurOffsets.Length; b++)
            {
                Vector2 blurShift = perp * blurOffsets[b];
                float w = blurWeights[b];

                for (int i = 0; i <= segments; i++)
                {
                    float t = (float)i / segments;

                    // Forward taper — wide at origin, narrows toward end
                    float taper = (float)Math.Pow(1f - t, 0.6f);

                    // High-freq ripple + low-freq undulation for organic turbulence
                    float noise = 1f
                        + (float)Math.Sin(t * 25f + Main.GameUpdateCount * 0.15f) * 0.08f
                        + (float)Math.Sin(t * 10f + Main.GameUpdateCount * 0.06f) * 0.05f;

                    float segWidth = maxWidth * taper * noise;
                    if (segWidth < 0.3f)
                        continue;

                    Vector2 pos = fireOrigin + dir * (t * beamLength) + blurShift - Main.screenPosition;

                    // Color shifts along beam length
                    Color segBaseColor = GetBeamColor(t * 2f);

                    // Sheen shimmer during fire phase
                    float sheenDist1 = Math.Abs(t - sheen1);
                    float sheenDist2 = Math.Abs(t - sheen2);
                    float sheen = isFire
                        ? (Math.Max(0f, 1f - sheenDist1 * 5f) + Math.Max(0f, 1f - sheenDist2 * 7f) * 0.6f)
                        : 0f;

                    Color segColor = segBaseColor * (phaseFade * w * flashMult);

                    // Wide glare
                    Main.EntitySpriteDraw(pixel, pos, src, segColor * (0.12f + sheen * 0.2f),
                        rotation, pixelOrigin,
                        new Vector2(segStep + 1f, segWidth * 2.5f),
                        SpriteEffects.None, 0);

                    // Outer glow
                    Main.EntitySpriteDraw(pixel, pos, src, segColor * (0.25f + sheen * 0.25f),
                        rotation, pixelOrigin,
                        new Vector2(segStep + 1f, segWidth * 1.5f),
                        SpriteEffects.None, 0);

                    // Mid body
                    Main.EntitySpriteDraw(pixel, pos, src, segColor * (0.45f + sheen * 0.2f),
                        rotation, pixelOrigin,
                        new Vector2(segStep + 1f, segWidth),
                        SpriteEffects.None, 0);

                    // Bright core — center blur pass only, white-hot
                    if (b == 2)
                    {
                        Color coreColor = Color.Lerp(segColor, Color.White * (phaseFade * flashMult), 0.65f)
                            * (0.7f + sheen * 0.4f);
                        Main.EntitySpriteDraw(pixel, pos, src, coreColor,
                            rotation, pixelOrigin,
                            new Vector2(segStep + 1f, segWidth * 0.3f),
                            SpriteEffects.None, 0);
                    }
                }
            }

            // ═══ IMPACT GLOW ═══
            // Circular glow at beam endpoint
            if (isFire || isFade)
            {
                float impactPulse = 1f + (float)Math.Sin(Main.GameUpdateCount * 0.25f) * 0.15f;
                float impactSize = 14f * breathe * impactPulse;
                if (isFade)
                {
                    float fadeProgress = 1f - (timer - 120f) / 20f;
                    impactSize *= (fadeProgress > 0.8f) ? 1.4f : fadeProgress;
                }

                Vector2 endPoint = fireOrigin + dir * beamLength - Main.screenPosition;
                DrawCircularGlow(pixel, src, endPoint, impactSize * 2.2f, baseColor * (phaseFade * flashMult * 0.12f));
                DrawCircularGlow(pixel, src, endPoint, impactSize * 1.3f, baseColor * (phaseFade * flashMult * 0.25f));
                DrawCircularGlow(pixel, src, endPoint, impactSize * 0.6f,
                    Color.Lerp(baseColor, Color.White, 0.6f) * (phaseFade * flashMult * 0.5f));
            }

            // ═══ LIGHTING ═══
            if (isFire)
            {
                Vector3 lightColor = baseColor.ToVector3() * 0.8f;
                float step = 32f;
                int count = (int)(beamLength / step);
                for (int i = 0; i <= count; i++)
                {
                    Vector2 lightPos = fireOrigin + dir * (i * step);
                    Lighting.AddLight(lightPos, lightColor);
                }
            }

            // Restore alpha blending
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
