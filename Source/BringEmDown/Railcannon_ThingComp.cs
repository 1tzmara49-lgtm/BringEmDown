using RimWorld;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Diagnostics;
using Verse;

namespace BringEmDown
{
    public class RailcannonCompProps : CompProperties
    {
        public RailcannonCompProps()
        {
            this.compClass = typeof(RailcannonComp);
        }

    }

    public class RailcannonComp : ThingComp
    {
        private TradeShip currentTarget;

        public TradeShip CurrentTarget
        {
            get
            {
                if (currentTarget != null)
                {
                    if (!parent.Spawned || parent.Map == null || !parent.Map.passingShipManager.passingShips.Contains(currentTarget)){
                        currentTarget = null; 
                    }
                }
                return currentTarget;
            }
            set => currentTarget = value;
        }
        public override IEnumerable<Gizmo> CompGetGizmosExtra() 
        {
            foreach (Gizmo g in base.CompGetGizmosExtra())
            {
                yield return g;
            }

            if (!parent.Spawned) yield break;

            string targetLabel = CurrentTarget != null ? CurrentTarget.name : "None";
            Command_Action fire_railcannon = new Command_Action
            {
                defaultLabel = "Fire railcannon",
                defaultDesc = "Shoot the orbital railcannon at the selected target ship. This will result in the ship falling down somewhere near your base with all it's cargo. This will anger the orbital trader's faction.",
                action = delegate
                {
                    FireRailcannon();
                }
            };

            Command_Action select_target = new Command_Action
            {
                defaultLabel = $"Current target {targetLabel}",
                action = delegate
                {
                    GetTargets();
                }
            };

            yield return fire_railcannon;
            yield return select_target;
        }


        public void FireRailcannon()
        {
            if (currentTarget == null)
            {
                Messages.Message("No target selected", parent, MessageTypeDefOf.RejectInput);
                return;
            }

            Map map = parent.Map;

            map.passingShipManager.RemoveShip(currentTarget);

            Slate slate = new Slate();

            Faction faction = currentTarget?.Faction;
            if (faction != null) {
                HistoryEventDef reason = BEDDefOf.BED_ShotDownShip;
                Faction.OfPlayer.TryAffectGoodwillWith(faction, -100, canSendMessage: true, canSendHostilityLetter: true, reason: reason);
            }

            List<Thing> finalGoods = RailgunUtility.GetGoods(currentTarget);

            slate.Set("targetFaction", faction);
            slate.Set("thingsToScatter", finalGoods);
            slate.Set("totalValue", RailgunUtility.GetValueFromList(finalGoods));

            QuestScriptDef questDef = DefDatabase<QuestScriptDef>.GetNamed("BED_Crashed_Trader");
            QuestUtility.GenerateQuestAndMakeAvailable(questDef, slate);
        }

        public void GetTargets()
        {
            Map map = parent.Map;

            List<TradeShip> availibleShips = RailgunUtility.GetAllTradeships(map);

            if (availibleShips.NullOrEmpty())
            {
                Messages.Message("No trade ships to target in orbit", parent, MessageTypeDefOf.RejectInput);
                return;
            }

            List<FloatMenuOption> options = new List<FloatMenuOption>();

            options.Add(new FloatMenuOption("Clear target", () =>
            {
                currentTarget = null;
            }));

            foreach (TradeShip ship in availibleShips)
            {
                TradeShip localShip = ship;

                options.Add(new FloatMenuOption($"{localShip.name} ({localShip.def.label})", () =>
                {
                    currentTarget = localShip;
                    Messages.Message($"{parent.Label} has locked onto {localShip.name}.", parent, MessageTypeDefOf.PositiveEvent);
                }));
            }

            Find.WindowStack.Add(new FloatMenu(options));

        }

        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_References.Look(ref currentTarget, "currentTarget");
        }
    }
}
