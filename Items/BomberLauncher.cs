using Terraria.ID;
using Terraria.ModLoader;
using RandomMod.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;

namespace RandomMod.Items
{
    public class BomberLauncher : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Bomber");
            Tooltip.SetDefault("The power of a hundred bombs in your hands");
        }

        public override void SetDefaults()
        {
            item.damage = 157;
            item.ranged = true;
            item.noMelee = true;
            item.autoReuse = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 4;
            item.useTime = 30;
            item.useAnimation = 30;
            item.value = 10000;
            item.shoot = ModContent.ProjectileType<BomberMissile>();
            item.shootSpeed = 15f;
            item.useAmmo = AmmoID.Rocket;
            item.UseSound = SoundID.Item11;
            item.rare = ItemRarityID.LightPurple;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(ItemID.SnowmanCannon);
            recipe.AddIngredient(ItemID.IllegalGunParts);
            recipe.AddIngredient(ItemID.Bomb, 100);

            recipe.AddTile(TileID.MythrilAnvil);

            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(10, 0);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(speedX, speedY)) * 25f;

            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
            return true;
        }
    }
}
