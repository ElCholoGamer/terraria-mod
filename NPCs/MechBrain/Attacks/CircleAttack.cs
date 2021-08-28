using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using System.IO;

namespace CholosRandomMod.NPCs.MechBrain.Attacks
{
    public class CircleAttack : NPCAttack<MechBrain>
    {
        public override float Duration => 230f;

        private float rotationSpeed;

        public CircleAttack(MechBrain brain) : base(brain)
        {

        }

        public override void Initialize()
        {
            rotationSpeed = 6;
        }

        public override void AI()
        {
            if (rotationSpeed > 0.1f)
            {
                float circleRadius = 250f;

                Vector2 direction = modNPC.npc.DirectionFrom(modNPC.Target.Center);
                Vector2 nextTargetPosition = modNPC.Target.Center + modNPC.Target.velocity; // Has to predict target's next position

                modNPC.npc.Center = nextTargetPosition + (direction.RotatedBy(MathHelper.ToRadians(rotationSpeed)) * circleRadius);

                if (rotationSpeed > 2f)
                {
                    rotationSpeed *= 0.985f;
                }
                else if (rotationSpeed > 1f)
                {
                    rotationSpeed *= 0.97f;
                }
                else if (rotationSpeed > 0.6f)
                {
                    rotationSpeed *= 0.93f;
                }
                else
                {
                    rotationSpeed *= 0.8f;
                }
            }
            else if (rotationSpeed > 0)
            {
                // Start dash
                rotationSpeed = 0f;

                modNPC.npc.velocity = modNPC.npc.DirectionTo(modNPC.Target.Center) * 15f;

                modNPC.RedRing();
                Main.PlaySound(SoundID.Roar, (int)modNPC.npc.Center.X, (int)modNPC.npc.Center.Y, 0, 1, 0.6f);
            }
            else
            {
                // Reduce dash velocity
                modNPC.npc.velocity *= 0.98f;
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
