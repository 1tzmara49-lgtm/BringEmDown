using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace BringEmDown
{
    public class BEDCargoCrate : Building_Crate
    {
        protected override void ReceiveCompSignal(string signal)
        {
            base.ReceiveCompSignal(signal);
            if (signal == "Hacked")
            {
                Open();
            }
        }

        public override void Open()
        {
            if (CanOpen)
            {

                base.Open();
                base.EjectContents();
            }
        }
    }
}
