using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace GoreUponDismemberment
{
    // Token: 0x0200000B RID: 11
    public class Projectile_FlyingHead : Projectile
    {
        // Token: 0x06000026 RID: 38 RVA: 0x00002D70 File Offset: 0x00000F70
        private bool HasBrainPartsForXenoType(Pawn pawn)
        {
            bool flag = pawn == null;
            bool flag2;
            if (flag)
            {
                flag2 = false;
            }
            else
            {
                bool flag3 = pawn.genes == null;
                if (flag3)
                {
                    flag2 = false;
                }
                else
                {
                    bool flag4 = pawn.genes.Xenotype == null;
                    if (flag4)
                    {
                        flag2 = false;
                    }
                    else
                    {
                        bool flag5 = !XenoGoreDictionary.dict.ContainsKey(pawn.genes.Xenotype);
                        flag2 = !flag5;
                    }
                }
            }
            return flag2;
        }

        // Token: 0x06000027 RID: 39 RVA: 0x00002DDC File Offset: 0x00000FDC
        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            base.Impact(hitThing, blockedByShield);
            bool flag = this.pawn != null && this.HasBrainPartsForXenoType(this.pawn);
            if (flag)
            {
                ThingDef brainPartsFilth = XenoGoreDictionary.dict[this.pawn.genes.Xenotype].brainPartsFilth;
                FilthMaker.TryMakeFilth(base.Position, this.headMap, brainPartsFilth, "", 1, FilthSourceFlags.None);
            }
            else
            {
                FilthMaker.TryMakeFilth(base.Position, this.headMap, GoreDefOf.BrainPieces, "", 1, FilthSourceFlags.None);
            }
        }

        // Token: 0x06000028 RID: 40 RVA: 0x00002E6C File Offset: 0x0000106C
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look<Map>(ref this.headMap, "headMap", false);
            Scribe_References.Look<Pawn>(ref this.pawn, "pawn", false);
            Scribe_Values.Look<Gender>(ref this.gender, "gender", Gender.None, false);
            Scribe_Values.Look<Color>(ref this.skinColor, "skinColor", default(Color), false);
        }

        // Token: 0x06000029 RID: 41 RVA: 0x00002ED4 File Offset: 0x000010D4
        protected override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            base.DrawAt(drawLoc, flip);
            bool flag = this.pawn == null;
            if (!flag)
            {
                Quaternion exactRotation = this.ExactRotation;
                bool flag2 = this.pawn.story.hairDef != HairDefOf.Bald;
                if (flag2)
                {
                    string text = this.pawn.story.hairDef.texPath + "_south";
                    Graphic graphic = GraphicDatabase.Get<Graphic_Single>(text, ShaderDatabase.CutoutHair, Vector2.one, this.pawn.story.HairColor);
                    graphic.Draw(drawLoc, base.Rotation, this, exactRotation.eulerAngles.y);
                }
                bool flag3 = this.pawn.style.beardDef != BeardDefOf.NoBeard;
                if (flag3)
                {
                    string text2 = this.pawn.style.beardDef.texPath + "_south";
                    Graphic graphic2 = GraphicDatabase.Get<Graphic_Single>(text2, ShaderDatabase.CutoutHair, Vector2.one, this.pawn.story.HairColor);
                    graphic2.Draw(drawLoc, base.Rotation, this, exactRotation.eulerAngles.y);
                }
            }
        }

        // Token: 0x17000003 RID: 3
        // (get) Token: 0x0600002A RID: 42 RVA: 0x00003005 File Offset: 0x00001205
        public override Graphic Graphic
        {
            get
            {
                return GUDUtil.FlyingHeadGraphic(this.gender, this.skinColor, CompDeathRecorder.DeathCause.Shred, ShaderDatabase.CutoutSkin, this.pawn);
            }
        }

        // Token: 0x17000004 RID: 4
        // (get) Token: 0x0600002B RID: 43 RVA: 0x00003024 File Offset: 0x00001224
        public override Material DrawMat
        {
            get
            {
                return this.Graphic.MatSingleFor(this);
            }
        }

        // Token: 0x0400001F RID: 31
        public Gender gender;

        // Token: 0x04000020 RID: 32
        public Map headMap;

        // Token: 0x04000021 RID: 33
        public Color skinColor;

        // Token: 0x04000022 RID: 34
        public Pawn pawn;
    }
}
