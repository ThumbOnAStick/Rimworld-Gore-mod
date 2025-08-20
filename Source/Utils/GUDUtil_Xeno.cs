using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace GoreUponDismemberment
{
    partial class GUDUtil
    {
        private static bool HasSpecialGoreForXenoType(Pawn pawn)
        {
            bool flag = !ModsConfig.BiotechActive;
            bool flag2;
            if (flag)
            {
                flag2 = false;
            }
            else
            {
                bool flag3 = pawn == null;
                if (flag3)
                {
                    flag2 = false;
                }
                else
                {
                    bool flag4 = pawn.genes == null;
                    if (flag4)
                    {
                        flag2 = false;
                    }
                    else
                    {
                        bool flag5 = pawn.genes.Xenotype == null;
                        flag2 = !flag5 && XenoGoreDictionary.dict.ContainsKey(pawn.genes.Xenotype);
                    }
                }
            }
            return flag2;
        }

        private static void MakeGoreFleckForAlien(Vector3 loc, Map map, Pawn pawn)
        {
            float num = Rand.Range(-0.05f, 0.05f);
            float num2 = Rand.Range(-0.05f, 0.05f);
            FleckDef gibDef = XenoGoreDictionary.dict[pawn.genes.Xenotype].gibDef;
            bool flag = gibDef == null;
            if (!flag)
            {
                FleckCreationData dataStatic = FleckMaker.GetDataStatic(loc + new Vector3(num, num2, 0f), map, gibDef, 1f);
                dataStatic.rotation = (float)Rand.Range(0, 360);
                dataStatic.velocityAngle = (float)Rand.Range(0, 360);
                dataStatic.velocitySpeed = Rand.Range(0.5f, 0.75f);
                map.flecks.CreateFleck(dataStatic);
            }
        }

        public static bool IsXenoBannedFromGore(XenotypeDef xenotypeDef)
        {
            bool flag = xenotypeDef == null;
            return !flag && XenoGoreDictionary.dict.ContainsKey(xenotypeDef) && XenoGoreDictionary.dict[xenotypeDef].disableCorpseVariety;
        }
    }
}
