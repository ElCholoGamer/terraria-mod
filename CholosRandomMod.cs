using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CholosRandomMod
{
	public class CholosRandomMod : Mod
	{
        public override void AddRecipeGroups()
        {
            RecipeGroup group = new RecipeGroup(() => "Any Evil Scale", new int[] { ItemID.ShadowScale, ItemID.TissueSample });

            RecipeGroup.RegisterGroup("CholosRandomMod:EvilScale", group);
        }
    }
}