using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CholosRandomMod.Items.MechBrain
{
    public class MechanicalProcessorPiece : ModItem
    {
        public override void SetDefaults()
        {
            item.width = item.height = 30;
            item.maxStack = 99;
            item.value = Item.buyPrice(0, 0, 80, 0);
            item.rare = ItemRarityID.Blue;
            item.expert = true;
        }
    }
}
