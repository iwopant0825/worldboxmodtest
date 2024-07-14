using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NCMS;
using NCMS.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ReflectionUtility;
using ai;
using ai.behaviours;

namespace CollectionMod
{
    class BoatManager : MonoBehaviour
    {
        public HashSet<Actor> battleBoats = new HashSet<Actor>();
        private static List<BuildingAsset> docks = new List<BuildingAsset>();

        public static void toggleNavalWarfare()
        {
            Main.modifyBoolOption("navalWarfareOption", PowerButtons.GetToggleValue("naval_warfare_dej"));
            if (PowerButtons.GetToggleValue("naval_warfare_dej"))
            {
                turnOnNavalWarfare();
                return;
            }
            turnOffNavalWarfare();
        }

        public static void turnOnNavalWarfare()
        {
            BuildingAsset human = AssetManager.buildings.get(SB.docks_human);
            human.boats_assets.Add("battle_boat");
            BuildingAsset human_fishing = AssetManager.buildings.get(SB.fishing_docks_human);
            human_fishing.boats_assets.Add("battle_boat");
            docks.Add(human);
            docks.Add(human_fishing);
            
            BuildingAsset elf = AssetManager.buildings.get(SB.docks_elf);
            elf.boats_assets.Add("battle_boat");
            BuildingAsset elf_fishing = AssetManager.buildings.get(SB.fishing_docks_elf);
            elf_fishing.boats_assets.Add("battle_boat");
            docks.Add(elf);
            docks.Add(elf_fishing);
            
            BuildingAsset dwarf = AssetManager.buildings.get(SB.docks_dwarf);
            dwarf.boats_assets.Add("battle_boat");
            BuildingAsset dwarf_fishing = AssetManager.buildings.get(SB.fishing_docks_dwarf);
            dwarf_fishing.boats_assets.Add("battle_boat");
            docks.Add(dwarf);
            docks.Add(dwarf_fishing);

            BuildingAsset orc = AssetManager.buildings.get(SB.docks_orc);
            orc.boats_assets.Add("battle_boat");
            BuildingAsset orc_fishing = AssetManager.buildings.get(SB.fishing_docks_orc);
            orc_fishing.boats_assets.Add("battle_boat");
            docks.Add(orc);
            docks.Add(orc_fishing);
        }

        public static void turnOffNavalWarfare()
        {
            foreach(BuildingAsset dock in docks)
            {
                dock.boats_assets.Remove("battle_boat");
            }
        }
    }

    public class BehBattleBoatFindEnemyDock : BehBoat
	{
		public override BehResult execute(Actor pActor)
		{
            if (pActor.kingdom == null)
            {
                return BehResult.Stop;
            }
            if (!pActor.kingdom.hasEnemies())
            {
                return BehResult.Stop;
            }
			Docks dockTradeTarget = getDockTarget(pActor);
			if (dockTradeTarget == null)
			{
                return BehResult.Stop;
			}
            pActor.beh_building_target = dockTradeTarget.building;
			return BehResult.Continue;
			
		}

        private static Docks getDockTarget(Actor pActor)
		{
            List<War> wars = World.world.wars.getWars(pActor.kingdom).ToList();
            if (wars.Count < 1)
            {
                return null;
            }
            Kingdom targetKingdom = null;
            if (wars[0].isAttacker(pActor.kingdom) && wars[0]._list_defenders.Count > 0)
            {
                targetKingdom = wars[0]._list_defenders[0];
            }
            else if (wars[0]._list_attackers.Count > 0)
            {
                targetKingdom = wars[0]._list_attackers[0];
            }

            if (targetKingdom == null)
            {
                return null;
            }
            
            foreach(City pCity in targetKingdom.cities)
            {
                if (!pCity.hasBuildingType(SB.type_docks, true))
                {
                    continue;
                }
                Building buildingType = pCity.getBuildingType(SB.type_docks, true, true);
                if (buildingType == null)
                {
                    continue;
                }
                if (!buildingType.currentTile.isSameIsland(pActor.currentTile))
                {
                    continue;;
                }
                return buildingType.component_docks;
                // I'm scared of the LINQ version:
                // return (from buildingType in from pCity in targetKingdom.cities where pCity.hasBuildingType(SB.type_docks) select pCity.getBuildingType(SB.type_docks, true, true) into buildingType where buildingType != null select buildingType where buildingType.currentTile.isSameIsland(pActor.currentTile) select buildingType.component_docks).FirstOrDefault();
            }
			return null;
		}
	}

    public class BehBattleBoatCheckEnemyIsOk : BehaviourActionActor
	{
		public override BehResult execute(Actor pActor)
		{
			if (pActor.attackTarget == null || !pActor.attackTarget.base_data.alive)
			{
				pActor.attackTarget = null;
				return BehResult.Stop;
			}
			if (!pActor.kingdom.isEnemy(pActor.attackTarget.kingdom))
			{
				pActor.attackTarget = null;
				return BehResult.Stop;
			}
			if (!pActor.canAttackTarget(pActor.attackTarget))
			{
				pActor._targets_to_ignore.Add(pActor.attackTarget);
				pActor.attackTarget = null;
				return BehResult.Stop;
			}
			if (!pActor.isInAttackRange(pActor.attackTarget))
			{
				// if (pActor.asset.oceanCreature)
				// {
				// 	if ((!pActor.attackTarget.isInLiquid() && !pActor.asset.landCreature) || pActor.attackTarget.isFlying())
				// 	{
				// 		pActor.targetsToIgnore.Add(pActor.attackTarget);
				// 		pActor.attackTarget = null;
				// 		return BehResult.Stop;
				// 	}
				// }
				/*else */if ((pActor.attackTarget.isInLiquid() && !pActor.asset.oceanCreature) || pActor.attackTarget.isFlying())
				{
					pActor._targets_to_ignore.Add(pActor.attackTarget);
					pActor.attackTarget = null;
					return BehResult.Stop;
				}
			}
			float x = pActor.currentTile.chunk.x;
			int y = pActor.currentTile.chunk.y;
			int x2 = pActor.attackTarget.currentTile.chunk.x;
			int y2 = pActor.attackTarget.currentTile.chunk.y;
			if (Toolbox.Dist(x, y, x2, y2) >= SimGlobals.m.unit_chunk_sight_range + 2)
			{
				pActor.attackTarget = null;
				return BehResult.Stop;
			}
			pActor.beh_actor_target = pActor.attackTarget;
			return BehResult.Continue;
		}
	}
}