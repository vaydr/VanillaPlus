using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Content.Tiles.Rapture;
using VanillaPlus.Content.Walls.Rapture;

namespace VanillaPlus.Common.Systems
{
    /// <summary>
    /// Handles Rapture biome conversion via Clentaminator (Golden Solution).
    /// Converts vanilla and evil biome tiles/walls to their Rapture equivalents.
    /// </summary>
    public static class RaptureWorldGeneration
    {
        /// <summary>
        /// Converts tiles and walls in a radius around the given position to Rapture variants.
        /// Called by the GoldenSolution projectile.
        /// </summary>
        /// <param name="i">Tile X coordinate</param>
        /// <param name="j">Tile Y coordinate</param>
        /// <param name="size">Conversion radius</param>
        public static void RaptureConvert(int i, int j, int size = 4)
        {
            // Calculate the circular conversion area
            for (int x = i - size; x <= i + size; x++)
            {
                for (int y = j - size; y <= j + size; y++)
                {
                    // Check bounds
                    if (!WorldGen.InWorld(x, y, 1))
                        continue;

                    // Circular radius check (same as Confection)
                    if (Math.Abs(x - i) + Math.Abs(y - j) >= Math.Sqrt(size * size + size * size))
                        continue;

                    // Convert walls
                    ConvertWall(x, y);

                    // Convert tiles
                    ConvertTile(x, y);
                }
            }
        }

        private static void ConvertTile(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            if (!tile.HasTile)
                return;

            int newType = -1;

            switch (tile.TileType)
            {
                // === STONE TYPES → BLISSTONE ===
                case TileID.Stone:
                case TileID.Ebonstone:
                case TileID.Crimstone:
                case TileID.Pearlstone:
                    newType = ModContent.TileType<Blisstone>();
                    break;

                // === GRASS TYPES → BLISSGRASS ===
                case TileID.Grass:
                case TileID.CorruptGrass:
                case TileID.CrimsonGrass:
                case TileID.HallowedGrass:
                    newType = ModContent.TileType<Blissgrass>();
                    break;

                // === SAND TYPES → BLISSAND ===
                case TileID.Sand:
                case TileID.Ebonsand:
                case TileID.Crimsand:
                case TileID.Pearlsand:
                    newType = ModContent.TileType<Blissand>();
                    break;

                // === ICE TYPES → GOLDEN ICE ===
                case TileID.IceBlock:
                case TileID.CorruptIce:
                case TileID.FleshIce:
                case TileID.HallowedIce:
                    newType = ModContent.TileType<GoldenIce>();
                    break;

                // === SANDSTONE TYPES → BLISSANDSTONE ===
                case TileID.Sandstone:
                case TileID.CorruptSandstone:
                case TileID.CrimsonSandstone:
                case TileID.HallowSandstone:
                    newType = ModContent.TileType<Blissandstone>();
                    break;

                // === HARDENED SAND TYPES → HARDENED BLISSAND ===
                case TileID.HardenedSand:
                case TileID.CorruptHardenedSand:
                case TileID.CrimsonHardenedSand:
                case TileID.HallowHardenedSand:
                    newType = ModContent.TileType<HardenedBlissand>();
                    break;

                // === THORNS → DESTROY ===
                case TileID.CorruptThorns:
                case TileID.CrimsonThorns:
                    WorldGen.KillTile(x, y);
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendTileSquare(-1, x, y, 1);
                    return;

                // === JUNGLE GRASS → BLISSGRASS (optional conversion) ===
                case TileID.JungleGrass:
                    newType = ModContent.TileType<Blissgrass>();
                    break;

                // === MUD NEAR BLISSGRASS → BLISSTONE ===
                case TileID.Mud:
                    // Only convert if adjacent to Blissgrass
                    if (HasAdjacentTile(x, y, ModContent.TileType<Blissgrass>()))
                    {
                        newType = ModContent.TileType<Blisstone>();
                    }
                    break;
            }

            // Apply tile conversion
            if (newType >= 0 && tile.TileType != newType)
            {
                tile.TileType = (ushort)newType;
                WorldGen.SquareTileFrame(x, y, true);
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendTileSquare(-1, x, y, 1);
            }
        }

