using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace BringEmDown
{
    public class CrashMapParent : MapParent
    {
        public List<Thing> thingsToScatter = new List<Thing>();

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref thingsToScatter, "thingsToScatter");
        }
    }
}
