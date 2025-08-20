using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace GoreUponDismemberment.HarmonyPatches
{
    public class HeadRenderPatch
    {
        public static void PatchHarmony()
        {
            MethodInfo original = AccessTools.Method(typeof(PawnRenderNode_Head), "GraphicFor");
            HarmonyMethod postfix = new HarmonyMethod(typeof(HeadRenderPatch).GetMethod("Postfix"));
            GoreHarmony.harmony.Patch(original, null, postfix);
        }

        public static void Postfix(object[] __args, ref Graphic __result, PawnRenderNode_Head __instance)
        {
            bool flag = __args[0] == null;
            if (!flag)
            {
                Pawn pawn = __args[0] as Pawn;
                bool flag2 = pawn == null || !pawn.Dead;
                if (!flag2)
                {
                    bool flag3 = GUDUtil.IsXenoBannedFromGore(pawn.genes.Xenotype);
                    if (!flag3)
                    {
                        CompDeathRecorder compDeathRecorder = pawn.TryGetComp<CompDeathRecorder>();
                        bool flag4 = compDeathRecorder == null;
                        if (!flag4)
                        {
                            Graphic graphic = GUDUtil.GoredHeadGraphic(pawn, compDeathRecorder.cause, __instance.ShaderFor(pawn), __instance.ColorFor(pawn));
                            bool flag5 = graphic != null;
                            if (flag5)
                            {
                                __result = graphic;
                            }
                        }
                    }
                }
            }
        }
    }
}

