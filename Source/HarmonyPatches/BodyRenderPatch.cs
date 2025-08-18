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
                    if (torsoPart != null)
                    {
                        // Calculate current health by subtracting damage from max health
                        float torsoMaxHP = torsoPart.def.hitPoints;
                        float totalDamage = 0f;

                        // Sum up all damage hediffs affecting the torso
                        foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
                        {
                            if (hediff.Part == torsoPart && hediff is Hediff_Injury injury)
                            {
                                totalDamage += injury.Severity;
                            }
                        }

                        float torsoCurrentHP = Mathf.Max(0f, torsoMaxHP - totalDamage);
                        float torsoHPPercentage = torsoCurrentHP / torsoMaxHP;

                        if (torsoHPPercentage < 0.01f)
                        {
                            // Split torso in half
                            __result = new Graphic_SplitWrapper(__result, pawn);
                        }
                    }
                }
            }
        }

    }
}