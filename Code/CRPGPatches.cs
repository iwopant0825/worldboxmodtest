using ReflectionUtility;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Globalization;
using System.Linq;
using System.Text;
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
using Object = UnityEngine.Object;

namespace CollectionMod {
  class CRPGPatches : MonoBehaviour {
    private static float updateStatsStartTime = 0;

    public static void init() {
      Patches.harmony.Patch(
        AccessTools.Method(typeof(Actor), "getExpToLevelup"),
        prefix: new HarmonyMethod(AccessTools.Method(typeof(CRPGPatches), "getExpToLevelup_Prefix"))
      );

      Localization.AddOrSet("crpg_age", "CRPG Max Age");
      Localization.AddOrSet("crpg_title", "CRPG Title");
      Patches.harmony.Patch(
        AccessTools.Method(typeof(WindowCreatureInfo), "OnEnable"),
        prefix: new HarmonyMethod(AccessTools.Method(typeof(CRPGPatches), "windowCreature_OnEnable_Prefix"))
      );

      Patches.harmony.Patch(
        AccessTools.Method(typeof(Actor), "updateAge"),
        prefix: new HarmonyMethod(AccessTools.Method(typeof(CRPGPatches), "updateAge_Prefix"))
      );

      // This patch might be causing issues with trait actions, unsure
      Patches.harmony.Patch(
        AccessTools.Method(typeof(Actor), "isAttackReady"),
        postfix: new HarmonyMethod(AccessTools.Method(typeof(CRPGPatches), "isAttackReady_Postfix"))
      );

      Patches.harmony.Patch(
        AccessTools.Method(typeof(ActorBase), "updateStats"),
        // prefix: new HarmonyMethod(AccessTools.Method(typeof(CRPGPatches), "updateStats_Prefix")),
        postfix: new HarmonyMethod(AccessTools.Method(typeof(CRPGPatches), "updateStats_Postfix"))
      );

      // Patches.harmony.Patch(
      //     AccessTools.Method(typeof(ActorBase), "updateStats"),
      //     postfix: new HarmonyMethod(AccessTools.Method(typeof(CRPGPatches), "updateStats_Postfix"))
      // );

      Patches.harmony.Patch(
        AccessTools.Method(typeof(Projectile), "checkHit"),
        prefix: new HarmonyMethod(AccessTools.Method(typeof(CRPGPatches), "checkHit_Prefix"))
      );
    }

    public static bool getExpToLevelup_Prefix(Actor __instance, ref int __result) {
      if (!Main.savedSettings.boolOptions["CRPGOption"]) {
        return true;
      }

      string talentID = CustomizableRPG.getTalent(__instance);
      int expGap = 20;
      if (Main.savedSettings.inputOptions["CRPG-expGapOption"].active) {
        expGap = int.Parse(Main.savedSettings.inputOptions["CRPG-expGapOption"].value);
      }

      int baseNum = 100 + (__instance.data.level - 1) * expGap;
      if (!string.IsNullOrEmpty(talentID)) {
        baseNum += (int)(baseNum * (Main.savedSettings.crpgTraits[talentID].EXPRequirement / 100f));
      }

      __result = baseNum;
      return false;
    }

