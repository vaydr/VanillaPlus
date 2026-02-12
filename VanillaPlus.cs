using Terraria.ModLoader;

namespace VanillaPlus
{
	public class VanillaPlus : Mod
	{
		public override void Load()
		{
			// Register vanilla Angel Wings texture for IcarusWings (Wings_2)
			EquipLoader.AddEquipTexture(this, "Terraria/Images/Wings_2", EquipType.Wings, name: "IcarusWings");
		}
	}
}
