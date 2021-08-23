using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CholosRandomMod.Items.Placeable
{
    public class SupremeManipulator : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Supreme Manipulator");
            Tooltip.SetDefault("Acts as most crafting stations and an altar\n'A crafting station used by the gods'");
        }

        public override void SetDefaults()
        {
            item.noMelee = true;
            item.createTile = ModContent.TileType<Tiles.SupremeManipulator>();
            item.maxStack = 99;
            item.value = Item.buyPrice(0, 50, 0, 0);
            item.useTime = 10;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.consumable = true;
            item.autoReuse = true;
            item.useTurn = true;
            item.rare = ItemRarityID.Purple;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            int[] workStations = Tiles.SupremeManipulator.workStations;
            for (int i = 0; i < workStations.Length; i++)
            {
                recipe.AddIngredient(workStations[i]);
            }

            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