    public static bool windowCreature_OnEnable_Prefix(WindowCreatureInfo __instance) {
      if (!Main.savedSettings.boolOptions["CRPGOption"]) {
        return true;
      }

      __instance.prepare();
      if (__instance.actor == null) {
        return false;
      }

      if (!__instance.actor.isAlive()) {
        return false;
      }

      __instance.pool_traits.clear(true);
      __instance.pool_equipment.clear(true);
      __instance.pool_status.clear(true);
      AchievementLibrary.achievementTheBroken.check(null, null, null);
      AchievementLibrary.achievementTheDemon.check(null, null, null);
      AchievementLibrary.achievementTheKing.check(null, null, null);
      AchievementLibrary.achievementTheAccomplished.check(null, null, null);
      if (__instance.actor.kingdom != null && __instance.actor.kingdom.isCiv()) {
        __instance.kingdomBanner.gameObject.SetActive(true);
        __instance.kingdomBanner.load(__instance.actor.kingdom);
      }

      Clan clan = World.world.clans.get(__instance.actor.data.clan);
      if (clan != null) {
        __instance.clanBanner.gameObject.SetActive(true);
        __instance.clanBanner.load(clan);
      }

      if (__instance.actor.asset.can_edit_traits) {
        __instance.buttonTraitEditor.SetActive(true);
      }

      __instance.nameInput.setText(__instance.actor.getName());
      if (__instance.actor.kingdom.isCiv()) {
        __instance.nameInput.textField.color = __instance.actor.kingdom.getColor().getColorText();
      } else {
        __instance.nameInput.textField.color = Toolbox.color_log_neutral;
      }

      __instance.health.setBar((float)__instance.actor.data.health, (float)__instance.actor.getMaxHealth(), "/" + __instance.actor.getMaxHealth().ToString(), true, false, true, false);
      if (__instance.actor.asset.needFood || __instance.actor.asset.unit) {
        __instance.hunger.gameObject.SetActive(true);
        int num = (int)((float)__instance.actor.data.hunger / (float)__instance.actor.asset.maxHunger * 100f);
        __instance.hunger.setBar((float)num, 100f, "%", true, false, true, false);
      }

      __instance.damage.gameObject.SetActive(true);
      __instance.armor.gameObject.SetActive(true);
      __instance.speed.gameObject.SetActive(true);
      __instance.attackSpeed.gameObject.SetActive(true);
      __instance.crit.gameObject.SetActive(true);
      __instance.diplomacy.gameObject.SetActive(true);
      __instance.warfare.gameObject.SetActive(true);
      __instance.stewardship.gameObject.SetActive(true);
      __instance.intelligence.gameObject.SetActive(true);
      if (!__instance.actor.asset.unit) {
        __instance.diplomacy.gameObject.SetActive(false);
        __instance.warfare.gameObject.SetActive(false);
        __instance.stewardship.gameObject.SetActive(false);
        __instance.intelligence.gameObject.SetActive(false);
      }

      if (!__instance.actor.asset.inspect_stats) {
        __instance.damage.gameObject.SetActive(false);
        __instance.armor.gameObject.SetActive(false);
        __instance.speed.gameObject.SetActive(false);
        __instance.diplomacy.gameObject.SetActive(false);
        __instance.attackSpeed.gameObject.SetActive(false);
        __instance.crit.gameObject.SetActive(false);
      }

      int num2 = (int)__instance.actor.stats[S.damage];
      int num3 = (int)((float)num2 * __instance.actor.stats[S.damage_range]);
      __instance.damage.text.text = num3.ToString() + "-" + num2.ToString();
      __instance.armor.setValue(__instance.actor.stats[S.armor], "%", "", false);
      __instance.speed.setValue(__instance.actor.stats[S.speed], "", "", false);
      __instance.crit.setValue(__instance.actor.stats[S.critical_chance] * 100f, "%", "", false);
      __instance.attackSpeed.setValue(__instance.actor.stats[S.attack_speed], "", "", false);
      __instance.showAttribute(__instance.diplomacy, (int)__instance.actor.stats[S.diplomacy]);
      __instance.showAttribute(__instance.stewardship, (int)__instance.actor.stats[S.stewardship]);
      __instance.showAttribute(__instance.intelligence, (int)__instance.actor.stats[S.intelligence]);
      __instance.showAttribute(__instance.warfare, (int)__instance.actor.stats[S.warfare]);
      Sprite sprite = (Sprite)Resources.Load("ui/Icons/" + __instance.actor.asset.icon, typeof(Sprite));
      __instance.icon.sprite = sprite;
      __instance.avatarElement.show(__instance.actor);
      if (__instance.actor.asset.canBeFavorited) {
        __instance.iconFavorite.transform.parent.gameObject.SetActive(true);
      }

      __instance.age.setValue((float)__instance.actor.getAge(), "", "", false);
      __instance.showStat("birthday", __instance.actor.getBirthday());
      __instance.showStat("crpg_age", (int)__instance.actor.stats[S.max_age] +
                                      (int)(Mathf.Floor(__instance.actor.data.level / 10) * int.Parse(Main.savedSettings.inputOptions["CRPG-levelAgeIncreaseOption"].value)));
      if (__instance.actor.asset.inspect_kills) {
        __instance.showStat("creature_statistics_kills", __instance.actor.data.kills);
      }

      if (__instance.actor.asset.inspect_experience) {
        __instance.showStat("creature_statistics_character_experience", __instance.actor.data.experience.ToString() + "/" + __instance.actor.getExpToLevelup().ToString());
      }

      if (__instance.actor.asset.inspect_experience) {
        __instance.showStat("creature_statistics_character_level", __instance.actor.data.level);
        __instance.showStat("crpg_title", CustomizableRPG.getLevelTitle(__instance.actor.data.level));
      }

      if (__instance.actor.asset.inspect_children) {
        __instance.showStat("creature_statistics_children", __instance.actor.data.children);
      }

      if (__instance.actor.asset.unit && !__instance.actor.asset.baby && !string.IsNullOrEmpty(__instance.actor.data.favoriteFood)) {
        LocalizedTextManager.getText(__instance.actor.data.favoriteFood, null);
        __instance.favoriteFoodBg.gameObject.SetActive(true);
        __instance.favoriteFoodSprite.gameObject.SetActive(true);
        __instance.favoriteFoodSprite.sprite = AssetManager.resources.get(__instance.actor.data.favoriteFood).getSprite();
      }

      if (__instance.actor.asset.unit) {
        __instance.moodBG.gameObject.SetActive(true);
        __instance.showStat("creature_statistics_mood", LocalizedTextManager.getText("mood_" + __instance.actor.data.mood, null));
        MoodAsset moodAsset = AssetManager.moods.get(__instance.actor.data.mood);
        __instance.moodSprite.sprite = moodAsset.getSprite();
        if (__instance.actor.s_personality != null) {
          __instance.showStat("creature_statistics_personality", LocalizedTextManager.getText("personality_" + __instance.actor.s_personality.id, null));
        }

        if (__instance.actor.hasClan()) {
          __instance.showStat(S.influence, __instance.actor.getInfluence());
        }
      }

      Text text8 = __instance.text_description;
      text8.text += "\n";
      Text text9 = __instance.text_values;
      text9.text += "\n";
      if (__instance.actor.asset.inspect_home) {
        string text3 = "creature_statistics_homeVillage";
        object obj = ((__instance.actor.city != null) ? __instance.actor.city.getCityName() : "??");
        Kingdom kingdom = __instance.actor.kingdom;
        string text4;
        if (kingdom == null) {
          text4 = null;
        } else {
          ColorAsset kingdomColor = kingdom.kingdomColor;
          text4 = ((kingdomColor != null) ? kingdomColor.color_text : null);
        }

        __instance.showStat(text3, obj, text4);
      }

      if (__instance.actor.kingdom != null && __instance.actor.kingdom.isCiv()) {
        string text5 = "kingdom";
        object name = __instance.actor.kingdom.name;
        Kingdom kingdom2 = __instance.actor.kingdom;
        string text6;
        if (kingdom2 == null) {
          text6 = null;
        } else {
          ColorAsset kingdomColor2 = kingdom2.kingdomColor;
          text6 = kingdomColor2?.color_text;
        }

        __instance.showStat(text5, name, text6);
      }

      Culture culture = World.world.cultures.get(__instance.actor.data.culture);
      if (culture != null) {
        string text7 = "";
        text7 += culture.data.name;
        text7 = text7 + "[" + culture.followers + "]";
        text7 = Toolbox.coloredString(text7, culture.getColor().color_text);
        __instance.showStat("culture", text7);
        __instance.cultureBanner.gameObject.SetActive(true);
        __instance.cultureBanner.load(culture);
      }

      if (__instance.actor.asset.isBoat) {
        Boat component = __instance.actor.GetComponent<Boat>();
        __instance.showStat("passengers", component._passengers.Count);
        if (component.isState(BoatState.TransportDoLoading)) {
          __instance.showStat("status", LocalizedTextManager.getText("status_waiting_for_passengers", null));
        }
      }

      __instance.text_description.GetComponent<LocalizedText>().checkSpecialLanguages();
      __instance.text_values.GetComponent<LocalizedText>().checkSpecialLanguages();
      if (LocalizedTextManager.isRTLLang()) {
        __instance.text_description.alignment = TextAnchor.UpperRight;
        __instance.text_values.alignment = TextAnchor.UpperLeft;
      } else {
        __instance.text_description.alignment = TextAnchor.UpperLeft;
        __instance.text_values.alignment = TextAnchor.UpperRight;
      }

      if (__instance.actor.city != null) {
        __instance.buttonCity.SetActive(true);
      }

      if (__instance.actor.hasClan() || __instance.actor.kingdom.isCiv()) {
        __instance.backgroundBottomLeft.SetActive(true);
      }

      if (__instance.actor.city != null || __instance.actor.hasCulture()) {
        __instance.backgroundBottomRight.SetActive(true);
      }

      __instance.updateFavoriteIconFor(__instance.actor);
      __instance.loadTraits();
      __instance.loadStatusEffects();
      __instance.loadEquipment();
      return false;
    }

