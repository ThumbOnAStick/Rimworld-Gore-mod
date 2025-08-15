using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Sound;

namespace GoreUponDismemberment.HarmonyPatches
{
    [HarmonyPatch(typeof(Hediff_MissingPart), "PostAdd")]
    public class MissingPartPatch
    {

        public static void PatchHarmony()
        {
            MethodInfo original = AccessTools.Method(typeof(Hediff_MissingPart), "PostAdd");
            HarmonyMethod prefix = new HarmonyMethod(typeof(MissingPartPatch).GetMethod("Prefix"));
            GoreHarmony.harmony.Patch(original, prefix, null);
        }


        private static bool IsHead(Hediff_MissingPart __instance)
        {
            return __instance.Part.def == BodyPartDefOf.Head || __instance.Part.def.defName == "Neck";
        }

        public static void Prefix(ref Hediff_MissingPart __instance, ref DamageInfo? dinfo)
        {
            Hediff_MissingPart hediff_MissingPart = __instance;
            bool flag = ((hediff_MissingPart != null) ? hediff_MissingPart.pawn : null) == null || __instance.pawn.Dead || dinfo == null || __instance.pawn.Map == null;
            if (!flag)
            {
                bool flag2 = __instance.pawn.def != ThingDefOf.Human || GoreUponDismembermentMod.settings.limbDropChance < Rand.Range(1, 100);
                if (!flag2)
                {
                    bool flag3 = IsHead(__instance);
                    if (flag3)
                    {
                        Graphic graphic = GUDUtil.GoredHeadGraphic(__instance.pawn, CompDeathRecorder.DeathCause.Shred, ShaderDatabase.CutoutSkin);
                        bool flag4 = graphic != null;
                        if (flag4)
                        {
                            GUDUtil.MakeFlyingHeadFor(__instance.pawn);
                        }
                    }
                    else
                    {
                        bool flag5 = __instance.Part.parts.Count > 0 && __instance.Part.def.IsSkinCovered(__instance.Part, __instance.pawn.health.hediffSet) && dinfo.Value.Def != DamageDefOf.SurgicalCut;
                        if (flag5)
                        {
                            bool allFilth = GoreUponDismembermentMod.settings.allFilth;
                            if (allFilth)
                            {
                                GUDUtil.MakeBrokenLimbFilth(__instance.pawn);
                            }
                            else
                            {
                                GUDUtil.MakeBrokenLimb(__instance.pawn);
                            }
                            GoreDefOf.GoreSound.PlayOneShot(SoundInfo.InMap(new TargetInfo(__instance.pawn.PositionHeld, __instance.pawn.MapHeld, false), MaintenanceType.None));
                        }
                    }
                }
            }
        }
    
    }
}
