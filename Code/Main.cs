using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Globalization;
using NCMS;
using NCMS.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ReflectionUtility;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CollectionMod {
  [ModEntry]
  class Main : MonoBehaviour {
    public static SavedSettings savedSettings = new SavedSettings();
    private static string correctSettingsVersion = "0.6.5";
    private static int kingAndLeaderYearCheck = 0;
    public static FilteredMagnet filteredMagnet = new FilteredMagnet();

    IEnumerator Start() {
      Debug.Log("GAME VERSION:");
      Debug.Log(Config.versionCodeDate);
      Debug.Log(Config.versionCodeText);
      loadSettings();
      Dictionary<string, ScrollWindow> allWindows = (Dictionary<string, ScrollWindow>)Reflection.GetField(typeof(ScrollWindow), null, "allWindows");
      Reflection.CallStaticMethod(typeof(ScrollWindow), "checkWindowExist", "inspect_unit");
      allWindows["inspect_unit"].gameObject.SetActive(false);
      Reflection.CallStaticMethod(typeof(ScrollWindow), "checkWindowExist", "village");
      allWindows["village"].gameObject.SetActive(false);
      Reflection.CallStaticMethod(typeof(ScrollWindow), "checkWindowExist", "debug");
      allWindows["debug"].gameObject.SetActive(false);
      Reflection.CallStaticMethod(typeof(ScrollWindow), "checkWindowExist", "kingdom");
      allWindows["kingdom"].gameObject.SetActive(false);
      yield return new WaitForSeconds(1f);
      HeroicTitles.init();
      Patches.init();
      CRPGPatches.init();
      FamilyTreePatches.init();
      NewJobs.init();
      NewActorAssets.init();
      NewGodPowers.init();
      NewUI.init();
      WindowManager.init();
      TabManager.init();
      NewMapIcons.init();
      CustomizableRPG.init();
      Traits.init();
      NewSpells.init();
      NewProjectiles.init();
      NewTerraformOptions.init();
      NewEffects.init();
      InvokeRepeating("updateKingAndLeaderCheck", 5f, 1f);
    }

    void Update() {
      if (World.world == null) {
        return;
      }

      if (World.world.kingdoms == null) {
        return;
      }

      filteredMagnet.magnetAction(true, null);
    }

    void updateKingAndLeaderCheck() {
      if (World.world == null || !Main.savedSettings.inputOptions["WorldLawElectionsOption"].active) {
        return;
      }

      if (World.world.kingdoms == null) {
        return;
      }

      if (kingAndLeaderYearCheck >= World.world.mapStats.getCurrentYear()) {
        return;
      }

      foreach (Kingdom pKingdom in World.world.kingdoms.list_civs) {
        pKingdom.removeKing();
        List<Actor> _units = new List<Actor>();
        Actor actor = null;
        if (actor == null) {
          _units.Clear();
          actor = getKingByLevel(pKingdom, ref _units);
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

      foreach (City city in World.world.cities.list) {
        CityBehCheckLeaderDej.checkLeaderClan(city);
        CityBehCheckLeaderDej.checkFindLeader(city);
      }

      kingAndLeaderYearCheck = World.world.mapStats.getCurrentYear() + int.Parse(Main.savedSettings.inputOptions["WorldLawElectionsOption"].value);
    }

    private static Actor getKingByLevel(Kingdom pKingdom, ref List<Actor> _units) {
      Actor actor = null;
      foreach (City city in pKingdom.cities) {
        foreach (Actor cityActor in city.units.getSimpleList()) {
          _units.Add(cityActor);
        }
      }

      if (_units.Count == 0) {
        return null;
      }

      _units.Shuffle<Actor>();
      int num = 0;
      foreach (Actor actor2 in _units) {
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

    public static void saveSettings(SavedSettings previousSettings = null) {
      if (previousSettings != null) {
        foreach (FieldInfo field in typeof(SavedSettings).GetFields()) {
          field.SetValue(savedSettings, field.GetValue(previousSettings));
        }

        savedSettings.settingVersion = correctSettingsVersion;
      }

      string json = JsonConvert.SerializeObject(savedSettings, Formatting.Indented);
      File.WriteAllText($"{Core.NCMSModsPath}/CollectionSettings.json", json);
    }

    public static bool loadSettings() {
      if (!File.Exists($"{Core.NCMSModsPath}/CollectionSettings.json")) {
        saveSettings();
        return false;
      }

      string data = File.ReadAllText($"{Core.NCMSModsPath}/CollectionSettings.json");
      SavedSettings loadedData = null;
      try {
        loadedData = JsonConvert.DeserializeObject<SavedSettings>(data);
      } catch {
        saveSettings();
        return false;
      }

      if (loadedData.settingVersion != correctSettingsVersion) {
        saveSettings(loadedData);
        return false;
      }

      savedSettings = loadedData;
      return true;
    }

    public static void modifyInputOption(string key, string value, bool active, UnityAction call = null) {
      Main.savedSettings.inputOptions[key] = new InputOption { active = active, value = value };
      saveSettings();
      if (call != null) {
        call.Invoke();
      }
    }

    public static void modifyBoolOption(string key, bool value, UnityAction call = null) {
      Main.savedSettings.boolOptions[key] = value;
      saveSettings();
      if (call != null) {
        call.Invoke();
      }
    }

    public static void modifyCRPGTrait(string key, FieldInfo field, string value) {
      if (field.FieldType == typeof(int)) {
        field.SetValue(Main.savedSettings.crpgTraits[key], int.Parse(value));
      }

      if (field.FieldType == typeof(float)) {
        field.SetValue(Main.savedSettings.crpgTraits[key], float.Parse(value, CultureInfo.InvariantCulture));
      }

      saveSettings();
    }

    public static void modifyMultipleInputOption(string key, string valueKey, string value) {
      savedSettings.multipleInputOptions[key][valueKey].value = value;
      saveSettings();
    }

    public static void modifyCRPGTitleOption(string key, string valueKey, string value) {
      savedSettings.CRPGTitles[key][valueKey] = value;
      saveSettings();
    }

    public static void modifySavedItems(string key) {
      if (savedSettings.savedItems.ContainsKey(key)) {
        savedSettings.savedItems.Remove(key);
      }

      SavedItems itemsToBeSaved = new SavedItems {
        itemAssets = ItemEditorWindow.itemAssets,
        itemModifiers = ItemEditorWindow.itemModifiers,
        itemNames = ItemEditorWindow.itemNames.ToDictionary(pair => pair.Key, pair => pair.Value.inputField.text)
      };
      savedSettings.savedItems.Add(key, itemsToBeSaved);
      saveSettings();
    }

    public static void modifyCRPGAttribute(string key, FieldInfo field, string value) {
      if (field.FieldType == typeof(int)) {
        field.SetValue(savedSettings.crpgAttributes[key], int.Parse(value));
      } else if (field.FieldType == typeof(float)) {
        field.SetValue(savedSettings.crpgAttributes[key], float.Parse(value));
        if (field.Name == "attackChance") {
          int numIndex = key.Length - 1;
          Spell spell = AssetManager.spells.get($"{key.Remove(numIndex)}Spell{key[numIndex]}");
          spell.chance = float.Parse(value);
        }
      } else if (field.FieldType == typeof(string)) {
        field.SetValue(savedSettings.crpgAttributes[key], value);
      }

      saveSettings();
    }

    public static void getLocalization(string id, ref string name, ref string desc, string descAddOn) {
      if (LocalizedTextManager.stringExists($"{id}")) {
        name = LocalizedTextManager.getText($"{id}");
      }

      if (LocalizedTextManager.stringExists($"{id}{descAddOn}")) {
        desc = LocalizedTextManager.getText($"{id}{descAddOn}");
      }
    }

    public static void updateDirtyStats() {
      foreach (Actor unit in World.world.units) {
        unit.setStatsDirty();
      }
    }
  }
}