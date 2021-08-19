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
            projectile.alpha = 0;

            projectile.timeLeft = 300;
            projectile.magic = true;
            projectile.friendly = true;

            drawOffsetX = 1;
            drawOriginOffsetY = -10;
        }

        public override void AI()
        {
            projectile.alpha = (int)MathHelper.Max(projectile.alpha - 50f, 0f);
            projectile.rotation += MathHelper.ToRadians(5);

            float maxVelocity = 15f;

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

        }
    }
}
