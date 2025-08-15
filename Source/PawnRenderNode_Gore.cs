using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace GoreUponDismemberment
{
    public class PawnRenderNode_Gore : PawnRenderNode
    {
        private static readonly string ShreddedOverlayPathThin = "Things/Overlays/Shred/Thin";

        private static readonly string ShreddedOverlayPathMale = "Things/Overlays/Shred/Male";

        private static readonly string ShreddedOverlayPathFemale = "Things/Overlays/Shred/Female";

        private static readonly string CutOverlayPathMale = "Things/Overlays/Cut/Male";

        private static readonly string CutOverlayPathFemale = "Things/Overlays/Cut/Female";

        private static readonly string ShotOverlayPathFemale = "Things/Overlays/Gunshot/Female";

        private static readonly string ShotOverlayPathMale = "Things/Overlays/Gunshot/Male";

        public PawnRenderNode_Gore(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree)
            : base(pawn, props, tree)
        {

        }


        public override Graphic GraphicFor(Pawn pawn)
        {
            string bodyNakedGraphicPath = pawn.story.bodyType.bodyNakedGraphicPath;
            bool flag = bodyNakedGraphicPath == null || pawn.Drawer.renderer == null || pawn.Drawer.renderer.CurRotDrawMode == RotDrawMode.Dessicated || pawn.TryGetComp<CompDeathRecorder>() == null;
            Graphic graphic;
            if (flag)
            {
                graphic = null;
            }
            else
            {
                try
                {
                    string text = "";
                    switch (pawn.GetComp<CompDeathRecorder>().cause)
                    {
                        case CompDeathRecorder.DeathCause.Cut:
                            text = ((pawn.gender == Gender.Male) ? PawnRenderNode_Gore.CutOverlayPathMale : PawnRenderNode_Gore.CutOverlayPathFemale);
                            break;
                        case CompDeathRecorder.DeathCause.Shred:
                            {
                                bool isThin = pawn.story.bodyType == BodyTypeDefOf.Thin;
                                if (isThin)
                                {
                                    text = PawnRenderNode_Gore.ShreddedOverlayPathThin;
                                }
                                else
                                {
                                    text = ((pawn.gender == Gender.Male) ? PawnRenderNode_Gore.ShreddedOverlayPathMale : PawnRenderNode_Gore.ShreddedOverlayPathFemale);
                                }
                                break;
                            }
                        case CompDeathRecorder.DeathCause.Shot:
                            text = ((pawn.gender == Gender.Male) ? PawnRenderNode_Gore.ShotOverlayPathMale : PawnRenderNode_Gore.ShotOverlayPathFemale);
                            break;
                        default:
                            return null;
                    }
                    graphic = GraphicDatabase.Get<Graphic_Multi>(text, ShaderDatabase.CutoutSkinColorOverride, Vector2.one, pawn.DrawColor, Color.white, null, bodyNakedGraphicPath);
                }
                catch (Exception ex)
                {
                    Log.Error(string.Format("Gore Mod: Unable to render skin overlay for {0}.\n{1}", pawn.kindDef.label, ex));
                    graphic = null;
                }
            }
            return graphic;
        }

        // Token: 0x06000020 RID: 32 RVA: 0x00002C44 File Offset: 0x00000E44
        protected override void EnsureMeshesInitialized()
        {
            base.EnsureMeshesInitialized();
            this.primaryGraphic = this.GraphicFor(this.tree.pawn);
        }

        // Token: 0x06000021 RID: 33 RVA: 0x00002C65 File Offset: 0x00000E65
        protected override void EnsureMaterialVariantsInitialized(Graphic g)
        {
            base.EnsureMaterialVariantsInitialized(g);
            this.primaryGraphic = this.GraphicFor(this.tree.pawn);
        }

    }
}
