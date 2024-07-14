using ReflectionUtility;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using NCMS;
using NCMS.Utils;
using Newtonsoft.Json;
using ai;
using ai.behaviours;
using HarmonyLib;


namespace CollectionMod {
  class Patches : MonoBehaviour {
    public static Harmony harmony = new Harmony("dej.mymod.wb.collectionmod");
    public static List<string> friendlyRaces = new List<string> { "human", "elf", "dwarf", "orc", "boat" };
    public static bool turnOnCancerFilledAbominationOfAFeature = false;

    public static void init() {
      // this patch is required because the game often has the bool for whether or not there's an attack target on true when the target is null, which the original method assumes can't be the case
      harmony.Patch(
        AccessTools.Method(typeof(BehFightCheckEnemyIsOk), nameof(BehFightCheckEnemyIsOk.execute)),
        prefix: new HarmonyMethod(AccessTools.Method(typeof(Patches), nameof(Patches.BehFightCheckEnemyIsOk_execute_Prefix)))
      );

      harmony.Patch(
        AccessTools.Method(typeof(WindowCreatureInfo), "OnEnable"),
        prefix: new HarmonyMethod(AccessTools.Method(typeof(Patches), "creatureWindowOnEnable_Prefix"))
      );

      harmony.Patch(
        AccessTools.Method(typeof(ButtonEvent), "startLoadSaveSlot"),
        prefix: new HarmonyMethod(AccessTools.Method(typeof(Patches), "startLoadSaveSlot_Prefix"))
      );

      harmony.Patch(
        AccessTools.Method(typeof(MapBox), "finishMakingWorld"),
        postfix: new HarmonyMethod(AccessTools.Method(typeof(Patches), "finishMakingWorld_Postfix"))
      );

      harmony.Patch(
        AccessTools.Method(typeof(City), "updateConquest"),
        prefix: new HarmonyMethod(AccessTools.Method(typeof(Patches), "updateConquest_Prefix"))
      );

      harmony.Patch(
        AccessTools.Method(typeof(BaseSimObject), "canAttackTarget"),
        prefix: new HarmonyMethod(AccessTools.Method(typeof(Patches), "canAttackTarget_Prefix"))
      );

      harmony.Patch(
        AccessTools.Method(typeof(Actor), "addExperience"),
        prefix: new HarmonyMethod(AccessTools.Method(typeof(Patches), "addExperience_Prefix"))
      );

      harmony.Patch(
        AccessTools.Method(typeof(UnitAvatarLoader), "load"),
        prefix: new HarmonyMethod(AccessTools.Method(typeof(Patches), "unitAvatarLoad_Prefix"))
      );

      harmony.Patch(
        AccessTools.Method(typeof(Docks), "getList"),
        postfix: new HarmonyMethod(AccessTools.Method(typeof(Patches), "getBoatList_Postfix"))
      );

      harmony.Patch(
        AccessTools.Method(typeof(Actor), "behaviourActorTargetCheck"),
        prefix: new HarmonyMethod(AccessTools.Method(typeof(Patches), "behaviourActorTargetCheck_Prefix"))
      );

      harmony.Patch(
        AccessTools.Method(typeof(Docks), "getList"),
        postfix: new HarmonyMethod(AccessTools.Method(typeof(Patches), "getBoatList_Postfix"))
      );

      harmony.Patch(
        AccessTools.Method(typeof(Docks), "create"),
        postfix: new HarmonyMethod(AccessTools.Method(typeof(Patches), "create_Postfix"))
      );

      harmony.Patch(
        AccessTools.Method(typeof(CityWindow), "OnEnable"),
        prefix: new HarmonyMethod(AccessTools.Method(typeof(Patches), "cityOnEnable_Prefix"))
      );

      harmony.Patch(
        AccessTools.Method(typeof(Actor), "updateAge"),
        postfix: new HarmonyMethod(AccessTools.Method(typeof(Patches), "updateAge_Postfix"))
      );

      // harmony.Patch(
      //     AccessTools.Method(typeof(TraitsWindow), "checkTraitButtonCreation"),
      //     prefix: new HarmonyMethod(AccessTools.Method(typeof(Patches), "checkTraitButtonCreation_Prefix"))
      // );

      harmony.Patch(
        AccessTools.Method(typeof(Heat), "addTile"),
        prefix: new HarmonyMethod(AccessTools.Method(typeof(Patches), "heatAddTile_Prefix"))
      );

      // harmony.Patch(
      //     AccessTools.Method(typeof(WarManager), "newWar"),
      //     postfix: new HarmonyMethod(AccessTools.Method(typeof(Patches), "newWar_Postfix"))
      // );

      harmony.Patch(
        AccessTools.Method(typeof(DiplomacyHelpers), nameof(DiplomacyHelpers.isWarNeeded)),
        prefix: new HarmonyMethod(AccessTools.Method(typeof(Patches), "isKingdomNeedsWar_Prefix"))
      );

      harmony.Patch(
        AccessTools.Method(typeof(ClanManager), "checkClanMembers"),
        prefix: new HarmonyMethod(AccessTools.Method(typeof(Patches), "checkClanMembers_Prefix"))
      );

      harmony.Patch(
        AccessTools.Method(typeof(WorldLaws), "check"),
        postfix: new HarmonyMethod(AccessTools.Method(typeof(Patches), "worldLawCheck_Postfix"))
      );

      harmony.Patch(
        AccessTools.Method(typeof(Kingdom), "createAI"),
        postfix: new HarmonyMethod(AccessTools.Method(typeof(Patches), "createKingdomAI_Postfix"))
      );

      harmony.Patch(
        AccessTools.Method(typeof(City), "createAI"),
        postfix: new HarmonyMethod(AccessTools.Method(typeof(Patches), "createCityAI_Postfix"))
      );
      harmony.Patch(
        AccessTools.Method(typeof(Actor), "checkLand"),
        prefix: new HarmonyMethod(AccessTools.Method(typeof(Patches), "checkLand_Prefix"))
      );
      harmony.Patch(
        AccessTools.Method(typeof(Docks), nameof(Docks.removeBoatFromDock)),
        prefix: new HarmonyMethod(AccessTools.Method(typeof(Patches), nameof(Patches.removeBoatFromDock_Prefix)))
      );
      // another patch to prevent a seemingly random NullReferenceException
      harmony.Patch(
        AccessTools.Method(typeof(ActionLibrary), nameof(ActionLibrary.pyromaniacEffect)),
        prefix: new HarmonyMethod(AccessTools.Method(typeof(Patches), nameof(Patches.pyromaniacEffect_Prefix)))
      );
    }

