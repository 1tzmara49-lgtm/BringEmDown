using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace BringEmDown
{
    public class BringEmDownSettings : ModSettings
    {
        public float cargoLossPercentage = 0.5f; // Discards amount of cargo
        public float survivorPointModifier = 0.5f; // Multiplies the survivor's pawngen budget by X
        public bool advancedLogging = false; // Logging for debug, no gameplay effect



        public int expensiveThreshold = 700; // 700 being the value of an archite capsule
        public string thresholdBuffer; // lil doodad the settings need


        public int maxItemsPerCarge = 5; // Max amount of items a player can find in a cargo crate.

        public int minDistance = 5; // pretty sure im not even using this
        public int maxDistance = 10;
        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref cargoLossPercentage, "cargoLossPercentage", 0.5f);
            Scribe_Values.Look(ref survivorPointModifier, "survivorPointModifier", 0.5f);
            Scribe_Values.Look(ref advancedLogging, "advancedLogging", false);

            Scribe_Values.Look(ref minDistance, "minDistance", 5);
            Scribe_Values.Look(ref minDistance, "minDistance", 5);

            Scribe_Values.Look(ref minDistance, "minDistance", 5);
            Scribe_Values.Look(ref maxDistance, "maxDistance", 10);
        }
    }
}
