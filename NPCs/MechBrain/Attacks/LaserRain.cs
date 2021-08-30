using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using CholosRandomMod.Projectiles;

namespace CholosRandomMod.NPCs.MechBrain.Attacks
{
    public class LaserRain : NPCAttack<MechBrain>
    {
        public override float Duration => 550f;

        private bool showRedRing;

        public LaserRain(MechBrain brain) : base(brain)
        {

        }

        public override void Initialize()
        {
            showRedRing = true;
        }

        public override void AI()
        {
            NPC npc = modNPC.npc;
            if (npc.alpha == 0 && showRedRing)
            {
                modNPC.RedRing();
                Main.PlaySound(SoundID.Roar, npc.Center, 0);

                showRedRing = false;
            }

            float initialDelay = 90f;
            if (modNPC.CycleTimer < initialDelay) return;

            float shootDuration = 120f;
            float rainDelay = 80f;
            float safeArea = 200f;

            float shootUntil = initialDelay + shootDuration;
            float rainAt = shootUntil + rainDelay;
            float rainUntil = Duration - 80f;

            if (modNPC.CycleTimer < rainAt)
            {
                // Dust stuff
                int range = 5000;
                int dustAmount = 100;
                for (int d = 0; d < dustAmount; d++)
                {
                    Vector2 velocity = new Vector2(Main.rand.NextFloat(10f), 0f);
                    float offsetFromNPC = safeArea / 2f;
                    if (d < dustAmount / 2)
                    {
                        offsetFromNPC *= -1f;
                        velocity.X *= -1f;
                    }

                    Vector2 position = new Vector2(npc.Center.X + offsetFromNPC, npc.Center.Y + Main.rand.NextFloat(range) - range / 2);
                    int dustID = Dust.NewDust(position, 0, 0, DustID.RedTorch, 0, 0, 0, default, 2f);
                    Main.dust[dustID].noGravity = true;
                    Main.dust[dustID].velocity = velocity;
                }
            }

            if (modNPC.CycleTimer < shootUntil)
            {
                // Laser shoot animation
                Vector2 direction = new Vector2(0f, -15f);
                float rotationSpread = MathHelper.ToRadians(40f);
                float positionSpread = 120f;

                Vector2 velocity = direction.RotatedByRandom(rotationSpread - rotationSpread / 2);
                Vector2 position = new Vector2(npc.Center.X + (Main.rand.NextFloat(positionSpread) - positionSpread / 2), npc.Center.Y - 20f);
                position += direction * 5;

                int projID = Projectile.NewProjectile(position, velocity, ProjectileID.DeathLaser, modNPC.laserDamage / 2, 0f, Main.myPlayer, 1f);
                Main.projectile[projID].alpha = 120;
                Main.projectile[projID].timeLeft = 180;
            }
            else if (modNPC.CycleTimer > rainAt && modNPC.CycleTimer < rainUntil)
            {
                // Rain down lasers
                float range = 4000f;
                Player myPlayer = Main.player[Main.myPlayer];

                for (int i = 0; i < 10; i++)
                {
                    float offsetFromNPC = (safeArea / 2) + Main.rand.NextFloat(range);
                    if (Main.rand.NextBool())
                        offsetFromNPC *= -1f;

                    Vector2 position = new Vector2(npc.Center.X + offsetFromNPC, myPlayer.Center.Y - 800f);

                    Projectile.NewProjectile(
                        position, 
                        new Vector2(0f, 15f), 
                        ProjectileID.DeathLaser, 
                        modNPC.laserDamage, 
                        0f, 
                        Main.myPlayer, -1f, -1f);
                }
            }
        }
    }
}
