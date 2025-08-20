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
            if (__instance?.hediffSet?.pawn != null && __instance.hediffSet.pawn.def == ThingDefOf.Human)
            {
                if (GoreUponDismembermentMod.settings.torsoExplosionChance >= Rand.Range(1, 100))
                {
                    Pawn pawn = __instance.hediffSet.pawn;
                    if (pawn.MapHeld != null)
                    {
                        List<Hediff> hediffs = __instance.hediffSet.hediffs;
                        if (hediffs != null && hediffs.Count >= 1 && hediffs.Any(TorsoCheck))
                        {
                            float severity = hediffs.Where(TorsoCheck).MaxBy((Hediff h) => h.tickAdded).Severity;
                            if (severity > (float)BodyPartDefOf.Torso.hitPoints)
                            {
                                // Create visual effects
                                GUDUtil.MakeGoreFleck(pawn);
                                // Record torso to be fully destroyed
                                __instance.hediffSet.pawn.TryGetComp<CompDeathRecorder>()?.SetTorsoDestroyed();
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