    public static bool pyromaniacEffect_Prefix(ref bool __result, BaseSimObject pTarget, WorldTile pTile = null) {
      __result = false;
      if (pTarget.a == null) return false;
      if (pTarget.a.attackTarget == null) return false;
      if (!pTarget.a.has_attack_target) return false;
      if (Toolbox.randomChance(0.95f)) return false;
      WorldTile tileNearby = null;
      if (Toolbox.randomChance(0.5f) && pTarget.a.has_attack_target) {
        if (pTarget.a.attackTarget.currentTile?.chunk == null) return false;
        if (pTarget.currentTile?.chunk == null) return false;
        if (pTarget.a.isEnemyTargetAlive()) tileNearby = ActorTool.getTileNearby(ActorTileTarget.RandomTileWithCivStructures, pTarget.a.attackTarget.currentTile.chunk);
        else tileNearby = ActorTool.getTileNearby(ActorTileTarget.RandomTileWithCivStructures, pTarget.currentTile.chunk);
      }
      if (tileNearby == null) return false;
      if (!ActionLibrary.canThrowBomb(pTarget, tileNearby)) return false;
      City city = tileNearby.zone?.city;
      if (city?.kingdom == null) return false;
      if (pTarget.kingdom == null) return false;
      if (!pTarget.kingdom.isEnemy(city.kingdom)) return false;
      ActionLibrary.throwTorchAtTile(pTarget, tileNearby);
      __result = true;
      return false;
    }

    public static bool removeBoatFromDock_Prefix(Actor pBoat, Docks __instance) {
      if (pBoat.asset == null) return false;
      if (__instance.getList(pBoat.asset) == null) return false;
      if (pBoat.callbacks_on_death == null) return false;
      return true;
    }

    public static void checkLand_Prefix(Actor __instance) {
      if (__instance.ai == null) __instance.ai = new AiSystemActor(__instance);
      if (__instance.has_attack_target && __instance.attackTarget == null) __instance.has_attack_target = false;
    }

