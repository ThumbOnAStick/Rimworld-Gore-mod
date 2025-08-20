using Verse;
using RimWorld;

namespace GoreUponDismemberment
{
    public class CompDeathRecorder : ThingComp
    {
        public enum DeathCause
        {
            Alive,
            Flame,
            Cut,
            Shred,
            Shot
        }
        public DeathCause cause = DeathCause.Alive;
        public bool isTorsoDestroyed = false;

        public void SetTorsoDestroyed()
        {
            this.isTorsoDestroyed = true;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref cause, "GUD_DeathCause", DeathCause.Alive);
            Scribe_Values.Look(ref isTorsoDestroyed, "GUD_TorsoDestroyed", false);
        }

     
    }

    public class CompProperties_DeathRecorder : CompProperties
    {
        public CompProperties_DeathRecorder()
        {
            this.compClass = typeof(CompDeathRecorder);
        }
    }
}