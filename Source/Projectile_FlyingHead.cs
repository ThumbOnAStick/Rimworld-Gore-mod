using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace GoreUponDismemberment
{
    public class Projectile_FlyingHead : Projectile
    {
        public Gender gender;
        public Map headMap;
        public Color skinColor;
        public Pawn pawn;
        int frame;
        Rot4 facing;
        private HeadTypeDef headtypeDef;

        public Projectile_FlyingHead() : base()
        {
            this.facing = new Rot4();
            this.frame = 0;
        }   

        public Projectile_FlyingHead(Gender gender, Map headMap, Color skinColor, Pawn pawn, HeadTypeDef headtypeDef) : this()
        {
            this.gender = gender;
            this.headMap = headMap;
            this.skinColor = skinColor;
            this.pawn = pawn;
            this.headtypeDef = headtypeDef;
        }

        private float ArcHeightFactor
        {
            get
            {
                float num = def.projectile.arcHeightFactor;
                float num2 = (destination - origin).MagnitudeHorizontalSquared();
                if (num * num > num2 * 0.2f * 0.2f)
                {
                    num = Mathf.Sqrt(num2) * 0.2f;
                }

                return num;
            }
        }
        private bool HasBrainPartsForXenoType(Pawn pawn)
        {
            if (pawn == null)
            {
                return false;
            }

            if (pawn.genes == null)
            {
                return false;
            }

            if (pawn.genes.Xenotype == null)
            {
                return false;
            }

            return XenoGoreDictionary.dict.ContainsKey(pawn.genes.Xenotype);
        }

        public void SetHeadTypeDef(HeadTypeDef path)
        {
            this.headtypeDef= path;
        }

        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            base.Impact(hitThing, blockedByShield);

            if (this.pawn != null && this.HasBrainPartsForXenoType(this.pawn))
            {
                ThingDef brainPartsFilth = XenoGoreDictionary.dict[this.pawn.genes.Xenotype].brainPartsFilth;
                FilthMaker.TryMakeFilth(base.Position, this.headMap, brainPartsFilth, "", 1, FilthSourceFlags.None);
            }
            else
            {
                FilthMaker.TryMakeFilth(base.Position, this.headMap, GoreDefOf.BrainPieces, "", 1, FilthSourceFlags.None);
            }
        }


        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look<Map>(ref this.headMap, "headMap", false);
            Scribe_References.Look<Pawn>(ref this.pawn, "pawn", false);
            Scribe_Values.Look<Gender>(ref this.gender, "gender", Gender.None, false);
            Scribe_Values.Look<Color>(ref this.skinColor, "skinColor", default(Color), false);
            Scribe_Defs.Look<HeadTypeDef>(ref this.headtypeDef, "headType");
        }

        void DrawHead(Vector3 drawLoc)
        {
            Graphic headGraphic = this.Graphic;
            if (headGraphic == null)
            {
                return; // Don't draw anything if we don't have a valid graphic
            }

            float num = ArcHeightFactor * GenMath.InverseParabola(DistanceCoveredFractionArc);
            // Head should be behind hair, so use a slightly lower z-position
            Vector3 vector = drawLoc + new Vector3(0f, 0f, 1f) * num + new Vector3(0f, 0f, -0.01f);
            Quaternion rotation = ExactRotation;
            headGraphic.drawSize = this.DrawSize;
            headGraphic.Draw(vector, facing, this, rotation.eulerAngles.y);
        }

        void DrawHair(Vector3 drawLoc)
        {
            if (this.pawn != null)
            {
                float num = ArcHeightFactor * GenMath.InverseParabola(DistanceCoveredFractionArc);
                // Hair should be in front of head, so use the base z-position
                Vector3 hairDrawLoc = drawLoc + new Vector3(0f, 0f, 1f) * num;
                
                Quaternion exactRotation = this.ExactRotation;
                
                if (this.pawn.story.hairDef != HairDefOf.Bald)
                {
                    string text = this.pawn.story.hairDef.texPath;
                    Graphic graphic = GraphicDatabase.Get<Graphic_Multi>(text, ShaderDatabase.CutoutHair, Vector2.one, this.pawn.story.HairColor);
                    graphic.drawSize = this.DrawSize;
                    graphic.Draw(hairDrawLoc, facing, this, exactRotation.eulerAngles.y);
                }
                
                if (this.pawn.style.beardDef != BeardDefOf.NoBeard)
                {
                    string text2 = this.pawn.style.beardDef.texPath;
                    Graphic graphic2 = GraphicDatabase.Get<Graphic_Multi>(text2, ShaderDatabase.CutoutHair, Vector2.one, this.pawn.story.HairColor);
                    graphic2.drawSize = this.DrawSize;
                    graphic2.Draw(hairDrawLoc, facing, this, exactRotation.eulerAngles.y);
                }
            }
        }
        protected override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            this.frame++;
            float period = 10;
            facing = Rot4.FromAngleFlat(360 * ((frame % period) / period));
            // Head
            DrawHead(drawLoc);

            // Hair
            DrawHair(drawLoc);
        }

        public override Quaternion ExactRotation => base.ExactRotation * Quaternion.Euler(0, 90, 0);

        public override Graphic Graphic
        {
            get
            {
                return GUDUtil.FlyingHeadGraphic(this.gender, this.skinColor, this.headtypeDef, this.pawn);
            }
        }

        float GetSizeMultiplier()
        {
            float distanceFrom05 = Mathf.Abs(0.5f - this.DistanceCoveredFraction);
            return Mathf.Max(1, 2f - (distanceFrom05 * 2f));
        }

        public override Vector2 DrawSize => pawn != null ? pawn.DrawSize * GetSizeMultiplier() : Vector2.one * GetSizeMultiplier();

        public override Material DrawMat
        {
            get
            {
                return this.Graphic.MatSingleFor(this);
            }
        }


    }
}
