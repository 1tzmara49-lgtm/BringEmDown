using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace BringEmDown
{
    [DefOf]
    public static class BEDDefOf
    {
        public static HistoryEventDef BED_ShotDownShip;

        static BEDDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(BEDDefOf));
        }
    }
}