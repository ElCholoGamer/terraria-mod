using Terraria.ModLoader;
using System.IO;

namespace CholosRandomMod.NPCs
{
    public abstract class NPCAttack<NPC> where NPC : ModNPC
    {
        public readonly NPC modNPC;

        public NPCAttack(NPC modNPC)
        {
            this.modNPC = modNPC;
        }

        public abstract float Duration
        {
            get;
        }

        public virtual void Initialize() { }

        public abstract void AI();

        public virtual void SendExtraAI(BinaryWriter writer) { }

        public virtual void ReceiveExtraAI(BinaryReader reader) { }
    }
}
