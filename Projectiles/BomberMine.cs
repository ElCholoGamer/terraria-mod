using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CholosRandomMod.Projectiles
{
    class BomberMine : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 0;
            projectile.ranged = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 150;

            drawOriginOffsetY = -11;
        }

        public override void AI()
        {
            projectile.scale *= 1.005f;
            if (++projectile.frameCounter >= 13)
            {
                projectile.frameCounter = 0;

                if (++projectile.frame >= Main.projFrames[projectile.type])
                {
                    projectile.frame = 0;
                }
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item14, projectile.Center); // Explosion sound

            // Smoke Dust
            for (int d = 0; d < 20; d++)
            {
                Dust.NewDust(
                    projectile.position,
                    projectile.width,
                    projectile.height,
                    DustID.Smoke,
                    0f, 0f, 100, default(Color), 1.5f);
            }

            // Chonky smoke 2: Electric Bogaloo
            for (int g = 0; g < 7; g++)
            {
                Gore.NewGore(projectile.Center, default(Vector2), Main.rand.Next(61, 64), 1);
            }

            // Spawn explosion
            if(Main.myPlayer == projectile.owner)
            {
                Projectile.NewProjectile(
                projectile.Center,
                Vector2.Zero,
                ModContent.ProjectileType<BomberExplosion>(),
                projectile.damage,
                projectile.knockBack,
                projectile.owner);
            }
        }
    }
}
