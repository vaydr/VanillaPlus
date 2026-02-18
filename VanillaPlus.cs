using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.ModLoader;
using VanillaPlus.Common.Systems;
using VanillaPlus.Common.UI;
using VanillaPlus.Content.Biomes;

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

			// Register hook for world list icons (show Rapture instead of Hallow)
			On_UIWorldListItem.ctor += RaptureWorldIconEdit.OnUIWorldListItemCtor;

			// Hook water style calculation to use Rapture water instead of Hallow water
			On_Main.CalculateWaterStyle += OnCalculateWaterStyle;
		}

		public override void Unload()
		{
			IL_UIWorldCreation.BuildPage -= RaptureSelectionMenu.ILBuildPage;
			IL_UIWorldCreation.MakeInfoMenu -= RaptureSelectionMenu.ILMakeInfoMenu;
			IL_UIWorldCreation.ShowOptionDescription -= RaptureSelectionMenu.ILShowOptionDescription;
			On_UIWorldCreation.SetDefaultOptions -= RaptureSelectionMenu.OnSetDefaultOptions;
			On_UIWorldListItem.ctor -= RaptureWorldIconEdit.OnUIWorldListItemCtor;
			On_Main.CalculateWaterStyle -= OnCalculateWaterStyle;
		}

		private static int OnCalculateWaterStyle(On_Main.orig_CalculateWaterStyle orig, bool ignoreFountains)
		{
			int result = orig(ignoreFountains);

			// If in Rapture biome, always use Rapture water regardless of what vanilla calculated
			var tileCount = ModContent.GetInstance<RaptureTileCount>();
			if (tileCount != null && tileCount.RaptureBlockCount >= RaptureTileCount.RaptureThreshold)
			{
				return ModContent.Find<ModWaterStyle>("VanillaPlus/RaptureWaterStyle").Slot;
			}

			return result;
		}
	}
}
