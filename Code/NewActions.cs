using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using NCMS;
using NCMS.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ReflectionUtility;

namespace CollectionMod
{
    class NewActions : MonoBehaviour
    {
        public static City convertCityA = null;
        public static City convertCityB = null;
        public static City cityToBeExpanded = null;
        public static Culture cultureToBeExpanded = null;

        public static bool action_useMagnet(WorldTile pTile, string pPower)
        {
            Main.filteredMagnet.magnetAction(false, pTile);
            return true;
        }

        public static bool action_spawn_boat(WorldTile pTile, string pPowerID)
        {
            if (pTile.zone.city == null)
            {
                return false;
            }
            City pCity = pTile.zone.city;
            if (!pCity.hasBuildingType(SB.type_docks, true))
            {
                return false;
            }
            Building buildingType = pCity.getBuildingType(SB.type_docks, true, true);
            if (buildingType == null)
            {
                return false;
            }
            Actor actor = NewGodPowers.spawnUnit(pTile, pPowerID);
            if (actor == null)
            {
                return false;
            }
            if (!buildingType.currentTile.isSameIsland(actor.currentTile))
            {
                return false;
            }
            actor.ai.task_library = NewJobs.battleBoatTasks;
            // actor.setHomeBuilding(buildingType);
            // actor.setCity(pCity);
            buildingType.component_docks.addBoatToDock(actor);
            actor.setKingdom(pCity.kingdom);
            actor.setCity(pCity);
            return true;
        }

        public static bool action_spawn_city(WorldTile pTile, string pPowerID)
        {
            Actor actor = ActionLibrary.getActorFromTile(pTile);
            if (actor == null)
            {
                Localization.AddOrSet("spawn_city_dej_fail_one", $"Need A Civ Unit To Make City!");
                WorldTip.showNow("spawn_city_dej_fail_one", true, "top", 3f);
                return false;
            }
            TileZone zone = pTile.zone;
			// if (!zone.goodForNewCity)
			// {
            //     Localization.AddOrSet("spawn_city_dej_fail_two", $"Zone Is Not Suitable For City!");
            //     WorldTip.showNow("spawn_city_dej_fail_two", true, "top", 3f);
			// 	return false;
			// }
			Kingdom kingdom = actor.kingdom;
			if (!kingdom.isNomads() && !kingdom.isCiv())
			{
                Localization.AddOrSet("spawn_city_dej_fail_three", $"Unit Is Not Suitable To Create A City!");
                WorldTip.showNow("spawn_city_dej_fail_three", true, "top", 3f);
				return false;
			}
			if (kingdom != null && kingdom.isNomads())
			{
				kingdom = null;
			}
			City city = World.world.cities.buildNewCity(zone, actor.race, kingdom);
			if (city == null)
			{
                Localization.AddOrSet("spawn_city_dej_fail_four", $"Creating A City Has Failed!");
                WorldTip.showNow("spawn_city_dej_fail_four", true, "top", 3f);
				return false;
			}
			city.newCityEvent();
			city.race = actor.race;
			city.setCulture(actor.data.culture);
			City city2 = actor.city;
			if (city2 != null)
			{
				city2.kingdom.newCityBuiltEvent(city);
				city2.removeCitizen(actor, false, AttackType.Other);
				actor.removeFromCity();
			}
			actor.becomeCitizen(city);
			WorldLog.logNewCity(city);
            Localization.AddOrSet("spawn_city_dej_success", $"A New City Has Been Found By {actor.getName()}");
            WorldTip.showNow("spawn_city_dej_success", true, "top", 3f);
			return true;
        }

        public static bool selectCultureExpandZone(string pPowerID)
        {
            Localization.AddOrSet("expand_culture_zone_dej_selected", "Choose A Culture To Expand!");
            WorldTip.showNow("expand_culture_zone_dej_selected", true, "top", 3f);
            cultureToBeExpanded = null;
            return false;
        }

        public static bool clickCultureExpandZone(WorldTile pTile, string pPowerID)
        {
            Culture culture = pTile.zone.culture;
            if (cultureToBeExpanded == null && culture != null)
            {
                cultureToBeExpanded = culture;
                return true;
            }
            if (cultureToBeExpanded == culture || culture != null)
            {
                return false;
            }
            Localization.AddOrSet("expand_culture_zone_dej_selected", "Culture Has Been Expanded!");
            WorldTip.showNow("expand_culture_zone_dej_selected", true, "top", 3f);
            cultureToBeExpanded.addZone(pTile.zone);
            return true;
        }