    public static bool updateAge_Prefix(Actor __instance) {
      if (!Main.savedSettings.boolOptions["CRPGOption"]) {
        return true;
      }

      if (!dataUpdateAge(__instance, __instance.race, __instance.asset) && !__instance.hasTrait("immortal")) {
        __instance.killHimself(false, AttackType.Age, true, true, true);
        return false;
      }

      if (__instance.city != null) {
        if (__instance.isKing()) {
          if (!Main.savedSettings.inputOptions["CRPG-kingExpGainOption"].active) {
            __instance.addExperience(20);
          } else {
            __instance.addExperience(int.Parse(Main.savedSettings.inputOptions["CRPG-kingExpGainOption"].value));
          }
        }

        if (__instance.isCityLeader()) {
          if (!Main.savedSettings.inputOptions["CRPG-leaderExpGainOption"].active) {
            __instance.addExperience(10);
          } else {
            __instance.addExperience(int.Parse(Main.savedSettings.inputOptions["CRPG-leaderExpGainOption"].value));
          }
        }
      }

      float num = (float)__instance.getAge();
      if (__instance.asset.unit && num > 300f && __instance.hasTrait("immortal") && Toolbox.randomBool()) {
        __instance.addTrait("evil", false);
      }

      if (num > 40f && Toolbox.randomChance(0.3f)) {
        __instance.addTrait("wise", false);
      }

      if (__instance.asset.unit && __instance.data.health < __instance.getMaxHealth() && Mathf.FloorToInt(num) % 2 == 0) {
        __instance.restoreHealth((int)(__instance.stats[S.health] * 0.2f));
      }

      return false;
    }

