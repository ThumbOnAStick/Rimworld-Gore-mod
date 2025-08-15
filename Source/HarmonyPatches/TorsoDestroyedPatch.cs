using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace GoreUponDismemberment.HarmonyPatches
{
    class TorsoDestroyedPatch
    {
        public static void PatchHarmony()
        {
            MethodInfo original = AccessTools.Method(typeof(Pawn_HealthTracker), "SetDead");
            HarmonyMethod postfix = new HarmonyMethod(typeof(TorsoDestroyedPatch).GetMethod("Postfix"));
            GoreHarmony.harmony.Patch(original, null, postfix);
        }

        public static void Postfix(ref Pawn_HealthTracker __instance)
        {
            Pawn_HealthTracker pawn_HealthTracker = __instance;
            bool flag;
            if (pawn_HealthTracker == null)
            {
                flag = null != null;
            }
            else
            {
                HediffSet hediffSet = pawn_HealthTracker.hediffSet;
                flag = ((hediffSet != null) ? hediffSet.pawn : null) != null;
            }
            bool flag2 = !flag || __instance.hediffSet.pawn.def != ThingDefOf.Human;
            if (!flag2)
            {
                bool flag3 = GoreUponDismembermentMod.settings.torsoExplosionChance < Rand.Range(1, 100);
                if (!flag3)
                {
                    Pawn pawn = __instance.hediffSet.pawn;
                    bool flag4 = pawn.MapHeld == null;
                    if (!flag4)
                    {
                        List<Hediff> hediffs = __instance.hediffSet.hediffs;
                        bool flag5 = hediffs == null || hediffs.Count < 1 || !hediffs.Any(new Predicate<Hediff>(TorsoCheck));
                        if (!flag5)
                        {
                            float severity = hediffs.Where(new Func<Hediff, bool>(TorsoCheck)).MaxBy((Hediff h) => h.tickAdded).Severity;
                            bool flag6 = severity > (float)BodyPartDefOf.Torso.hitPoints;
                            if (flag6)
                            {
                                GUDUtil.MakeGoreFleck(pawn);
                            }
                        }
                    }
                }
            }
        }
        internal static bool TorsoCheck(Hediff h)
        {
            BodyPartRecord part = h.Part;
            return ((part != null) ? part.def : null) == BodyPartDefOf.Torso;
        }


    }

}

