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

namespace CollectionMod {
  class NewJobs : MonoBehaviour {
    public static BehaviourTaskActorLibrary battleBoatTasks = new BehaviourTaskActorLibrary();
    public static ActorJobLibrary ignoreWarriorCheckLimitJobs = new ActorJobLibrary();

    public static void init() {
      ignoreWarriorCheckLimitJobs.initJobsCivs();
      ActorJob unitFireJob = new ActorJob {
        id = "unit_on_fire"
      };
      unitFireJob.addTask("short_move");
      unitFireJob.addTask("short_move");
      unitFireJob.addTask("short_move");
      unitFireJob.addTask("run_to_water");
      unitFireJob.addTask("end_job");
      ignoreWarriorCheckLimitJobs.add(unitFireJob);

      battleBoatTasks.initTasksBoats();
      loadBeh();
      loadJobs();
    }

    private static void loadBeh() {
      BehaviourTaskActor battleBoatBeh = new BehaviourTaskActor();
      battleBoatBeh.id = "boat_battle_dej";
      battleBoatBeh.addBeh(new BehBoatCheckHomeDocks());
      battleBoatBeh.addBeh(new BehBattleBoatFindEnemyDock());
      battleBoatBeh.addBeh(new BehBoatFindTileInDock());
      battleBoatBeh.addBeh(new BehGoToTileTarget());
      battleBoatBeh.addBeh(new BehRandomWait(2f, 3f));
      battleBoatBeh.addBeh(new BehSetNextTask("boat_return_to_dock", true));
      AssetManager.tasks_actor.add(battleBoatBeh);
      battleBoatTasks.add(battleBoatBeh);

      BehaviourTaskActor battleBoatFightBeh = new BehaviourTaskActor();
      battleBoatFightBeh.id = "fight";
      battleBoatFightBeh.fighting = true;
      battleBoatFightBeh.addBeh(new BehFightCheckEnemyIsOk());
      battleBoatFightBeh.addBeh(new BehBoatFindWaterTile());
      battleBoatFightBeh.addBeh(new BehGoToTileTarget());
      battleBoatFightBeh.addBeh(new BehRandomWait(1f, 2f));
      battleBoatFightBeh.addBeh(new BehBoatFindWaterTile());
      battleBoatFightBeh.addBeh(new BehGoToTileTarget());
      battleBoatFightBeh.addBeh(new BehRandomWait(1f, 2f));
      battleBoatFightBeh.addBeh(new BehGoToActorTarget("sameRegion", true));
      // battleBoatFightBeh.addBeh(new BehRestartTask());
      battleBoatTasks.add(battleBoatFightBeh);

      BehaviourTaskKingdom doKingLevelChecksBeh = new BehaviourTaskKingdom();
      doKingLevelChecksBeh.id = "do_checks_dej";
      doKingLevelChecksBeh.addBeh(new KingdomBehCheckCapital());
      doKingLevelChecksBeh.addBeh(new KingdomBehCheckKingDej());
      doKingLevelChecksBeh.addBeh(new KingdomBehRandomWait(0f, 1f));
      AssetManager.tasks_kingdom.add(doKingLevelChecksBeh);

      BehaviourTaskCity doLeaderLevelChecksBeh = new BehaviourTaskCity();
      doLeaderLevelChecksBeh.id = "do_checks_dej";
      doLeaderLevelChecksBeh.addBeh(new CityBehCheckLeaderDej());
      doLeaderLevelChecksBeh.addBeh(new CityBehRandomWait(0.1f, 1f));
      doLeaderLevelChecksBeh.addBeh(new CityBehCheckAttackZone());
      doLeaderLevelChecksBeh.addBeh(new CityBehRandomWait(0.1f, 1f));
      doLeaderLevelChecksBeh.addBeh(new CityBehCheckSettleTarget());
      doLeaderLevelChecksBeh.addBeh(new CityBehRandomWait(0.1f, 1f));
      doLeaderLevelChecksBeh.addBeh(new CityBehCheckCitizenTasks());
      doLeaderLevelChecksBeh.addBeh(new CityBehRandomWait(0.1f, 1f));
      AssetManager.tasks_city.add(doLeaderLevelChecksBeh);
    }

    private static void loadJobs() {
      ActorJob boatBattleJob = new ActorJob();
      boatBattleJob.id = "boat_battle_job_dej";
      boatBattleJob.addTask("boat_check_existence");
      boatBattleJob.addTask("boat_danger_check");
      boatBattleJob.addTask("boat_idle");
      boatBattleJob.addTask("boat_battle_dej");
      AssetManager.job_actor.add(boatBattleJob);

      ActorJob newAttackerJob = ignoreWarriorCheckLimitJobs.get("attacker");
      newAttackerJob.tasks.Remove(newAttackerJob.tasks[newAttackerJob.tasks.Count - 1]);

      KingdomJob doKingLevelChecksJob = new KingdomJob();
      doKingLevelChecksJob.id = "kingdom_dej";
      doKingLevelChecksJob.addTask("do_checks_dej");
      doKingLevelChecksJob.addTask("wait1");
      doKingLevelChecksJob.addTask("wait1");
      doKingLevelChecksJob.addTask("wait_random");
      doKingLevelChecksJob.addTask("check_culture");
      AssetManager.job_kingdom.add(doKingLevelChecksJob);

      JobCityAsset doLeaderLevelJob = AssetManager.job_city.add(new JobCityAsset {
        id = "city_dej"
      });
      doLeaderLevelJob.addTask("check_army");
      doLeaderLevelJob.addTask("wait1");
      doLeaderLevelJob.addTask("do_checks_dej");
      doLeaderLevelJob.addTask("wait1");
      doLeaderLevelJob.addTask("border_steal");
      doLeaderLevelJob.addTask("wait1");
      doLeaderLevelJob.addTask("produce_unit");
      doLeaderLevelJob.addTask("wait1");
      doLeaderLevelJob.addTask("give_inventory_item");
      doLeaderLevelJob.addTask("wait1");
      doLeaderLevelJob.addTask("produce_boat");
      doLeaderLevelJob.addTask("wait1");
      doLeaderLevelJob.addTask("supply_kingdom_cities");
      doLeaderLevelJob.addTask("wait1");
      doLeaderLevelJob.addTask("produce_resources");
      doLeaderLevelJob.addTask("wait1");
      doLeaderLevelJob.addTask("check_pop_points");
      doLeaderLevelJob.addTask("wait1");
      doLeaderLevelJob.addTask("check_culture");
      doLeaderLevelJob.addTask("check_farms");
    }

