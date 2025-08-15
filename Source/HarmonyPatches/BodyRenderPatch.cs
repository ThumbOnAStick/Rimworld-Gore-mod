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
        }

        public static void Postfix(object[] __args, ref Graphic __result, PawnRenderNode_Body __instance)
        {
            bool flag = __args[0] == null;
            if (!flag)
            {
                Pawn pawn = __args[0] as Pawn;
                bool flag2 = pawn == null || !pawn.Dead;
                if (!flag2)
                {
                    CompDeathRecorder compDeathRecorder = pawn.TryGetComp<CompDeathRecorder>();
                    bool flag3 = compDeathRecorder == null;
                    if (!flag3)
                    {
                        bool flag4 = compDeathRecorder.cause == CompDeathRecorder.DeathCause.Flame;
                        if (flag4)
                        {
                            Shader shader = __instance.ShaderFor(pawn);
                            string text = ((pawn.story.bodyType == BodyTypeDefOf.Thin) ? BodyRenderPatch.bodyBurnedGraphicPathThin : BodyRenderPatch.bodyBurnedGraphicPath);
                            __result = GraphicDatabase.Get<Graphic_Multi>(text, shader);
                        }
                    }
                }
            }
        }

    }
}