using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaPlus.Content.Tiles.Rapture;

namespace VanillaPlus.Content.Projectiles.Rapture
{
    /// <summary>
    /// Falling sand projectile for Blissand.
    /// Places Blissand tile when it lands.
    /// </summary>
    public class BlissandProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.FallingBlockDoesNotFallThroughPlatforms[Type] = true;
            ProjectileID.Sets.ForcePlateDetection[Type] = true;
            ProjectileID.Sets.FallingBlockTileItem[Type] = new(
                ModContent.TileType<Blissand>(),
                ModContent.ItemType<Items.Rapture.Blissand>());
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            // Gravity
            Projectile.velocity.Y += 0.41f;
            if (Projectile.velocity.Y > 10f)
                Projectile.velocity.Y = 10f;

            // Rotation
            Projectile.rotation += 0.1f * (Projectile.velocity.X > 0 ? 1 : -1);

            // Dust
            if (Main.rand.NextBool(3))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Sand);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // Let OnKill handle placement
            Projectile.Kill();
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            Point p = Projectile.Center.ToTileCoordinates();

            if (!WorldGen.InWorld(p.X, p.Y, 1))
                return;

            Tile t = Main.tile[p.X, p.Y];

            // Handle half blocks
            if (t.IsHalfBlock && Projectile.velocity.Y > 0f && System.Math.Abs(Projectile.velocity.Y) > System.Math.Abs(Projectile.velocity.X))
            {
                p.Y--;
                t = Main.tile[p.X, p.Y];
            }

            // Clear cuttable tiles (grass, flowers, etc.)
            if (Main.tileCut[t.TileType])
            {
                WorldGen.KillTile(p.X, p.Y);
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, p.X, p.Y);
            }

            // If landing on non-solid, drop as item
            if (!t.HasTile || !Main.tileSolid[t.TileType] || TileID.Sets.IsATreeTrunk[t.TileType])
            {
                if (!t.HasTile && t.TileType != TileID.MinecartTrack)
                {
                    // Fix slopes on tile below
                    Tile tBelow = Main.tile[p.X, p.Y + 1];
                    if (tBelow.Slope != SlopeType.Solid)
                        tBelow.Slope = SlopeType.Solid;
                    if (tBelow.IsHalfBlock)
                        tBelow.IsHalfBlock = false;

                    // Place Blissand tile
                    WorldGen.PlaceTile(p.X, p.Y, ModContent.TileType<Blissand>(), forced: true);
                    WorldGen.SquareTileFrame(p.X, p.Y);

                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendTileSquare(-1, p.X, p.Y, 1);
                }
                else if (t.HasTile && (!Main.tileSolid[t.TileType] || TileID.Sets.IsATreeTrunk[t.TileType]))
                {
                    // Drop as item if can't place
                    Item.NewItem(
                        WorldGen.GetItemSource_FromTileBreak(p.X, p.Y),
                        p.X * 16, p.Y * 16, 16, 16,
                        ModContent.ItemType<Items.Rapture.Blissand>());
                }
            }
        }
    }
}
