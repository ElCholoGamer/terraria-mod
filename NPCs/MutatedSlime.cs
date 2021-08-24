using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CholosRandomMod.NPCs
{
    public class MutatedSlime : ModNPC
    {
        private int ColorDirection = 1;
        private float SpriteRed = 0f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mutated Slime");
            Main.npcFrameCount[npc.type] = 2;
        }
        public override void SetDefaults()
        {
            npc.width = 40;
            npc.height = 28;
            npc.aiStyle = -1;
            npc.damage = 15;
            npc.defense = 6;
            npc.lifeMax = 150;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.value = 25f;

            npc.buffImmune[BuffID.Ichor] = true;
            npc.buffImmune[BuffID.CursedInferno] = true;

            drawOffsetY = -3;
        }

        public override float SpawnChance(NPCSpawnInfo info)
        {
            return (info.player.ZoneCorrupt || info.player.ZoneCrimson) && info.player.ZoneOverworldHeight
                ? 0.1f
                : 0f;
        }

        // Aliases for AI slots
        private float JumpTimer
        {
            get => npc.ai[0];
            set => npc.ai[0] = value;
        }

        private float BigJumpTimer
        {
            get => npc.ai[1];
            set => npc.ai[1] = value;
        }

        private float JumpDirection
        {
            get => npc.ai[2];
            set => npc.ai[2] = value;
        }

        private float PreviousYVelocity
        {
            get => npc.ai[3];
            set => npc.ai[3] = value;
        }

        public override void AI()
        {
            // Change color
            float colorSpeed = 0.02f;
            SpriteRed = MathHelper.Clamp(SpriteRed + (colorSpeed * ColorDirection), 0f, 1f);

            if (SpriteRed >= 1f || SpriteRed <= 0f)
            {
                ColorDirection *= -1;
            }

            float minimumRed = 150f;
            float minimumBlue = 60f;
            float r = minimumRed + (SpriteRed * (255f - minimumRed));
            float b = minimumBlue + ((1f - SpriteRed) * (255f - minimumBlue));

            npc.color = new Color(r / 255f, 0f, b / 255f, 0.8f);

            float jumpCooldown = 30f;

            if (npc.velocity.Y == 0f)
            {
                JumpDirection = 0f;

                if (npc.HasValidTarget)
                {
                    if (PreviousYVelocity > 0f) // Has just landed
                    {
                        // Shoot spikes
                        for (int p = 0; p < 10; p++)
                        {
                            float minimumYVelocity = 5f;
                            float maxXVelocity = 5f;
                            float maxMagnitude = 8f;
                            float minMagnitude = 4f;

                            float spreadX = 2f;
                            float spreadY = 1f;

                            Vector2 velocity = (npc.Center - Main.player[npc.target].Center) * 0.4f;

                            float magnitude = velocity.Length();
                            if(magnitude > maxMagnitude)
                            {
                                velocity *= maxMagnitude / magnitude;
                            } else if(magnitude < minMagnitude)
                            {
                                velocity *= minMagnitude / magnitude;
                            }

                            velocity.Y = -Math.Max(velocity.Y, minimumYVelocity);
                            velocity.X = -Math.Min(velocity.X * 0.4f, maxXVelocity);

                            velocity.X += (spreadX * Main.rand.NextFloat(1f)) - (spreadX / 2);
                            velocity.Y += (spreadY * Main.rand.NextFloat(1f)) - (spreadY / 2);

                            Projectile.NewProjectile(npc.Center, velocity, ProjectileID.SpikedSlimeSpike, 20, 0f);
                        }

                        JumpTimer = jumpCooldown;
                    }
                    JumpTimer--;
                }

                // Friction
                npc.velocity.X *= 0.74f;

                if (Math.Abs(npc.velocity.X) < 0.05f)
                {
                    npc.velocity.X = 0f;
                }
            }
            else
            {
                // Gravity
                float gravity = 0.08f;
                float maxGravity = 80f;
                npc.velocity.Y = Math.Min(npc.velocity.Y + gravity, maxGravity);

                // Horizontal movement
                float horizontalSpeed = -3.5f * JumpDirection;
                npc.velocity.X = horizontalSpeed;
            }


            npc.TargetClosest();

            if (!npc.HasValidTarget) return;

            if (JumpTimer == 0f)
            {
                // Jump
                BigJumpTimer++;

                float jumpForce = -6f;

                if (BigJumpTimer == 3)
                {
                    jumpForce *= 1.4f;
                    BigJumpTimer = 0f;
                }

                npc.velocity.Y = jumpForce;

                float distanceToPlayer = npc.position.X - Main.player[npc.target].position.X;
                JumpDirection = Math.Sign(distanceToPlayer);

                JumpTimer = jumpCooldown;
            }

            PreviousYVelocity = npc.velocity.Y;
        }

        public override void FindFrame(int frameHeight)
        {
            float targetCounter = (JumpTimer > 10f) ? 5f : 5f;
            if (++npc.frameCounter >= targetCounter)
            {
                npc.frameCounter = 0f;
                int frameIndex = (npc.frame.Y + 1) % 2;

                npc.frame.Y = frameIndex * frameHeight;
            }
        }
    }
}
