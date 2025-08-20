using LudeonTK;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace GoreUponDismemberment
{
    partial class GUDUtil
    {
        public static void MakeBrokenLimb(Pawn pawn)
        {
            bool flag = GUDUtil.HasSpecialGoreForXenoType(pawn);
            bool flag2 = flag;
            if (flag2)
            {
                GUDUtil.MakeBrokenLimbForAlien(pawn);
            }
            else
            {
                GUDUtil.MakeBrokenLimbForHuman(pawn);
            }
        }

        public static void MakeBrokenLimbFilth(Pawn pawn)
        {
            bool flag = GUDUtil.HasSpecialGoreForXenoType(pawn);
            bool flag2 = flag;
            if (flag2)
            {
                GUDUtil.MakeBrokenLimbFilthForAlien(pawn);
            }
            else
            {
                GUDUtil.MakeBrokenLimbFilthForHuman(pawn);
            }
        }

        public static void MakeBrokenLimbFilthForAlien(Pawn pawn)
        {
            ThingDef goreFilth = XenoGoreDictionary.dict[pawn.genes.Xenotype].goreFilth;
            bool flag = goreFilth == null;
            if (!flag)
            {
                FilthMaker.TryMakeFilth(pawn.PositionHeld, pawn.Map, goreFilth, pawn.LabelIndefinite(), 1, FilthSourceFlags.Pawn);
            }
        }

        // Token: 0x06000014 RID: 20 RVA: 0x00002592 File Offset: 0x00000792
        public static void MakeBrokenLimbFilthForHuman(Pawn pawn)
        {
            FilthMaker.TryMakeFilth(CellFinder.RandomClosewalkCellNear(pawn.Position, pawn.Map, 5, null), pawn.Map, GoreDefOf.GoreFilth, pawn.LabelIndefinite(), 1, FilthSourceFlags.Pawn);
        }

        [DebugAction("GUD", "MakeBrokenLimb", false, false, false, false, false, 0, false, actionType = DebugActionType.ToolMapForPawns)]
        public static void MakeBrokenLimbForHuman(Pawn pawn)
        {
            Thing thing = ThingMaker.MakeThing(GoreDefOf.Gore, null);
            thing.stackCount = 1;
            GenPlace.TryPlaceThing(thing, pawn.Position, pawn.Map, ThingPlaceMode.Near, null, null, new Rot4?(Rot4.Random), 1);
            thing.SetForbidden(GoreUponDismembermentMod.settings.isForbidden, true);
        }

        public static void MakeBrokenLimbForAlien(Pawn pawn)
        {
            ThingDef gore = XenoGoreDictionary.dict[pawn.genes.Xenotype].gore;
            bool flag = gore == null;
            if (!flag)
            {
                Thing thing = ThingMaker.MakeThing(gore, null);
                thing.stackCount = 1;
                thing.Rotation = Rot4.Random;
                GenPlace.TryPlaceThing(thing, pawn.Position, pawn.Map, ThingPlaceMode.Near, null, null, new Rot4?(Rot4.Random), 1);
                thing.SetForbidden(GoreUponDismembermentMod.settings.isForbidden, true);
            }
        }
    }
}
