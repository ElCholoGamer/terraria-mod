using Terraria.ID;
using Terraria.ModLoader;

namespace CholosRandomMod.Projectiles
{
    class MartianSpark : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_" + ProjectileID.SaucerScrap; 

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.SaucerScrap);
            projectile.magic = true;
            projectile.friendly = true;
            projectile.hostile = false;

            projectile.aiStyle = 14;
            aiType = ProjectileID.SaucerScrap;
        }

        public override void AI()
        {
            
        }
    }
}
