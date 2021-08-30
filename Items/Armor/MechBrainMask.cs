using Terraria.ID;
using Terraria.ModLoader;

namespace CholosRandomMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class MechBrainMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Steel Mind Mask");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 20;
            item.rare = ItemRarityID.Blue;
            item.vanity = true;
        }

        public override bool DrawHead()
        {
            return false;
        }
    }
}
