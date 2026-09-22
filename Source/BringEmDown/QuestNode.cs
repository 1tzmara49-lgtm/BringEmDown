using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace BringEmDown
{
    public class SetupCrashSite : QuestNode
    {
        public WorldObjectDef worldObjectDef;

        public List<Thing> thingsToScatter;
        protected override void RunInt()
        {
            throw new NotImplementedException();
        }

        protected override bool TestRunInt(Slate slate)
        {
            PlanetTile tile;
            return TryFindSiteTile(out tile);
        }

        private bool TryFindSiteTile(out PlanetTile tile)
        {
        }
    }
}
