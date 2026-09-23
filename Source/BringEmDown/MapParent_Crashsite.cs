using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace BringEmDown
{
    public class CrashMapParent : MapParent
    {
        public static readonly Texture2D AttackCommand = ContentFinder<Texture2D>.Get("UI/Commands/AttackSettlement");

        public List<Thing> thingsToScatter = new List<Thing>();

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref thingsToScatter, "thingsToScatter");
        }

        //public override IEnumerable<Gizmo> GetCaravanGizmos(Caravan caravan)
        //{
        //    yield return new Command_Action
        //    {
        //        icon = AttackCommand,
        //        defaultLabel = "CommandAttackSettlement".Translate(),
        //        defaultDesc = "CommandAttackSettlementDesc".Translate(),
        //        action = delegate
        //        {
        //            SettlementUtility.Attack(caravan, this);
        //        }
        //    };
        //}
    }
}
