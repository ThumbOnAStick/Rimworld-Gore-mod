using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Profiling;
using Verse;

namespace GoreUponDismemberment
{
    public static class GUDUtil
    {
        public static Graphic GoredHeadGraphic(Gender gender, Color skinColor, CompDeathRecorder.DeathCause cause, Shader shader)
        {

            string path;

            switch (cause)
            {
                case CompDeathRecorder.DeathCause.Flame:
                    path = headBurnedGraphicPath;
                    break;
                case CompDeathRecorder.DeathCause.Cut:

                    path = gender == Gender.Male ? headCutGraphicPathMale : headCutGraphicPathFemale;
                    break;
                case CompDeathRecorder.DeathCause.Shred:

                    path = gender == Gender.Male ? headShrededGraphicPathMale : headShrededGraphicPathFemale;
                    break;

                case CompDeathRecorder.DeathCause.Shot:
                    path = gender == Gender.Male ? headShotPathMale : headShotPathFemale;
                    break;
                default:
                    return null;
            }
            return GraphicDatabase.Get<Graphic_Multi>(path, shader, Vector2.one, skinColor);
        }
        public static Graphic GoredHeadGraphic(Pawn pawn, CompDeathRecorder.DeathCause cause, Shader shader)
        {

            string path;
            bool dessicated = pawn.Drawer.renderer.CurRotDrawMode == RotDrawMode.Dessicated;
            switch (cause)
            {
                case CompDeathRecorder.DeathCause.Flame:
                    path = headBurnedGraphicPath;
                    break;
                case CompDeathRecorder.DeathCause.Cut:
                    if (dessicated)
                    {
                        return null;
                    }
                    path = pawn.gender == Gender.Male ? headCutGraphicPathMale : headCutGraphicPathFemale;
                    break;
                case CompDeathRecorder.DeathCause.Shred:
                    if (dessicated)
                    {
                        return null;
                    }
                    path = pawn.gender == Gender.Male ? headShrededGraphicPathMale : headShrededGraphicPathFemale;
                    break;

                case CompDeathRecorder.DeathCause.Shot:
                    if (dessicated)
                    {
                        return null;
                    }
                    path = pawn.gender == Gender.Male ? headShotPathMale : headShotPathFemale;
                    break;
                default:
                    return null;
            }
            return GraphicDatabase.Get<Graphic_Multi>(path, shader, Vector2.one, pawn.story.SkinColor);
        }
        public static void TryToSetLimbColor(this DismemberedLimb limb,Pawn pawn)
        {
            limb.pawnSkinColor = pawn.story.SkinColor;
        }
        public static void TryToMakeGibFleck(IntVec3 cell, Map map, float scale)
        {
            TryToMakeGibFleck(cell.ToVector3() + new Vector3(Rand.Value, 0f, Rand.Value), map, scale);
        }
        static void TryToMakeGibFleck(Vector3 loc, Map map, float scale)
        {
            if (!loc.ShouldSpawnMotesAt(map, true))
            {
                return;
            }
            FleckCreationData dataStatic = FleckMaker.GetDataStatic(loc, map, GoreDef.FlyingGibs, 1.9f * scale);
            dataStatic.rotationRate = (float)Rand.Range(-60, 60);
            dataStatic.velocityAngle = (float)Rand.Range(0, 360);
            dataStatic.velocitySpeed = Rand.Range(0.6f, 0.75f);
            map.flecks.CreateFleck(dataStatic);
        }

        static readonly string headBurnedGraphicPath = "Things/Heads/Crisp/Crisp_Average_Skull";
        static readonly string headCutGraphicPathFemale = "Things/Heads/Cut/Female_cut";
        static readonly string headCutGraphicPathMale = "Things/Heads/Cut/Male_cut";
        static readonly string headShrededGraphicPathMale = "Things/Heads/Shred/Male_boom";
        static readonly string headShrededGraphicPathFemale = "Things/Heads/Shred/Female_boom";
        static readonly string headShotPathMale = "Things/Heads/Shot/Male_shot";
        static readonly string headShotPathFemale = "Things/Heads/Shot/Female_shot";
    }
}
