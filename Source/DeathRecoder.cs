using Verse;
using RimWorld;

namespace GoreUponDismemberment
{
    public class CompDeathRecorder : ThingComp
    {
        public DeathCause cause = DeathCause.Alive;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref cause, "GUD_DeathCause", DeathCause.Alive);
        }

        public enum DeathCause
        {
            Alive,
            Flame,
            Cut,
            Shred,
            Shot
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