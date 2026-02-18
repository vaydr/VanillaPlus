using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace VanillaPlus.Content.Tiles.Rapture
{
    /// <summary>
    /// GlowingRadiantShard - a glowing variant of RadiantShard.
    /// Emits bright gold/white light. Spawns ~17% of the time.
    /// TileFrameY determines orientation: 0=floor, 18=ceiling, 36=right wall, 54=left wall
    /// TileFrameX determines visual variant (0-17 * 18)
    /// </summary>
    public class GlowingRadiantShard : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileLighted[Type] = true;  // EMITS LIGHT
            Main.tileNoFail[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileShine2[Type] = true;
            Main.tileShine[Type] = 3500;

            TileID.Sets.ChecksForMerge[Type] = true;

            // Counts as a torch for housing
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);

            Main.tileMerge[Type][ModContent.TileType<Blisstone>()] = true;
            Main.tileMerge[Type][ModContent.TileType<RadiantShard>()] = true;

            DustType = DustID.YellowTorch;
            HitSound = SoundID.Item27;

            // Map entry with hover text
            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(255, 240, 150), name); // Brighter gold

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
                // Baby blue crystals - bright pastel baby blue glow
                r = 0.75f;
                g = 0.9f;
                b = 1.0f;
            }
            else if (colorIndex == 1)
            {
                // Yellow/gold crystals - bright saturated yellow glow
                r = 1.0f;
                g = 0.95f;
                b = 0.2f;
            }
            else
            {
                // White/silver crystals - bright white glow
                r = 0.9f;
                g = 0.9f;
                b = 0.9f;
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
        /// Glowing shards can also spawn more shards (like vanilla crystals).
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

            // Glowing shards tend to spawn more glowing shards (50% chance)
            ushort shardType = WorldGen.genRand.NextBool(2)
                ? Type  // 50% chance for another glowing shard
                : (ushort)ModContent.TileType<RadiantShard>();

            RadiantShard.PlaceShardWithFrame(growX, growY, shardType);
        }
    }
}
