using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace VanillaPlus.Content.Players
{
    public class ReflectiveCollarPlayer : ModPlayer
    {
        public bool hasReflectiveCollar;
        public bool shineEnabled;

        public override void Initialize()
        {
            hasReflectiveCollar = false;
            shineEnabled = false;
        }

        public override void PostUpdate()
        {
            if (hasReflectiveCollar && shineEnabled)
            {
                Lighting.AddLight(Player.Center, 0.8f, 0.95f, 1f);
            }
        }

        public override void SaveData(TagCompound tag)
        {
            if (hasReflectiveCollar)
                tag["hasReflectiveCollar"] = true;
            if (shineEnabled)
                tag["shineEnabled"] = true;
        }

        public override void LoadData(TagCompound tag)
        {
            hasReflectiveCollar = tag.GetBool("hasReflectiveCollar");
            shineEnabled = tag.GetBool("shineEnabled");
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)1); // Message type for ReflectiveCollar sync
            packet.Write((byte)Player.whoAmI);
            packet.Write(hasReflectiveCollar);
            packet.Write(shineEnabled);
            packet.Send(toWho, fromWho);
        }

        public override void CopyClientState(ModPlayer targetCopy)
        {
            ReflectiveCollarPlayer clone = (ReflectiveCollarPlayer)targetCopy;
            clone.hasReflectiveCollar = hasReflectiveCollar;
            clone.shineEnabled = shineEnabled;
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            ReflectiveCollarPlayer clone = (ReflectiveCollarPlayer)clientPlayer;
            if (hasReflectiveCollar != clone.hasReflectiveCollar || shineEnabled != clone.shineEnabled)
            {
                SyncPlayer(toWho: -1, fromWho: Main.myPlayer, newPlayer: false);
            }
        }
    }
}