        public static bool clickCultureRemoveZone(WorldTile pTile, string pPowerID)
        {
            Culture culture = pTile.zone.culture;
            if (cultureToBeExpanded == null && culture != null)
            {
                cultureToBeExpanded = culture;
                return true;
            }
            if (cultureToBeExpanded == culture)
            {
                Localization.AddOrSet("expand_culture_zone_dej_selected_removed", "Zone Has Been Removed From Culture!");
                WorldTip.showNow("expand_culture_zone_dej_selected_removed", true, "top", 3f);
                cultureToBeExpanded.removeZone(pTile.zone);
            }
            return true;
        }

        public static bool selectBuildingPower(string pPowerID)
        {
            BuildingAsset bAsset = AssetManager.buildings.get(BuildingsWindow.currentBuilding);
            int size = (int)(bAsset.fundament.left + bAsset.fundament.right + 1)/2;
            if (!AssetManager.brush_library.dict.ContainsKey($"sqr_{size.ToString()}"))
            {
                BrushData bData = AssetManager.brush_library.clone($"sqr_{size.ToString()}", "sqr_10");
                bData.size = size;
                bData.generate_action = delegate(BrushData pAsset)
                {
                    int size2 = pAsset.size;
                    new Vector2Int(size2 / 2, size2 / 2);
                    List<BrushPixelData> list2 = new List<BrushPixelData>();
                    for (int k = -size2; k <= size2; k++)
                    {
                        for (int l = -size2; l <= size2; l++)
                        {
                            list2.Add(new BrushPixelData(k, l, (float)size2));
                        }
                    }
                    pAsset.pos = list2.ToArray();
                };
            }
            AssetManager.brush_library.post_init();
            AssetManager.powers.get(pPowerID).forceBrush = $"sqr_{size.ToString()}";
            return false;
        }

        public static bool action_spawn_building(WorldTile pTile, GodPower pPower)
        {
            Building newBuilding = World.world.buildings.addBuilding(BuildingsWindow.currentBuilding, pTile, false, false, BuildPlacingType.New);
            if (newBuilding == null)
            {
                EffectsLibrary.spawnAtTile("fx_bad_place", pTile, 0.25f);
                return false;
            }
            if (newBuilding.asset.cityBuilding && pTile.zone.city != null)
			{
				pTile.zone.city.addBuilding(newBuilding);
				newBuilding.retake();
			} else {
                newBuilding.makeRuins();
            }
            return true;
        }

        public static void turnOnImperialThinking()
        {
            Main.modifyBoolOption("imperialThinkingOption", PowerButtons.GetToggleValue("imperial_thinking_dej"));
            Patches.turnOnCancerFilledAbominationOfAFeature = PowerButtons.GetToggleValue("imperial_thinking_dej");
        }

        public static bool selectExpandZone(string pPowerID)
        {
            Localization.AddOrSet("expand_zone_dej_selected", "Choose A Village To Expand!");
            WorldTip.showNow("expand_zone_dej_selected", true, "top", 3f);
            cityToBeExpanded = null;
            return false;
        }

        public static bool clickCityCapital(WorldTile pTile, string pPowerID)
        {
            City city = pTile.zone.city;
            if (city == null)
            {
                Localization.AddOrSet("force_capital_dej_fail", $"No City To Make Capital!");
                WorldTip.showNow("force_capital_dej_fail", true, "top", 3f);
                return false;
            }
            Localization.AddOrSet("force_capital_dej_success", $"{city.getCityName()} Is Now The Capital Of {city.kingdom.data.name}!");
            WorldTip.showNow("force_capital_dej_success", true, "top", 3f);
            city.kingdom.capital = city;
			city.kingdom.data.capitalID = city.kingdom.capital.data.id;
			city.kingdom.location = city.kingdom.capital.cityCenter;
            return true;
        }