    public static bool BehFightCheckEnemyIsOk_execute_Prefix(Actor pActor) {
      if (pActor == null) return false;
      if (pActor.has_attack_target) {
        if (pActor.attackTarget == null) pActor.has_attack_target = false;
      }

      return true;
    }

    public static bool creatureWindowOnEnable_Prefix(WindowCreatureInfo __instance) {
      __instance.nameInput.inputField.characterLimit = 200;
      return true;
    }

    public static bool startLoadSaveSlot_Prefix() {
      HeroicTitles.coroutineIsRunning = false;
      HeroicTitles.instance.StopCoroutine(HeroicTitles.currentCoroutine);
      return true;
    }

    public static void finishMakingWorld_Postfix() {
      if (HeroicTitles.instance != null) {
        HeroicTitles.instance.startChecks();
      }

      return;
    }

    public static bool updateConquest_Prefix(Actor pActor, City __instance) {
      if (!turnOnCancerFilledAbominationOfAFeature) {
        return true;
      }

      if (!pActor.kingdom.isCiv()) {
        return false;
      }

      // if (pActor.kingdom.race != __instance.kingdom.race)
      // {
      //     return false;
      // }
      if (pActor.kingdom != __instance.kingdom && !pActor.kingdom.isEnemy(__instance.kingdom)) {
        return false;
      }

      __instance.addCapturePoints(pActor, 1);
      return false;
    }

    public static bool canAttackTarget_Prefix(BaseSimObject pTarget, BaseSimObject __instance, ref bool __result) {
      if (pTarget == null) {
        __result = false;
        return false;
      }
      if (!__instance.isAlive() || !pTarget.isAlive()) {
        __result = false;
        return false;
      }
      bool flag = __instance.isActor();
      if (flag && (__instance.a == null || __instance.a.asset == null)) {
        __result = false;
        return false;
      }
      if (!turnOnCancerFilledAbominationOfAFeature && ((flag && __instance.a.asset.id != "battle_boat") || !flag)) {
        __result = true;
        return false;
      }

      Race race;
      WeaponType weaponType;
      if (flag) {
        if (__instance.a.asset.skipFightLogic) {
          __result = false;
          return false;
        }
        race = __instance.a.race;
        
        weaponType = __instance.a.s_attackType;
      } else {
        if (__instance.b == null || __instance.b.kingdom == null) {
          __result = false;
          return false;
        }
        race = __instance.b.kingdom.race;
        weaponType = WeaponType.Range;
      }
      if (race == null) {
        __result = false;
        return false;
      }
      if (pTarget.isActor()) {
        Actor a = pTarget.a;
        if (a == null || a.asset == null || a.ai == null || a.professionAsset == null || a.race == null) {
          __result = false;
          return false;
        }
        if (!a.asset.canBeKilledByStuff || a.isInsideSomething() || a.ai.action != null && a.ai.action.special_prevent_can_be_attacked || a.isInMagnet() || flag && __instance.a.s_attackType == WeaponType.Melee && (double)pTarget.zPosition.y > 0.0 || !a.kingdom.asset.mad && !__instance.kingdom.asset.mad && !World.world.worldLaws.world_law_angry_civilians.boolVal && (a.race == race && a.professionAsset.is_civilian || a.race == race & flag && __instance.a.professionAsset.is_civilian) || a.isFlying() && weaponType == WeaponType.Melee) {
          if (__instance == null || __instance.a == null || __instance.a.race == null) {
            __result = false;
            return false;
          }

          City cityOfWhichYouWantToGetTheKingdom = new City();
          Kingdom kingdom = (Kingdom) AccessTools.Field(typeof(City), nameof(City.kingdom)).GetValue(cityOfWhichYouWantToGetTheKingdom);
          string text = __instance.a.race.id;
          bool raceFlag = friendlyRaces.Contains(__instance.a.race.id);
          if (((a.race.id == text && !raceFlag) || (friendlyRaces.Contains(a.race.id) && raceFlag)) &&
              (a.professionAsset.is_civilian || (__instance.a.asset.id == "battle_boat" && a.professionAsset.is_civilian))) {
            __result = false;
            return false;
          }

          if (((a.race.id == text && !raceFlag) || (friendlyRaces.Contains(a.race.id) && raceFlag)) && flag &&
              (__instance.a.professionAsset.is_civilian || (__instance.a.asset.id == "battle_boat" && a.professionAsset.is_civilian))) {
            __result = false;
            return false;
          }
        }
      } else {
        Building b = pTarget.b;
        if (b == null || b.asset == null) {
          __result = false;
          return false;
        }
        if (__instance.kingdom.isCiv() && b.asset.cityBuilding && b.asset.tower && flag && __instance.a.professionAsset.is_civilian && !World.world.worldLaws.world_law_angry_civilians.boolVal && b.kingdom.race == __instance.kingdom.race) {
          __result = false;
          return false;
        }

        if (flag) {
          __result = __instance.a.asset.canAttackBuildings || __instance.a.asset.canAttackBrains && pTarget.kingdom.asset.brain;
          return false;
        }
      }

      if (flag) {
        if (__instance.a.asset.oceanCreature && !__instance.a.asset.landCreature && __instance.a.asset.id != "battle_boat") {
          if (!pTarget.isInLiquid() || !pTarget.currentTile.isSameIsland(__instance.currentTile)) {
            __result = false;
            return false;
          }
        } else if (weaponType == WeaponType.Melee && pTarget.isInLiquid() && !__instance.a.asset.oceanCreature) {
          __result = false;
          return false;
        }
      }

      __result = true;
      return false;
    }

