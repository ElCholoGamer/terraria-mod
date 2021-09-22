﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CholosRandomMod.Items.Placeable
{
    class LancerPainting : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lancer");
            Tooltip.SetDefault("'(SIGNED, LANCER)'");
        }

        public override void SetDefaults()
        {
            item.width = 46;
            item.height = 32;
            item.maxStack = 99;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.consumable = true;
            item.value = Item.buyPrice(0, 20, 0, 0);
            item.createTile = ModContent.TileType<Tiles.LancerPainting>();
        }
    }
}
