using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace BringEmDown
{
    public class CrashSiteUtility
    {
        public static void Attack(Caravan caravan, MapParent site)
        {
            if (!site.HasMap)
            {
                LongEventHandler.QueueLongEvent(delegate
                {
                    AttackNow(caravan, site);
                }, "GeneratingMapForNewEncounter", doAsynchronously: false, null);
            }
            else
            {
                AttackNow(caravan, site);
            }
        }

        private static void AttackNow(Caravan caravan, MapParent site)
        {
            bool num = !site.HasMap;
            Map orGenerateMap = GetOrGenerateMapUtility.GetOrGenerateMap(site.Tile, null);
            TaggedString letterLabel = "LetterLabelCaravanEnteredEnemyBase".Translate();
            TaggedString letterText = "LetterCaravanEnteredEnemyBase".Translate(caravan.Label, site.Label.ApplyTag(TagType.Settlement, site.Faction.GetUniqueLoadID())).CapitalizeFirst();
            if (num)
            {
                Find.TickManager.Notify_GeneratedPotentiallyHostileMap();
                PawnRelationUtility.Notify_PawnsSeenByPlayer_Letter(orGenerateMap.mapPawns.AllPawns, ref letterLabel, ref letterText, "LetterRelatedPawnsSettlement".Translate(Faction.OfPlayer.def.pawnsPlural), informEvenIfSeenBefore: true);
            }
            Find.LetterStack.ReceiveLetter(letterLabel, letterText, LetterDefOf.NeutralEvent, caravan.PawnsListForReading, site.Faction);
            CaravanEnterMapUtility.Enter(caravan, orGenerateMap, CaravanEnterMode.Edge, CaravanDropInventoryMode.DoNotDrop, draftColonists: true);
        }
    }
}
