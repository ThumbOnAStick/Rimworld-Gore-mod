using System;
using RimWorld;
using Verse;

namespace GoreUponDismemberment
{
    public class PawnRenderNodeWorker_Body_Gore : PawnRenderNodeWorker
    {
        private bool IsGoredHuman(Pawn pawn)
        {
            bool isXenoBanned = ModsConfig.BiotechActive && GUDUtil.IsXenoBannedFromGore(pawn.genes.Xenotype);
            bool hasRecorder = (pawn.Dead && pawn.HasComp<CompDeathRecorder>());
            if (hasRecorder)
            {
                bool hasDeathCause = pawn.GetComp<CompDeathRecorder>().cause > CompDeathRecorder.DeathCause.Alive;
                return hasDeathCause;
            }
            return false;
        }

        public override bool CanDrawNow(PawnRenderNode node, PawnDrawParms parms)
        {
            return !parms.pawn.IsDessicated() && this.IsGoredHuman(parms.pawn);
        }
    }
}
