using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace BringEmDown
{
    [StaticConstructorOnStartup]
    public class CrashMapParent : MapParent
    {
        public static readonly Texture2D AttackCommand = ContentFinder<Texture2D>.Get("UI/Commands/AttackSettlement");

        public List<Thing> thingsToScatter = new List<Thing>();

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref thingsToScatter, "thingsToScatter", LookMode.Deep);
        }

        // remove if no player pawns left

        public override bool ShouldRemoveMapNow(out bool alsoRemoveWorldObject)
        {
            if (!Map.mapPawns.AnyColonistSpawned)
            {
                alsoRemoveWorldObject = true;
                return true;
            }

            alsoRemoveWorldObject = false;
            return false;
        }

        // attack gizmos tomfoolery

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan)
        {
            foreach (FloatMenuOption option in base.GetFloatMenuOptions(caravan))
            {
                yield return option;
            }

            if (this.Map == null)
            {
                yield return new FloatMenuOption($"Attack {this.Label}", delegate
                {
                    caravan.pather.StartPath(this.Tile, new CaravanArrivalAction_AttackCrashsite(this), repathImmediately: true);
                });
                yield return new FloatMenuOption($"Attack {this.Label} (Dev: instantly)", delegate
                {
                    CrashSiteUtility.Attack(caravan, this);
                });
            }
        }
        public class CaravanArrivalAction_AttackCrashsite : CaravanArrivalAction
        {
            private MapParent site;

            public override string Label => "Attack crash site";

            public override string ReportString => "Attacking crash site";

            public CaravanArrivalAction_AttackCrashsite()
            {
            }

            public CaravanArrivalAction_AttackCrashsite(MapParent site)
            {
                this.site = site;
            }
            public override void Arrived(Caravan caravan)
            {
                CrashSiteUtility.Attack(caravan, site);
            }

            public override void ExposeData()
            {
                base.ExposeData();
                Scribe_References.Look(ref site, "site");
            }
        }

        // checking for dead enemies

        protected override void TickInterval(int delta)
        {
            base.TickInterval(delta);

            if (Find.TickManager.TicksGame % 125 != 0)
                return;

            if (this.Map == null) return;

            if (!GenHostility.AnyHostileActiveThreatToPlayer(this.Map, true))
            {
                if (BringEmDownMod.settings.advancedLogging) Log.Message("All enemies defeated, completing the quest");
                QuestUtility.SendQuestTargetSignals(
                    Map.Parent.questTags,
                    "AllEnemiesDefeated",
                    Map.Parent.Named("SUBJECT")
                );

            }


        }
    }
}
