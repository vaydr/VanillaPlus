using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace VanillaPlus.Content.Players
{
    public class LuckyMementoPlayer : ModPlayer
    {
        public bool hasLuckyMemento;
        public bool showLuckDisplay;

        public override void Initialize()
        {
            hasLuckyMemento = false;
            showLuckDisplay = false;
        }

        public override void SaveData(TagCompound tag)
        {
            if (hasLuckyMemento)
                tag["hasLuckyMemento"] = true;
            if (showLuckDisplay)
                tag["showLuckDisplay"] = true;
        }

        public override void LoadData(TagCompound tag)
        {
            hasLuckyMemento = tag.GetBool("hasLuckyMemento");
            showLuckDisplay = tag.GetBool("showLuckDisplay");
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)0); // Message type for LuckyMemento sync
            packet.Write((byte)Player.whoAmI);
            packet.Write(hasLuckyMemento);
            packet.Write(showLuckDisplay);
            packet.Send(toWho, fromWho);
        }

        public override void CopyClientState(ModPlayer targetCopy)
        {
            LuckyMementoPlayer clone = (LuckyMementoPlayer)targetCopy;
            clone.hasLuckyMemento = hasLuckyMemento;
            clone.showLuckDisplay = showLuckDisplay;
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            LuckyMementoPlayer clone = (LuckyMementoPlayer)clientPlayer;
            if (hasLuckyMemento != clone.hasLuckyMemento || showLuckDisplay != clone.showLuckDisplay)
            {
                SyncPlayer(toWho: -1, fromWho: Main.myPlayer, newPlayer: false);
            }
        }
    }
}
