using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI.Group;

namespace BringEmDown
{
    public class ScatterGoods : GenStep
    {
        public override int SeedPart => 69696969;
        public override void Generate(Map map, GenStepParams parms)
        {
            if (!RCellFinder.TryFindRandomCellNearTheCenterOfTheMapWith(
                            (IntVec3 c) => c.Standable(map) && !c.Fogged(map) && c.GetRoom(map).TouchesMapEdge == false,
                            map, out IntVec3 defendCenter))
            {
                defendCenter = map.Center;
            }

            CrashMapParent mapParent = map.Parent as CrashMapParent;
            
            


        }
    }
    public class GenerateSurvivors : GenStep
    {
        public override int SeedPart => 84729185;

        public static readonly IntVec3 WanderRadius = new IntVec3();

        TraverseParms traverseParms = TraverseParms.For(TraverseMode.ByPawn);
        public override void Generate(Map map, GenStepParams parms)
        {
            if (!RCellFinder.TryFindRandomCellNearTheCenterOfTheMapWith(
                            (IntVec3 c) => c.Walkable(map) && map.reachability.CanReachUnfogged(c, traverseParms) && !c.Fogged(map) && c.GetRoom(map).TouchesMapEdge == false,
                            map, out IntVec3 defendCenter))
            {
                defendCenter = map.Center;
            }

            Faction faction = (map.ParentFaction != null && map.ParentFaction != Faction.OfPlayer)
            ? map.Parent.Faction
            : Find.FactionManager.RandomEnemyFaction();


            if (faction == null) return;

            CrashMapParent mapParent = map.Parent as CrashMapParent;
            List<Thing> thingsToScater = mapParent.thingsToScatter;

            float points = RailgunUtility.GetValueFromList(thingsToScater);

            //if (faction.def.pawnGroupMakers != null && faction.def.pawnGroupMakers.NullOrEmpty())
            //{
            //    faction = FactionDefOf.TradersGuild;
            //}

            PawnGroupMakerParms pawnParms = new PawnGroupMakerParms
            {
                groupKind = PawnGroupKindDefOf.Combat,
                tile = map.Tile,
                faction = faction,
                points = points * BringEmDownMod.settings.survivorPointModifier,
            };
            bool preserveFaction = false;
            if (PawnGroupMakerUtility.CanGenerateAnyNormalGroup(pawnParms.faction, pawnParms.points))
            {
                if (BringEmDownMod.settings.advancedLogging) Log.Message("[Bring em Down] CanGenerateAnyNormalGroup returned true. Proceeding with normal generation");
            }
            else
            {
                if (BringEmDownMod.settings.advancedLogging) Log.Message("[Bring em Down] CanGenerateAnyNormalGroup returned false. Using fallback parms");
                pawnParms = new PawnGroupMakerParms
                {
                    groupKind = PawnGroupKindDefOf.Combat,
                    tile = map.Tile,
                    faction = Find.FactionManager.FirstFactionOfDef(FactionDefOf.AncientsHostile),
                    points = points * BringEmDownMod.settings.survivorPointModifier,
                };
                preserveFaction = true;
               
            }
            if (BringEmDownMod.settings.advancedLogging)
            {
                Log.Message($"Generating survivors | faction : {faction.Name}, points : {points}");
            }
            List<Pawn> pawns = PawnGroupMakerUtility.GeneratePawns(pawnParms).ToList();
            if (!pawns.Any()) return;

            foreach (Pawn pawn in pawns)
            {
                IntVec3 spawnLoc = CellFinder.RandomClosewalkCellNear(defendCenter, map, 20);
                if (!spawnLoc.IsValid) spawnLoc = defendCenter;
                if (preserveFaction)
                {
                    pawn.SetFaction(faction);
                }
                GenSpawn.Spawn(pawn, spawnLoc, map);
                if (BringEmDownMod.settings.advancedLogging)
                {
                    Log.Message($"Placing pawn: {pawn} with worth: {pawn.MarketValue} of faction: {pawn.Faction}");
                }

            }

            LordJob_DefendBase lordJob = new LordJob_DefendBase(faction, defendCenter, 6000, true);

            LordMaker.MakeNewLord(faction, lordJob, map, pawns);
        }
    }
}
