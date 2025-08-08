using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse.Sound;
using Verse;
using HarmonyLib;
using UnityEngine;
using System.Reflection;
using System.Runtime.CompilerServices;
using Verse.AI;
using UnityEngine.Profiling;
using System.Diagnostics;

namespace GoreUponDismemberment.HarmonyPatches
{
    [HarmonyPatch(typeof(Hediff_MissingPart), "PostAdd")]
    public class SpreadGore
    {
        [HarmonyPostfix]
        public static void Postfix(ref Hediff_MissingPart __instance, ref DamageInfo? dinfo)
        {
            if (__instance == null)
            {
                return;
            }
            Pawn pawn = __instance.pawn;
            if (pawn == null)
            {
                return;
            }
            if (pawn.Dead)
            {
                return;
            }
            if (dinfo == null)
            {
                return;
            }

            bool flag3 = pawn.Map == null;
            if (!flag3)
            {
                bool isHuman = pawn.def == ThingDefOf.Human;
                if (!isHuman)
                {
                    return;
                }
                bool flag5 = __instance.Part.LabelShort == "head" || __instance.Part.LabelShort == "neck";
                if (flag5)
                {
                    IntRange range = new IntRange(3, 5);
                    int rndDist = range.RandomInRange;
                    LocalTargetInfo rndtargetInfo;
                    if (CellFinder.TryFindRandomCellNear(pawn.Position, pawn.Map, rndDist, x => (x - pawn.Position).LengthHorizontalSquared >= 3, out IntVec3 targetLocation))
                    {
                        rndtargetInfo = new LocalTargetInfo(targetLocation);
                    }
                    else
                    {
                        return;
                    }

                    Graphic head;
                    if (pawn != null)
                    {
                        head = GUDUtil.GoredHeadGraphic(pawn, CompDeathRecorder.DeathCause.Shred, ShaderDatabase.CutoutSkin);
                        if (head == null)
                        {
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }

                    Projectile_FlyingHead projectile = (Projectile_FlyingHead)GenSpawn.Spawn(GoreDef.FlyingHead, pawn.Position, pawn.Map, WipeMode.Vanish);

                    projectile.gender = pawn.gender;
                    projectile.skinColor = pawn.story.SkinColor;
                    projectile.headMap = pawn.Map;
                    projectile.Launch(pawn, rndtargetInfo, rndtargetInfo, ProjectileHitFlags.None);
                }
                else
                {
                    bool noChildParts = __instance.Part.parts.Count < 1;
                    if (!noChildParts)
                    {
                        bool skinCovered = !__instance.Part.def.IsSkinCovered(__instance.Part, pawn.health.hediffSet);
                        DamageInfo dinfo2 = (DamageInfo)dinfo;

                        if (!skinCovered && dinfo2.Def != DamageDefOf.SurgicalCut)
                        {
                            bool allfilth = !GoreUponDismembermentMod.settings.AllFilth;
                            if (allfilth)
                            {
                                Thing thing2 = ThingMaker.MakeThing(GoreDef.Gore, null);
                                thing2.stackCount = 1;
                                GenPlace.TryPlaceThing(thing2, __instance.pawn.Position, __instance.pawn.Map, ThingPlaceMode.Near, null, null, default(Rot4));
                                thing2.SetForbidden(GoreUponDismembermentMod.settings.isForbidden, true);
                            }
                            else
                            {

                                Thing thing3 = ThingMaker.MakeThing(GoreDef.GoreFilth, null);
                                FilthMaker.TryMakeFilth(pawn.PositionHeld, pawn.Map, thing3.def, pawn.LabelIndefinite(), 1, FilthSourceFlags.None);

                            }
                            //Play break sound

                            SoundDef goreSound = GoreDef.GoreSound;
                            Pawn pawn2 = __instance.pawn;
                            SoundInfo info = SoundInfo.InMap(new TargetInfo(pawn2.PositionHeld, pawn2.MapHeld, false), MaintenanceType.None);
                            goreSound.PlayOneShot(info);

                        }
                    }
                }

            }


        }
    }

