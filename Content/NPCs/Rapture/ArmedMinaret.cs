using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
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

            // Movement parameters (Clinger values)
            float acceleration = 0.035f;
            float minDistance = 225f;
            float maxVelocity = 2f;

            // Periodic distance extension cycle (300-450 ticks)
            NPC.ai[2] += 1f;
            if (NPC.ai[2] > 300f)
            {
                minDistance *= 1.3f;

                if (NPC.ai[2] > 450f)
                    NPC.ai[2] = 0f;
            }

            // Calculate target position: anchor + clamped direction toward player
            Vector2 anchorPosition = new Vector2(NPC.ai[0] * 16f + 8f, NPC.ai[1] * 16f + 8f);
            Vector2 distanceVector = Main.player[NPC.target].Center - anchorPosition;
            float distanceMagnitude = distanceVector.Length();
            if (distanceMagnitude > minDistance)
            {
                distanceVector *= minDistance / distanceMagnitude;
            }

            // Accelerate toward target point
            if (NPC.position.X < NPC.ai[0] * 16f + 8f + distanceVector.X)
            {
                NPC.velocity.X += acceleration;
                if (NPC.velocity.X < 0f && distanceVector.X > 0f)
                    NPC.velocity.X += acceleration * 1.5f;
            }
            else if (NPC.position.X > NPC.ai[0] * 16f + 8f + distanceVector.X)
            {
                NPC.velocity.X -= acceleration;
                if (NPC.velocity.X > 0f && distanceVector.X < 0f)
                    NPC.velocity.X -= acceleration * 1.5f;
            }

            if (NPC.position.Y < NPC.ai[1] * 16f + 8f + distanceVector.Y)
            {
                NPC.velocity.Y += acceleration;
                if (NPC.velocity.Y < 0f && distanceVector.Y > 0f)
                    NPC.velocity.Y += acceleration * 1.5f;
            }
            else if (NPC.position.Y > NPC.ai[1] * 16f + 8f + distanceVector.Y)
            {
                NPC.velocity.Y -= acceleration;
                if (NPC.velocity.Y > 0f && distanceVector.Y < 0f)
                    NPC.velocity.Y -= acceleration * 1.5f;
            }

            // Clamp velocity
            NPC.velocity = Vector2.Clamp(NPC.velocity, new Vector2(-maxVelocity), new Vector2(maxVelocity));

            // Rotation and sprite direction (faces player)
            NPC.spriteDirection = (distanceVector.X > 0f).ToDirectionInt();
            NPC.rotation = NPC.AngleTo(Main.player[NPC.target].Center) + (distanceVector.X < 0f).ToInt() * MathHelper.Pi;

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

            // Fire sword beams periodically (~2.5 seconds)
            NPC.localAI[3] += 1f;
            if (NPC.localAI[3] >= 150f)
            {
                NPC.localAI[3] = 0f;
                if (Main.netMode != NetmodeID.MultiplayerClient && NPC.HasValidTarget)
                {
                    Player target = Main.player[NPC.target];
                    Vector2 direction = target.Center - NPC.Center;
                    if (direction.Length() < 400f)
                    {
                        direction.Normalize();
                        direction *= 10f;
                        Projectile.NewProjectile(
                            NPC.GetSource_FromAI(),
                            NPC.Center,
                            direction,
                            ModContent.ProjectileType<MinaretSwordBeam>(),
                            60,
                            0f,
                            Main.myPlayer
                        );
                    }
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 1.0;
            if (NPC.frameCounter >= 8.0)
            {
                NPC.frameCounter = 0.0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y >= frameHeight * Main.npcFrameCount[Type])
                    NPC.frame.Y = 0;
            }
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
                    Color color = Lighting.GetColor((int)center.X / 16, (int)(center.Y / 16f));
                    spriteBatch.Draw(chain, new Vector2(center.X - screenPos.X, center.Y - screenPos.Y),
                        new Rectangle(0, 0, chain.Width, chain.Height), color, rotation,
                        new Vector2(chain.Width * 0.5f, chain.Height * 0.5f), 1f, SpriteEffects.None, 0f);
                }
            }

            return true;
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
