using RimWorld;
using Verse;

namespace GoreUponDismemberment
{
    // Token: 0x02000005 RID: 5
    [DefOf]
    public static class GoreDefOf
    {
        // Token: 0x06000008 RID: 8 RVA: 0x0000215B File Offset: 0x0000035B
        static GoreDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(GoreDefOf));
        }

        // Token: 0x04000006 RID: 6
        public static ThingDef Gore;

        // Token: 0x04000007 RID: 7
        public static ThingDef BrainPieces;

        // Token: 0x04000008 RID: 8
        public static ThingDef GoreFilth;

        // Token: 0x04000009 RID: 9
        public static ThingDef GutsFilth;

        // Token: 0x0400000A RID: 10
        public static ThingDef FlyingHead;

        // Token: 0x0400000B RID: 11
        public static SoundDef GoreSound;

        // Token: 0x0400000C RID: 12
        public static SoundDef GibSound;

        // Token: 0x0400000D RID: 13
        public static FleckDef FlyingGibs;
    }
}