    [HarmonyPatch(typeof(Pawn_HealthTracker), "SetDead")]
    public class TorsoExplode
    {
        public static void Postfix(ref Pawn_HealthTracker __instance)
        {
            if (__instance == null)
            {
                return;
            }
            if (__instance.hediffSet == null)
            {
                return;
            }
            Pawn pawn = __instance.hediffSet.pawn;
            if (pawn == null)
            {
                return;
            }
            if (pawn.MapHeld == null)
            {
                return;
            }
            var hediffs = __instance.hediffSet.hediffs;
            if (hediffs == null || hediffs.Count < 1)
            {
                return;
            }
            float sum = 0;
            foreach (Hediff hediff in hediffs)
            {
                bool isTorsoDamage = hediff.Part != null && hediff.Part.LabelShort == "torso";
                if (isTorsoDamage)
                {
                    sum += hediff.Severity;
                    if (sum > hediff.Part.def.hitPoints)
                    {
                        Thing thing = ThingMaker.MakeThing(GoreDef.GutsFilth, null);
                        FilthMaker.TryMakeFilth(pawn.PositionHeld, pawn.Map, thing.def, pawn.LabelIndefinite(), 1, FilthSourceFlags.None);
                        break;
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(Pawn), nameof(Pawn.Kill))]
    public class PawnKillPatch
    {
        [HarmonyPostfix]
        static void Postfix(ref Pawn __instance, object[] __args)
        {
            if (__instance == null)
            {
                return;
            }
            if (__instance.MapHeld== null)
            {
                return;
            }
            CompDeathRecorder recorder = __instance.TryGetComp<CompDeathRecorder>();
            if (recorder == null)
            {
                return;
            }
            if (__instance.health == null)
            {
                return;
            }
            if (__instance.health.hediffSet == null)
            {
                return;
            }
            if (__instance.Drawer == null)
            {
                return;
            }
            if (__instance.Drawer.renderer==null)
            {
                return;
            }

            var hediffs = __instance.health.hediffSet.hediffs;
            if (hediffs == null || hediffs.Count < 1)
            {
                return;
            }
            if (!hediffs.Any(x => x.Part != null))
            {
                return;
            }
            var wounds = hediffs.Where(x => x.Part != null);
            var orderdWounds = wounds.OrderBy(x => x.tickAdded).ToList();
            var last_hediff = orderdWounds.Last();
            if (last_hediff.def == null || wounds.Count() < 2)
            {
                return;
            }
            if (last_hediff.def == HediffDefOf.MissingBodyPart)
            {
                last_hediff = orderdWounds[orderdWounds.Count() - 2];
            }
            string defName = last_hediff.def.defName;
            if (defName == "Burn")
            {
                recorder.cause = CompDeathRecorder.DeathCause.Flame;
                __instance.Drawer.renderer.EnsureGraphicsInitialized();

            }
            else if (defName == "Cut" || defName == "Scratch" || defName == "Stab")
            {
                recorder.cause = CompDeathRecorder.DeathCause.Cut;
                __instance.Drawer.renderer.EnsureGraphicsInitialized();
            }
            else if (defName == "Shredded" || defName == "Crack")
            {
                recorder.cause = CompDeathRecorder.DeathCause.Shred;
                __instance.Drawer.renderer.EnsureGraphicsInitialized();
            }
            else if (defName == "Gunshot")
            {
                recorder.cause = CompDeathRecorder.DeathCause.Shot;
                __instance.Drawer.renderer.EnsureGraphicsInitialized();
            }
        }
    }

    [HarmonyPatch(typeof(PawnRenderNode_Body), nameof(PawnRenderNode_Body.GraphicFor))]
    public class BodyRenderPatch
    {
        static void Postfix(object[] __args, ref Graphic __result, PawnRenderNode_Body __instance)
        {
            if (__args[0] == null)
            {
                return;
            }
            Pawn pawn = __args[0] as Pawn;
            if (pawn == null||!pawn.Dead)
            {
                return;
            }
            CompDeathRecorder recorder = pawn.TryGetComp<CompDeathRecorder>();
            if (recorder == null)
            {
                return;
            }
            if (recorder.cause == CompDeathRecorder.DeathCause.Flame)
            {
                Shader shader = __instance.ShaderFor(pawn);
                string path = pawn.story.bodyType == BodyTypeDefOf.Thin ? bodyBurnedGraphicPathThin : bodyBurnedGraphicPath;
                __result = GraphicDatabase.Get<Graphic_Multi>(path, shader);
            }
        }

        static readonly string bodyBurnedGraphicPath = "Things/Bodies/Crisp/Crisp_Male";
        static readonly string bodyBurnedGraphicPathThin = "Things/Bodies/Crisp/Crisp_Thin";

    }

    [HarmonyPatch(typeof(PawnRenderNode_Head), nameof(PawnRenderNode_Head.GraphicFor))]
    public class HeadRenderPatch
    {
        [HarmonyPostfix]
        static void Postfix(object[] __args, ref Graphic __result, PawnRenderNode_Head __instance)
        {
            if (__args[0] == null)
            {
                return;
            }
            Pawn pawn = __args[0] as Pawn;
            if (pawn==null||!pawn.Dead)
            {
                return;
            }
            CompDeathRecorder recorder = pawn.TryGetComp<CompDeathRecorder>();
            if (recorder == null)
            {
                return;
            }
            var potentialResult = GUDUtil.GoredHeadGraphic(pawn, recorder.cause, __instance.ShaderFor(pawn));
            if (potentialResult != null)
            {
                __result = potentialResult;
            }
        }
    }

    [HarmonyPatch(typeof(Pawn), nameof(Pawn.MakeCorpse), new Type[] { typeof(Building_Grave), typeof(Building_Bed) })]
    public class MakeCorpsePatch
    {
        static void Postfix(Pawn __instance, Corpse __result)
        {
            if (__instance == null)
            {
                return;
            }
            var recorder = __instance.TryGetComp<CompDeathRecorder>();
            if (recorder == null)
            {
                return;
            }
            if (__instance.health == null)
            {
                return;
            }
            if (__instance.health.hediffSet == null)
            {
                return;
            }
            var hediffs = __instance.health.hediffSet.hediffs;
            if (hediffs == null || hediffs.Count < 1)
            {
                return;
            }
            if (!hediffs.Any(x => x.Part != null))
            {
                return;
            }
            var wounds = hediffs.Where(x => x.Part != null);
            if (wounds.Count() < 1)
            {
                return;
            }
            var orderdWounds = wounds.OrderBy(x => x.tickAdded).ToList();
            var last_hediff = orderdWounds.Last();
            if (last_hediff.def == null || wounds.Count() < 2)
            {
                return;
            }
            if (last_hediff.def == HediffDefOf.MissingBodyPart)
            {
                last_hediff = orderdWounds[orderdWounds.Count() - 2];
            }
            if (last_hediff.def.defName == "Burn")
            {
                recorder.cause = CompDeathRecorder.DeathCause.Flame;
                if (__result.HasComp<CompRottable>())
                {
                    __result.GetComp<CompRottable>().RotImmediately(RotStage.Dessicated);
                }
            }
        }
    }

    [HarmonyPatch]
    static class FacialAnimationPatch3
    {
        [HarmonyPrepare]
        static bool Prepare()
        {
            return TargetMethod() != null;
        }

        static MethodBase TargetMethod()
        {
            Type type = AccessTools.TypeByName(facialAnimationPatch);
            if (type == null)
            {
                return null;
            }
            var type1 = type.GetNestedTypes().First(x => x.Name == "NLFacialAnimationPawnRenderNode");
            bool hasType = type1 != null;
            if (hasType)
            {
                Log.Message("Gore Upon Dismemeberment: Found facial render node!");
                NLFacialAnimationPawnRenderNode = type1;
                var method = AccessTools.GetDeclaredMethods(type1).LastOrDefault((MethodInfo x) => x.Name.Contains("GraphicFor"));
                return method;
            }
            else
            {
                Log.Message("Gore Upon Dismemeberment: Could not find facial animation render node");
                return null;
            }


        }

        static void Postfix(object[] __args, PawnRenderNode __instance, ref Graphic __result, object ___info)
        {
            GraphicInfo info = ___info as GraphicInfo;
            if (!info.isBottom)
            {
                return;
            }
            var pawn = (Pawn)__args[0];
            if (!pawn.Dead)
            {
                return;
            }
            if (!pawn.HasComp<CompDeathRecorder>())
            {
                return;
            }
            var recorder = pawn.GetComp<CompDeathRecorder>();
            if (recorder.cause == CompDeathRecorder.DeathCause.Alive)
            {
                return;
            }
            var potentialResult = GUDUtil.GoredHeadGraphic(pawn, recorder.cause,ShaderDatabase.CutoutSkin);
            if (potentialResult != null)
                __result = potentialResult;
        }

        static readonly string facialAnimationPatch = "FacialAnimation.DrawFaceGraphicsComp";
        static Type NLFacialAnimationPawnRenderNode;
    }
}