        public static bool clickExpandZone(WorldTile pTile, string pPowerID)
        {
            City city = pTile.zone.city;
            if (cityToBeExpanded == null && city != null)
            {
                cityToBeExpanded = city;
                return true;
            }
            if (cityToBeExpanded == city || city != null)
            {
                return false;
            }
            Localization.AddOrSet("expand_zone_dej_selected", "City Has Been Expanded!");
            WorldTip.showNow("expand_zone_dej_selected", true, "top", 3f);
            cityToBeExpanded.CallMethod("addZone", pTile.zone);
            return true;
        }

        public static bool clickRemoveZone(WorldTile pTile, string pPowerID)
        {
            City city = pTile.zone.city;
            if (city == null)
            {
                return false;
            }
            if (cityToBeExpanded == null && city != null)
            {
                cityToBeExpanded = city;
                return true;
            }
            if (cityToBeExpanded == city)
            {
                Localization.AddOrSet("expand_zone_dej_selected_removed", "Zone Has Been Removed From City!");
                WorldTip.showNow("expand_zone_dej_selected_removed", true, "top", 3f);
                cityToBeExpanded.CallMethod("removeZone", pTile.zone, true);
            }
            return true;
        }

        public static bool clickCityConvert(WorldTile pTile, string pPowerID)
        {
            City city = pTile.zone.city;
            if (city == null)
            {
                return false;
            }
            if (convertCityA == null)
            {
                convertCityA = city;
                Localization.AddOrSet("alliance_dej_selected_first", "Select The Kingdom That $kingdom_A$ Will Convert To!");
                showConvertTip("alliance_dej_selected_first");
                return false;
            }
            if (convertCityB == null && convertCityA == city)
            {
                showConvertTip("whisper_cancelled");
                convertCityA = null;
                convertCityB = null;
                return false;
            }
            if (convertCityB == null)
            {
                convertCityB = city;
            }
            if (convertCityB != convertCityA)
            {
                if ((Kingdom)Reflection.GetField(typeof(City), convertCityB, "kingdom") == (Kingdom)Reflection.GetField(typeof(City), convertCityA, "kingdom") ||
                (Race)Reflection.GetField(typeof(City), convertCityB, "race") != (Race)Reflection.GetField(typeof(City), convertCityA, "race"))
                {
                    showConvertTip("whisper_cancelled");
                    convertCityA = null;
                    convertCityB = null;
                    return false;
                }
                convertCityA.joinAnotherKingdom((Kingdom)Reflection.GetField(typeof(City), convertCityB, "kingdom"));
                Localization.AddOrSet("alliance_dej_success", "$kingdom_A$ Has Now Joined $kingdom_B$!");
                showConvertTip("alliance_dej_success");
            }
            return true;
        } 

        private static void showConvertTip(string pText)
        {
            string text = LocalizedTextManager.getText(pText, null);
            if (convertCityA != null)
            {
                text = text.Replace("$kingdom_A$", convertCityA.name);
            }
            if (convertCityB != null)
            {
                text = text.Replace("$kingdom_B$", convertCityB.name);
            }
            WorldTip.showNow(text, false, "top", 6f);
        }
        
        public static bool selectCityConvert(string pPowerID)
        {
            Localization.AddOrSet("city_convert_dej_selected", "Choose A Village To Convert!");
            WorldTip.showNow("city_convert_dej_selected", true, "top", 3f);
            convertCityA = null;
            convertCityB = null;
            return false;
        }

        public static bool selectWhisperOfAlliance(string pPowerID)
        {
            Localization.AddOrSet("alliance_dej_selected", "Select Another Kingdom To Create Alliance With!");
            WorldTip.showNow("whisper_selected", true, "top", 3f);
            Config.whisperA = null;
            Config.whisperB = null;
            return false;
        }

        private static void showWhisperTip(string pText)
        {
            string text = LocalizedTextManager.getText(pText, null);
            if (Config.whisperA != null)
            {
                text = text.Replace("$kingdom_A$", Config.whisperA.name);
            }
            if (Config.whisperB != null)
            {
                text = text.Replace("$kingdom_B$", Config.whisperB.name);
            }
            WorldTip.showNow(text, false, "top", 6f);
        }

