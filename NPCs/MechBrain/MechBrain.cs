using CholosRandomMod.NPCs.MechBrain.Attacks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CholosRandomMod.NPCs.MechBrain
{

    [AutoloadBossHead]
    public class MechBrain : ModNPC
    {
        public const string MechBrainHead = "CholosRandomMod/NPCs/MechBrain/MechBrain_Head_Boss";
        private const int MaxCreepers = 20;
        private readonly NPCAttack<MechBrain>[] phase2Attacks;

        // Local AI
        private bool spawnedCreepers = false;
        private bool forceSprite = false;
        private bool transitioning = false;

        private readonly List<(Vector2 position, Vector2 velocity)> illusions = new List<(Vector2, Vector2)>();
        private float illusionTimer = 0f;

        public MechBrain()
        {
            phase2Attacks = new NPCAttack<MechBrain>[] { new CircleAttack(this) };
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Steel Mind");
            Main.npcFrameCount[npc.type] = 8;
        }

        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            npc.lifeMax = 29000;
            npc.damage = 45;
            npc.defense = 25;
            npc.knockBackResist = 0f;
            npc.width = 158;
            npc.height = 144;
            npc.value = Item.buyPrice(0, 15, 0, 0);
            npc.npcSlots = 15f;
            npc.boss = true;
            npc.lavaImmune = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            npc.buffImmune[BuffID.Poisoned] = true;
            npc.dontTakeDamage = true;
            music = MusicID.Boss3;

            drawOffsetY = 22;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.6f * bossLifeScale);
            npc.damage = (int)(npc.damage * 0.6f);
        }

        public override void BossHeadSlot(ref int index)
        {
            index = SecondPhase ? -1 : ModContent.GetModBossHeadSlot(MechBrainHead);
        }

        // Just for convenience
        public Player Target
        {
            get => Main.player[npc.target];
        }
        
        public NPCAttack<MechBrain> CurrentAttack
        {
            get => phase2Attacks[(int)AttackIndex];
        }

        // AI slot aliases
        public float CycleTimer
        {
            get => npc.ai[0];
            set => npc.ai[0] = value;
        }

        public float CycleDuration
        {
            get => npc.ai[1];
            set => npc.ai[1] = value;
        }

        private float AttackIndex
        {
            get => npc.ai[2];
            set => npc.ai[2] = value;
        }

        private bool SecondPhase
        {
            get => npc.ai[3] == 1f;
            set => npc.ai[3] = value.ToInt();
        }

        public override void AI()
        {
            // Spawn creepers
            // This will check that they will only spawn on the server
            if (Main.netMode != NetmodeID.MultiplayerClient && !spawnedCreepers)
            {
                for (int c = 0; c < MaxCreepers; c++)
                {
                    float x = Main.rand.NextFloat(npc.width);
                    float y = Main.rand.NextFloat(npc.height);

                    NPC.NewNPC(
                        (int)(npc.position.X + x),
                        (int)(npc.position.Y + y),
                        ModContent.NPCType<MechCreeper>(),
                        0, 0, 0, 180f + Main.rand.NextFloat(240f));
                }

                spawnedCreepers = true;
                npc.netUpdate = true;
            }

            if (Main.dayTime || !npc.HasValidTarget)
            {
                // Despawn
                CycleTimer = 0f;
                DespawnAI();
                return;
            }

            if (!SecondPhase)
                Phase1AI();
            else
                Phase2AI();
        }

        private void Phase1AI()
        {
            // This value will increase agressiveness the more creepers have been killed
            int killedCreepers = GetKilledCreepers();

            // Transition to second phase
            if (killedCreepers == MaxCreepers)
            {
                if (!transitioning)
                {
                    // Prepare transition
                    npc.alpha = Math.Max(npc.alpha - 5, 0);
                    npc.velocity *= 0.95f;

                    if (Main.netMode != NetmodeID.MultiplayerClient && npc.velocity.Length() < 0.3f && npc.alpha == 0)
                    {
                        // Start transition
                        transitioning = true;
                        npc.velocity = Vector2.Zero;

                        npc.netUpdate = true;
                    }
                }
                else
                {
                    if (npc.alpha == 0)
                    {
                        RedRing();
                        Main.PlaySound(SoundID.Roar, npc.Center, 0);
                    }
                    npc.alpha = Math.Min(npc.alpha + 3, 255); // Transition

                    if (Main.netMode != NetmodeID.MultiplayerClient && npc.alpha == 255)
                    {
                        // End transition
                        CycleTimer = 0f;
                        CycleDuration = 0f;

                        forceSprite = true;
                        npc.dontTakeDamage = false;

                        SecondPhase = true;
                        TeleportAround(Target);

                        npc.netUpdate = true;
                    }
                }

                return;
            }

            float fadeDuration = 37f;
            int fadeSpeed = (int)Math.Ceiling(255f / fadeDuration);

            float speed = 3.5f;

            float dashDuration = 100f;
            float moveDuration = CycleDuration - (fadeDuration * 2) - dashDuration;

            // Set initial cycle duration
            if (CycleDuration == 0f)
                CycleDuration = 300f;

            // Each cycle ends with a teleport, and consists of various
            // actions such as fade in, move, dash, and fade out
            CycleTimer++;

            // Movement stuff
            if (CycleTimer < fadeDuration + moveDuration)
            {
                // Move towards target
                npc.TargetClosest(false);
                npc.velocity = npc.DirectionTo(Target.Center) * speed;
            }
            else if (CycleTimer == fadeDuration + moveDuration)
            {
                // Start dash
                float extraDashVelocity = killedCreepers * 0.15f;

                npc.velocity *= 4f;

                float velocityMagnitude = npc.velocity.Length();
                npc.velocity *= (velocityMagnitude + extraDashVelocity) / velocityMagnitude;

                Main.PlaySound(SoundID.Roar, (int)npc.Center.X, (int)npc.Center.Y, 0, 1, 0.6f);
            }
            else
            {
                // Reduce dash velocity
                npc.velocity *= 0.983f;
            }

            if (CycleTimer < fadeDuration)
            {
                // Fade in
                npc.alpha = Math.Max(npc.alpha - fadeSpeed, 0);
            }
            else if (CycleTimer >= CycleDuration - fadeDuration)
            {
                // Fade out
                npc.alpha = Math.Min(npc.alpha + fadeSpeed, 255);
            }

            // Telegraph dash
            if (CycleTimer == fadeDuration + moveDuration - 45f)
            {
                Main.PlaySound(SoundID.Roar, npc.Center, 0);
                RedRing();
            }

            // Reset cycle and teleport
            if (Main.netMode != NetmodeID.MultiplayerClient && CycleTimer >= CycleDuration)
            {
                TeleportAround(Target);

                CycleTimer -= CycleDuration;

                // Set new cycle duration
                float newMoveDuration = 120f - (killedCreepers * (60f / MaxCreepers));
                CycleDuration = (fadeDuration * 2) + newMoveDuration + dashDuration;

                npc.netUpdate = true;
            }
        }

        private void Phase2AI()
        {
            // Random illusions
            if (--illusionTimer <= 0)
            {
                NewIllusion();
                illusionTimer += 5f + (40f * ((float)npc.life / npc.lifeMax)); // Increase illusions as life decreases
            }

            List<int> illusionsToDelete = new List<int>();
            for (int i = 0; i < illusions.Count; i++)
            {
                var illusion = illusions[i];
                (Vector2 position, Vector2 velocity) = illusion;
                illusions[i] = (position += velocity, velocity);

                float halfWidth = npc.frame.Width / 2f;
                float halfHeight = npc.frame.Height / 2f;

                if (position.X > Main.screenWidth + halfWidth || position.X < -halfWidth || position.Y > Main.screenHeight + halfHeight || position.Y < -halfHeight)
                {
                    illusionsToDelete.Add(i);
                }
            }

            illusionsToDelete.Reverse();
            illusionsToDelete.ForEach(index => illusions.RemoveRange(index, 1));

            // Actual phase 2 AI
            float fadeDuration = 30f;
            int fadeSpeed = (int)Math.Ceiling(255f / fadeDuration);

            // Initial setup
            if (CycleDuration == 0f)
            {
                CycleDuration = CurrentAttack.Duration;
                CurrentAttack.Initialize();
            }

            if (Main.netMode != NetmodeID.MultiplayerClient && CycleTimer >= CycleDuration)
            {
                CycleTimer -= CycleDuration;

                AttackIndex = (AttackIndex + 1) % phase2Attacks.Length;
                CurrentAttack.Initialize();
                CycleDuration = CurrentAttack.Duration;

                npc.velocity = Vector2.Zero;
                TeleportAround(Target);

                npc.netUpdate = true;
            }

            CurrentAttack.AI();

            if (CycleTimer <= fadeDuration)
            {
                npc.alpha = Math.Max(npc.alpha - fadeSpeed, 0); // Fade in
            }
            else if (CycleTimer > CycleDuration - fadeDuration)
            {
                npc.alpha = Math.Min(npc.alpha + fadeSpeed, 255); // Fade out
            }

            CycleTimer++;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npc.dontTakeDamage);

            if (!SecondPhase)
                writer.Write(transitioning);

            if (SecondPhase)
            {
                for (int i = 0; i < phase2Attacks.Length; i++)
                {
                    phase2Attacks[i].SendExtraAI(writer);
                }
            }
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.dontTakeDamage = reader.ReadBoolean();

            if (!SecondPhase)
                transitioning = reader.ReadBoolean();

            if (SecondPhase)
            {
                for (int i = 0; i < phase2Attacks.Length; i++)
                {
                    phase2Attacks[i].ReceiveExtraAI(reader);
                }
            }
        }

        public void TeleportAround(Player target, float length = -1f)
        {
            if (length == -1f)
                length = 350f + Main.rand.NextFloat(150f);

            Vector2 distance = Main.rand.NextVector2Unit() * length;

            npc.Center = target.Center + distance;
        }

        private void DespawnAI()
        {
            npc.alpha = Math.Min(npc.alpha + 4, 255);
            npc.velocity *= 0.93f;

            if (npc.alpha == 255)
            {
                npc.life = -1;
            }
        }

        private void NewIllusion()
        {
            Vector2 spawnAt;
            Vector2 velocity;

            float halfWidth = npc.frame.Width / 2f;
            float halfHeight = npc.frame.Height / 2f;

            // Choose a starting side
            switch (Main.rand.Next(4))
            {
                case 0: // Up
                    spawnAt = new Vector2(Main.rand.NextFloat(Main.screenWidth), -halfHeight);
                    velocity = new Vector2(0f, 1f);
                    break;
                case 1: // Right
                    spawnAt = new Vector2(Main.screenWidth + halfWidth, Main.rand.NextFloat(Main.screenHeight));
                    velocity = new Vector2(-1f, 0f);
                    break;
                case 2: // Down
                    spawnAt = new Vector2(Main.rand.NextFloat(Main.screenWidth), Main.screenHeight + halfHeight);
                    velocity = new Vector2(0f, -1f);
                    break;
                case 3: // Left
                    spawnAt = new Vector2(-halfWidth, Main.rand.NextFloat(Main.screenHeight));
                    velocity = new Vector2(1f, 0f);
                    break;
                default:
                    return;
            }

            // Adjust velocity
            float spreadDegrees = 135f;
            velocity = velocity.RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(spreadDegrees) - (spreadDegrees / 2)));
            velocity *= 15f + Main.rand.NextFloat(15f);

            illusions.Add((spawnAt, velocity));
        }

        private int GetKilledCreepers()
        {
            return MaxCreepers - Math.Min(NPC.CountNPCS(ModContent.NPCType<MechCreeper>()), MaxCreepers);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            if (!SecondPhase) return true;

            Texture2D texture = Main.npcTexture[npc.type];
            float opacity = (1f - ((float)npc.life / npc.lifeMax));
            Vector2 drawOffset = new Vector2(npc.frame.Width / 2f, npc.frame.Height / 2f);

            // Parallel illusions
            if (Main.expertMode)
            {
                Player player = Main.player[Main.myPlayer];

                float npcOpacity = (1f - ((float)npc.alpha / 255f));

                Vector2 center = player.Center - Main.screenPosition;
                Vector2 drawPosition = player.Center - npc.Center;

                drawPosition.X *= -1f;
                spriteBatch.Draw(texture, center + drawPosition - drawOffset, npc.frame, drawColor * opacity * npcOpacity);

                drawPosition.X *= -1f;
                spriteBatch.Draw(texture, center + drawPosition - drawOffset, npc.frame, drawColor * opacity * npcOpacity);

                drawPosition.Y *= -1f;
                spriteBatch.Draw(texture, center + drawPosition - drawOffset, npc.frame, drawColor * opacity * npcOpacity);
            }

            // Random illusions
            for (int i = 0; i < illusions.Count; i++)
            {
                (Vector2 position, Vector2 _) = illusions[i];

                spriteBatch.Draw(texture, position - drawOffset, npc.frame, drawColor * opacity * 0.6f);
            }

            return true;
        }

        public void RedRing()
        {
            for (int d = 0; d < 360; d++)
            {
                Vector2 velocity = new Vector2(0, 10f).RotatedBy(MathHelper.ToRadians(d));
                int dustId = Dust.NewDust(npc.Center, 0, 0, DustID.RedTorch);
                Main.dust[dustId].noGravity = true;
                Main.dust[dustId].velocity = velocity;
                Main.dust[dustId].scale *= 3f;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (forceSprite)
            {
                npc.frameCounter = -1f;
                forceSprite = false;
            }

            float frameDuration = SecondPhase ? 7f : 10f;
            if (npc.frameCounter != -1f && ++npc.frameCounter < frameDuration) return;

            npc.frameCounter = 0;
            int frameIndex = npc.frame.Y / frameHeight;

            int frameCount = Main.npcFrameCount[npc.type] / 2;
            int nextFrame = (frameIndex + 1) % frameCount;

            if (transitioning || SecondPhase)
            {
                frameIndex = nextFrame + 4;
            }
            else
            {
                frameIndex = nextFrame;
            }

            npc.frame.Y = frameIndex * frameHeight;
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = SecondPhase ? 0f : 1.5f;
            return null;
        }
    }
}
