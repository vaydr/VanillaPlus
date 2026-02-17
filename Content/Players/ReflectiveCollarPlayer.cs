using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using VanillaPlus.Common.BuilderToggles;

namespace VanillaPlus.Content.Players
{
    public class ReflectiveCollarPlayer : ModPlayer
    {
        public bool hasReflectiveCollar;

        public override void Initialize()
        {
            hasReflectiveCollar = false;
        }

        public override void PostUpdate()
        {
            var toggle = ModContent.GetInstance<ShineBuilderToggle>();
            if (hasReflectiveCollar && toggle.CurrentState == 1)
            {
                Lighting.AddLight(Player.Center, 0.8f, 0.95f, 1f);
            }
        }

        public override void SaveData(TagCompound tag)
        {
            if (hasReflectiveCollar)
                tag["hasReflectiveCollar"] = true;
        }

        public override void LoadData(TagCompound tag)
        {
            hasReflectiveCollar = tag.GetBool("hasReflectiveCollar");
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)1); // Message type for ReflectiveCollar sync
            packet.Write((byte)Player.whoAmI);
            packet.Write(hasReflectiveCollar);
            packet.Send(toWho, fromWho);
        }

        public override void CopyClientState(ModPlayer targetCopy)
        {
            ReflectiveCollarPlayer clone = (ReflectiveCollarPlayer)targetCopy;
            clone.hasReflectiveCollar = hasReflectiveCollar;
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            ReflectiveCollarPlayer clone = (ReflectiveCollarPlayer)clientPlayer;
            if (hasReflectiveCollar != clone.hasReflectiveCollar)
            {
                SyncPlayer(toWho: -1, fromWho: Main.myPlayer, newPlayer: false);
            }
        }
    }
}
