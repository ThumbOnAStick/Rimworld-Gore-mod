using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace GoreUponDismemberment.HarmonyPatches
{
    // Token: 0x02000010 RID: 16
    public class PawnKillPatch
    {

        public static void PatchHarmony()
        {
            MethodInfo original = AccessTools.Method(typeof(Pawn), "Kill");
            HarmonyMethod postfix = new HarmonyMethod(typeof(PawnKillPatch).GetMethod("Postfix"));
            GoreHarmony.harmony.Patch(original, null, postfix);
        }

        static void EvaluateDeathCause(CompDeathRecorder compDeathRecorder, Hediff hediff, ref Pawn __instance)
        {
            string defName = hediff.def.defName;
            switch (defName)
            {
                case "Burn":
                    compDeathRecorder.cause = CompDeathRecorder.DeathCause.Flame;
                    __instance.Drawer.renderer.EnsureGraphicsInitialized();
                    return;
                case "Cut":
                case "Scratch":
                case "Stab":
                    compDeathRecorder.cause = CompDeathRecorder.DeathCause.Cut;
                    __instance.Drawer.renderer.EnsureGraphicsInitialized();
                    return;
                case "Shredded":
                case "Crack":
                    compDeathRecorder.cause = CompDeathRecorder.DeathCause.Shred;
                    __instance.Drawer.renderer.EnsureGraphicsInitialized();
                    return;
                case "Gunshot":
                    compDeathRecorder.cause = CompDeathRecorder.DeathCause.Shot;
                    __instance.Drawer.renderer.EnsureGraphicsInitialized();
                    return;
            }
        }


        public static void Postfix(ref Pawn __instance, object[] __args)
        {
            if (__instance == null || __instance.MapHeld == null)
                return;

            CompDeathRecorder compDeathRecorder = __instance.TryGetComp<CompDeathRecorder>();
            if (compDeathRecorder == null)
                return;

            if (__instance.health == null || __instance.health.hediffSet == null)
                return;

            if (__instance.Drawer == null || __instance.Drawer.renderer == null)
                return;

            List<Hediff> hediffs = __instance.health.hediffSet.hediffs;
            if (hediffs == null || hediffs.Count < 1)
                return;

            var enumerable = hediffs.Where(x => x.Part != null);
            if (!enumerable.Any())
                return;
            List<Hediff> list = enumerable.OrderBy(x => x.tickAdded).ToList();
            Hediff hediff = list.Last();
            if (hediff.def == null || enumerable.Count() < 2)
                return;

            if (hediff.def == HediffDefOf.MissingBodyPart)
            {
                hediff = list[list.Count - 2];
            }
            EvaluateDeathCause(compDeathRecorder, hediff, ref __instance);
        }
    }
}