        public static bool clickWhisperOfAlliance(WorldTile pTile, string pPowerID)
        {
            City city = pTile.zone.city;
            if (city == null)
            {
                return false;
            }
            Kingdom kingdom = (Kingdom)Reflection.GetField(typeof(City), city, "kingdom");
            if (Config.whisperA == null)
            {
                Config.whisperA = kingdom;
                Localization.AddOrSet("alliance_dej_selected_first", "Select Another Kingdom To Create Alliance With!");
                showWhisperTip("alliance_dej_selected_first");
                return false;
            }
            if (Config.whisperB == null && Config.whisperA == kingdom)
            {
                showWhisperTip("whisper_cancelled");
                Config.whisperA = null;
                Config.whisperB = null;
                return false;
            }
            if (Config.whisperB == null)
            {
                Config.whisperB = kingdom;
            }
            if (Config.whisperB != Config.whisperA)
            {
                foreach (War war in World.world.wars.getWars(Config.whisperA))
                {
                    if (war.isInWarWith(Config.whisperA, Config.whisperB))
                    {
                        war.removeFromWar(Config.whisperA);
                        war.removeFromWar(Config.whisperB);
                    }
                }
                if (Alliance.isSame(Config.whisperA.getAlliance(), Config.whisperB.getAlliance()))
                {
                    Localization.AddOrSet("alliance_dej_already_allied", "$kingdom_A$ Is Already Allied With $kingdom_B$!");
                    showWhisperTip("alliance_dej_already_allied");
                    Config.whisperB = null;
                    return false;
                }
                Alliance allianceA = Config.whisperA.getAlliance();
                Alliance allianceB = Config.whisperB.getAlliance();
                if (allianceA != null)
                {
                    if (allianceB != null)
                    {
                        World.world.alliances.dissolveAlliance(allianceB);
                    }
                    forceAllianceJoin(allianceA, Config.whisperB, true);
                }
                else
                {
                    forceNewAlliance(Config.whisperA, Config.whisperB);
                }
                Localization.AddOrSet("alliance_dej_success", "$kingdom_A$ Has Successfully Been Allied With $kingdom_B$!");
                showWhisperTip("alliance_dej_success");
                if (Config.whisperA.king != null)
                {
                    Config.whisperA.king.endPlots(null);
                }
                if (Config.whisperB.king != null)
                {
                    Config.whisperB.king.endPlots(null);
                }
                Config.whisperA = null;
                Config.whisperB = null;
            }
            return true;
        }

        private static void forceAllianceJoin(Alliance alliance, Kingdom pKingdom, bool pRecalc)
        {
            alliance.kingdoms_hashset.Add(pKingdom);
            pKingdom.allianceJoin(alliance);
            if (pRecalc)
            {
                alliance.recalculate();
            }
            alliance.data.timestamp_member_joined = World.world.getCurWorldTime();
        }

        public static Alliance forceNewAlliance(Kingdom pKingdom, Kingdom pKingdom2)
        {
            Alliance alliance = World.world.alliances.newObject(null);
            alliance.createAlliance();
            forceAddFounders(alliance, pKingdom, pKingdom2);
            WorldLog.logAllianceCreated(alliance);
            return alliance;
        }

        public static void forceAddFounders(Alliance alliance, Kingdom pKingdom1, Kingdom pKingdom2)
        {
            alliance.data.founder_kingdom_1 = pKingdom1.data.name;
            if (pKingdom1.king != null)
            {
                alliance.data.founder_name_1 = pKingdom1.king.getName();
            }
            forceAllianceJoin(alliance, pKingdom1, true);
            forceAllianceJoin(alliance, pKingdom2, true);
        }

        public static void action_force_citizen(WorldTile pTile = null, string pDropID = null)
        {
            City newCity = pTile.zone.city;
            if (newCity == null)
            {
                return;
            }
            MapBox.instance.getObjectsInChunks(pTile, 3, MapObjectType.Actor);
            List<BaseSimObject> temp_objs = World.world.temp_map_objects;
            for (int i = 0; i < temp_objs.Count; i++)
            {
                Actor pActor = (Actor)temp_objs[i];

                if (!pActor.asset.unit)
                {
                    continue;
                }
                if (pActor.city != null && pActor.city != newCity)
                {
                    pActor.city.removeCitizen(pActor, false, AttackType.Other);
                    pActor.removeFromCity();
                }
                Localization.AddOrSet("force_citizen_success", $"{pActor.getName()} Has Joined The City {newCity.getCityName()}!");
                WorldTip.showNow("force_citizen_success", true, "top", 3f);
                pActor.becomeCitizen(newCity);
            }
        }

