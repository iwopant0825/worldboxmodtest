using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NCMS;
using NCMS.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ReflectionUtility;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CollectionMod
{
    class HeroicTitles : MonoBehaviour
    {
        public static HeroicTitles instance;
        public static SavedSettings savedSettings = new SavedSettings();
        private static string correctSettingsVersion = "0.0.2";
        public static Coroutine currentCoroutine;
        public static bool coroutineIsRunning = false;

        public void Awake() {
            instance = this;
        }

        public static void init()
        {
            instance = new GameObject("HeroicTitlesInstance").AddComponent<HeroicTitles>();
            loadHeroicStats();
            instance.startChecks();
        }

        public void startChecks()
        {
            Debug.Log("hello");
            coroutineIsRunning = true;
            currentCoroutine = StartCoroutine(this.checkActorsConditions());
        }

        IEnumerator checkActorsConditions()
        {
            int index = 0;
            bool reset = false;
            while(coroutineIsRunning){
                if (getRaceList().Count <= 0){
                    yield return new WaitForSeconds(0.5f);
                    continue;
                }
                if (index+1 >= getRaceList().Count){
                    index = 0;
                    reset = true;
                    yield return new WaitForSeconds(0.02f);
                }
                resetTitleNumbers(reset);
                reset = false;
                Actor pActor = null;
                pActor = getRaceList()[index];
                if (!checkAllTitles(pActor)){
                    continue;
                }
                index++;
                yield return new WaitForSeconds(savedSettings.titlesCheckTime);
            }
        }

        private static List<Actor> getRaceList()
        {
            List<Actor> raceList = new List<Actor>();
            foreach(Race race in AssetManager.raceLibrary.list)
            {
                ActorContainer raceUnits = (ActorContainer)Reflection.GetField(
                    typeof(Race), 
                    race,
                    "units"
                );
                raceList.AddRange(raceUnits.getSimpleList());
            }
            return raceList;
        }

        private static void resetTitleNumbers(bool reset)
        {
            foreach(Title pTitle in Main.savedSettings.titles)
            {
                if (reset)
                {
                    pTitle.previousNumber = pTitle.currentNumber;
                    pTitle.currentNumber = 0;
                }
                if (pTitle.currentNumber > pTitle.previousNumber)
                {
                    pTitle.previousNumber = pTitle.currentNumber;
                }
            }
        }

        private static bool checkAllTitles(Actor pActor)
        {
            foreach(Title pTitle in Main.savedSettings.titles)
            {
                if (checkTraits(pActor, pTitle) && checkStats(pActor, pTitle) && checkBools(pActor, pTitle) && checkNames(pActor, pTitle))
                {
                    pTitle.currentNumber++;
                    pTitle.currentNumberName++;
                    checkResults(pActor, pTitle);
                    continue;
                }
            }
            return true;
        }

        private static bool checkStats(Actor pActor, Title pTitle)
        {
            if (pTitle.titleStats.Count <= 0)
            {
                return true;
            }
            ActorData pData = (ActorData)Reflection.GetField(typeof(ActorBase), pActor, "data");
            foreach(KeyValuePair<string, int> kv in pTitle.titleStats)
            {
                int stats = -1;
                switch(kv.Key)
                {
                    case "health":
                        stats = pData.health;
                        break;
                    case "kill":
                        stats = pData.kills;
                        break;
                    case "level":
                        stats = pData.level;
                        break;
                    case "age":
                        stats = pActor.getAge();
                        break;
                }
                if (stats < kv.Value)
                {
                    return false;
                }
            }
            return true;
        }

        private static bool checkTraits(Actor pActor, Title pTitle)
        {
            if (pTitle.titleTraits.Count <= 0)
            {
                return true;
            }
            ActorData pData = (ActorData)Reflection.GetField(typeof(ActorBase), pActor, "data");
            foreach(string pTrait in pTitle.titleTraits)
            {
                if(!pData.traits.Contains(pTrait))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool checkBools(Actor pActor, Title pTitle)
        {
            if (pTitle.titleBools.Count <= 0)
            {
                return true;
            }
            foreach(KeyValuePair<string, bool> kv in pTitle.titleBools)
            {
                if(pActor.asset.id == kv.Key && kv.Value)
                {
                    return true;
                }
                if (pActor.city == null){
                    continue;
                }
                if (pActor.city.data.id == kv.Key)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool checkNames(Actor pActor, Title pTitle)
        {
            if (pTitle.titleNames.Count <= 0)
            {
                return true;
            }
            foreach(string pName in pTitle.titleNames)
            {
                if(pActor.getName().Contains(pName))
                {
                    return true;
                }
            }
            return false;
        }

        private static void checkResults(Actor pActor, Title pTitle)
        {
            List<string> prefixes = new List<string>();
            List<string> suffixes = new List<string>();
            List<string> nameNums = new List<string>();
            ActorData pData = (ActorData)Reflection.GetField(typeof(ActorBase), pActor, "data");
            foreach(KeyValuePair<string, string> kv in pTitle.titleResults)
            {
                switch(kv.Key){
                    case "Favorite":
                        if(Convert.ToBoolean(kv.Value))
                        {
                            pData.favorite = true;
                        }
                        continue;
                    case "Prefix":
                        if (pActor.getName().Contains(kv.Value) || kv.Value == "")
                        {
                            continue;
                        }
                        prefixes.Add(kv.Value);
                        continue;
                    case "Suffix":
                        if (pActor.getName().Contains(kv.Value) || kv.Value == "")
                        {
                            continue;
                        }
                        suffixes.Add(kv.Value);
                        continue;
                    case "NameNumber":
                        if (pActor.getName().Contains(pTitle.name) || !Convert.ToBoolean(kv.Value))
                        {
                            continue;
                        }
                        nameNums.Add($"{numberText(pTitle.currentNumberName.ToString())}-{pTitle.name} ");
                        continue;
                }
                if (AssetManager.traits.get(kv.Key) != null && Convert.ToBoolean(kv.Value) && !pActor.hasTrait(kv.Key))
                {
                    if (pActor.hasTrait(kv.Key))
                    {
                        continue;
                    }
                    removeOppositeTraits(pActor, kv.Key);
                    pActor.addTrait(kv.Key);
                    continue;
                }
                if (kv.Key.Contains("Stat"))
                {
                    continue;
                }
            }

            pData.setName(addNameAdditions(pActor.getName(), prefixes, suffixes, nameNums));
        }

        private static void removeOppositeTraits(Actor pActor, string pTrait)
        {
            ActorTrait actorTrait = AssetManager.traits.get(pTrait);
            if (actorTrait == null)
            {
                return;
            }
            if (actorTrait.oppositeArr == null)
            {
                return;
            }
            ActorData pData = (ActorData)Reflection.GetField(typeof(ActorBase), pActor, "data");
            for (int i = 0; i < actorTrait.oppositeArr.Length; i++)
            {
                string text = actorTrait.oppositeArr[i];
                pActor.removeTrait(text);
                HashSet<string> pTraits_ids = (HashSet<string>)Reflection.GetField(typeof(ActorData), pData, "s_traits_ids");
                pTraits_ids.Remove(text);
            }
        }

        private static string addNameAdditions(string name, List<string> prefixes, List<string> suffixes, List<string> nameNums)
        {
            string result = name;
            foreach(string pre in prefixes)
            {
                result = $"{pre} {result}";
            }
            foreach(string suf in suffixes)
            {
                result = $"{result} {suf}";
            }
            foreach(string nn in nameNums)
            {
                result = $"{nn} {result}";
            }
            return result;
        }

        private static string numberText(string text)
        {
            switch(text)
            {
                case "1":
                    return "1st";
                    break;
                case "2":
                    return "2nd";
                    break;
                case "3":
                    return "3rd";
                    break;
                default:
                    return $"{text}th";
                    break;
            }
        }

        public static void upTitlePriority(Title pTitle)
        {
            int index = Main.savedSettings.titles.IndexOf(pTitle);
            if (index == 0)
            {
                return;
            }
            Title aboveTitle = Main.savedSettings.titles[index-1];
            Main.savedSettings.titles[index] = aboveTitle;
            Main.savedSettings.titles[index-1] = pTitle;
        }

        public static void getLocalization(string id, ref string name, ref string desc, string descAddOn)
        {
            if (LocalizedTextManager.stringExists($"{id}"))
            {
                name = LocalizedTextManager.getText($"{id}");
            }
            if (LocalizedTextManager.stringExists($"{id}{descAddOn}"))
            {
                desc = LocalizedTextManager.getText($"{id}{descAddOn}");
            }
        }

        public static void saveTitleSettings()
        {
            if (savedSettings.titles.Count <= 0){
                return;
            }

            string json = JsonConvert.SerializeObject(savedSettings, Formatting.Indented);
            File.WriteAllText($"{Core.NCMSModsPath}/HeroicTitlesSettings.json", json);
        }

        private static void loadHeroicStats()
        {
            if (!Main.loadSettings())
            {
                return;
            }
            foreach(Title title in Main.savedSettings.titles){
                title.currentNumber = 0;
            }
        }
    }
}