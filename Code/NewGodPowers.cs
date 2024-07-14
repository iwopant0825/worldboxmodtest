using System;
using System.Collections;
using System.Collections.Generic;
using NCMS;
using NCMS.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ReflectionUtility;

namespace CollectionMod
{
    class NewGodPowers
    {
        public static void init()
        {
            initBuildingPower("building_drop_dej", "b_drop_dej");
            initCreaturePower("creature_drop_dej", SA.unit_human);
            initPowers();
        }

        public static bool callSpawnDrops(WorldTile tTile, GodPower pPower)
        {
            AssetManager.powers.CallMethod("spawnDrops", tTile, pPower);
            return true;
        }

        public static bool callFlashPixel(WorldTile pTile, GodPower pPower)
        {
            AssetManager.powers.CallMethod("flashPixel", pTile, pPower);
            return true;
        }

        public static bool callLoopBrush(WorldTile pTile, GodPower pPower)
        {
            AssetManager.powers.CallMethod("loopWithCurrentBrushPower", pTile, pPower);
            return true;
        }

        public static bool callLoopBrushID(WorldTile pTile, string pPowerID)
        {
            AssetManager.powers.CallMethod("loopWithCurrentBrush", pTile, pPowerID);
            return true;
        }

        public static bool callSpawnUnit(WorldTile pTile, string pPowerID)
        {
            AssetManager.powers.CallMethod("spawnUnit", pTile, pPowerID);
            return true;
        }

        public static Actor spawnUnit(WorldTile pTile, string pPowerID)
        {
            GodPower godPower = AssetManager.powers.get(pPowerID);
            MusicBox.playSound("event:/SFX/UNIQUE/SpawnWhoosh", (float)pTile.pos.x, (float)pTile.pos.y, false, false);
            if (godPower.id == SA.sheep && pTile.Type.lava)
            {
                AchievementLibrary.achievementSacrifice.check(null, null, null);
            }
            EffectsLibrary.spawn("fx_spawn", pTile, null, null, 0f, -1f, -1f);
            string text;
            if (godPower.actor_asset_ids.Count > 0)
            {
                text = godPower.actor_asset_ids.GetRandom<string>();
            }
            else
            {
                text = godPower.actor_asset_id;
            }
            Actor actor = World.world.units.spawnNewUnit(text, pTile, true, godPower.actorSpawnHeight);
            actor.addTrait("miracle_born", false);
            actor.data.age_overgrowth = 18;
            actor.data.had_child_timeout = 8f;
            return actor;
        }

        public static void initPowers()
        {
            createDropPower("warrior_dej", "warrior_dej_drop", NewActions.action_warrior);
            createDropPower("citizen_dej", "citizen_dej_drop", NewActions.action_civilian);
            createDropPower("king_dej", "king_dej_drop", NewActions.action_king);
            createDropPower("level_up_dej", "level_up_dej_drop", NewActions.action_level_up);
            createDropPower("specific_level_up_dej", "specific_level_up_dej_drop", NewActions.action_specific_level_up);
            createDropPower("Item_drop_dej", "Item_drop_dej_drop", NewActions.action_give_item);
            createDropPower("force_citizen_dej", "force_citizen_dej_drop", NewActions.action_force_citizen);
            createSelectPower(
                "alliance_dej", 
                MapMode.Alliances,
                new PowerButtonClickAction(NewActions.selectWhisperOfAlliance), 
                new PowerActionWithID(NewActions.clickWhisperOfAlliance)
            );
            createSelectPower(
                "city_convert_dej",
                MapMode.Cities, 
                new PowerButtonClickAction(NewActions.selectCityConvert), 
                new PowerActionWithID(NewActions.clickCityConvert)
            );
            createSelectPower(
                "force_capital_dej",
                MapMode.Cities, 
                null, 
                new PowerActionWithID(NewActions.clickCityCapital)
            );

            createZonePower(
                "expand_zone_dej",
                MapMode.Cities,
                new PowerButtonClickAction(NewActions.selectExpandZone),
                new PowerActionWithID(NewActions.clickExpandZone)
            );
            createZonePower(
                "remove_zone_dej",
                MapMode.Cities,
                new PowerButtonClickAction(NewActions.selectExpandZone),
                new PowerActionWithID(NewActions.clickRemoveZone)
            );
            createZonePower(
                "expand_culture_zone_dej",
                MapMode.Cultures,
                new PowerButtonClickAction(NewActions.selectCultureExpandZone),
                new PowerActionWithID(NewActions.clickCultureExpandZone)
            );
            createZonePower(
                "remove_culture_zone_dej",
                MapMode.Cultures,
                new PowerButtonClickAction(NewActions.selectCultureExpandZone),
                new PowerActionWithID(NewActions.clickCultureRemoveZone)
            );

            GodPower spawnCityPower = AssetManager.powers.add(new GodPower
            {
                id = "spawn_city_dej",
                name = "spawn_city_dej"
            });
            spawnCityPower.tester_enabled = false;
            spawnCityPower.click_action = (PowerActionWithID)Delegate.Combine(spawnCityPower.click_action, new PowerActionWithID(NewActions.action_spawn_city));
            spawnCityPower.allow_unit_selection = false;

            GodPower boatPower = AssetManager.powers.clone("boat_spawn_dej", "_spawnActor");
            boatPower.name = "boat_spawn_dej";
            boatPower.actor_asset_id = "battle_boat";
            boatPower.click_action = new PowerActionWithID(NewActions.action_spawn_boat);

            GodPower magnetPower = AssetManager.powers.clone("magnet_dej", "magnet");
            magnetPower.name = "magnet_dej";
            magnetPower.click_action = new PowerActionWithID(NewActions.action_useMagnet);
            magnetPower.click_action = (PowerActionWithID)Delegate.Combine(magnetPower.click_action, new PowerActionWithID(AssetManager.powers.fmodDrawingSound));
        }

