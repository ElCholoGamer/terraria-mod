using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using CholosRandomMod.Items.MechBrain;

namespace CholosRandomMod
{
    public class CholosRandomMod : Mod
    {
        public override void AddRecipeGroups()
        {
            RecipeGroup group = new RecipeGroup(() => "Any Evil Scale", new int[] { ItemID.ShadowScale, ItemID.TissueSample });

            RecipeGroup.RegisterGroup("CholosRandomMod:EvilScale", group);
        }

        public override void AddRecipes()
        {
            RecipeFinder finder = new RecipeFinder();
            finder.AddIngredient(ItemID.MechanicalWagonPiece);
            finder.AddIngredient(ItemID.MechanicalWheelPiece);
            finder.AddIngredient(ItemID.MechanicalBatteryPiece);
            finder.SetResult(ItemID.MinecartMech);

            foreach (Recipe recipe in finder.SearchRecipes())
            {
                RecipeEditor editor = new RecipeEditor(recipe);
                editor.AddIngredient(ModContent.ItemType<MechanicalProcessorPiece>());
            }

            finder = new RecipeFinder();
            finder.AddIngredient(ItemID.SoulofSight);
            finder.AddIngredient(ItemID.SoulofMight);
            finder.AddIngredient(ItemID.SoulofFright);

            foreach (Recipe recipe in finder.SearchRecipes())
            {
                int amount = 1;

                foreach (Item item in recipe.requiredItem)
                {
                    if (item.type == ItemID.SoulofSight || item.type == ItemID.SoulofMight || item.type == ItemID.SoulofFright)
                    {
                        amount = (int)MathHelper.Max(item.stack, amount);
                    }
                }

                RecipeEditor editor = new RecipeEditor(recipe);
                editor.AddIngredient(ModContent.ItemType<SoulofPlight>(), amount);
            }
        }
    }
}