        public static void action_give_item(WorldTile pTile = null, string pDropID = null)
        {
            MapBox.instance.getObjectsInChunks(pTile, 3, MapObjectType.Actor);
            List<BaseSimObject> temp_objs = World.world.temp_map_objects;
            for (int i = 0; i < temp_objs.Count; i++)
            {
                Actor pActor = (Actor)temp_objs[i];

                if (!pActor.asset.use_items)
                {
                    continue;
                }
                bool gainedItem = false;
                foreach(KeyValuePair<string, ItemOption> kv in ItemEditorWindow.itemAssets)
                {
                    if (!kv.Value.active)
                    {
                        continue;
                    }
                    ActorEquipmentSlot slot = pActor.equipment.getSlot(kv.Value.asset.equipmentType);
                    ItemData data = ItemGenerator.generateItem(kv.Value.asset, kv.Value.material);
                    data.name = ItemEditorWindow.itemNames[kv.Value.id.ToString()].inputField.text;
                    data.modifiers.Clear();
                    if (ItemEditorWindow.itemModifiers.ContainsKey(kv.Value.id.ToString()))
                    {
                        foreach(ItemAsset modifier in ItemEditorWindow.itemModifiers[kv.Value.id.ToString()])
                        {
                            // ItemGenerator.tryToAddMod(data, modifier);
                            data.modifiers.Add(modifier.id);
                        }
                    }
                    slot.setItem(data);
                    gainedItem = true;
                }
                if (gainedItem)
                {
                    Localization.AddOrSet("give_item_success", $"{pActor.getName()} Has Been Dropped Equipment!");
                    WorldTip.showNow("give_item_success", true, "top", 3f);
                }
                pActor.setStatsDirty();
                pActor.startShake(0.3f, 0.1f, true, true);
                pActor.startColorEffect(ActorColorEffect.White);
            }
        }

        public static void action_warrior(WorldTile pTile = null, string pDropID = null)
        {
            MapBox.instance.getObjectsInChunks(pTile, 3, MapObjectType.Actor);
            List<BaseSimObject> temp_objs = World.world.temp_map_objects;
            for (int i = 0; i < temp_objs.Count; i++)
            {
                Actor pActor = (Actor)temp_objs[i];

                if (!pActor.asset.unit || pActor.city == null || pActor.asset.baby)
                {
                    continue;
                }
                pActor.CallMethod("setProfession", UnitProfession.Warrior, true);
                var pAI = (AiSystemActor)Reflection.GetField(typeof(Actor), pActor, "ai");
                if (pActor.equipment.weapon.isEmpty())
                {
                    City.giveItem(pActor, pActor.city.getEquipmentList(EquipmentType.Weapon), pActor.city);
                }
                if (pActor.city.getArmy() == 0 && pActor.city.army == null)
                {
                    UnitGroup army = MapBox.instance.unitGroupManager.createNewGroup(pActor.city);
                    pActor.city.army = army;
                }
                pActor.city.status.warriors_current++;
                if (Main.savedSettings.boolOptions["IgnoreWarriorLimitOnlyForWarriorPowerOption"])
                {
                    pAI.jobs_library = NewJobs.ignoreWarriorCheckLimitJobs;
                }
                pAI.setJob("attacker");
                pActor.setStatsDirty();
                pActor.startShake(0.3f, 0.1f, true, true);
                pActor.startColorEffect(ActorColorEffect.White);
            }
        }

