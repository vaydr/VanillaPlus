using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.ModLoader;
using VanillaPlus.Common.UI;

namespace VanillaPlus
{
	public class VanillaPlus : Mod
	{
		public override void Load()
		{
			// Register vanilla Angel Wings texture for IcarusWings (Wings_2)
			EquipLoader.AddEquipTexture(this, "Terraria/Images/Wings_2", EquipType.Wings, name: "IcarusWings");

			// Register hooks for world creation UI (Rapture selection)
			IL_UIWorldCreation.BuildPage += RaptureSelectionMenu.ILBuildPage;
			IL_UIWorldCreation.MakeInfoMenu += RaptureSelectionMenu.ILMakeInfoMenu;
			IL_UIWorldCreation.ShowOptionDescription += RaptureSelectionMenu.ILShowOptionDescription;
			On_UIWorldCreation.SetDefaultOptions += RaptureSelectionMenu.OnSetDefaultOptions;
			// Gamepad points hook disabled - requires exact IL index matching
			// IL_UIWorldCreation.SetupGamepadPoints += RaptureSelectionMenu.ILSetUpGamepadPoints;

			// Register hook for world list icons (show Rapture instead of Hallow)
			On_UIWorldListItem.ctor += RaptureWorldIconEdit.OnUIWorldListItemCtor;
		}

		public override void Unload()
		{
			// Unregister hooks
			IL_UIWorldCreation.BuildPage -= RaptureSelectionMenu.ILBuildPage;
			IL_UIWorldCreation.MakeInfoMenu -= RaptureSelectionMenu.ILMakeInfoMenu;
			IL_UIWorldCreation.ShowOptionDescription -= RaptureSelectionMenu.ILShowOptionDescription;
			On_UIWorldCreation.SetDefaultOptions -= RaptureSelectionMenu.OnSetDefaultOptions;
			// IL_UIWorldCreation.SetupGamepadPoints -= RaptureSelectionMenu.ILSetUpGamepadPoints;

			On_UIWorldListItem.ctor -= RaptureWorldIconEdit.OnUIWorldListItemCtor;
		}
	}
}
