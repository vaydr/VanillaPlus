using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using VanillaPlus.Common.BuilderToggles;

namespace VanillaPlus.Content.Players
{
    public class ReflectiveTapePlayer : ModPlayer
    {
        public bool hasReflectiveTape;

        public override void Initialize()
        {
            hasReflectiveTape = false;
        }

        public override void PostUpdate()
        {
            var toggle = ModContent.GetInstance<ShineBuilderToggle>();
            if (hasReflectiveTape && toggle.CurrentState == 1)
            {
                Lighting.AddLight(Player.Center, 0.8f, 0.95f, 1f);
            }
        }

        public override void SaveData(TagCompound tag)
        {
            if (hasReflectiveTape)
                tag["hasReflectiveTape"] = true;
        }

        public override void LoadData(TagCompound tag)
        {
            hasReflectiveTape = tag.GetBool("hasReflectiveTape");
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)1); // Message type for ReflectiveTape sync
            packet.Write((byte)Player.whoAmI);
            packet.Write(hasReflectiveTape);
            packet.Send(toWho, fromWho);
        }

        public override void CopyClientState(ModPlayer targetCopy)
        {
            ReflectiveTapePlayer clone = (ReflectiveTapePlayer)targetCopy;
            clone.hasReflectiveTape = hasReflectiveTape;
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            ReflectiveTapePlayer clone = (ReflectiveTapePlayer)clientPlayer;
            if (hasReflectiveTape != clone.hasReflectiveTape)
            {
                SyncPlayer(toWho: -1, fromWho: Main.myPlayer, newPlayer: false);
            }
        }
    }
}
