using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Chat;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;
using VanillaPlus.Content.Tiles.Rapture;
using VanillaPlus.Content.Walls.Rapture;

namespace VanillaPlus.Common.Systems
{
    /// <summary>
    /// World system for the Rapture biome. Handles:
    /// - World flag for whether this world has Rapture instead of Hallow
    /// - Save/load of world data
    /// - Hardmode GERunner hook to generate Rapture V-stripe
    /// - Multiplayer sync
    /// </summary>
    public class RaptureWorldSystem : ModSystem
    {
        /// <summary>
        /// Whether this world has Rapture instead of Hallow.
        /// Set during hardmode generation (WoF kill).
        /// </summary>
        public static bool HasRapture { get; set; }

        /// <summary>
        /// Random style variants for trees/backgrounds (future use).
        /// </summary>
        public static int RaptureTreeStyle { get; set; }
        public static int RaptureBGStyle { get; set; }

        public override void Load()
        {
            // Hook into the GERunner to intercept Hallow generation
            On_WorldGen.GERunner += On_WorldGen_GERunner;
        }

        public override void Unload()
        {
            On_WorldGen.GERunner -= On_WorldGen_GERunner;
        }

        public override void OnWorldLoad()
        {
            // Reset to defaults when loading a world
            HasRapture = false;
            RaptureTreeStyle = 0;
            RaptureBGStyle = 0;
        }

        public override void OnWorldUnload()
        {
            HasRapture = false;
            RaptureTreeStyle = 0;
            RaptureBGStyle = 0;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag["VanillaPlus:HasRapture"] = HasRapture;
            tag["VanillaPlus:RaptureTreeStyle"] = RaptureTreeStyle;
            tag["VanillaPlus:RaptureBGStyle"] = RaptureBGStyle;
        }

        public override void SaveWorldHeader(TagCompound tag)
        {
            // Save to header so world select screen can show info
            tag["HasRapture"] = HasRapture;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            HasRapture = tag.GetBool("VanillaPlus:HasRapture");
            RaptureTreeStyle = tag.GetInt("VanillaPlus:RaptureTreeStyle");
            RaptureBGStyle = tag.GetInt("VanillaPlus:RaptureBGStyle");
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(HasRapture);
            writer.Write(RaptureTreeStyle);
            writer.Write(RaptureBGStyle);
        }

        public override void NetReceive(BinaryReader reader)
        {
            HasRapture = reader.ReadBoolean();
            RaptureTreeStyle = reader.ReadInt32();
            RaptureBGStyle = reader.ReadInt32();
        }

        public override void ModifyHardmodeTasks(List<GenPass> list)
        {
            // Decide if this world gets Rapture or Hallow
            // DEBUG: Always Rapture for testing. Change to Main.rand.NextBool() for 50/50
            HasRapture = true; // TODO: Make this random or configurable

            if (HasRapture)
            {
                // Replace the hardmode announcement to mention Rapture
                int announcementIndex = list.FindIndex(g => g.Name.Equals("Hardmode Announcement"));
                if (announcementIndex != -1)
                {
                    list.Insert(announcementIndex + 1, new PassLegacy("Rapture Announcement", RaptureAnnouncement));
                    list.RemoveAt(announcementIndex);
                }
            }

            // Set random style variants
            RaptureTreeStyle = Main.rand.Next(3);
            RaptureBGStyle = Main.rand.Next(3);
        }

        private void RaptureAnnouncement(GenerationProgress progress, GameConfiguration config)
        {
            // Announce Rapture spread instead of Hallow
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText(Language.GetTextValue("Mods.VanillaPlus.WorldGen.RaptureSpread"), 50, 255, 130);
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(
                    NetworkText.FromKey("Mods.VanillaPlus.WorldGen.RaptureSpread"),
                    new Color(50, 255, 130)
                );
            }
        }

