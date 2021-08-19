using Terraria.ID;
using Terraria.ModLoader;
using CholosRandomMod.Projectiles;

namespace CholosRandomMod.Items
{
    class MartianTome : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Extraterrestrial Tome");
            Tooltip.SetDefault("Fires a homing spectral tesla turret\n'They want to phone home'");
        }

        public override void SetDefaults()
        {
            item.damage = 140;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 4;
            item.useTime = 15;
            item.useAnimation = 15;
            item.value = 150000;
            item.magic = true;
            item.mana = 10;
            item.autoReuse = true;
            item.UseSound = SoundID.NPCHit53;
            item.rare = ItemRarityID.Yellow;

            item.shoot = ModContent.ProjectileType<MartianProjectile>();
            item.shootSpeed = 10f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(ItemID.SpellTome);
            recipe.AddIngredient(ItemID.MartianConduitPlating, 150);

            recipe.AddTile(TileID.Bookcases);
            recipe.SetResult(this);

            recipe.AddRecipe();
        }
    }
}
