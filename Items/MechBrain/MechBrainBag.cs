using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CholosRandomMod.Items.Armor;

namespace CholosRandomMod.Items.MechBrain
{
    public class MechBrainBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<NPCs.MechBrain.MechBrain>();

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Treasure Bag");
            Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
        }

        public override void SetDefaults()
        {
            item.maxStack = 999;
            item.consumable = true;
            item.width = item.height = 24;
            item.rare = ItemRarityID.Cyan;
            item.expert = true;
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void OpenBossBag(Player player)
        {
            player.TryGettingDevArmor();

            if (Main.rand.NextBool(7))
                player.QuickSpawnItem(ModContent.ItemType<MechBrainMask>());

            player.QuickSpawnItem(ModContent.ItemType<MechanicalProcessorPiece>());
            player.QuickSpawnItem(ModContent.ItemType<SoulofPlight>(), 25 + Main.rand.Next(16));
            player.QuickSpawnItem(ItemID.HallowedBar, 20 + Main.rand.Next(16));
        }
    }
}
