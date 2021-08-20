using Terraria;
using Terraria.ModLoader;
using CholosRandomMod.Projectiles.Minions;

namespace CholosRandomMod.Buffs
{
    public class SpiritMinionBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Spirit Minion");
            Description.SetDefault("A friendly dungeon spirit will fight for you");

            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<SpiritMinion>()] > 0)
            {
                player.buffTime[buffIndex] = 18000;
            }
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}
