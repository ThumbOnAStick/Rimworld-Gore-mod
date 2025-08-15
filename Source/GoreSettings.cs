using Verse;

namespace GoreUponDismemberment
{
    // Token: 0x02000002 RID: 2
    public class GoreSettings : ModSettings
    {
        // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        public GoreSettings()
        {
            this.isForbidden = true;
            this.allFilth = false;
            this.limbDropChance = 100;
            this.torsoExplosionChance = 50;
        }

        // Token: 0x06000002 RID: 2 RVA: 0x00002078 File Offset: 0x00000278
        public override void ExposeData()
        {
            Scribe_Values.Look<bool>(ref this.allFilth, "AllFilth", false, false);
            Scribe_Values.Look<bool>(ref this.isForbidden, "isForbidden", true, false);
            Scribe_Values.Look<int>(ref this.limbDropChance, "limbDropChance", 100, false);
            Scribe_Values.Look<int>(ref this.torsoExplosionChance, "torsoExplosionChance", 50, false);
            base.ExposeData();
        }

        // Token: 0x04000001 RID: 1
        public bool allFilth;

        // Token: 0x04000002 RID: 2
        public bool isForbidden;

        // Token: 0x04000003 RID: 3
        public int limbDropChance;

        // Token: 0x04000004 RID: 4
        public int torsoExplosionChance;
    }
}
