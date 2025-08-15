using LudeonTK;
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
using Verse.Sound;

namespace GoreUponDismemberment
{
    public partial class GUDUtil
    {
        private static readonly string headBurnedGraphicPath = "Things/Heads/Crisp/Crisp_Average_Skull";

        private static readonly string headCutGraphicPathFemale = "Things/Heads/Cut/Female_cut";

        private static readonly string headCutGraphicPathMale = "Things/Heads/Cut/Male_cut";

        private static readonly string headShrededGraphicPathMale = "Things/Heads/Shred/Male_boom";

        private static readonly string headShrededGraphicPathFemale = "Things/Heads/Shred/Female_boom";

        private static readonly string headShotPathMale = "Things/Heads/Shot/Male_shot";

        private static readonly string headShotPathFemale = "Things/Heads/Shot/Female_shot";

        public static Graphic FlyingHeadGraphic(Gender gender, Color skinColor, CompDeathRecorder.DeathCause cause, Shader shader, Pawn pawn = null)
        {
            bool flag = pawn != null && pawn.Drawer.renderer.renderTree.HeadGraphic != null;
            Graphic graphic;
            if (flag)
            {
                graphic = pawn.story.headType.GetGraphic(pawn, pawn.story.SkinColor);
            }
            else
            {
                string text;
                switch (cause)
                {
                    case CompDeathRecorder.DeathCause.Cut:
                        text = ((gender == Gender.Male) ? GUDUtil.headCutGraphicPathMale : GUDUtil.headCutGraphicPathFemale);
                        break;
                    case CompDeathRecorder.DeathCause.Flame:
                        text = GUDUtil.headBurnedGraphicPath;
                        break;
                    case CompDeathRecorder.DeathCause.Shred:
                        text = ((gender == Gender.Male) ? GUDUtil.headShrededGraphicPathMale : GUDUtil.headShrededGraphicPathFemale);
                        break;
                    case CompDeathRecorder.DeathCause.Shot:
                        text = ((gender == Gender.Male) ? GUDUtil.headShotPathMale : GUDUtil.headShotPathFemale);
                        break;
                    default:
                        return null;
                }
                graphic = GraphicDatabase.Get<Graphic_Multi>(text, shader, Vector2.one, skinColor);
            }
            return graphic;
        }

        public static Graphic GoredHeadGraphic(Pawn pawn, CompDeathRecorder.DeathCause cause, Shader shader)
        {
            bool flag = pawn.Drawer.renderer.CurRotDrawMode == RotDrawMode.Dessicated;
            string text;
            switch (cause)
            {
                case CompDeathRecorder.DeathCause.Cut:
                    {
                        bool flag2 = flag;
                        if (flag2)
                        {
                            return null;
                        }
                        text = ((pawn.gender == Gender.Male) ? GUDUtil.headCutGraphicPathMale : GUDUtil.headCutGraphicPathFemale);
                        break;
                    }
                case CompDeathRecorder.DeathCause.Flame:
                    text = GUDUtil.headBurnedGraphicPath;
                    break;
                case CompDeathRecorder.DeathCause.Shred:
                    {
                        bool flag3 = flag;
                        if (flag3)
                        {
                            return null;
                        }
                        text = ((pawn.gender == Gender.Male) ? GUDUtil.headShrededGraphicPathMale : GUDUtil.headShrededGraphicPathFemale);
                        break;
                    }
                case CompDeathRecorder.DeathCause.Shot:
                    {
                        bool flag4 = flag;
                        if (flag4)
                        {
                            return null;
                        }
                        text = ((pawn.gender == Gender.Male) ? GUDUtil.headShotPathMale : GUDUtil.headShotPathFemale);
                        break;
                    }
                default:
                    return null;
            }
            return GraphicDatabase.Get<Graphic_Multi>(text, shader, Vector2.one, pawn.story.SkinColor);
        }
    }
}