        /// <summary>
        /// Hook into WorldGen.GERunner to generate Rapture instead of Hallow.
        /// GERunner is the "Good/Evil Runner" that creates the V-stripe.
        /// When good=true, it generates Hallow. We intercept this for Rapture.
        /// </summary>
        private void On_WorldGen_GERunner(On_WorldGen.orig_GERunner orig, int i, int j, double speedX, double speedY, bool good)
        {
            if (good && HasRapture)
            {
                // Generate Rapture instead of Hallow
                RaptureGERunner(i, j, speedX, speedY);
                return;
            }

            // Let vanilla handle it (evil biomes or Hallow if not Rapture world)
            orig(i, j, speedX, speedY, good);
        }

        /// <summary>
        /// Generate the Rapture V-stripe. This is adapted from Confection's ConfectGERunner.
        /// </summary>
        private static void RaptureGERunner(int i, int j, double speedX = 0.0, double speedY = 0.0)
        {
            // Calculate runner parameters (same as vanilla GERunner)
            int runnerSize = WorldGen.genRand.Next(200, 250);
            double worldScale = Main.maxTilesX / 4200.0;
            runnerSize = (int)(runnerSize * worldScale);
            int runnerWidth = runnerSize;

            Vector2D position = new Vector2D(i, j);
            Vector2D velocity = new Vector2D(
                WorldGen.genRand.Next(-10, 11) * 0.1,
                WorldGen.genRand.Next(-10, 11) * 0.1
            );

            if (speedX != 0.0 || speedY != 0.0)
            {
                velocity.X = speedX;
                velocity.Y = speedY;
            }

            bool running = true;
            while (running)
            {
                int minX = (int)(position.X - runnerWidth * 0.5);
                int maxX = (int)(position.X + runnerWidth * 0.5);
                int minY = (int)(position.Y - runnerWidth * 0.5);
                int maxY = (int)(position.Y + runnerWidth * 0.5);

                // Clamp to world bounds
                minX = Math.Max(0, minX);
                maxX = Math.Min(Main.maxTilesX, maxX);
                minY = Math.Max(0, minY);
                maxY = Math.Min(Main.maxTilesY - 5, maxY);

                for (int x = minX; x < maxX; x++)
                {
                    for (int y = minY; y < maxY; y++)
                    {
                        // Check if within elliptical area (same as Confection)
                        if (!(Math.Abs((double)x - position.X) + Math.Abs((double)y - position.Y) < (double)runnerSize * 0.5 * (1.0 + (double)WorldGen.genRand.Next(-10, 11) * 0.015)))
                            continue;

                        // Convert walls first (matching Confection's pattern)
                        ushort wallType = Main.tile[x, y].WallType;

                        // Grass walls -> BlissGrassWall
                        if (wallType == WallID.GrassUnsafe || wallType == WallID.FlowerUnsafe ||
                            wallType == WallID.Grass || wallType == WallID.Flower ||
                            wallType == WallID.CorruptGrassUnsafe || wallType == WallID.CrimsonGrassUnsafe ||
                            wallType == WallID.HallowedGrassUnsafe)
                        {
                            Main.tile[x, y].WallType = (ushort)ModContent.WallType<BlissGrassWall>();
                        }
                        // Hardened sand walls -> HardenedBlissandWall
                        else if (wallType == WallID.HardenedSand || wallType == WallID.CorruptHardenedSand ||
                                 wallType == WallID.CrimsonHardenedSand || wallType == WallID.HallowHardenedSand)
                        {
                            Main.tile[x, y].WallType = (ushort)ModContent.WallType<HardenedBlissandWall>();
                        }
                        // Sandstone walls -> BlissandstoneWall
                        else if (wallType == WallID.Sandstone || wallType == WallID.CorruptSandstone ||
                                 wallType == WallID.CrimsonSandstone || wallType == WallID.HallowSandstone)
                        {
                            Main.tile[x, y].WallType = (ushort)ModContent.WallType<BlissandstoneWall>();
                        }
                        // Stone walls (ebonstone/crimstone) -> BlisstoneWall
                        else if (wallType == WallID.EbonstoneUnsafe || wallType == WallID.CrimstoneUnsafe ||
                                 wallType == WallID.Stone || wallType == WallID.PearlstoneBrickUnsafe)
                        {
                            Main.tile[x, y].WallType = (ushort)ModContent.WallType<BlisstoneWall>();
                        }
                        // Ice walls -> GoldenIceWall
                        else if (wallType == WallID.IceUnsafe)
                        {
                            Main.tile[x, y].WallType = (ushort)ModContent.WallType<GoldenIceWall>();
                        }

                        // Convert tiles (matching Confection's exact pattern)
                        if (Main.tile[x, y].TileType == TileID.Grass)
                        {
                            Main.tile[x, y].TileType = (ushort)ModContent.TileType<Blissgrass>();
                            WorldGen.SquareTileFrame(x, y);
                        }
                        else if (Main.tile[x, y].TileType == TileID.Stone || Main.tile[x, y].TileType == TileID.Ebonstone || Main.tile[x, y].TileType == TileID.Crimstone)
                        {
                            Main.tile[x, y].TileType = (ushort)ModContent.TileType<Blisstone>();
                            WorldGen.SquareTileFrame(x, y);
                        }
                        else if (Main.tile[x, y].TileType == TileID.Sand || Main.tile[x, y].TileType == TileID.Ebonsand || Main.tile[x, y].TileType == TileID.Crimsand)
                        {
                            Main.tile[x, y].TileType = (ushort)ModContent.TileType<Blissand>();
                            WorldGen.SquareTileFrame(x, y);
                        }
                        else if (Main.tile[x, y].TileType == TileID.CorruptJungleGrass || Main.tile[x, y].TileType == TileID.CrimsonJungleGrass)
                        {
                            Main.tile[x, y].TileType = TileID.JungleGrass;
                            WorldGen.SquareTileFrame(x, y);
                        }
                        else if (Main.tile[x, y].TileType == TileID.CorruptGrass || Main.tile[x, y].TileType == TileID.CrimsonGrass)
                        {
                            Main.tile[x, y].TileType = (ushort)ModContent.TileType<Blissgrass>();
                            WorldGen.SquareTileFrame(x, y);
                        }
                        else if (Main.tile[x, y].TileType == TileID.IceBlock || Main.tile[x, y].TileType == TileID.CorruptIce || Main.tile[x, y].TileType == TileID.FleshIce)
                        {
                            Main.tile[x, y].TileType = (ushort)ModContent.TileType<GoldenIce>();
                            WorldGen.SquareTileFrame(x, y);
                        }
                        else if (Main.tile[x, y].TileType == TileID.Sandstone || Main.tile[x, y].TileType == TileID.CorruptSandstone || Main.tile[x, y].TileType == TileID.CrimsonSandstone)
                        {
                            Main.tile[x, y].TileType = (ushort)ModContent.TileType<Blissandstone>();
                            WorldGen.SquareTileFrame(x, y);
                        }
                        else if (Main.tile[x, y].TileType == TileID.HardenedSand || Main.tile[x, y].TileType == TileID.CorruptHardenedSand || Main.tile[x, y].TileType == TileID.CrimsonHardenedSand)
                        {
                            Main.tile[x, y].TileType = (ushort)ModContent.TileType<HardenedBlissand>();
                            WorldGen.SquareTileFrame(x, y);
                        }
                    }
                }

                // Move the runner
                position.X += velocity.X;
                position.Y += velocity.Y;

                // Adjust velocity
                velocity.X += WorldGen.genRand.Next(-10, 11) * 0.05;
                velocity.Y += WorldGen.genRand.Next(-10, 11) * 0.05;

                // Clamp velocity
                if (velocity.X > speedX + 1.0) velocity.X = speedX + 1.0;
                if (velocity.X < speedX - 1.0) velocity.X = speedX - 1.0;
                if (velocity.Y > speedY + 1.0) velocity.Y = speedY + 1.0;
                if (velocity.Y < speedY - 1.0) velocity.Y = speedY - 1.0;

                // Adjust size
                runnerSize += WorldGen.genRand.Next(-10, 11);
                runnerSize = Math.Clamp(runnerSize, runnerWidth - 50, runnerWidth + 50);

                // Check if runner should stop
                if (position.Y < 0 || position.Y > Main.maxTilesY)
                    running = false;
                if (position.X < 0 || position.X > Main.maxTilesX)
                    running = false;
            }
        }
    }
}
