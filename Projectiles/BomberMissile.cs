using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace RandomMod.Projectiles
{
    public class BomberMissile : ModProjectile
    {
        private readonly int initialSize = 12;

        public override void SetDefaults()
        {
            projectile.width = initialSize;
            projectile.height = initialSize;
            projectile.ranged = true;
            projectile.friendly = true;
            projectile.scale = 1.5f;
            projectile.Opacity = 0f;
            aiType = ProjectileID.Bullet;
        }

        public override void AI()
        {
            projectile.ai[0] += 1f; // Timer

            projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
            projectile.rotation = projectile.velocity.ToRotation() + (projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);

            // Kill the projectile after 90 ticks (1.5 seconds)
            if (projectile.ai[0] >= 90f)
            {
                projectile.Kill();
            }

            // Increase projectile size
            float increment = 0.02f;
            projectile.scale += increment;

            // Make projectile opaque
            if(projectile.Opacity < 1f)
            {
                projectile.Opacity = MathHelper.Min(projectile.Opacity + 0.05f, 1f);
            }
        }

        public override bool PreKill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item14, (int) projectile.Center.X, (int) projectile.Center.Y);
            return true;
        }
    }
}