    public static bool canAttackTarget_Prefix_old(BaseSimObject pTarget, BaseSimObject __instance, ref bool __result) {
      bool flag = __instance.isActor();
      if (!turnOnCancerFilledAbominationOfAFeature && ((flag && __instance.a.asset.id != "battle_boat") || !flag)) {
        return true;
      }

      if (pTarget == null || pTarget.object_destroyed) {
        Debug.Log("canAttackTarget-destroyed?");
        __result = false;
        return false;
      }

      if (!pTarget.isAlive()) {
        __result = false;
        return false;
      }

      string text;
      WeaponType weaponType;
      bool raceFlag = false;
      if (flag) {
        if (__instance.a.asset.skipFightLogic) {
          __result = false;
          return false;
        }

        text = __instance.a.race.id;
        weaponType = __instance.a.s_attackType;
        raceFlag = friendlyRaces.Contains(__instance.a.race.id);
      } else {
        text = __instance.b.asset.race;
        weaponType = WeaponType.Range;
      }

      if (pTarget.isActor()) {
        Actor actor = pTarget.a;
        if (!actor.asset.canBeKilledByStuff) {
          __result = false;
          return false;
        }

        if (actor.isInsideSomething()) {
          __result = false;
          return false;
        }

        if (actor.ai.action != null && actor.ai.action.special_prevent_can_be_attacked) {
          __result = false;
          return false;
        }

        if (pTarget.a.isInMagnet()) {
          __result = false;
          return false;
        }

        if (!actor.kingdom.asset.mad && !__instance.kingdom.asset.mad && !World.world.worldLaws.world_law_angry_civilians.boolVal) {
          if (((actor.race.id == text && !raceFlag) || (friendlyRaces.Contains(actor.race.id) && raceFlag)) &&
              (actor.professionAsset.is_civilian || (__instance.a.asset.id == "battle_boat" && actor.professionAsset.is_civilian))) {
            __result = false;
            return false;
          }

          if (((actor.race.id == text && !raceFlag) || (friendlyRaces.Contains(actor.race.id) && raceFlag)) && flag &&
              (__instance.a.professionAsset.is_civilian || (__instance.a.asset.id == "battle_boat" && actor.professionAsset.is_civilian))) {
            __result = false;
            return false;
          }
        }

        if (actor.isFlying() && weaponType == WeaponType.Melee) {
          __result = false;
          return false;
        }
      } else {
        Building building = pTarget.b;
        if (__instance.kingdom.isCiv() && building.asset.cityBuilding && !building.asset.tower && raceFlag && /*__instance.a.professionAsset.is_civilian &&*/ !World.world.worldLaws.world_law_angry_civilians.boolVal /*&& building.kingdom.race == __instance.kingdom.race*/) {
          __result = false;
          return false;
        }

        if (__instance.kingdom.isNomads() && building.asset.cityBuilding) {
          __result = false;
          return false;
        }

        if (flag && !__instance.a.asset.canAttackBuildings) {
          __result = __instance.a.asset.canAttackBrains && pTarget.kingdom != null && pTarget.kingdom.asset.brain;
          return false;
        }
      }

      if (flag) {
        if (__instance.a.asset.oceanCreature && !__instance.a.asset.landCreature && __instance.a.asset.id != "battle_boat") {
          if (!pTarget.isInLiquid()) {
            __result = false;
            return false;
          }

          if (!pTarget.currentTile.isSameIsland(__instance.currentTile)) {
            __result = false;
            return false;
          }
        } else if (weaponType == WeaponType.Melee && pTarget.isInLiquid() && !__instance.a.asset.oceanCreature) {
          __result = false;
          return false;
        }
      }

      __result = true;
      return false;
    }

