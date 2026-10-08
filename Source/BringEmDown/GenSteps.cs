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
            TraverseParms traverseParms = TraverseParms.For(TraverseMode.PassDoors);
            if (!RCellFinder.TryFindRandomCellNearTheCenterOfTheMapWith(
                            (IntVec3 c) => c.Standable(map) && !c.Fogged(map) && !c.CloseToEdge(map, 50) && map.reachability.CanReachMapEdge(c, traverseParms),
                            map, out IntVec3 defendCenter))
            {
                defendCenter = map.Center;
            }

            CrashMapParent mapParent = map.Parent as CrashMapParent;
            Thing container = null;
            float totalWeight = 0;
            Thing expensiveContainer = null;
            Thing weaponContainer = null;
            Thing cryptoContainer = null;

            foreach (Thing thing in mapParent.thingsToScatter)
            {
                // TBD: Mechanoid crate (?) for compatabiliy since putting mechanoids in cryptosleep is just stupid, might just spawn them as they are idk
                // high priority!!!! if a living thing, place in this crate.
                if (thing is Pawn pawn)
                {
                    cryptoContainer = ThingMaker.MakeThing(BEDDefOf.BED_CryptosleepCrate);
                    if (cryptoContainer is IThingHolder holder)
                    {
                        holder.GetDirectlyHeldThings().TryAddOrTransfer(thing);
                    }
                    Utility.TrySpawnContainer(cryptoContainer, map, defendCenter);
                    cryptoContainer = null;
                }
                // if is a weapon(s) shove it inside the weapon crate.
                else if (thing.def.IsWeapon && thing.MarketValue >= 110) // 110 silver price check so the players wont hack the crate just to get a fucking recursive bow
                {
                    weaponContainer = ThingMaker.MakeThing(BEDDefOf.BED_WeaponCrate);
                    if (weaponContainer is IThingHolder holder)
                    {
                        holder.GetDirectlyHeldThings().TryAddOrTransfer(thing);
                    }
                    Utility.TrySpawnContainer(weaponContainer, map, defendCenter);
                    weaponContainer = null;
                }
                //if expensive AND not bulk bullshit (like 7 morbillion plasteel units) == get into the expensive crate.
                else if (thing.MarketValue * thing.stackCount >= BringEmDownMod.settings.expensiveThreshold && thing.stackCount <= 50)
                {
                    expensiveContainer = ThingMaker.MakeThing(BEDDefOf.BED_ExpensiveCargoCrate);
                    if (expensiveContainer is IThingHolder holder)
                    {
                        holder.GetDirectlyHeldThings().TryAddOrTransfer(thing);
                    }
                    IntVec3 spawnPos = CellFinder.RandomClosewalkCellNear(defendCenter, map, 15);
                    GenSpawn.Spawn(expensiveContainer, spawnPos, map);
                    expensiveContainer = null;
                }
                // shove whatever else inside a large crate. if over a weight threshold (whatever the fuck i changed it to) spawn the cargo.
                else
                {
                    if (container == null)
                    {
                        container = ThingMaker.MakeThing(BEDDefOf.BED_LargeCargoContanier);
                        totalWeight = 0;
                    }

                    if (container is IThingHolder holder)
                    {
                        holder.GetDirectlyHeldThings().TryAddOrTransfer(thing);

                        foreach (Thing item in holder.GetDirectlyHeldThings())
                        {
                            totalWeight += item.GetStatValue(StatDefOf.Mass) * item.stackCount;
                        }

                        if (totalWeight >= 25.5f)
                        {
                            Utility.TrySpawnContainer(container, map, defendCenter);
                            container = null;
                        }
                    }
                }
            }
            if (container != null)
            {
                Utility.TrySpawnContainer(container, map, defendCenter);
                container = null;
            }



        }
    }
    public class GenerateSurvivors : GenStep
    {
        public override int SeedPart => 84729185;

        public static readonly IntVec3 WanderRadius = new IntVec3();

        TraverseParms traverseParms = TraverseParms.For(TraverseMode.ByPawn);
        public override void Generate(Map map, GenStepParams parms)
        {
            TraverseParms traverseParms = TraverseParms.For(TraverseMode.PassDoors);
            if (!RCellFinder.TryFindRandomCellNearTheCenterOfTheMapWith(
                            (IntVec3 c) => c.Standable(map) && !c.Fogged(map) && !c.CloseToEdge(map, 10) && map.reachability.CanReachMapEdge(c, traverseParms),
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

            float points = Utility.GetValueFromList(thingsToScater);

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
