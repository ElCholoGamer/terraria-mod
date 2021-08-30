using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using CholosRandomMod.Items.MechBrain;
using CholosRandomMod.Items.Armor;
using CholosRandomMod.Items.Placeable;
using CholosRandomMod.NPCs.MechBrain;

namespace CholosRandomMod
{
    public class CholosRandomMod : Mod
    {
        public override void PostSetupContent()
        {
            Mod yabhb = ModLoader.GetMod("FKBossHealthBar");
            if(yabhb != null)
            {
                yabhb.Call("RegisterMechHealthBarMulti", 
                    ModContent.NPCType<MechBrain>(), ModContent.NPCType<MechCreeper>());
            }

            Mod bossChecklist = ModLoader.GetMod("BossChecklist");
            if (bossChecklist != null)
            {
                bossChecklist.Call(
                    "AddBoss",
                    9.1f,
                    ModContent.NPCType<MechBrain>(),
                    this,
                    "The Steel Mind",
                    (Func<bool>)(() => CholosModWorld.downedMechBrain),
                    ModContent.ItemType<MechSpine>(),
                    new List<int> { 
                        ModContent.ItemType<MechBrainTrophy>(), 
                        ModContent.ItemType<MechBrainMask>(), 
                        ItemID.MusicBoxBoss3 },
                    new List<int> {
                        ModContent.ItemType<MechBrainBag>(),
                        ModContent.ItemType<MechanicalProcessorPiece>(),
                        ModContent.ItemType<SoulofPlight>(),
                        ItemID.HallowedBar,
                        ItemID.GreaterHealingPotion
                    },
                    "Use [i:" + ModContent.ItemType<MechSpine>() + "] at night.",
                    "The Steel Mind has done its deed");
            }
        }

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