    public static bool addExperience_Prefix(Actor __instance, int pValue) {
      if (!__instance.asset.canLevelUp) {
        return false;
      }

      if (!__instance.data.alive) {
        return false;
      }

      int num = 10;
      if (Main.savedSettings.inputOptions["levelLimit"].active) {
        num = int.Parse(Main.savedSettings.inputOptions["levelLimit"].value);
      }

      Culture culture = __instance.getCulture();
      if (culture != null) {
        num += culture.getMaxLevelBonus();
      }

      num += (int)__instance.kingdom.stats.bonus_max_unit_level.value;
      string talentID = CustomizableRPG.getTalent(__instance);
      if (!string.IsNullOrEmpty(talentID)) {
        num = Main.savedSettings.crpgTraits[talentID].talentLevelCap;
      }

      if (__instance.data.level >= num) {
        return false;
      }

      int expToLevelup = __instance.getExpToLevelup();
      __instance.data.experience += pValue;
      if (__instance.data.experience >= expToLevelup) {
        __instance.data.experience = 0;
        __instance.data.level++;
        if (__instance.data.level == num) {
          __instance.data.experience = expToLevelup;
          if (__instance.asset.flag_turtle) {
            AchievementLibrary.achievementNinjaTurtle.check(null, null, null);
          }
        }

        __instance.setStatsDirty();
        if (__instance.data.level % 10 == 0) {
          __instance.event_full_heal = true;
        }
      }

      return false;
    }

    public static bool unitAvatarLoad_Prefix(UnitAvatarLoader __instance, Actor pActor) {
      while (__instance.transform.childCount > 0) {
        Transform child = __instance.transform.GetChild(0);
        child.SetParent(null);
        Destroy(child.gameObject);
      }

      __instance.transform.localScale = new Vector3(pActor.asset.inspectAvatarScale * __instance.avatarSize, pActor.asset.inspectAvatarScale * __instance.avatarSize, pActor.asset.inspectAvatarScale);
      Sprite spriteToRender = pActor.getSpriteToRender();
      __instance.showSpritePart(spriteToRender, pActor, new Vector3 {
        x = pActor.asset.inspectAvatar_offset_x,
        y = pActor.asset.inspectAvatar_offset_y
      });
      if (pActor.asset.use_items && !pActor.equipment.weapon.isEmpty()) {
        AnimationFrameData animationFrameData = pActor.getAnimationFrameData();
        if (animationFrameData == null) {
          return false;
        }

        if (!animationFrameData.showItem) {
          return false;
        }

        Sprite item = ActorAnimationLoader.getItem(pActor.getWeaponTextureId());
        __instance.showSpritePart(item, pActor, new Vector3 {
          x = pActor.asset.inspectAvatar_offset_x + animationFrameData.posItem.x,
          y = pActor.asset.inspectAvatar_offset_y + animationFrameData.posItem.y
        });
      }

      __instance.gameObject.name = "UnitAvatar_" + pActor.data.id;
      return false;
    }

    public static void getBoatList_Postfix(ActorAsset pAsset, Docks __instance, ref HashSet<Actor> __result) {
      string id = pAsset.id;
      if (id != null && __instance.building != null) {
        if (__instance.building.gameObject.GetComponent<BoatManager>() == null) {
          __instance.building.gameObject.AddComponent<BoatManager>();
        }

        if (id == "battle_boat") {
          __result = __instance.building.gameObject.GetComponent<BoatManager>().battleBoats;
        }
      }
    }

