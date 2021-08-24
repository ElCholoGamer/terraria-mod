using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using CholosRandomMod.Buffs;

namespace CholosRandomMod.Projectiles.Minions
{
    public class SpiritMinion : ModProjectile
    {
        private readonly float RANGE = 1500f;
        private NPC target = null;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 3;
            projectile.minion = true;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.minionSlots = 1f;
            projectile.alpha = 60;

            drawOffsetX = -12;
            drawOriginOffsetY = -14;

            Main.projFrames[projectile.type] = 3;
        }

        public override void AI()
        {
            // Check that player is alive
            Player player = Main.player[projectile.owner];
            if (player.dead || !player.active)
            {
                player.ClearBuff(ModContent.BuffType<SpiritMinionBuff>());
            }

            if (player.HasBuff(ModContent.BuffType<SpiritMinionBuff>()))
            {
                projectile.timeLeft = 2;
            }

            float accelerationFactor = 0.2f;
            Vector2 acceleration;
            float maxVelocity = 12f;

            if (player.HasMinionAttackTargetNPC)
            {
                target = Main.npc[player.MinionAttackTargetNPC];
            }
            else if (target == null || !target.active || target.behindTiles)
            {
                projectile.ai[0] = Main.rand.Next(30); // Reset shoot timer
                target = FindTarget();
            }

            // Teleport to player if too far away
            Vector2 idlePosition = FindIdlePosition();

            Vector2 idleDistance = idlePosition - projectile.Center;

            if (idleDistance.Length() > RANGE)
            {
                target = FindTarget();
                if (target == null)
                {
                    projectile.position = idlePosition;
                    projectile.velocity *= 0.1f;
                }
            }

            if (target == null)
            {
                // Idle
                if (idleDistance.Length() <= 3f && player.velocity.Length() < 3f)
                {
                    projectile.Center = idlePosition;
                    projectile.velocity = acceleration = Vector2.Zero;

                    float targetRotation = MathHelper.Pi * player.direction;
                    projectile.rotation = (projectile.rotation + targetRotation) * 0.91f - targetRotation;
                }
                else
                {
                    idleDistance.Normalize();
                    acceleration = idleDistance * accelerationFactor * (0.7f + (Main.rand.Next(7) / 10f));
                    projectile.rotation = acceleration.ToRotation() + MathHelper.PiOver2;
                }

            }
            else
            {
                // Attack
                Vector2 distanceFromTarget = projectile.DirectionFrom(target.Center);

                float shootingRange = 150f;
                distanceFromTarget *= shootingRange;
                distanceFromTarget = distanceFromTarget.RotatedBy(MathHelper.ToRadians(projectile.minionPos * 0.1f));

                Vector2 targetPosition;
                int targetX;
                int targetY;

                // Avoid solid blocks
                do
                {
                    targetPosition = target.Center + distanceFromTarget;

                    targetX = (int)(targetPosition.X / 16f);
                    targetY = (int)(targetPosition.Y / 16f);

                    distanceFromTarget = distanceFromTarget.RotatedBy(MathHelper.ToRadians(1));
                } while (Main.tile[targetX, targetY].active());

                Vector2 distanceToTargetPosition = targetPosition - projectile.Center;
                acceleration = distanceToTargetPosition * accelerationFactor;

                // Look towards target
                Vector2 distanceToTarget = target.Center - projectile.Center;
                projectile.rotation = distanceToTarget.ToRotation() + MathHelper.PiOver2;

                // Shoot projectile
                if (Main.myPlayer == projectile.owner && distanceToTargetPosition.Length() < 200f && ++projectile.ai[0] >= 30f)
                {
                    projectile.ai[0] = 0f;

                    Vector2 towardsTarget = projectile.DirectionTo(target.Center);

                    int projId = Projectile.NewProjectile(
                        projectile.Center,
                        new Vector2(20f, 0f).RotatedBy(towardsTarget.ToRotation()),
                        ProjectileID.LostSoulFriendly,
                        projectile.damage,
                        projectile.knockBack,
                        projectile.owner);

                    Main.projectile[projId].tileCollide = false;
                }
            }

            float friction = 0.95f;
            ApplyFriction(friction, ref projectile.velocity.X, acceleration.X);
            ApplyFriction(friction, ref projectile.velocity.Y, acceleration.Y);
            projectile.velocity += acceleration;

            // Limit velocity
            float velocityLength = projectile.velocity.Length();
            if (velocityLength > maxVelocity)
            {
                projectile.velocity *= maxVelocity / velocityLength;
            }

            // Dust
            if (Main.rand.NextBool(3) && velocityLength > 3f)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.WhiteTorch);
            }

            // Frame loop
            if (++projectile.frameCounter >= 10)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= Main.projFrames[projectile.type])
                {
                    projectile.frame = 0;
                }
            }

            Lighting.AddLight(projectile.Center, Color.White.ToVector3() * 0.8f);
        }

        private void ApplyFriction(float friction, ref float velocity, float acceleration)
        {
            if (Math.Sign(velocity) + Math.Sign(acceleration) == 0)
            {
                velocity *= friction;
            }
        }

        private Vector2 FindIdlePosition()
        {
            Player player = Main.player[projectile.owner];
            Vector2 pos = player.Center;

            pos.X += (50f + (projectile.minionPos * 30f)) * -player.direction;

            return pos;
        }

        private bool BehindTile()
        {
            int tileX = (int)(projectile.position.X / 16f);
            int tileY = (int)(projectile.position.Y / 16f);

            return Main.tile[tileX, tileY].active();
        }

        private NPC FindTarget()
        {
            Player player = Main.player[projectile.owner];

            NPC target = null;
            float currentDistanceSquared = (float)Math.Pow(RANGE, 2);

            // Find closest target
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC possibleTarget = Main.npc[i];
                if (!possibleTarget.CanBeChasedBy() || possibleTarget.behindTiles) continue;

                Vector2 distanceFromProjectile = possibleTarget.Center - projectile.Center;
                Vector2 distanceFromPlayer = possibleTarget.Center - player.Center;
                float distanceSquared = distanceFromProjectile.LengthSquared();

                if (distanceSquared < currentDistanceSquared && distanceFromPlayer.Length() <= RANGE)
                {
                    target = possibleTarget;
                    currentDistanceSquared = distanceSquared;
                }
            }

            return target;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }
    }
}
