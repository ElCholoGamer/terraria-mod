using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using System.IO;

namespace CholosRandomMod.NPCs.MechBrain.Attacks
{
    public class ChainDash : NPCAttack<MechBrain>
    {
        public override float Duration => 460f;

        private int laserDamage = 40;
        private float dashes;
        private float dashCooldown;
        private bool showRedRing;
        private bool shoot;

        public ChainDash(MechBrain brain) : base(brain)
        {

        }

        public override void Initialize()
        {
            dashes = 4f;
            shoot = true;
            showRedRing = true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            laserDamage = (int)(laserDamage * 0.6f);
        }

        public override void AI()
        {
            NPC npc = modNPC.npc;

            if (npc.alpha == 0 && showRedRing)
            {
                modNPC.RedRing();
                showRedRing = false;
            }

            if (modNPC.CycleTimer < 60f) return;

            float velocityLength = npc.velocity.Length();
            float dashVelocity = 20f;

            if (velocityLength > 4f)
            {
                npc.velocity *= 0.978f;

                // Shoot lasers
                if (velocityLength < 8f && shoot && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    float spread = MathHelper.ToRadians(20f);
                    Vector2 directionToPlayer = npc.DirectionTo(modNPC.Target.Center);

                    if (Main.expertMode)
                    {
                        ShootLaser(directionToPlayer.RotatedBy(spread));
                        ShootLaser(directionToPlayer.RotatedBy(-spread));
                    }

                    if (dashes % 2 == 1)
                    {
                        ShootLaser(directionToPlayer);
                    } else
                    {
                        float innerSpread = spread / 3;
                        ShootLaser(directionToPlayer.RotatedBy(innerSpread));
                        ShootLaser(directionToPlayer.RotatedBy(-innerSpread));
                    }

                    shoot = false;
                    npc.netUpdate = true;
                }
            }
            else if (--dashCooldown <= 0f && dashes-- > 0)
            {
                // Start dash
                npc.velocity = npc.DirectionTo(modNPC.Target.Center) * dashVelocity;
                dashCooldown = 20f;
                shoot = true;

                Main.PlaySound(SoundID.Roar, (int)npc.Center.X, (int)npc.Center.Y, 0, 1, 0.6f);
            }
            else
            {
                npc.velocity = Vector2.Zero;
            }
        }

        private void ShootLaser(Vector2 direction)
        {
            Vector2 position = modNPC.npc.Center + (direction * 25f);
            Projectile.NewProjectile(position, direction * 6f, ProjectileID.DeathLaser, laserDamage / 2, 0f, Main.myPlayer);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //writer.Write(shoot);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //shoot = reader.ReadBoolean();
        }
    }
}
