using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CholosRandomMod.Tiles;

namespace CholosRandomMod.Items.Placeable
{
    public class MechBrainTrophy : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Steel Mind Trophy");
        }

        public override void SetDefaults()
        {
            item.width = item.height = 30;
            item.maxStack = 99;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.consumable = true;
            item.value = Item.buyPrice(0, 1, 0, 0);
            item.createTile = ModContent.TileType<Tiles.MechBrainTrophy>();
            item.placeStyle = 0;
        }
    }
}
