using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace BringEmDown
{

    public class QuestNode_SetupCrashsite : QuestNode
    {
        public WorldObjectDef worldObjectDef;

        private static readonly IntRange TimeoutDays = new IntRange(5, 10);
        protected override bool TestRunInt(Slate slate)
        {
            return true;
        }
        protected override void RunInt()
        {
            Quest quest = QuestGen.quest;
            Slate slate = QuestGen.slate;
            Faction siteFaction = slate.Get<Faction>("siteFaction");
            List<Thing> thingsToScatter = slate.Get<List<Thing>>("thingsToScatter");
            
            PlanetTile tile = slate.Get<PlanetTile>("siteTile");

            CrashMapParent crashMapParent = (CrashMapParent)WorldObjectMaker.MakeWorldObject(worldObjectDef);
            crashMapParent.Tile = tile;
            crashMapParent.thingsToScatter = thingsToScatter;

            if (siteFaction != null)
            {
                crashMapParent.SetFaction(siteFaction);
            }

            slate.Set("worldObject", crashMapParent);
            quest.SpawnWorldObject(crashMapParent);


            string inSignal = QuestGenUtility.HardcodedSignalWithQuestID("worldObject.MapRemoved");
            int delayTicks = TimeoutDays.RandomInRange * 60000;
            quest.WorldObjectTimeout(crashMapParent, delayTicks);
            quest.Delay(delayTicks, delegate
            {
                QuestGen_End.End(quest, QuestEndOutcome.Fail);
            });
            quest.End(QuestEndOutcome.Success, 0, null, inSignal);


        }
    }
}
