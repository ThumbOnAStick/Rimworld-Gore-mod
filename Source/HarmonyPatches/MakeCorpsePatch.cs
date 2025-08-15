using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace GoreUponDismemberment.HarmonyPatches
{
    // Token: 0x02000013 RID: 19
    public class MakeCorpsePatch
    {

        public static void PatchHarmony()
        {
            MethodInfo original = AccessTools.Method(typeof(Pawn), "MakeCorpse", new Type[]
    {
        typeof(Building_Grave),
        typeof(Building_Bed)
    });
            HarmonyMethod postfix = new HarmonyMethod(typeof(MakeCorpsePatch).GetMethod("Postfix"));
            GoreHarmony.harmony.Patch(original, null, postfix);
        }

        private static bool ValidateRecorder(CompDeathRecorder compDeathRecorder, Pawn pawn)
        {
            return compDeathRecorder != null && pawn.health != null && pawn.health.hediffSet != null;
        }


        public static void Postfix(Pawn __instance, Corpse __result)
        {
            if (__instance == null)
                return;

            CompDeathRecorder compDeathRecorder = __instance.TryGetComp<CompDeathRecorder>();
            if (!ValidateRecorder(compDeathRecorder, __instance))
                return;

            List<Hediff> hediffs = __instance.health.hediffSet.hediffs;
            if (hediffs == null || hediffs.Count == 0)
                return;

            var enumerable = hediffs.Where(x => x.Part != null);
            if (!enumerable.Any())
                return;

            List<Hediff> list = enumerable.OrderBy(x => x.tickAdded).ToList();
            Hediff hediff = list.Last();

            if (hediff.def == null || enumerable.Count() <= 1)
                return;

            if (hediff.def == HediffDefOf.MissingBodyPart)
            {
                hediff = list[list.Count - 2];
            }

            if (hediff.def.defName == "Burn")
            {
                compDeathRecorder.cause = CompDeathRecorder.DeathCause.Flame;
                if (__result.HasComp<CompRottable>())
                {
                    __result.GetComp<CompRottable>()?.RotImmediately(RotStage.Dessicated);
                }
            }
        }
    }
}
