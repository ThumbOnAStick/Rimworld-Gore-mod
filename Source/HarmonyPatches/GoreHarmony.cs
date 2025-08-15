using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace GoreUponDismemberment.HarmonyPatches
{
    [StaticConstructorOnStartup]
    public static class GoreHarmony
    {
        public static Harmony harmony;

        static GoreHarmony()
        {
            Harmony harmony;
            if ((harmony = GoreHarmony.harmony) == null)
            {
                harmony = (GoreHarmony.harmony = new Harmony("thumb.GoreMod"));
            }
            try
            {
                GoreHarmony.harmony = harmony;
                BodyRenderPatch.PatchHarmony();
                HeadRenderPatch.PatchHarmony();
                MakeCorpsePatch.PatchHarmony();
                TorsoDestroyedPatch.PatchHarmony();
                MissingPartPatch.PatchHarmony();
                PawnKillPatch.PatchHarmony();
                // Compatiblity with Facial animation -- Not implemented
                GoreHarmony.harmony.PatchAll();
            } catch (Exception e)
            {
                Dialog_MessageBox box = new Dialog_MessageBox($"Gore mod: an error occured while trying to patch harmony, you can report this bug on bug report thread. {e}");
                Find.WindowStack.Add(box);
            }

            Log.Message("GUD: patches were successful");


        }



    }
}
