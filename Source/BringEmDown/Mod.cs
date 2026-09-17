using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace BringEmDown
{
    public class BringEmDownMod : Mod
    {
        public static BringEmDownSettings settings;

        public BringEmDownMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<BringEmDownSettings>();
            Log.Message("[Bring 'Em Down] Mod settings initialized successfully!");
        }
        public override string SettingsCategory()
        {
            return "Bring 'Em Down";
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);

            listing.Label($"Cargo loss percentage: {settings.cargoLossPercentage*100:F1}%");
            settings.cargoLossPercentage = listing.Slider(settings.cargoLossPercentage, 0f, 1f);
            listing.Gap();

            listing.CheckboxLabeled($"[DEV] More logging", ref settings.advancedLogging, "Enables the logging, WILL clutter up the log window so use carefully.");
            listing.GapLine();

            listing.End();
            base.DoSettingsWindowContents(inRect);
        }
    }
}
