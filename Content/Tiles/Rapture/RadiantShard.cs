using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
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

            // Merge with Blisstone
            Main.tileMerge[Type][ModContent.TileType<Blisstone>()] = true;

            DustType = DustID.YellowTorch; // Gold dust
            HitSound = SoundID.Item27;

            // Map entry with hover text
            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(255, 215, 100), name); // Gold

            RegisterItemDrop(ModContent.ItemType<Items.Rapture.RadiantShard>());
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        /// <summary>
        /// Called when the tile frame is being recalculated.
        /// Returns false to destroy the tile if the attached block is gone.
        /// </summary>
        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            Tile tile = Main.tile[i, j];
            short frameY = tile.TileFrameY;

            // Check if the attached tile still exists based on orientation
            Tile attachedTile;
            if (frameY == 0)
                attachedTile = Main.tile[i, j + 1]; // Floor attachment - check below
            else if (frameY == 18)
                attachedTile = Main.tile[i, j - 1]; // Ceiling attachment - check above
            else if (frameY == 36)
                attachedTile = Main.tile[i + 1, j]; // Right wall attachment - check right
            else if (frameY == 54)
                attachedTile = Main.tile[i - 1, j]; // Left wall attachment - check left
            else
                return true; // Unknown orientation, let it be

            // If attached tile is no longer solid, kill this shard
            if (!attachedTile.HasTile || !Main.tileSolid[attachedTile.TileType] ||
                attachedTile.IsHalfBlock || attachedTile.Slope != SlopeType.Solid)
            {
                WorldGen.KillTile(i, j);
                return false;
            }

            return true;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            // Get the frame to determine crystal color
            Tile tile = Main.tile[i, j];
            int frameColumn = tile.TileFrameX / 18; // 0-17

            // Sprite sheet cycles: blue, yellow, white repeating
            // Column 0,3,6,9,12,15 = blue | 1,4,7,10,13,16 = yellow | 2,5,8,11,14,17 = white
            int colorIndex = frameColumn % 3;

            if (colorIndex == 0)
            {
                // Baby blue crystals - pastel baby blue glow
                r = 0.5f;
                g = 0.7f;
                b = 0.85f;
            }
            else if (colorIndex == 1)
            {
                // Yellow/gold crystals - warm yellow glow
                r = 0.55f;
                g = 0.5f;
                b = 0.15f;
            }
            else
            {
                // White/silver crystals - subtle white glow
                r = 0.4f;
                g = 0.4f;
                b = 0.4f;
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

            PlaceShardWithFrame(growX, growY, Type);
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
