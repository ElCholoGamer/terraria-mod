using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CholosRandomMod.Items.Placeable;

namespace CholosRandomMod.NPCs
{
    public class CholosGlobalNPC : GlobalNPC
    {
        public override void SetupShop(int type, Chest shop, ref int nextSlot)
        {
            if (type == NPCID.Clothier)
            {
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<LancerPainting>());
            }
        }
    }
}