    public static bool behaviourActorTargetCheck_Prefix(Actor __instance, ref bool __result) {
      if (__instance.asset.skipFightLogic) {
        __result = false;
        return false;
      }

      if (__instance.attackTarget != null) {
        if (!__instance.attackTarget.base_data.alive || (__instance.kingdom != null && __instance.kingdom.isCiv() && !__instance.attackTarget.kingdom.isEnemy(__instance.kingdom))) {
          __instance.attackTarget = null;
        } else {
          if (__instance.kingdom != null && __instance.kingdom.isCiv()) {
            __instance.kingdom.isEnemy(__instance.attackTarget.kingdom);
          }

          if (__instance.canAttackTarget(__instance.attackTarget)) {
            if ((__instance.attackTimer > 0f || (!__instance.isAttackReady() && __instance.isInAttackRange(__instance.attackTarget))) && __instance.asset.id != "battle_boat") {
              __instance.stopMovement();
              __result = true;
              return false;
            }

            if (__instance.asset.id != "battle_boat" && __instance.tryToAttack(__instance.attackTarget) /*&& __instance.asset.id != "battle_boat"*/) {
              __instance.stopMovement();
              __result = true;
              return false;
            }
          }
        }
      }

      __result = false;
      return false;
    }

    public static void create_Postfix(Docks __instance) {
      __instance.building.gameObject.AddComponent<BoatManager>();
    }

    public static bool cityOnEnable_Prefix(CityWindow __instance) {
      EditResourcesWindow.currentCity = __instance.city;
      return true;
    }

    public static void updateAge_Postfix(Actor __instance) {
      if (!__instance.isAlive() || !Main.savedSettings.inputOptions["expRateOption"].active) {
        return;
      }

      if (!__instance.asset.unit) {
        return;
      }

      __instance.addExperience(int.Parse(Main.savedSettings.inputOptions["expRateOption"].value));
    }

    public static bool checkTraitButtonCreation_Prefix(TraitsWindow __instance) {
      __instance._all_traits_buttons.Clear();
      __instance.dict_groups.Clear();
      foreach (Transform child in __instance.transform_content) {
        if (child.name == "TraitGroup(Clone)") {
          Destroy(child.gameObject);
        }
      }

      __instance._listInitiated = false;
      return true;
    }

    public static bool heatAddTile_Prefix(Heat __instance, WorldTile pTile, int pHeat = 1) {
      if (pTile.heat == 0) {
        __instance.tiles.Add(pTile);
      }

      pTile.heat += pHeat;
      if (pTile.heat > 404) {
        pTile.heat = 404;
      }

      if (pTile.heat > 5) {
        if (pTile.building != null && pTile.building.data.alive) {
          pTile.building.getHit(0f, true, AttackType.Other, null, true, false);
        }

        if (pTile.Type.layerType == TileLayerType.Ocean) {
          MapAction.removeLiquid(pTile);
          World.world.particlesSmoke.spawn(pTile.posV3);
        }

        if (pTile.isTemporaryFrozen()) {
          pTile.unfreeze(1);
        }

        if (pTile.Type.greyGoo) {
          pTile.startFire(false);
        }

        pTile.setBurned(-1);
      }

      if (pTile.heat > 10) {
        if (pTile.Type.burnable) {
          if (pTile.Type.IsType("tnt") || pTile.Type.IsType("tnt_timed")) {
            AchievementLibrary.achievementTntAndHeat.check(null, null, null);
          }

          pTile.startFire(false);
        }

        if (pTile.building != null && pTile.building.data.alive) {
          pTile.building.getHit(0f, true, AttackType.Other, null, true, false);
        }

        if (pTile.building != null) {
          pTile.startFire(false);
        }

        pTile.doUnits(delegate(Actor tActor) {
          if (tActor.asset.very_high_flyer) {
            return;
          }

          ActionLibrary.addBurningEffectOnTarget(null, tActor, null);
          tActor.getHit(50f, true, AttackType.Other, null, true, false);
        });
      }

      if (pTile.heat > 20) {
        if (pTile.Type.explodable && pTile.explosionWave == 0) {
          World.world.explosionLayer.explodeBomb(pTile, false);
        }

        if (pTile.building != null) {
          pTile.startFire(false);
        }

        if (pTile.top_type != null) {
          MapAction.decreaseTile(pTile, "flash");
        }
      }

      if (pTile.heat > 30) {
        if (pTile.Type.lava) {
          LavaHelper.addLava(pTile, "lava3");
        }

        if (pTile.Type.IsType("soil_low") || pTile.Type.IsType("soil_high")) {
          pTile.setTileType("sand");
        }

        if (pTile.Type.road) {
          pTile.setTileType("sand");
        }
      }

      if (Main.savedSettings.boolOptions["NoLavaWhenApplyingHeatOption"]) {
        return false;
      }

      if (pTile.heat > 100 && pTile.Type.IsType("sand")) {
        LavaHelper.addLava(pTile, "lava3");
      }

      if (pTile.heat > 160) {
        if (pTile.Type.IsType("mountains")) {
          LavaHelper.addLava(pTile, "lava3");
        }

        if (pTile.Type.IsType("hills")) {
          LavaHelper.addLava(pTile, "lava3");
        }
      }

      return false;
    }

