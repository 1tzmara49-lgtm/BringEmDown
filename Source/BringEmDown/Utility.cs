using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

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
    }
}
