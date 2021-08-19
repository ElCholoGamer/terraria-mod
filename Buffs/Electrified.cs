using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CholosRandomMod.Buffs
{
    class Electrified : GlobalBuff
    {
        public override void Update(int type, NPC npc, ref int buffIndex)
        {
            if (type != BuffID.Electrified) return;

            if (npc.lifeRegen > 0)
            {
                npc.lifeRegen = 0;
            }

            npc.lifeRegen -= 32;

            Dust.NewDust(npc.position, npc.width, npc.height, DustID.Electric, default, default, default, default, 0.5f);
        }
    }
}
