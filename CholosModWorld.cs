using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CholosRandomMod
{
    public class CholosModWorld : ModWorld
    {
        public static bool downedMechBrain;

        public override void Initialize()
        {
            downedMechBrain = false;
        }

        public override TagCompound Save()
        {
            var downed = new List<string>();

            if(downedMechBrain)
            {
                downed.Add("mechBrain");
            }

            return new TagCompound { ["downed"] = downed };
        }

        public override void Load(TagCompound tag)
        {
            var downed = tag.GetList<string>("downed");
            downedMechBrain = downed.Contains("mechBrain");
        }

        public override void NetSend(BinaryWriter writer)
        {
            var flags = new BitsByte();
            flags[0] = downedMechBrain;
            writer.Write(flags);
        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            downedMechBrain = flags[0];
        }
    }
}
