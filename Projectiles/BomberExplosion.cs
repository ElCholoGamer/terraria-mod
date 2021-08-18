using Terraria.ID;
using Terraria.ModLoader;

namespace RandomMod.Projectiles
{
    class BomberExplosion : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.penetrate = -1;
            projectile.width = projectile.height = 200;
            projectile.timeLeft = 3;
            projectile.friendly = true;
            projectile.ranged = true;

            aiType = ProjectileID.Bullet;


        }
    }
}
