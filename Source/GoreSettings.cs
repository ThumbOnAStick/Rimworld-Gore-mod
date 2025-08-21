using Verse;

namespace GoreUponDismemberment
{
    // Token: 0x02000002 RID: 2
    public class GoreSettings : ModSettings
    {
        public bool allFilth;
        public bool isForbidden;
        public bool enableTorsoSplit;
        public int limbDropChance;
        public int torsoExplosionChance;

        // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        public GoreSettings()
        {
            this.isForbidden = true;
            this.allFilth = false;
            this.enableTorsoSplit = true;
            this.limbDropChance = 100;
            this.torsoExplosionChance = 50;
        }

        // Token: 0x06000002 RID: 2 RVA: 0x00002078 File Offset: 0x00000278
        public override void ExposeData()
        {
            Scribe_Values.Look<bool>(ref this.allFilth, "AllFilth", false, false);
            Scribe_Values.Look<bool>(ref this.isForbidden, "isForbidden", true, false);
            Scribe_Values.Look<bool>(ref this.enableTorsoSplit, "enableTorsoSplit", true, false);
            Scribe_Values.Look<int>(ref this.limbDropChance, "limbDropChance", 100, false);
            Scribe_Values.Look<int>(ref this.torsoExplosionChance, "torsoExplosionChance", 50, false);
            base.ExposeData();
        }


    }
}
