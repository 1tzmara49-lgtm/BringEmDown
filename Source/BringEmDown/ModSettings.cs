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
        // How much of the loot is discarded when the site is generated. 0f - no loot (why) 1f - all the loot (awesome)
        public float cargoLossPercentage = 0.5f;
        public bool advancedLogging = false;

        // Range settings for site gen. Default : 5 - 10 tiles
        public int minDistance = 5;
        public int maxDistance = 10;
        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref cargoLossPercentage, "cargoLossPercentage", 0.5f);
            Scribe_Values.Look(ref advancedLogging, "advancedLogging", false);

            Scribe_Values.Look(ref minDistance, "minDistance", 5);
            Scribe_Values.Look(ref maxDistance, "maxDistance", 10);
        }
    }
}
