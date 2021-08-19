using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace CholosRandomMod.Tiles
{
    public class SupremeManipulator : ModTile
    {
        public static int[] workStations = new int[] {
                TileID.WorkBenches,
                TileID.MythrilAnvil,
                TileID.AdamantiteForge,
                TileID.AlchemyTable,
                TileID.Sinks,
                TileID.Bookcases,
                TileID.CookingPots,
                TileID.TinkerersWorkbench,
                TileID.ImbuingStation,
                TileID.HeavyWorkBench,
                TileID.DemonAltar,
                TileID.CrystalBall,
                TileID.Autohammer,
                TileID.LunarCraftingStation
        };

        public override void SetDefaults()
        {
            Main.tileLavaDeath[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileFrameImportant[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 18 };
            TileObjectData.newTile.Origin = new Point16(1, 2);
            TileObjectData.addTile(Type);

            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Supreme Manipulator");
            AddMapEntry(new Color(78, 255, 51), name);

            disableSmartCursor = true;
            dustType = DustID.GreenFairy;

            adjTiles = workStations;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3; // Create 1 dust if hit, 3 if tile broken
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 48, 48, ModContent.ItemType<Items.SupremeManipulator>());
        }
    }
}