        private static GodPower createDropPower(string id, string dropID, DropsAction call)
        {
            DropAsset warriorDrop = AssetManager.drops.clone(dropID, "blessing");
            warriorDrop.action_landed = new DropsAction(call);

            GodPower warriorPower = AssetManager.powers.clone(id, "_drops");
            warriorPower.name = id;
            warriorPower.dropID = dropID;
            warriorPower.fallingChance = 0.01f;
            warriorPower.click_power_action = new PowerAction(callSpawnDrops);
            warriorPower.click_power_brush_action = new PowerAction(callLoopBrush);
            return warriorPower;
        }

        private static GodPower createSelectPower(string id, MapMode mode, PowerButtonClickAction callSelect, PowerActionWithID callClick)
        {
            GodPower selectPower = new GodPower();
            selectPower.id = id;
            selectPower.name = id;
            selectPower.force_map_text = mode;
            selectPower.select_button_action = callSelect;
            selectPower.click_special_action = (PowerActionWithID)Delegate.Combine(selectPower.click_special_action, new PowerActionWithID(callClick));
            AssetManager.powers.add(selectPower);
            return selectPower;
        }

        public static void initBuildingPower(string powerID, string dropID)
        {
            GodPower buildingPower = new GodPower();
            buildingPower.id = powerID;
            buildingPower.name = powerID;
            buildingPower.rank = PowerRank.Rank3_good;
            buildingPower.select_button_action = (PowerButtonClickAction)Delegate.Combine(
                buildingPower.select_button_action, 
                new PowerButtonClickAction(NewActions.selectBuildingPower)
            );
            buildingPower.click_power_action = new PowerAction(NewActions.action_spawn_building);
            AssetManager.powers.add(buildingPower);
        }

        private static GodPower createZonePower(string powerID, MapMode mode, PowerButtonClickAction select, PowerActionWithID click)
        {
            GodPower expandPower = AssetManager.powers.clone(powerID, "_terraformTiles");
            expandPower.name = powerID;
            expandPower.force_map_text = mode;
            expandPower.select_button_action = (PowerButtonClickAction)Delegate.Combine(expandPower.select_button_action, select);
            expandPower.click_action = (PowerActionWithID)Delegate.Combine(expandPower.click_action, click);
            expandPower.click_brush_action = (PowerActionWithID)Delegate.Combine(expandPower.click_brush_action, new PowerActionWithID(callLoopBrushID));
            return expandPower;
        }

        public static void initCreaturePower(string powerID, string actorAssetID)
        {
            GodPower creaturePower = AssetManager.powers.clone(powerID, "_spawnActor");
            creaturePower.name = powerID;
            creaturePower.actor_asset_id = actorAssetID;
            creaturePower.click_action = new PowerActionWithID(AssetManager.powers.spawnUnit);
        }
    }
}