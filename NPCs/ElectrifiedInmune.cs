using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CholosRandomMod.NPCs
{
    class ElectrifiedInmune : GlobalNPC
    {
        public override void SetDefaults(NPC npc)
        {
            if (npc.type == NPCID.MartianTurret || npc.type == NPCID.MartianEngineer)
            {
                npc.buffImmune[BuffID.Electrified] = true;
            }
        }
    }
}