    public static void newWar_Postfix(War __result) {
      return;
    }

    public static bool isKingdomNeedsWar_Prefix(Kingdom pKingdom, ref bool __result) {
      if (pKingdom.cities.Count == 0) {
        __result = false;
        return false;
      }

      if (pKingdom.capital == null) {
        __result = false;
        return false;
      }

      if (pKingdom.data.timestamp_last_war != -1f && World.world.getYearsSince(pKingdom.data.timestamp_last_war) <= SimGlobals.m.diplomacy_years_war_timeout) {
        __result = false;
        return false;
      }

      if (DiplomacyHelpers.wars.getWars(pKingdom).ToList().Count > 0) {
        __result = false;
        return false;
      }

      if (pKingdom.getArmy() <= SimGlobals.m.diplomacy_years_war_min_warriors) {
        __result = false;
        return false;
      }

      float num = (float)pKingdom.getPopulationTotal();
      float num2 = (float)pKingdom.getPopulationTotalPossible();
      float popNum = Main.savedSettings.boolOptions["FasterWarPlotsOption"] ? 0.3f : 0.6f;
      CityPlaceFinder cityPlaceFinder = World.world.city_zone_helper.city_place_finder;
      if (pKingdom.cities.Count == 3 || pKingdom.cities.Count == 2) {
        if (!cityPlaceFinder.hasPossibleZones()) {
          __result = true;
          return false;
        }

        if (cityPlaceFinder.zones.Count > 200 && !Main.savedSettings.boolOptions["FasterWarPlotsOption"]) {
          __result = false;
          return false;
        }

        if (num < num2 * popNum) {
          __result = false;
          return false;
        }
      }

      if (pKingdom.cities.Count == 1) {
        if (!cityPlaceFinder.hasPossibleZones()) {
          __result = true;
          return false;
        }

        if (cityPlaceFinder.zones.Count > 200 && !Main.savedSettings.boolOptions["FasterWarPlotsOption"]) {
          __result = false;
          return false;
        }

        City city = pKingdom.cities[0];
        if (num < num2 * popNum) {
          __result = false;
          return false;
        }
      }

      __result = true;
      return false;
    }

    public static bool checkClanMembers_Prefix(ClanManager __instance) {
      if (!World.world.worldLaws.world_law_diplomacy.boolVal) {
        return false;
      }

      for (int i = 0; i < __instance.list.Count; i++) {
        foreach (Actor actor in __instance.list[i].units.Values) {
          if (actor.data.alive) {
            __instance.updatePointsGain(actor);
            if ( /*World.world.getWorldTimeElapsedSince(__instance._timestamp_last_plot) >= 10f &&*/ actor.getAge() > 18) {
              List<Plot> plotsFor = World.world.plots.getPlotsFor(actor, true);
              if (plotsFor == null || plotsFor.Count <= 0) {
                if (actor.isKing()) {
                  __instance.checkActionKing(actor);
                } else if (actor.isCityLeader()) {
                  __instance.checkActionLeader(actor);
                }
              }
            }
          }
        }
      }

      return false;
    }

    public static void worldLawCheck_Postfix() {
      foreach (EraAsset eraAsset in AssetManager.era_library.list) {
        if (World.world.worldLaws.dict.ContainsKey(eraAsset.id)) {
          continue;
        }

        World.world.worldLaws.add(new PlayerOptionData(eraAsset.id) {
          boolVal = true
        });
      }
    }

    public static void createKingdomAI_Postfix(Kingdom __instance) {
      if (!Main.savedSettings.inputOptions["WorldLawElectionsOption"].active) {
        return;
      }

      __instance.ai.nextJobDelegate = new GetNextJobID(NewJobs.getNextKingdomJobDej);
    }

    public static void createCityAI_Postfix(City __instance) {
      if (!Main.savedSettings.inputOptions["WorldLawElectionsOption"].active) {
        return;
      }

      __instance.ai.nextJobDelegate = new GetNextJobID(NewJobs.getNextCityJobDej);
    }
  }
}