    private static bool dataUpdateAge(Actor pActor, Race pRace, ActorAsset pAsset) {
      float num = (float)pActor.data.getAge();
      if (pAsset.baby && num < (float)pAsset.years_to_grow_to_adult) {
        int num2 = (int)(18f - num);
        if (num2 > 4) {
          num2 = 4;
        }

        pActor.data.age_overgrowth += Toolbox.randomInt(1, num2);
        num = (float)pActor.data.getAge();
      }

      pActor.data.updateAttributes(pAsset, pRace, false);
      if (!World.world.worldLaws.world_law_old_age.boolVal) {
        return true;
      }

      int num3 = (int)pActor.stats[S.max_age];
      num3 += (int)(Mathf.Floor(pActor.data.level / 10) * int.Parse(Main.savedSettings.inputOptions["CRPG-levelAgeIncreaseOption"].value));
      return num3 == 0 || (float)num3 > num || !Toolbox.randomChance(0.13f);
    }

    public static void isAttackReady_Postfix(Actor __instance, ref bool __result) {
      if (!Main.savedSettings.boolOptions["CRPGOption"]) {
        return;
      }

      __result = __instance.attackTimer <= 0f;
    }

    public static bool updateStats_Prefix_old(ActorBase __instance) {
      if (!Main.savedSettings.boolOptions["CRPGOption"]) {
        return true;
      }

      // __instance.updateStats();
      __instance.statsDirty = false;
      if (!__instance.data.alive) {
        return false;
      }

      if (__instance.asset.useSkinColors && __instance.data.skin_set == -1 && __instance.asset.color_sets != null) {
        __instance.setSkinSet("default");
      }

      if (__instance.asset.useSkinColors && __instance.data.skin == -1) {
        if (__instance.asset.color_sets != null) {
          //
          // array
          //
          if (__instance.asset.color_sets.Count > __instance.data.skin_set) {
            __instance.data.skin = Toolbox.randomInt(0, AssetManager.skin_color_set_library.get(__instance.asset.color_sets[__instance.data.skin_set]).colors.Count);
          } else {
            Debug.Log("CRPG: Skin set out of range");
          }
        }
      }

      if (string.IsNullOrEmpty(__instance.data.mood)) {
        __instance.data.mood = "normal";
      }

      MoodAsset moodAsset = AssetManager.moods.get(__instance.data.mood);
      __instance.stats.clear();
      __instance.stats.mergeStats(__instance.asset.base_stats);
      __instance.stats.mergeStats(moodAsset.base_stats);
      BaseStats baseStats = __instance.stats;
      string text = S.diplomacy;
      baseStats[text] += (float)__instance.data.diplomacy;
      baseStats = __instance.stats;
      text = S.stewardship;
      baseStats[text] += (float)__instance.data.stewardship;
      baseStats = __instance.stats;
      text = S.intelligence;
      baseStats[text] += (float)__instance.data.intelligence;
      baseStats = __instance.stats;
      text = S.warfare;
      baseStats[text] += (float)__instance.data.warfare;
      if (__instance.hasAnyStatusEffect()) {
        foreach (StatusEffectData statusEffectData in __instance.activeStatus_dict.Values) {
          __instance.stats.mergeStats(statusEffectData.asset.base_stats);
        }
      }

      if (!__instance.hasWeapon()) {
        ItemAsset itemAsset = AssetManager.items.get(__instance.asset.defaultAttack);
        if (itemAsset != null) {
          __instance.stats.mergeStats(itemAsset.base_stats);
        }
      }

      __instance.s_attackType = __instance.getWeaponAsset().attackType;
      __instance.s_slashType = __instance.getWeaponAsset().path_slash_animation;
      __instance.dirty_sprite_item = true;
      // TODO: I think this code became redundant but unsure
      /*
      if (__instance.hasWeapon())
      {
          __instance.s_weapon_texture = __instance.getWeaponId();
      }
      else
      {
          __instance.s_weapon_texture = string.Empty;
      }
      */
      foreach (ActorTrait actorTrait in __instance.data.traits.Select(text2 => AssetManager.traits.get(text2)).Where(actorTrait => actorTrait != null && (!actorTrait.only_active_on_era_flag || ((!actorTrait.era_active_moon || World.world_era.flag_moon) && (!actorTrait.era_active_night || World.world_era.overlay_darkness))))) {
        __instance.stats.mergeStats(actorTrait.base_stats);
      }

      if (__instance.asset.unit) {
        __instance.s_personality = null;
        if ((__instance.kingdom != null && __instance.kingdom.isCiv() && __instance.isKing()) || (__instance.city != null && __instance.city.leader == __instance)) {
          string text3 = "balanced";
          float num = __instance.stats[S.diplomacy];
          if (__instance.stats[S.diplomacy] > __instance.stats[S.stewardship]) {
            text3 = "diplomat";
            num = __instance.stats[S.diplomacy];
          } else if (__instance.stats[S.diplomacy] < __instance.stats[S.stewardship]) {
            text3 = "administrator";
            num = __instance.stats[S.stewardship];
          }

          if (__instance.stats[S.warfare] > num) {
            text3 = "militarist";
          }

          __instance.s_personality = AssetManager.personalities.get(text3);
          __instance.stats.mergeStats(__instance.s_personality.base_stats);
        }
      }

      Clan clan = __instance.getClan();
      if (clan?.bonus_stats != null) {
        __instance.stats.mergeStats(clan.bonus_stats.base_stats);
      }
      // baseStats = __instance.stats;
      // text = S.health;
      // baseStats[text] += (float)((__instance.data.level - 1) * 20);
      // baseStats = __instance.stats;
      // text = S.damage;
      // baseStats[text] += (float)((__instance.data.level - 1) / 2);
      // baseStats = __instance.stats;
      // text = S.armor;
      // baseStats[text] += (float)((__instance.data.level - 1) / 3);
      // baseStats = __instance.stats;
      // text = S.attack_speed;
      // baseStats[text] += (float)(__instance.data.level - 1);

      foreach (KeyValuePair<string, InputOption> kv in Main.savedSettings.multipleInputOptions["CRPG-StatBoostOption"]) {
        addStatBoost(__instance, kv);
      }

      foreach (KeyValuePair<string, InputOption> kv in Main.savedSettings.multipleInputOptions["CRPG-MoreStatBoostOption"]) {
        addStatBoost(__instance, kv);
      }

      bool flag = __instance.hasTrait("madness");
      __instance.data.s_traits_ids.Clear();
      __instance.s_action_attack_target = null;
      List<ItemAsset> list = __instance.s_special_effect_items;
      list?.Clear();
      Dictionary<ItemAsset, double> dictionary = __instance.s_special_effect_items_timers;
      dictionary?.Clear();
      List<ActorTrait> list2 = __instance.s_special_effect_traits;
      list2?.Clear();
      Dictionary<ActorTrait, double> dictionary2 = __instance.s_special_effect_traits_timers;
      dictionary2?.Clear();
      foreach (string text4 in __instance.data.traits) {
        ActorTrait actorTrait2 = AssetManager.traits.get(text4);
        if (actorTrait2 != null) {
          __instance.data.s_traits_ids.Add(text4);
          if (actorTrait2.action_special_effect != null) {
            if (__instance.s_special_effect_traits == null) {
              __instance.s_special_effect_traits = new List<ActorTrait>();
              __instance.s_special_effect_traits_timers = new Dictionary<ActorTrait, double>();
            }

            __instance.s_special_effect_traits.Add(actorTrait2);
          }

          if (actorTrait2.action_attack_target != null) {
            __instance.s_action_attack_target = (AttackAction)Delegate.Combine(__instance.s_action_attack_target, actorTrait2.action_attack_target);
          }
        }
      }

      __instance.has_trait_light = __instance.hasTrait("light_lamp");
      __instance.has_trait_weightless = __instance.hasTrait("weightless");
      __instance.has_status_frozen = __instance.hasStatus("frozen");
      if (!__instance.hasWeapon()) {
        ItemAsset weaponAsset = __instance.getWeaponAsset();
        __instance.addItemActions(weaponAsset);
        if (weaponAsset.item_modifiers != null) {
          foreach (ItemAsset itemAsset2 in weaponAsset.item_modifiers.Select(text5 => AssetManager.items_modifiers.get(text5)).Where(itemAsset2 => itemAsset2 != null)) {
            __instance.addItemActions(itemAsset2);
          }
        }
      }

      if (__instance.asset.use_items) {
        List<ActorEquipmentSlot> list3 = ActorEquipment.getList(__instance.equipment);
        foreach (ActorEquipmentSlot actorEquipmentSlot in list3) {
          if (actorEquipmentSlot.data != null) {
            ItemData itemData = actorEquipmentSlot.data;
            ItemAsset itemAsset3 = AssetManager.items.get(itemData.id);
            __instance.addItemActions(itemAsset3);
            foreach (ItemAsset itemAsset4 in itemData.modifiers.Select(text6 => AssetManager.items_modifiers.get(text6))) {
              __instance.addItemActions(itemAsset4);
            }
          }
        }
      }

      bool flag2 = __instance.hasTrait("madness");
      List<ActorTrait> list4 = __instance.s_special_effect_traits;
      if (list4 != null && list4.Count == 0) {
        __instance.s_special_effect_traits = null;
        __instance.s_special_effect_traits_timers = null;
      }

      List<ItemAsset> list5 = __instance.s_special_effect_items;
      if (list5 != null && list5.Count == 0) {
        __instance.s_special_effect_items = null;
        __instance.s_special_effect_items_timers = null;
      }

      if (flag2 != flag) {
        __instance.checkMadness(flag2);
      }

      __instance.has_trait_peaceful = __instance.hasTrait("peaceful");
      __instance.has_trait_fire_resistant = __instance.hasTrait("fire_proof");
      if (__instance.asset.use_items) {
        List<ActorEquipmentSlot> list6 = ActorEquipment.getList(__instance.equipment);
        foreach (ActorEquipmentSlot actorEquipmentSlot2 in list6.Where(actorEquipmentSlot2 => actorEquipmentSlot2.data != null)) {
          ItemTools.mergeStatsWithItem(__instance.stats, actorEquipmentSlot2.data, false);
        }
      }

      __instance.stats.normalize();
      __instance.stats.checkMods();
      if (__instance.event_full_heal) {
        __instance.event_full_heal = false;
        __instance.stats.normalize();
        __instance.data.health = __instance.getMaxHealth();
      }

      Culture culture = __instance.getCulture();
      if (culture != null) {
        baseStats = __instance.stats;
        text = S.damage;
        baseStats[text] += __instance.stats[S.damage] * culture.stats.bonus_damage.value;
        baseStats = __instance.stats;
        text = S.armor;
        baseStats[text] += __instance.stats[S.armor] * culture.stats.bonus_armor.value;
        baseStats = __instance.stats;
        text = S.max_age;
        baseStats[text] += (float)culture.getMaxAgeBonus();
      }

      if (__instance.kingdom != null && World.world.kingdoms.list_civs.Contains(__instance.kingdom)) {
        baseStats = __instance.stats;
        text = S.damage;
        baseStats[text] += __instance.stats[S.damage] * __instance.kingdom.stats.bonus_damage.value;
        baseStats = __instance.stats;
        text = S.armor;
        baseStats[text] += __instance.stats[S.armor] * __instance.kingdom.stats.bonus_armor.value;
      }

      if (__instance.asset.unit) {
        __instance.calculateFertility();
      }

      baseStats = __instance.stats;
      text = S.zone_range;
      baseStats[text] += (float)((int)(__instance.stats[S.stewardship] / 10f));
      baseStats = __instance.stats;
      text = S.cities;
      baseStats[text] += (float)((int)__instance.stats[S.stewardship] / 6 + 1);
      baseStats = __instance.stats;
      text = S.army;
      baseStats[text] += (float)((int)__instance.stats[S.warfare] + 5);
      baseStats = __instance.stats;
      text = S.bonus_towers;
      baseStats[text] += (float)((int)(__instance.stats[S.warfare] / 10f));
      if (__instance.s_attackType == WeaponType.Range) {
        baseStats = __instance.stats;
        text = S.range;
        baseStats[text] += __instance.stats[S.range] * World.world_era.range_weapons_mod;
      }

      __instance.attackTimer = 0f;
      __instance.stats.normalize();
      if (__instance.data.health > __instance.getMaxHealth()) {
        __instance.data.health = __instance.getMaxHealth();
      }

      __instance.target_scale = __instance.stats[S.scale];
      __instance.s_attackSpeed_seconds = (300f - __instance.stats[S.attack_speed]) / (100f + __instance.stats[S.attack_speed]);
      WorldAction action_recalc_stats = __instance.asset.action_recalc_stats;
      action_recalc_stats?.Invoke(__instance, null);
      return false;
    }

