using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Noise;

namespace BringEmDown
{
    public static class RailgunUtility
    {
        public static List<TradeShip> GetAllTradeships(Map map)
        {
            if (map == null) return null;

            List<TradeShip> activeTraders = map.passingShipManager.passingShips
                                                    .OfType<TradeShip>()
                                                    .ToList();
            activeTraders.RemoveAll(ship => ship.Faction.def != null && BEDDefOf.BED_FactionBlacklist.blacklistedFactionDefs.Contains(ship.Faction.def));
            return activeTraders;
        }
        public static List<Thing> GetGoods(TradeShip ship)
        {
            if (ship == null) return null;

            List<Thing> goods = ship.Goods.ToList();

            int itemLostAmount = (int)(goods.Count() * BringEmDownMod.settings.cargoLossPercentage);

            for(int i = 0; i < itemLostAmount; i++)
            {
                goods.Remove(goods.RandomElement());
            }
            if (BringEmDownMod.settings.advancedLogging)
            {
                Log.Message("[Bring Em Down] === Get goods ===");
                Log.Message($"Initial goods count {ship.Goods.ToList().Count()}");
                Log.Message($"Lost percentage: {BringEmDownMod.settings.cargoLossPercentage}");
                Log.Message($"Items to remove: {itemLostAmount}");
                Log.Message($"Final item count: {goods.Count()}");
                Log.Message($"Final goods list:");
                foreach(Thing good in goods)
                {
                    Log.Message($"• {good.Label}");
                }

            }
            return goods;
        }

        public static float GetValueFromList(List<Thing> list)
        {
            float totalValue = 0;
            foreach (Thing thing in list)
            {
                totalValue += thing.MarketValue;
            }
            if (BringEmDownMod.settings.advancedLogging)
            {
                Log.Message("[Bring Em Down] === Get Value From List ===");
                Log.Message($"Goods count {list.Count()}");
                Log.Message($"Total value {totalValue}");
            }
            return totalValue;
        }

        public static bool TryGetClearRotation(Thing thing, IntVec3 center, Map map, out List<Rot4> clearRotations)
        {
            clearRotations = new List<Rot4>();

            foreach (Rot4 rot in Rot4.AllRotations)
            {
                CellRect occupiedRect = GenAdj.OccupiedRect(center, rot, thing.def.size);
                bool hasOverlap = false;

                foreach (IntVec3 cell in occupiedRect)
                {
                    if (!cell.InBounds(map) || !cell.Standable(map) || cell.GetEdifice(map) != null)
                    {
                        hasOverlap = true;
                        break;
                    }
                }

                if (!hasOverlap)
                {
                    clearRotations.Add(rot);
                }
            }

            return clearRotations.Count > 0;
        }

        public static void TrySpawnLargeContainer(Thing crateToSpawn, Map map, IntVec3 defendCenter)
        {
            int attempts = 0;
            while (!crateToSpawn.Spawned && attempts <= 50)
            {
                IntVec3 spawnPos = CellFinder.RandomClosewalkCellNear(defendCenter, map, 30);
                if (TryGetClearRotation(crateToSpawn, spawnPos, map, out List<Rot4> safeRot))
                {
                    Thing crate = GenSpawn.Spawn(crateToSpawn, spawnPos, map, safeRot.RandomElement());
                    crate.DrawColor = map.ParentFaction?.Color ?? UnityEngine.Color.grey;
                    return;
                }
                attempts++;
            }

            if (!crateToSpawn.Spawned)
            {
                Log.Warning("[Bring Em Down] failed to find a valid spawn location for a large cargo container after 50 attempts. spawning it wherever the fuck it can, prepare for overlaps and issues.");
                IntVec3 spawnPos = CellFinder.RandomClosewalkCellNear(defendCenter, map, 30);
                if (TryGetClearRotation(crateToSpawn, spawnPos, map, out List<Rot4> safeRot))
                {
                    Thing crate = GenSpawn.Spawn(crateToSpawn, spawnPos, map, safeRot.RandomElement());
                    crate.DrawColor = map.ParentFaction?.Color ?? UnityEngine.Color.grey;
                    return;
                }
            }
        }
    }
}
