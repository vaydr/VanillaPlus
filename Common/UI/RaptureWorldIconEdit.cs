using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent.UI.Elements;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.UI;
using VanillaPlus.Common.Systems;

namespace VanillaPlus.Common.UI
{
    /// <summary>
    /// Hooks to replace Hallow world icons with Rapture icons when a world has Rapture.
    /// Based on TheConfectionRebirth's world icon implementation.
    /// </summary>
    internal static class RaptureWorldIconEdit
    {
        public static void OnUIWorldListItemCtor(On_UIWorldListItem.orig_ctor orig, UIWorldListItem self, WorldFileData data, int orderInList, bool canBePlayed)
        {
            orig.Invoke(self, data, orderInList, canBePlayed);

            // Try to get Rapture data from world header
            bool hasRaptureData = self.Data.TryGetHeaderData(ModContent.GetInstance<RaptureWorldSystem>(), out var raptureData);
            if (!hasRaptureData)
                return;

            bool hasRapture = raptureData.GetBool("HasRapture");
            if (!hasRapture)
                return;

            // Only show Rapture icon for hardmode worlds (where Rapture would exist)
            if (!self.Data.IsHardMode)
                return;

            // Get the world icon element via reflection
            UIElement worldIcon = (UIElement)typeof(UIWorldListItem)
                .GetField("_worldIcon", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(self);

            if (worldIcon == null)
                return;

            WorldFileData worldData = self.Data;

            // Determine which icon to use based on secret seeds
            string iconPath = GetRaptureIconPath(worldData);

            // Create and append the Rapture overlay
            UIImage raptureOverlay = new UIImage(ModContent.Request<Texture2D>(iconPath))
            {
                Top = new StyleDimension(0f, 0f),
                Left = new StyleDimension(0f, 0f),
                IgnoresMouseInteraction = true
            };
            worldIcon.Append(raptureOverlay);
        }

        /// <summary>
        /// Get the appropriate Rapture icon path based on world's secret seed status.
        /// </summary>
        private static string GetRaptureIconPath(WorldFileData data)
        {
            // Zenith/Everything seed (Remix + Drunk combined)
            if (data.RemixWorld && data.DrunkWorld)
                return "VanillaPlus/Assets/WorldIcons/RaptureEverything";

            // Anniversary seed
            if (data.Anniversary)
                return "VanillaPlus/Assets/WorldIcons/RaptureAnniversary";

            // Don't Starve seed
            if (data.DontStarve)
                return "VanillaPlus/Assets/WorldIcons/RaptureDontStarve";

            // For The Worthy seed
            if (data.ForTheWorthy)
                return "VanillaPlus/Assets/WorldIcons/RaptureForTheWorthy";

            // Not The Bees seed
            if (data.NotTheBees)
                return "VanillaPlus/Assets/WorldIcons/RaptureNotTheBees";

            // No Traps seed
            if (data.NoTrapsWorld)
                return "VanillaPlus/Assets/WorldIcons/RaptureTraps";

            // Remix seed (without Drunk)
            if (data.RemixWorld)
                return "VanillaPlus/Assets/WorldIcons/RaptureRemix";

            // Drunk seed (without Remix) - use normal since drunk just has both evils
            if (data.DrunkWorld)
                return "VanillaPlus/Assets/WorldIcons/RaptureNormal";

            // Normal world - no secret seeds
            return "VanillaPlus/Assets/WorldIcons/RaptureNormal";
        }
    }
}