    public static bool updateStats_Prefix(ActorBase __instance) {
      if (!Main.savedSettings.boolOptions["CRPGOption"]) {
        return true;
      }

      __instance.statsDirty = false;
      if (!__instance.data.alive) {
        return false;
      }

      __instance.checkColorSets();
      if (string.IsNullOrEmpty(__instance.data.mood))
        __instance.data.mood = "normal";
      MoodAsset moodAsset = AssetManager.moods.get(__instance.data.mood);
      __instance.stats.clear();
      __instance.stats.mergeStats(__instance.asset.base_stats);
      __instance.stats.mergeStats(moodAsset.base_stats);
      __instance.stats[S.diplomacy] += (float)__instance.data.diplomacy;
      __instance.stats[S.stewardship] += (float)__instance.data.stewardship;
      __instance.stats[S.intelligence] += (float)__instance.data.intelligence;
      __instance.stats[S.warfare] += (float)__instance.data.warfare;
      if (__instance.hasAnyStatusEffect()) {
        foreach (StatusEffectData statusEffectData in __instance.activeStatus_dict.Values)
          __instance.stats.mergeStats(statusEffectData.asset.base_stats);
      }

      if (!__instance.hasWeapon()) {
        ItemAsset itemAsset = AssetManager.items.get(__instance.asset.defaultAttack);
        if (itemAsset != null)
          __instance.stats.mergeStats(itemAsset.base_stats);
      }

      __instance.s_attackType = __instance.getWeaponAsset().attackType;
      __instance.s_slashType = __instance.getWeaponAsset().path_slash_animation;
      __instance.dirty_sprite_item = true;
      for (int index = 0; index < __instance.data.traits.Count; ++index) {
        string trait = __instance.data.traits[index];
        ActorTrait actorTrait = AssetManager.traits.get(trait);
        if (actorTrait != null && (!actorTrait.only_active_on_era_flag || (!actorTrait.era_active_moon || World.world_era.flag_moon) && (!actorTrait.era_active_night || World.world_era.overlay_darkness)))
          __instance.stats.mergeStats(actorTrait.base_stats);
      }

      if (__instance.asset.unit) {
        __instance.s_personality = (PersonalityAsset)null;
        if (__instance.kingdom != null && __instance.kingdom.isCiv() && __instance.isKing() || __instance.city != null && (UnityEngine.Object)__instance.city.leader == (UnityEngine.Object)__instance) {
          string pID = "balanced";
          float stat = __instance.stats[S.diplomacy];
          if ((double)__instance.stats[S.diplomacy] > (double)__instance.stats[S.stewardship]) {
            pID = "diplomat";
            stat = __instance.stats[S.diplomacy];
          } else if ((double)__instance.stats[S.diplomacy] < (double)__instance.stats[S.stewardship]) {
            pID = "administrator";
            stat = __instance.stats[S.stewardship];
          }

          if ((double)__instance.stats[S.warfare] > (double)stat)
            pID = "militarist";
          __instance.s_personality = AssetManager.personalities.get(pID);
          __instance.stats.mergeStats(__instance.s_personality.base_stats);
        }
      }

      Clan clan = __instance.getClan();
      if (clan != null && clan.bonus_stats != null)
        __instance.stats.mergeStats(clan.bonus_stats.base_stats);
      __instance.stats[S.health] += (float)((__instance.data.level - 1) * 20);
      __instance.stats[S.damage] += (float)((__instance.data.level - 1) / 2);
      __instance.stats[S.armor] += (float)((__instance.data.level - 1) / 3);
      __instance.stats[S.attack_speed] += (float)(__instance.data.level - 1);


      foreach (KeyValuePair<string, InputOption> kv in Main.savedSettings.multipleInputOptions["CRPG-StatBoostOption"]) {
        addStatBoost(__instance, kv);
      }

      foreach (KeyValuePair<string, InputOption> kv in Main.savedSettings.multipleInputOptions["CRPG-MoreStatBoostOption"]) {
        addStatBoost(__instance, kv);
      }


      bool flag = __instance.hasTrait("madness");
      __instance.data.s_traits_ids.Clear();
      __instance.s_action_attack_target = (AttackAction)null;
      __instance.s_special_effect_items?.Clear();
      __instance.s_special_effect_items_timers?.Clear();
      __instance.s_special_effect_traits?.Clear();
      __instance.s_special_effect_traits_timers?.Clear();
      foreach (string trait in __instance.data.traits) {
        ActorTrait actorTrait = AssetManager.traits.get(trait);
        if (actorTrait != null) {
          __instance.data.s_traits_ids.Add(trait);
          if (actorTrait.action_special_effect != null) {
            if (__instance.s_special_effect_traits == null) {
              __instance.s_special_effect_traits = new List<ActorTrait>();
              __instance.s_special_effect_traits_timers = new Dictionary<ActorTrait, double>();
            }

            __instance.s_special_effect_traits.Add(actorTrait);
          }

          if (actorTrait.action_attack_target != null)
            __instance.s_action_attack_target += actorTrait.action_attack_target;
        }
      }

      __instance.has_trait_light = __instance.hasTrait("light_lamp");
      __instance.has_trait_weightless = __instance.hasTrait("weightless");
      __instance.has_status_frozen = __instance.hasStatus("frozen");
      if (!__instance.hasWeapon()) {
        ItemAsset weaponAsset = __instance.getWeaponAsset();
        __instance.addItemActions(weaponAsset);
        if (weaponAsset.item_modifiers != null) {
          foreach (string itemModifier in weaponAsset.item_modifiers) {
            ItemAsset pItemAsset = AssetManager.items_modifiers.get(itemModifier);
            if (pItemAsset != null)
              __instance.addItemActions(pItemAsset);
          }
        }
      }

      if (__instance.asset.use_items) {
        List<ActorEquipmentSlot> list = ActorEquipment.getList(__instance.equipment);
        for (int index = 0; index < list.Count; ++index) {
          ActorEquipmentSlot actorEquipmentSlot = list[index];
          if (actorEquipmentSlot.data != null) {
            ItemData data = actorEquipmentSlot.data;
            __instance.addItemActions(AssetManager.items.get(data.id));
            foreach (string modifier in data.modifiers)
              __instance.addItemActions(AssetManager.items_modifiers.get(modifier));
          }
        }
      }

      bool pHaveMadness = __instance.hasTrait("madness");
      if (__instance.s_special_effect_traits == null || __instance.s_special_effect_traits.Count == 0) {
        __instance.s_special_effect_traits = (List<ActorTrait>)null;
        __instance.s_special_effect_traits_timers = (Dictionary<ActorTrait, double>)null;
        __instance.batch.c_trait_effects.Remove(__instance.a);
      } else
        __instance.batch.c_trait_effects.Add(__instance.a);

      if (__instance.s_special_effect_items == null || __instance.s_special_effect_items.Count == 0) {
        __instance.s_special_effect_items = (List<ItemAsset>)null;
        __instance.s_special_effect_items_timers = (Dictionary<ItemAsset, double>)null;
        __instance.batch.c_item_effects.Remove(__instance.a);
      } else
        __instance.batch.c_item_effects.Add(__instance.a);

      if (pHaveMadness != flag)
        __instance.checkMadness(pHaveMadness);
      __instance.has_trait_peaceful = __instance.hasTrait("peaceful");
      __instance.has_trait_fire_resistant = __instance.hasTrait("fire_proof");
      __instance.has_status_burning = __instance.hasStatus("burning");
      __instance.has_trait_madness = __instance.hasTrait("madness");
      if (__instance.asset.use_items) {
        List<ActorEquipmentSlot> list = ActorEquipment.getList(__instance.equipment);
        for (int index = 0; index < list.Count; ++index) {
          ActorEquipmentSlot actorEquipmentSlot = list[index];
          if (actorEquipmentSlot.data != null)
            ItemTools.mergeStatsWithItem(__instance.stats, actorEquipmentSlot.data, false);
        }
      }

      __instance.stats.normalize();
      __instance.stats.checkMods();
      if (__instance.event_full_heal) {
        __instance.event_full_heal = false;
        __instance.stats.normalize();
        __instance.data.health = __instance.getMaxHealth();
      }

      Culture culture = __instance.getCulture();
      if (culture != null) {
        __instance.stats[S.damage] += __instance.stats[S.damage] * culture.stats.bonus_damage.value;
        __instance.stats[S.armor] += __instance.stats[S.armor] * culture.stats.bonus_armor.value;
        __instance.stats[S.max_age] += (float)culture.getMaxAgeBonus();
      }

      if (__instance.kingdom != null) {
        __instance.stats[S.damage] += __instance.stats[S.damage] * __instance.kingdom.stats.bonus_damage.value;
        __instance.stats[S.armor] += __instance.stats[S.armor] * __instance.kingdom.stats.bonus_armor.value;
      }

      if (__instance.asset.unit)
        __instance.calculateFertility();
      __instance.stats[S.zone_range] += (float)(int)((double)__instance.stats[S.stewardship] / 10.0);
      __instance.stats[S.cities] += (float)((int)__instance.stats[S.stewardship] / 6 + 1);
      __instance.stats[S.bonus_towers] += (float)(int)((double)__instance.stats[S.warfare] / 10.0);
      if (__instance.s_attackType == WeaponType.Range)
        __instance.stats[S.range] += __instance.stats[S.range] * World.world_era.range_weapons_mod;
      __instance.attackTimer = 0.0f;
      __instance.stats.normalize();
      if (__instance.data.health > __instance.getMaxHealth())
        __instance.data.health = __instance.getMaxHealth();
      __instance.target_scale = __instance.stats[S.scale];
      __instance.s_attackSpeed_seconds = (float)((300.0 - (double)__instance.stats[S.attack_speed]) / (100.0 + (double)__instance.stats[S.attack_speed]));
      WorldAction actionRecalcStats = __instance.asset.action_recalc_stats;
      if (actionRecalcStats == null)
        return false;
      int num = actionRecalcStats((BaseSimObject)__instance) ? 1 : 0;
      return false;
    }

