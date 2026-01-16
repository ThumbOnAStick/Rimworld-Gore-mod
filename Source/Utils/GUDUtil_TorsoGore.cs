using LudeonTK;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace GoreUponDismemberment
{
    partial class GUDUtil
    {
        [DebugAction("GUD", "MakeGoreFleck", false, false, false, false, false, 0, false, actionType = DebugActionType.ToolMapForPawns)]
        public static void MakeGoreFleck(Pawn pawn)
        {
            bool flag = pawn == null;
            if (!flag)
            {
                Map map = pawn.Map;
                bool flag2 = map == null;
                if (!flag2)
                {
                    Vector3 vector = pawn.Position.ToVector3();
                    bool flag3 = !vector.ShouldSpawnMotesAt(map, true);
                    if (!flag3)
                    {
                        int num = Rand.Range(3, 5);
                        for (int i = 0; i < num; i++)
                        {
                            bool flag4 = map == null;
                            if (flag4)
                            {
                                return;
                            }
                            bool flag5 = GUDUtil.HasSpecialGoreForXenoType(pawn);
                            bool flag6 = flag5;
                            if (flag6)
                            {
                                GUDUtil.MakeGoreFleckForAlien(vector, map, pawn);
                            }
                            else
                            {
                                GUDUtil.MakeGoreFleckForHuman(vector, map);
                            }
                        }
                        GUDUtil.PlayGibSound(vector, map);
                    }
                }
            }
        }

        private static void MakeGoreFleckForHuman(Vector3 loc, Map map)
        {
            float num = Rand.Range(-0.05f, 0.05f);
            float num2 = Rand.Range(-0.05f, 0.05f);
            FleckCreationData dataStatic = FleckMaker.GetDataStatic(loc + new Vector3(num, num2, 0f), map, GoreDefOf.FlyingGibs, 1f);
            dataStatic.rotation = (float)Rand.Range(0, 360);
            dataStatic.velocityAngle = (float)Rand.Range(0, 360);
            dataStatic.velocitySpeed = Rand.Range(0.5f, 0.75f);
            map.flecks.CreateFleck(dataStatic);
        }

        private static void PlayGibSound(Vector3 loc, Map map)
        {
            if (GoreUponDismembermentMod.settings.mute)
            {
                return;
            }
            SoundDef gibSound = GoreDefOf.GibSound;
            SoundInfo soundInfo = SoundInfo.InMap(new TargetInfo(loc.ToIntVec3(), map, false), MaintenanceType.None);
            gibSound.PlayOneShot(soundInfo);
        }

        internal static bool TorsoCheck(Hediff h)
        {
            BodyPartRecord part = h.Part;
            var def = ((part != null) ? part.def : null);
            return def == BodyPartDefOf.Torso || def == DefDatabase<BodyPartDef>.GetNamed("Body") ||
                     def == DefDatabase<BodyPartDef>.GetNamed("MechanicalThorax");
        }

        public static bool IsTorsoDestroyed(Pawn pawn)
        {
            bool result = false;
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
            if (hediffs != null && hediffs.Count >= 1 && hediffs.Any(TorsoCheck))
            {
                float severity = hediffs.Where(TorsoCheck).MaxBy((Hediff h) => h.tickAdded).Severity;
                if (severity > (float)BodyPartDefOf.Torso.hitPoints)
                {
                    // Create visual effects
                    GUDUtil.MakeGoreFleck(pawn);
                    // Record torso to be fully destroyed
                    pawn.health.hediffSet.pawn.TryGetComp<CompDeathRecorder>()?.SetTorsoDestroyed();
                }
            }
            return result;
        }

        public static bool IsBodyPartDestroyed(BodyPartRecord part, Pawn pawn)
        {
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
            if (hediffs != null && hediffs.Count >= 1 && hediffs.Any(x => x.Part == part))
            {
                float severity = hediffs.Where(x => x.Part == part).MaxBy((Hediff h) => h.tickAdded).Severity;
                if (severity > part.def.hitPoints)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
