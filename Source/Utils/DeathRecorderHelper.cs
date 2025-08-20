using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace GoreUponDismemberment
{
    public static class DeathRecorderHelper
    {
        public static bool IsPawnTorsoDestroyed(Pawn pawn)
        {
            return pawn.TryGetComp<CompDeathRecorder>() != null && pawn.TryGetComp<CompDeathRecorder>().isTorsoDestroyed;
        }
    }
}
