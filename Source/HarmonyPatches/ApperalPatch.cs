using GoreUponDismemberment.ApperalTearingSystem;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace GoreUponDismemberment.HarmonyPatches
{
    public class ApperalPatch
    {
        public static void PatchHarmony()
        {
            MethodInfo original = AccessTools.Method(typeof(ApparelGraphicRecordGetter), "TryGetGraphicApparel");
            HarmonyMethod postfix = new HarmonyMethod(typeof(ApperalPatch).GetMethod("Postfix"));
            GoreHarmony.harmony.Patch(original, null, postfix);
        }

     
        public static void Postfix(Apparel apparel, BodyTypeDef bodyType, ref bool __result, ref ApparelGraphicRecord rec)
        {
            if (!__result) return;
            if (apparel.def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.Torso))
            {
                rec.graphic = new Graphic_SplitWrapper(rec.graphic, apparel);
            }
        }
    }
}