    public static void updateStats_Postfix(ActorBase __instance) {
      if (!Main.savedSettings.boolOptions["CRPGOption"]) {
        return;
      }

      foreach (KeyValuePair<string, InputOption> kv in Main.savedSettings.multipleInputOptions["CRPG-StatBoostOption"]) {
        addStatBoost(__instance, kv);
      }

      foreach (KeyValuePair<string, InputOption> kv in Main.savedSettings.multipleInputOptions["CRPG-MoreStatBoostOption"]) {
        addStatBoost(__instance, kv);
      }
    }

    private static void addStatBoost(ActorBase __instance, KeyValuePair<string, InputOption> kv) {
      int trueLevel = __instance.data.level - 1;
      __instance.stats[kv.Key] += (float)(trueLevel * float.Parse(kv.Value.value, CultureInfo.InvariantCulture));
      switch (kv.Key) {
        // case S.stewardship:
        //     __instance.stats[S.cities] += (float)((int)__instance.stats[S.stewardship] / 6 + 1);
        //     __instance.stats[S.zone_range] += (float)((int)(__instance.stats[S.stewardship] / 10f));
        //     break;
        case "warfare":
          __instance.stats[S.army] += (float)((int)__instance.stats[S.warfare] + 5);
          __instance.stats[S.bonus_towers] += (float)((int)(__instance.stats[S.warfare] / 10f));
          break;
      }
    }

    public static bool checkHit_Prefix(Projectile __instance, ref bool __result) {
      if (!Main.savedSettings.boolOptions["CRPGOption"]) {
        return true;
      }

      if (__instance.gameObject.GetComponent<isAttributeProjectile>() != null) {
        // 0 clue why this is needed, but some discord user called verecho who seemingly knows how Unity works unlike me told me that this fixes some weird bug
        Object.Destroy(__instance.gameObject.GetComponent<isAttributeProjectile>());
        __result = true;
        return false;
      }

      return true;
    }
  }
}