        private static void ConvertWall(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            if (tile.WallType == WallID.None)
                return;

            int newWall = -1;

            switch (tile.WallType)
            {
                // === STONE WALLS → BLISSTONE WALL ===
                case WallID.Stone:
                case WallID.EbonstoneUnsafe:
                case WallID.CrimstoneUnsafe:
                case WallID.PearlstoneBrickUnsafe:
                case WallID.CaveUnsafe:
                case WallID.Cave2Unsafe:
                case WallID.Cave3Unsafe:
                case WallID.Cave4Unsafe:
                case WallID.Cave5Unsafe:
                case WallID.Cave6Unsafe:
                case WallID.Cave7Unsafe:
                case WallID.Cave8Unsafe:
                case WallID.CorruptionUnsafe1:
                case WallID.CorruptionUnsafe2:
                case WallID.CorruptionUnsafe3:
                case WallID.CorruptionUnsafe4:
                case WallID.CrimsonUnsafe1:
                case WallID.CrimsonUnsafe2:
                case WallID.CrimsonUnsafe3:
                case WallID.CrimsonUnsafe4:
                case WallID.HallowUnsafe1:
                case WallID.HallowUnsafe2:
                case WallID.HallowUnsafe3:
                case WallID.HallowUnsafe4:
                    newWall = ModContent.WallType<BlisstoneWall>();
                    break;

                // === GRASS WALLS → BLISSGRASS WALL ===
                case WallID.Grass:
                case WallID.GrassUnsafe:
                case WallID.CorruptGrassUnsafe:
                case WallID.CrimsonGrassUnsafe:
                case WallID.HallowedGrassUnsafe:
                case WallID.Jungle:
                case WallID.JungleUnsafe:
                    newWall = ModContent.WallType<BlissGrassWall>();
                    break;

                // === ICE WALLS → GOLDEN ICE WALL ===
                case WallID.IceUnsafe:
                case WallID.SnowWallUnsafe:
                    newWall = ModContent.WallType<GoldenIceWall>();
                    break;

                // === SANDSTONE WALLS → BLISSANDSTONE WALL ===
                case WallID.Sandstone:
                case WallID.CorruptSandstone:
                case WallID.CrimsonSandstone:
                case WallID.HallowSandstone:
                    newWall = ModContent.WallType<BlissandstoneWall>();
                    break;

                // === HARDENED SAND WALLS → HARDENED BLISSAND WALL ===
                case WallID.HardenedSand:
                case WallID.CorruptHardenedSand:
                case WallID.CrimsonHardenedSand:
                case WallID.HallowHardenedSand:
                    newWall = ModContent.WallType<HardenedBlissandWall>();
                    break;
            }

            // Handle NewWall1-4 conversions using the Sets system
            if (newWall < 0)
            {
                ushort wallType = tile.WallType;

                // NewWall1 (Cavern walls) → RaptureCavernWall
                if (WallID.Sets.Conversion.NewWall1[wallType] && wallType != ModContent.WallType<RaptureCavernWall>())
                {
                    newWall = ModContent.WallType<RaptureCavernWall>();
                }
                // NewWall2 (Crystalline walls) → RaptureCrystallineWall
                else if (WallID.Sets.Conversion.NewWall2[wallType] && wallType != ModContent.WallType<RaptureCrystallineWall>())
                {
                    newWall = ModContent.WallType<RaptureCrystallineWall>();
                }
                // NewWall3 (Prism walls) → RapturePrismWall
                else if (WallID.Sets.Conversion.NewWall3[wallType] && wallType != ModContent.WallType<RapturePrismWall>())
                {
                    newWall = ModContent.WallType<RapturePrismWall>();
                }
                // NewWall4 (Shard walls) → RaptureShardWall
                else if (WallID.Sets.Conversion.NewWall4[wallType] && wallType != ModContent.WallType<RaptureShardWall>())
                {
                    newWall = ModContent.WallType<RaptureShardWall>();
                }
            }

            // Apply wall conversion
            if (newWall >= 0 && tile.WallType != newWall)
            {
                tile.WallType = (ushort)newWall;
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendTileSquare(-1, x, y, 1);
            }
        }

        /// <summary>
        /// Checks if a tile at the given position has an adjacent tile of the specified type.
        /// </summary>
        private static bool HasAdjacentTile(int x, int y, int tileType)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0)
                        continue;

                    int nx = x + dx;
                    int ny = y + dy;

                    if (!WorldGen.InWorld(nx, ny, 1))
                        continue;

                    if (Main.tile[nx, ny].HasTile && Main.tile[nx, ny].TileType == tileType)
                        return true;
                }
            }
            return false;
        }
    }
}
