using LudeonTK;
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
        [DebugAction("GUD", "MakeFlyingHead", false, false, false, false, false, 0, false, actionType = DebugActionType.ToolMapForPawns)]
        public static void MakeFlyingHeadFor(Pawn pawn)
        {
            IntRange intRange = new IntRange(3, 5);
            int randomInRange = intRange.RandomInRange;
            IntVec3 intVec;
            bool flag = !CellFinder.TryFindRandomCellNear(pawn.Position, pawn.Map, randomInRange, (IntVec3 x) => (x - pawn.Position).LengthHorizontalSquared >= 3, out intVec, -1);
            if (!flag)
            {
                LocalTargetInfo localTargetInfo = new LocalTargetInfo(intVec);
                Projectile_FlyingHead projectile_FlyingHead = (Projectile_FlyingHead)GenSpawn.Spawn(GoreDefOf.FlyingHead, pawn.Position, pawn.Map, WipeMode.Vanish);
                projectile_FlyingHead.gender = pawn.gender;
                projectile_FlyingHead.skinColor = pawn.story.SkinColor;
                projectile_FlyingHead.headMap = pawn.Map;
                projectile_FlyingHead.pawn = pawn;
                projectile_FlyingHead.Launch(pawn, localTargetInfo, localTargetInfo, ProjectileHitFlags.None, false, null);
            }
        }
    }
}
