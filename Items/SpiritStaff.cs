using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using CholosRandomMod.Buffs;
using CholosRandomMod.Projectiles.Minions;

namespace CholosRandomMod.Items
{
    public class SpiritStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dungeon Spirit Staff");
            Tooltip.SetDefault("Summons a friendly dungeon spirit to fight for you\n'Somehow they think you're Skeletron'");

            ItemID.Sets.GamepadWholeScreenUseRange[item.type] = true;
            ItemID.Sets.LockOnIgnoresCollision[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 45;
            item.summon = true;
            item.mana = 10;
            item.width = item.height = 46;
            item.useTime = 36;
            item.useAnimation = 36;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.autoReuse = true;
            item.knockBack = 2;
            item.value = Item.buyPrice(0, 15, 0 ,0);
            item.rare = ItemRarityID.Cyan;
            item.UseSound = SoundID.NPCHit36;
            item.shoot = ModContent.ProjectileType<SpiritMinion>();
            item.buffType = ModContent.BuffType<SpiritMinionBuff>();
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            player.AddBuff(item.buffType, 2);
            position = Main.MouseWorld;

            // Dust effect
            for (int d = 0; d < 20; d++)
            {
                int dustId = Dust.NewDust(position, 1, 1, DustID.WhiteTorch);
                Main.dust[dustId].noGravity = true;
            }

            return true;
        }
    }
}
