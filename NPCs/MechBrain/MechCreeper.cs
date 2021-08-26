using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.IO;

namespace CholosRandomMod.NPCs.MechBrain
{
    public class MechCreeper : ModNPC
    {
        private Vector2 homeOffset;
        private float cycleTimer = 0f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mechanical Creeper");
        }

        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            npc.lifeMax = 2900;
            npc.damage = 55;
            npc.defense = 15;
            npc.knockBackResist = 0f;
            npc.width = npc.height = 16;
            npc.lavaImmune = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            npc.buffImmune[BuffID.Poisoned] = true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.6f * bossLifeScale);
            npc.damage = (int)(npc.damage * 0.6f);
        }

        private float ChargeAt
        {
            get => npc.ai[0];
            set => npc.ai[0] = value;
        }

        private float ChargeDuration
        {
            get => npc.ai[1];
            set => npc.ai[1] = value;
        }

        public override void AI()
        {
            // Find MechBrain owner
            int ownerID = NPC.FindFirstNPC(ModContent.NPCType<MechBrain>());
            NPC owner = ownerID > -1 ? Main.npc[ownerID] : null;

            // Despawn if no owner found
            if (owner == null || !owner.active)
            {
                npc.life = -1;
                return;
            }

            // Target player
            if (!npc.HasValidTarget)
            {
                npc.TargetClosest(false);
                return;
            }

            // Set initial timers
            if (ChargeAt == 0f || ChargeDuration == 0f)
                SetRandomStuff(owner);

            float cycleDuration = ChargeAt + ChargeDuration;
            cycleTimer++;

            // Reset cycle
            if (cycleTimer >= cycleDuration) {
                SetRandomStuff(owner);
                cycleTimer = 0;
            }

            bool charging = cycleTimer >= ChargeAt;

            // Go to respective position
            float speed = 15f;
            float inertia = 30f;

            Player player = Main.player[npc.target];
            Vector2 targetPosition = charging ? player.Center : GetHomePosition(owner);

            Vector2 direction = npc.DirectionTo(targetPosition) * speed;

            npc.velocity = (npc.velocity * (inertia - 1f) + direction) / inertia;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WritePackedVector2(homeOffset);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            homeOffset = reader.ReadPackedVector2();
        }

        private void SetRandomStuff(NPC owner)
        {
            homeOffset = new Vector2(Main.rand.NextFloat(owner.width), Main.rand.NextFloat(owner.height));
            ChargeAt = 80f + Main.rand.NextFloat(500f);
            ChargeDuration = 60f + Main.rand.NextFloat(80f);

            npc.netUpdate = true;
        }

        private Vector2 GetHomePosition(NPC owner)
        {
            return new Vector2(
                owner.position.X + homeOffset.X,
                owner.position.Y + homeOffset.Y);
        }
    }
}
