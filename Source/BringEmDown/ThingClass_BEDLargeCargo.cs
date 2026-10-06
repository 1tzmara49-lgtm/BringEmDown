using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace BringEmDown
{

    public class LargeCargoCrate : Building_Crate
    {
        // yes this shit is hardocded for the time being. no im not using mod extensions.
        public string OpenedTexPath = "Building/LargeCargoCrate/cargoContainer_Open";
        public override Graphic Graphic
        {
            get
            {
                if (this?.innerContainer?.Count == 0)
                {

                    return GraphicDatabase.Get<Graphic_Multi>(OpenedTexPath, ShaderDatabase.CutoutComplex, def.graphicData.drawSize, this.DrawColor, this.DrawColorTwo);
                }
                return base.Graphic;
            }
        }

    }
}
