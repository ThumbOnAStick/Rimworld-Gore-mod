using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GoreSettings;
using HarmonyLib;
using RimWorld;
using Verse;
using UnityEngine;

namespace GoreUponDismemberment
{
    public class GoreUponDismembermentMod : Mod
    {
        public GoreUponDismembermentMod(ModContentPack content) : base(content)
        {
            settings = base.GetSettings<Settings>();
            harmonyInstance = new Harmony("DeathGore");
            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.Begin(inRect);
            listing_Standard.CheckboxLabeled("AllFilth".Translate(), ref settings.allFilth, "GoreSpawnsAsFilth".Translate(), 0f, 1f);
            if (settings.allFilth == false)
                listing_Standard.CheckboxLabeled("IsForbidden".Translate(), ref settings.isForbidden, "ForbidGoreAfterSpawning".Translate(), 0f, 1f);
            settings.limbDropChance=(int)listing_Standard.SliderLabeled("LimbDropChance".Translate(settings.limbDropChance),settings.limbDropChance,0,100);
            settings.torsoExplosionChance= (int)listing_Standard.SliderLabeled("TorsoExplosionChance".Translate(settings.torsoExplosionChance), settings.torsoExplosionChance, 0, 100);
            listing_Standard.End();
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "Gore".Translate();
        }

        public static Settings settings;
        public static Harmony harmonyInstance;
    }
}
