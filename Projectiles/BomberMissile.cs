using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;

namespace CholosRandomMod.Projectiles
{
    public class BomberMissile : ModProjectile
    {

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 12;
            projectile.scale = 1.5f;
            projectile.alpha = 255;

            projectile.timeLeft = 90;
            projectile.ranged = true;
            projectile.friendly = true;

            aiType = ProjectileID.Bullet;
        }

        public override void AI()
        {
            // Sprite direction
            projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
            projectile.rotation = projectile.velocity.ToRotation() + (projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);

            projectile.alpha = Math.Max(projectile.alpha - 20, 0); // Increment opacity
            projectile.scale += 0.02f; // Increment size

            // Trail dust
            for (int d = 0; d < 3; d++)
            {
                int dustId = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y - (projectile.height / 4)), projectile.width, projectile.height, DustID.Smoke, 0f, 0f, 100, default(Color));
                Main.dust[dustId].velocity *= 0.1f;
            }

            // Fire trail
            int fireId = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y - (projectile.height / 4)), projectile.width, projectile.height, DustID.Fire, 0f, 0f, 100, default(Color));
            Main.dust[fireId].velocity *= 0.1f;


            // Spawn trailing mines
            if(Main.myPlayer == projectile.owner && ++projectile.ai[0] >= 15)
            {
                projectile.ai[0] = 0;

                Projectile.NewProjectile(
                    projectile.Center,
                    Vector2.Zero,
                    ModContent.ProjectileType<BomberMine>(),
                    projectile.damage / 2,
                    projectile.knockBack, projectile.owner);
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item14, projectile.Center);

            // Smoke dust
            for (int d = 0; d < 20; d++)
            {
                Dust.NewDust(
                    projectile.position,
                    projectile.width,
                    projectile.height,
                    DustID.Smoke,
                    0f, 0f, 100, default(Color), 1.5f);
            }

            // Fire dust
            for (int d = 0; d < 40; d++)
            {
                int dustId = Dust.NewDust(
                    projectile.position,
                    projectile.width,
                    projectile.height,
                    DustID.Fire,
                    0f, 0f, 100, default(Color), 1.5f);
                Main.dust[dustId].noGravity = true;
                Main.dust[dustId].velocity *= 5f;
            }

            // Chonky smoke
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
                projectile.damage / 2,
                projectile.knockBack,
                projectile.owner);
            }
        }
    }
}
