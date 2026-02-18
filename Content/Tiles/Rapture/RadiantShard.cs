using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaPlus.Content.Tiles.Rapture
{
    /// <summary>
    /// RadiantShard - the Rapture equivalent of Crystal Shards.
    /// Grows naturally on Blisstone underground.
    /// TileFrameY determines orientation: 0=floor, 18=ceiling, 36=right wall, 54=left wall
    /// TileFrameX determines visual variant (0-17 * 18)
    /// </summary>
    public class RadiantShard : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileLighted[Type] = true; // All shards glow slightly
            Main.tileNoFail[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileShine2[Type] = true;
            Main.tileShine[Type] = 4500;

            TileID.Sets.ChecksForMerge[Type] = true;

            // Merge with Blisstone and GlowingRadiantShard
            Main.tileMerge[Type][ModContent.TileType<Blisstone>()] = true;
            Main.tileMerge[Type][ModContent.TileType<GlowingRadiantShard>()] = true;

            DustType = DustID.YellowTorch; // Gold dust
            HitSound = SoundID.Item27;
            AddMapEntry(new Color(255, 215, 100)); // Gold

            RegisterItemDrop(ModContent.ItemType<Items.Rapture.RadiantShard>());
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            // Get the frame to determine crystal color
            Tile tile = Main.tile[i, j];
            int frameColumn = tile.TileFrameX / 18;

            // Sprite sheet has 3 color groups repeating: blue, gold, white
            // Pattern appears to be roughly: 0-5 blue, 6-11 gold, 12-17 white
            int colorGroup = frameColumn % 18;

            if (colorGroup < 6)
            {
                // Blue/cyan crystals - subtle blue glow
                r = 0.15f;
                g = 0.25f;
                b = 0.35f;
            }
            else if (colorGroup < 12)
            {
                // Gold/yellow crystals - subtle gold glow
                r = 0.35f;
                g = 0.30f;
                b = 0.1f;
            }
            else
            {
                // White/silver crystals - subtle white glow
                r = 0.3f;
                g = 0.3f;
                b = 0.3f;
            }
        }

        public override bool CanPlace(int i, int j)
        {
            Tile belowTile = Main.tile[i, j + 1];
            Tile aboveTile = Main.tile[i, j - 1];
            Tile rightTile = Main.tile[i + 1, j];
            Tile leftTile = Main.tile[i - 1, j];

            // Check if any adjacent tile is solid and valid for attachment
            if ((belowTile.Slope == SlopeType.Solid && !belowTile.IsHalfBlock && belowTile.HasTile && Main.tileSolid[belowTile.TileType]) ||
                (aboveTile.Slope == SlopeType.Solid && !aboveTile.IsHalfBlock && aboveTile.HasTile && Main.tileSolid[aboveTile.TileType]) ||
                (rightTile.Slope == SlopeType.Solid && !rightTile.IsHalfBlock && rightTile.HasTile && Main.tileSolid[rightTile.TileType]) ||
                (leftTile.Slope == SlopeType.Solid && !leftTile.IsHalfBlock && leftTile.HasTile && Main.tileSolid[leftTile.TileType]))
                return true;

            return false;
        }

        public override void PlaceInWorld(int i, int j, Item item)
        {
            Tile belowTile = Main.tile[i, j + 1];
            Tile aboveTile = Main.tile[i, j - 1];
            Tile rightTile = Main.tile[i + 1, j];
            Tile leftTile = Main.tile[i - 1, j];

            // Set TileFrameY based on attachment direction
            // 0 = floor (attached to solid below), 18 = ceiling, 36 = right wall, 54 = left wall
            if (belowTile.Slope == SlopeType.Solid && !belowTile.IsHalfBlock && belowTile.HasTile && Main.tileSolid[belowTile.TileType])
                Main.tile[i, j].TileFrameY = 0;
            else if (aboveTile.Slope == SlopeType.Solid && !aboveTile.IsHalfBlock && aboveTile.HasTile && Main.tileSolid[aboveTile.TileType])
                Main.tile[i, j].TileFrameY = 18;
            else if (rightTile.Slope == SlopeType.Solid && !rightTile.IsHalfBlock && rightTile.HasTile && Main.tileSolid[rightTile.TileType])
                Main.tile[i, j].TileFrameY = 36;
            else if (leftTile.Slope == SlopeType.Solid && !leftTile.IsHalfBlock && leftTile.HasTile && Main.tileSolid[leftTile.TileType])
                Main.tile[i, j].TileFrameY = 54;

            // Random visual variant (18 variants per orientation row)
            Main.tile[i, j].TileFrameX = (short)(WorldGen.genRand.Next(18) * 18);
        }

        /// <summary>
        /// Shards can spawn more shards in the direction they're facing (like vanilla crystals).
        /// </summary>
        public override void RandomUpdate(int i, int j)
        {
            // Only underground
            if (j <= Main.rockLayer)
                return;

            // 10% chance to try growing
            if (!WorldGen.genRand.NextBool(10))
                return;

            Tile tile = Main.tile[i, j];
            short frameY = tile.TileFrameY;

            // Determine growth direction based on current orientation
            int growX = i, growY = j;
            if (frameY == 0) growY = j - 1;      // Floor-attached grows up
            else if (frameY == 18) growY = j + 1; // Ceiling-attached grows down
            else if (frameY == 36) growX = i - 1; // Right-wall grows left
            else if (frameY == 54) growX = i + 1; // Left-wall grows right

            if (!WorldGen.InWorld(growX, growY, 1))
                return;

            Tile growTile = Main.tile[growX, growY];
            if (growTile.HasTile || growTile.LiquidType == LiquidID.Lava)
                return;

            // 15% chance for glowing variant when growing from existing shard
            ushort shardType = WorldGen.genRand.NextBool(6)  // ~17% for glowing
                ? (ushort)ModContent.TileType<GlowingRadiantShard>()
                : Type;

            PlaceShardWithFrame(growX, growY, shardType);
        }

        /// <summary>
        /// Helper method to place a shard with proper framing at the given location.
        /// Used by RaptureGlobalTile and RandomUpdate for natural growth.
        /// </summary>
        public static bool PlaceShardWithFrame(int i, int j, ushort shardType)
        {
            if (!WorldGen.InWorld(i, j, 1))
                return false;

            Tile tile = Main.tile[i, j];
            if (tile.HasTile || tile.LiquidAmount > 0)
                return false;

            Tile belowTile = Main.tile[i, j + 1];
            Tile aboveTile = Main.tile[i, j - 1];
            Tile rightTile = Main.tile[i + 1, j];
            Tile leftTile = Main.tile[i - 1, j];

            short frameY = -1;

            // Determine attachment direction and set TileFrameY
            if (belowTile.HasTile && Main.tileSolid[belowTile.TileType] &&
                belowTile.Slope == SlopeType.Solid && !belowTile.IsHalfBlock)
            {
                frameY = 0; // Floor attachment
            }
            else if (aboveTile.HasTile && Main.tileSolid[aboveTile.TileType] &&
                     aboveTile.Slope == SlopeType.Solid && !aboveTile.IsHalfBlock)
            {
                frameY = 18; // Ceiling attachment
            }
            else if (rightTile.HasTile && Main.tileSolid[rightTile.TileType] &&
                     rightTile.Slope == SlopeType.Solid && !rightTile.IsHalfBlock)
            {
                frameY = 36; // Right wall attachment
            }
            else if (leftTile.HasTile && Main.tileSolid[leftTile.TileType] &&
                     leftTile.Slope == SlopeType.Solid && !leftTile.IsHalfBlock)
            {
                frameY = 54; // Left wall attachment
            }

            if (frameY < 0)
                return false; // No valid attachment point

            // Place the shard
            tile.TileType = shardType;
            tile.HasTile = true;
            tile.TileFrameY = frameY;
            tile.TileFrameX = (short)(WorldGen.genRand.Next(18) * 18);

            WorldGen.SquareTileFrame(i, j, true);

            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendTileSquare(-1, i, j, 1);

            return true;
        }
    }
}
