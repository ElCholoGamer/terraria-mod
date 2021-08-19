using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CholosRandomMod.Projectiles
{
    class MartianProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = projectile.height = 32;
            projectile.alpha = 255;

            projectile.timeLeft = 300;
            projectile.magic = true;
            projectile.friendly = true;

            drawOffsetX = 1;
            drawOriginOffsetY = -10;
        }

        public override void AI()
        {
            projectile.alpha = (int)MathHelper.Max(projectile.alpha - 50f, 100f);
            projectile.rotation += MathHelper.ToRadians(5);

            float maxVelocity = 15f;

            // Dust trail
            int dustId = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.GreenFairy);
            Main.dust[dustId].noGravity = true;
            Main.dust[dustId].velocity = Vector2.Zero;

            // Initial acceleration
            if (++projectile.ai[0] < 20)
            {
                projectile.velocity *= 1.05f;
            }
            else
            {
                // Find nearest hostile NPC 10 frames ahead
                Vector2 nextPosition = projectile.position + (projectile.velocity * 10);

                float currentDistance = 160000f;
                NPC target = null;

                for (int i = 0; i < 200; i++)
                {
                    NPC possibleTarget = Main.npc[i];
                    if (!possibleTarget.active || possibleTarget.friendly || possibleTarget.lifeMax <= 5) continue;

                    float distance = Vector2.DistanceSquared(nextPosition, possibleTarget.position);
                    if (distance < currentDistance)
                    {
                        currentDistance = distance;
                        target = possibleTarget;
                    }
                }

                // Home in on target
                if (target != null)
                {
                    Vector2 distance = target.position - projectile.position;

                    projectile.velocity = ((10 * projectile.velocity) + distance) / 11f;
                }
            }

            // Limit velocity
            float velocityLength = projectile.velocity.Length();
            if (velocityLength > maxVelocity)
            {
                projectile.velocity *= maxVelocity / velocityLength;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Electrified, 300);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.NPCDeath56, projectile.Center);

            if (Main.myPlayer == projectile.owner)
            {
                int amount = 5 + Main.rand.Next(6);
                for (int s = 0; s < amount; s++)
                {
                    Vector2 velocity = Main.rand.NextVector2Unit(-(MathHelper.Pi * 3/4), MathHelper.PiOver2) * 5f;

                    int sparkId = Projectile.NewProjectile(
                    projectile.Center,
                    velocity,
                    ProjectileID.SaucerScrap,
                    projectile.damage,
                    projectile.knockBack / 2,
                    projectile.owner);

                    Main.projectile[sparkId].friendly = true;
                    Main.projectile[sparkId].hostile = false;
                    Main.projectile[sparkId].magic = true;
                    Main.projectile[sparkId].alpha = 200;
                }

            }
        }
    }
}