    public static void turnOnIgnoreWarriorLimit() {
      ActorJob attackerJob = AssetManager.job_actor.get("attacker");
      if (Main.savedSettings.boolOptions["IgnoreWarriorLimitOption"]) {
        attackerJob.tasks.Remove(attackerJob.tasks[attackerJob.tasks.Count - 1]);
        return;
      } else if (attackerJob.tasks[attackerJob.tasks.Count - 1].id != "check_warrior_limit") {
        attackerJob.tasks.Add(new TaskContainer<BehaviourActorCondition, Actor> { id = "check_warrior_limit" });
        Debug.Log("off");
        return;
      }
    }

    public static string getNextKingdomJobDej() {
      return "kingdom_dej";
    }

    public static string getNextKingdomJob() {
      return "kingdom";
    }

    public static string getNextCityJobDej() {
      return "city_dej";
    }

    public static string getNextCityJob() {
      return "city";
    }
  }

  public class KingdomBehCheckKingDej : BehaviourActionKingdom {
    private List<Actor> _units = new List<Actor>();

    private List<Kingdom> _new_kingdoms = new List<Kingdom>();

    public override BehResult execute(Kingdom pKingdom) {
      if (pKingdom.data.timer_new_king > 0f) {
        return BehResult.Continue;
      }

      Actor king = pKingdom.king;
      if (king != null && king.data.alive) {
        this.tryToGiveGoldenTooth(king);
        this.checkClan(king);
        return BehResult.Continue;
      }

      this._new_kingdoms.Clear();
      pKingdom.clearKingData();
      this.findKing(pKingdom);
      if (pKingdom.capital != null && pKingdom.capital.leader != null) {
        this._units.Remove(pKingdom.capital.leader);
      }

      return BehResult.Continue;
    }

    private void checkClan(Actor pActor) {
      if (pActor.getClan() == null) {
        BehaviourActionBase<Kingdom>.world.clans.newClan(pActor).addUnit(pActor);
      }
    }

    private void tryToGiveGoldenTooth(Actor pActor) {
      if (pActor.getAge() > 45 && Toolbox.randomChance(0.05f)) {
        pActor.addTrait("golden_tooth", false);
      }
    }

    private void findKing(Kingdom pKingdom) {
      this._units.Clear();
      Actor actor = null;
      if (actor == null) {
        this._units.Clear();
        actor = this.getKingByLevel(pKingdom);
      }

      if (actor == null) {
        return;
      }

      if (actor.city != null && actor.city.leader == actor) {
        actor.city.removeLeader();
      }

      if (pKingdom.capital != null && actor.city != pKingdom.capital) {
        if (actor.city != null) {
          actor.city.removeCitizen(actor, false, AttackType.Other);
        }

        pKingdom.capital.addNewUnit(actor, true);
      }

      pKingdom.setKing(actor);
      WorldLog.logNewKing(pKingdom);
    }

    private Actor getKingByLevel(Kingdom pKingdom) {
      Actor actor = null;
      foreach (City city in pKingdom.cities) {
        foreach (Actor cityActor in city.units.getSimpleList()) {
          this._units.Add(cityActor);
        }
      }

      if (this._units.Count == 0) {
        return null;
      }

      this._units.Shuffle<Actor>();
      int num = 0;
      foreach (Actor actor2 in this._units) {
        int num2 = actor2.data.level;
        if (actor == null || num2 > num) {
          num = num2;
          actor = actor2;
        }
      }

      if (actor == null) {
        return null;
      }

      return actor;
    }
  }

  public class CityBehCheckLeaderDej : BehaviourActionCity {
    public override BehResult execute(City pCity) {
      checkLeaderClan(pCity);
      checkFindLeader(pCity);
      return BehResult.Continue;
    }

    public static void checkLeaderClan(City pCity) {
      Actor leader = pCity.leader;
      if (leader == null) {
        return;
      }

      if (leader.getClan() == null) {
        BehaviourActionBase<City>.world.clans.newClan(leader).addUnit(leader);
      }
    }

    public static void checkFindLeader(City pCity) {
      if (pCity.units.Count < 3) {
        return;
      }

      if (pCity.leader != null) {
        return;
      }

      if (pCity._capture_ticks > 0f) {
        return;
      }

      Actor actor = null;
      actor = null;
      int num = 0;
      foreach (Actor actor2 in pCity.units) {
        if (actor2.data.profession == UnitProfession.Unit) {
          int num3 = actor2.data.level;
          if (actor == null || num3 > num) {
            actor = actor2;
            num = num3;
          }
        }
      }

      if (actor != null) {
        City.makeLeader(actor, pCity);
      }
    }
  }
}