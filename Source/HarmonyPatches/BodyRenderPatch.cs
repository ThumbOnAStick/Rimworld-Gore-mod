using GoreUponDismemberment.SpriteSplitingSystem;
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

    public class BodyRenderPatch
    {
        private static readonly string bodyBurnedGraphicPath = "Things/Bodies/Crisp/Crisp_Male";

        private static readonly string bodyBurnedGraphicPathThin = "Things/Bodies/Crisp/Crisp_Thin";

        public static void PatchHarmony()
        {
            MethodInfo original = AccessTools.Method(typeof(PawnRenderNode_Body), "GraphicFor");
            HarmonyMethod postfix = new HarmonyMethod(typeof(BodyRenderPatch).GetMethod("Postfix"));
            GoreHarmony.harmony.Patch(original, null, postfix);

            MethodInfo original1 = AccessTools.Method(typeof(PawnRenderNode_AnimalPart), "GraphicFor");
            HarmonyMethod postfix1 = new HarmonyMethod(typeof(BodyRenderPatch).GetMethod("Postfix1"));
            GoreHarmony.harmony.Patch(original1, null, postfix1);
 
        }



        public static void Postfix(object[] __args, ref Graphic __result, PawnRenderNode_Body __instance)
        {
            if (__args[0] != null)
            {
                Pawn pawn = __args[0] as Pawn;
                if (pawn != null && pawn.Dead)
                {
                    // Check if pawn is burned
                    CompDeathRecorder compDeathRecorder = pawn.TryGetComp<CompDeathRecorder>();
                    if (compDeathRecorder != null)
                    {
                        if (compDeathRecorder.cause == CompDeathRecorder.DeathCause.Flame)
                        {
                            Shader shader = __instance.ShaderFor(pawn);
                            string text = ((pawn.story.bodyType == BodyTypeDefOf.Thin) ? BodyRenderPatch.bodyBurnedGraphicPathThin : BodyRenderPatch.bodyBurnedGraphicPath);
                            __result = GraphicDatabase.Get<Graphic_Multi>(text, shader);
                        }
                    }

                    //Check torso hp
                    BodyPartRecord torsoPart = pawn.health.hediffSet.GetNotMissingParts().FirstOrDefault(part => part.def == BodyPartDefOf.Torso);
                    if (torsoPart != null && DeathRecorderHelper.IsPawnTorsoDestroyed(pawn) && GoreUponDismembermentMod.settings.enableTorsoSplit) // When pawn has death recorder
                    {
                        __result = new Graphic_SplitWrapper(__result, pawn, false, true);
                        return;
                    }



                }
            }
        }


        public static void Postfix1(object[] __args, ref Graphic __result)
        {
            if (!GoreUponDismembermentMod.settings.enableTorsoSplit)
            {
                return;
            }
            if (__args[0] != null)
            {
                Pawn pawn = __args[0] as Pawn;
                if (pawn != null && pawn.Dead)
                {
                    BodyPartRecord torsoPart = pawn.health.hediffSet.GetBodyPartRecord(DefDatabase<BodyPartDef>.GetNamed("Body")); 
                    if (torsoPart != null && GUDUtil.IsBodyPartDestroyed(torsoPart, pawn)) // When pawn is not human
                    {
                        __result = new Graphic_SplitWrapper(__result, pawn, true, true);
                    }
                }
            }
        }
    }
}