using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using System;
using System.IO;

namespace CholosRandomMod.NPCs.MechBrain.Attacks
{
    public class CircleAttack : NPCAttack<MechBrain>
    {
        public override float Duration => 230f;

        private float rotationSpeed;
        private bool showRing;

        public CircleAttack(MechBrain brain) : base(brain)
        {

        }

        public override void Initialize()
        {
            showRing = true;

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                rotationSpeed = 6f;

                if (Main.rand.NextBool())
                    rotationSpeed *= -1f;

                modNPC.npc.netUpdate = true;
            }
        }

        public override void AI()
        {
            NPC npc = modNPC.npc;

            if (npc.alpha == 0 && showRing)
            {
                showRing = false;
                modNPC.RedRing();
            }


            float absRotationSpeed = Math.Abs(rotationSpeed);

            if (absRotationSpeed > 0.1f)
            {
                float circleRadius = 250f;

                Vector2 direction = npc.DirectionFrom(modNPC.Target.Center);
                Vector2 nextTargetPosition = modNPC.Target.Center + modNPC.Target.velocity; // Has to predict target's next position

                bool rotate = modNPC.CycleTimer > 30f;
                if (rotate)
                    direction = direction.RotatedBy(MathHelper.ToRadians(rotationSpeed));

                npc.Center = nextTargetPosition + (direction * circleRadius);

                if (rotate)
                {
                    if (absRotationSpeed > 2f)
                    {
                        rotationSpeed *= 0.985f;
                    }
                    else if (absRotationSpeed > 1f)
                    {
                        rotationSpeed *= 0.97f;
                    }
                    else if (absRotationSpeed > 0.6f)
                    {
                        rotationSpeed *= 0.93f;
                    }
                    else
                    {
                        rotationSpeed *= 0.8f;
                    }
                }
            }
            else if (absRotationSpeed > 0)
            {
                // Start dash
                rotationSpeed = 0f;

                npc.velocity = modNPC.npc.DirectionTo(modNPC.Target.Center) * 15f;

                Main.PlaySound(SoundID.Roar, (int)modNPC.npc.Center.X, (int)modNPC.npc.Center.Y, 0, 1, 0.6f);
            }
            else
            {
                // Reduce dash velocity
                npc.velocity *= 0.98f;
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(rotationSpeed);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            rotationSpeed = reader.ReadSingle();
        }
    }
}
