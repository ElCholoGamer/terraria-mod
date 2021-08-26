using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework;

namespace CholosRandomMod.NPCs.MechBrain
{
    public class MechBrain : ModNPC
    {
        public const string MechBrainHead = "CholosRandomMod/NPCs/MechBrain/MechBrain_Head_Boss";
        private const int MaxCreepers = 20;

        // Local AI
        private bool spawnedCreepers = false;
        private bool secondPhase = false;
        private bool forceSprite = false;

        public override bool Autoload(ref string name)
        {
            mod.AddBossHeadTexture(MechBrainHead);
            return base.Autoload(ref name);
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
            index = ModContent.GetModBossHeadSlot(MechBrainHead);
        }

        // Just for convenience
        private Player Target
        {
            get => Main.player[npc.target];
        }

        // AI slot aliases

        private float CycleTimer
        {
            get => npc.ai[1];
            set => npc.ai[1] = value;
        }

        private float CycleDuration
        {
            get => npc.ai[2];
            set => npc.ai[2] = value;
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
                        ModContent.NPCType<MechCreeper>());
                }

                spawnedCreepers = true;
                npc.netUpdate = true;
            }

            // Despawn during daytime
            if (Main.dayTime || !npc.HasValidTarget)
            {
                CycleTimer = 0f;
                DespawnAI();
                return;
            }

            if (!secondPhase)
                Phase1AI();
            else
                Phase2AI();
        }

        private void Phase1AI()
        {
            // This value will increase agressiveness the more creepers have been killed
            int killedCreepers = MaxCreepers - Math.Min(NPC.CountNPCS(ModContent.NPCType<MechCreeper>()), MaxCreepers);

            float fadeDuration = 37f;
            int fadeSpeed = (int)Math.Ceiling(255f / fadeDuration);

            float speed = 3.5f;

            float dashDuration = 100f;
            float moveDuration = CycleDuration - (fadeDuration * 2) - dashDuration;

            // Set initial cycle duration
            if (CycleDuration == 0f) SetCycleDuration(fadeDuration, dashDuration);

            // Transition to second phase
            if (killedCreepers == MaxCreepers)
            {
                npc.alpha = Math.Max(npc.alpha - 5, 0); // Turn visible just in case
                npc.velocity *= 0.95f; // Slow down

                if (Main.netMode != NetmodeID.MultiplayerClient && npc.velocity.Length() < 0.3f && npc.alpha == 0)
                {
                    forceSprite = true;
                    npc.velocity = Vector2.Zero;
                    npc.dontTakeDamage = false;
                    secondPhase = true;

                    Main.PlaySound(SoundID.Roar, npc.Center, 0);
                    npc.netUpdate = true;
                }

                return;
            }

            // Each cycle ends with a teleport, and consists of various
            // actions such as fade in, move, dash, and fade out
            CycleTimer++;

            // Movement stuff
            if (CycleTimer < fadeDuration + moveDuration)
            {
                // Move towards target or despawn
                npc.TargetClosest(false);
                npc.velocity = npc.DirectionTo(Target.Center) * speed;
            }
            else if (CycleTimer == fadeDuration + moveDuration)
            {
                // Start dash
                float extraDashVelocity = (killedCreepers * 0.15f);

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
                Main.PlaySound(SoundID.Item12, npc.Center);

                for (int d = 0; d < 360; d++)
                {
                    Vector2 velocity = new Vector2(0, 10f).RotatedBy(MathHelper.ToRadians(d));
                    int dustId = Dust.NewDust(npc.Center, 0, 0, DustID.RedTorch);
                    Main.dust[dustId].noGravity = true;
                    Main.dust[dustId].velocity = velocity;
                    Main.dust[dustId].scale *= 3f;
                }
            }

            // Reset cycle and teleport
            if (Main.netMode != NetmodeID.MultiplayerClient && CycleTimer >= CycleDuration)
            {
                Vector2 distance = Main.rand.NextVector2Unit() * 400f;
                npc.Center = Target.Center + distance;

                CycleTimer -= CycleDuration;
                SetCycleDuration(fadeDuration, dashDuration);

                npc.netUpdate = true;
            }
        }

        private void Phase2AI()
        {

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

        private int GetKilledCreepers()
        {
            return MaxCreepers - Math.Min(NPC.CountNPCS(ModContent.NPCType<MechCreeper>()), MaxCreepers);
        }

        private void SetCycleDuration(float fadeDuration, float dashDuration)
        {
            float moveDuration = 120 - (GetKilledCreepers() * (60f / MaxCreepers));
            CycleDuration = (fadeDuration * 2) + moveDuration + dashDuration;
        }

        public override void FindFrame(int frameHeight)
        {
            if (forceSprite)
            {
                npc.frameCounter = -1f;
                forceSprite = false;
            }

            float frameDuration = secondPhase ? 7f : 10f;
            if (npc.frameCounter != -1f && ++npc.frameCounter < frameDuration) return;

            npc.frameCounter = 0;
            int frameIndex = npc.frame.Y / frameHeight;
            frameIndex++;

            if (frameIndex >= Main.npcFrameCount[npc.type])
            {
                frameIndex = 4;
            }
            else if (frameIndex > 3 && !secondPhase)
            {
                frameIndex = 0;
            }

            npc.frame.Y = frameIndex * frameHeight;
        }
    }
}
