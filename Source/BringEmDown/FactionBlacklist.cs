using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace BringEmDown
{
    public class FactionBlacklist : Def
    {
        public List<FactionDef> blacklistedFactionDefs = new List<FactionDef>();
    }
}