        public static void action_civilian(WorldTile pTile = null, string pDropID = null)
        {
            MapBox.instance.getObjectsInChunks(pTile, 3, MapObjectType.Actor);
            List<BaseSimObject> temp_objs = World.world.temp_map_objects;
            for (int i = 0; i < temp_objs.Count; i++)
            {
                Actor pActor = (Actor)temp_objs[i];
                if (!pActor.asset.unit || (bool)Reflection.CallMethod(pActor, "isProfession", UnitProfession.Unit) || pActor.city == null || pActor.asset.baby)
                {
                    continue;
                }
                if((bool)Reflection.CallMethod(pActor, "isProfession", UnitProfession.Warrior) && pActor.city != null)
                {
                    if ((UnitGroup)Reflection.GetField(typeof(Actor), pActor, nameof(pActor.unit_group)) != null)
                    {
                        ((UnitGroup)Reflection.GetField(typeof(Actor), pActor,  nameof(pActor.unit_group))).removeUnit(pActor);
                        Reflection.SetField<UnitGroup>(pActor,  nameof(pActor.unit_group), null);
                        pActor.is_group_leader = false;
                    }
                    pActor.city.status.warriors_current--;
                }
                if ((bool)Reflection.CallMethod(pActor, "isProfession", UnitProfession.Leader) && pActor.city != null)
                {
                    pActor.city.removeLeader();
                }
                pActor.CallMethod("setProfession", UnitProfession.Unit, true);
                pActor.setStatsDirty();
                pActor.startShake(0.3f, 0.1f, true, true);
                pActor.startColorEffect(ActorColorEffect.White);
            }
        }

        public static void action_king(WorldTile pTile = null, string pDropID = null)
        {
            MapBox.instance.getObjectsInChunks(pTile, 3, MapObjectType.Actor);
            List<BaseSimObject> temp_objs = World.world.temp_map_objects;
            for (int i = 0; i < temp_objs.Count; i++)
            {
                Actor pActor = (Actor)temp_objs[i];
                if (!pActor.asset.unit || pActor.kingdom == null || pActor.city == null || pActor.asset.baby)
                {
                    continue;
                }
                if ((bool)Reflection.CallMethod(pActor, "isProfession", UnitProfession.Leader) && pActor.city != null)
                {
                    pActor.city.removeLeader();
                }
                if ((UnitGroup)Reflection.GetField(typeof(Actor), pActor, nameof(pActor.unit_group)) != null)
                {
                    ((UnitGroup)Reflection.GetField(typeof(Actor), pActor, nameof(pActor.unit_group))).removeUnit(pActor);
                    Reflection.SetField<UnitGroup>(pActor, nameof(pActor.unit_group), null);
                    pActor.is_group_leader = false;
                }
                Reflection.CallMethod(pActor.kingdom, "removeKing");
                pActor.kingdom.setKing(pActor);
                if (pActor.equipment.weapon.isEmpty())
                {
                    City.giveItem(pActor, pActor.city.getEquipmentList(EquipmentType.Weapon), pActor.city);
                }
                pActor.setStatsDirty();
                pActor.startShake(0.3f, 0.1f, true, true);
                pActor.startColorEffect(ActorColorEffect.White);
            }
        }

        public static void action_level_up(WorldTile pTile = null, string pDropID = null)
        {
            World.world.getObjectsInChunks(pTile, 3, MapObjectType.Actor);
            List<BaseSimObject> temp_objs = World.world.temp_map_objects;
            for (int i = 0; i < temp_objs.Count; i++)
            {
                Actor pActor = (Actor)temp_objs[i];
                ActorData pData = (ActorData)Reflection.GetField(typeof(Actor), pActor, "data");
                pData.level += int.Parse(Main.savedSettings.inputOptions["LevelRate"].value);
                Reflection.SetField(pActor, "event_full_heal", true);
                pActor.setStatsDirty();
                pActor.startShake(0.3f, 0.1f, true, true);
                pActor.startColorEffect(ActorColorEffect.White);
            }
        }

        public static void action_specific_level_up(WorldTile pTile = null, string pDropID = null)
        {
            World.world.getObjectsInChunks(pTile, 3, MapObjectType.Actor);
            List<BaseSimObject> temp_objs = World.world.temp_map_objects;
            for (int i = 0; i < temp_objs.Count; i++)
            {
                Actor pActor = (Actor)temp_objs[i];
                ActorData pData = (ActorData)Reflection.GetField(typeof(Actor), pActor, "data");
                pData.level = int.Parse(Main.savedSettings.inputOptions["LevelRate"].value);
                Reflection.SetField(pActor, "event_full_heal", true);
                pActor.setStatsDirty();
                pActor.startShake(0.3f, 0.1f, true, true);
                pActor.startColorEffect(ActorColorEffect.White);
            }
        }
    }
}
