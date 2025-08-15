using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace GoreUponDismemberment
{
    [StaticConstructorOnStartup]
    public static class XenoGoreDictionary
    {
        // Token: 0x0600002E RID: 46 RVA: 0x00003044 File Offset: 0x00001244
        static XenoGoreDictionary()
        {
            IEnumerable<XenoGoreDef> allDefs = DefDatabase<XenoGoreDef>.AllDefs;
            bool flag = allDefs.Count<XenoGoreDef>() < 1;
            if (!flag)
            {
                foreach (XenoGoreDef xenoGoreDef in allDefs)
                {
                    XenoGoreDictionary.dict.Add(xenoGoreDef.targetXenoType, xenoGoreDef);
                }
                int count = XenoGoreDictionary.dict.Count;
                Log.Message(string.Format("GUD: {0} xenotype gore def(s) found!", count));
            }
        }

        // Token: 0x0400002A RID: 42
        public static Dictionary<XenotypeDef, XenoGoreDef> dict = new Dictionary<XenotypeDef, XenoGoreDef>();
    }
}
