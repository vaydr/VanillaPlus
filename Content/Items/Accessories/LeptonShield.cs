using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaPlus.Content.Items.Accessories
{
    public class LeptonShield : ModItem
    {
        public override string Texture => $"Terraria/Images/Item_{ItemID.PaladinsShield}";

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 28;
            Item.value = Item.buyPrice(platinum: 1);
            Item.rare = ItemRarityID.Purple;
            Item.accessory = true;
            Item.defense = 500;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            // +100% move speed
            player.moveSpeed += 1f;

            // +100% damage (all classes)
            player.GetDamage(DamageClass.Generic) += 1f;

            // +100% critical strike chance (all classes)
            player.GetCritChance(DamageClass.Generic) += 100;

            // +100% melee attack speed
            player.GetAttackSpeed(DamageClass.Melee) += 1f;

            // Regenerate 25 HP/s (game runs at 60fps, lifeRegen is in half-HP per second)
            player.lifeRegen += 50;

            // Immunity to all debuffs
            player.buffImmune[BuffID.Poisoned] = true;
            player.buffImmune[BuffID.Darkness] = true;
            player.buffImmune[BuffID.Cursed] = true;
            player.buffImmune[BuffID.OnFire] = true;
            player.buffImmune[BuffID.Bleeding] = true;
            player.buffImmune[BuffID.Confused] = true;
            player.buffImmune[BuffID.Slow] = true;
            player.buffImmune[BuffID.Weak] = true;
            player.buffImmune[BuffID.Silenced] = true;
            player.buffImmune[BuffID.BrokenArmor] = true;
            player.buffImmune[BuffID.CursedInferno] = true;
            player.buffImmune[BuffID.Ichor] = true;
            player.buffImmune[BuffID.Chilled] = true;
            player.buffImmune[BuffID.Frozen] = true;
            player.buffImmune[BuffID.Webbed] = true;
            player.buffImmune[BuffID.Stoned] = true;
            player.buffImmune[BuffID.VortexDebuff] = true;
            player.buffImmune[BuffID.Obstructed] = true;
            player.buffImmune[BuffID.Electrified] = true;
            player.buffImmune[BuffID.Suffocation] = true;
            player.buffImmune[BuffID.OnFire3] = true;
            player.buffImmune[BuffID.Frostburn] = true;
            player.buffImmune[BuffID.Frostburn2] = true;
            player.buffImmune[BuffID.ShadowFlame] = true;
            player.buffImmune[BuffID.BoneJavelin] = true;
            player.buffImmune[BuffID.DryadsWardDebuff] = true;
            player.buffImmune[BuffID.Daybreak] = true;
            player.buffImmune[BuffID.BetsysCurse] = true;
            player.buffImmune[BuffID.Oiled] = true;
            player.buffImmune[BuffID.WitheredArmor] = true;
            player.buffImmune[BuffID.WitheredWeapon] = true;
            player.buffImmune[BuffID.OgreSpit] = true;
            player.buffImmune[BuffID.Burning] = true;
            player.buffImmune[BuffID.Rabies] = true;
            player.buffImmune[BuffID.Blackout] = true;
            player.buffImmune[BuffID.WindPushed] = true;
            player.buffImmune[BuffID.Shimmer] = true;
        }
    }
}
