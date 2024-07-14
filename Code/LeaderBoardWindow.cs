using System;
using System.IO;
using System.Text;
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

namespace CollectionMod
{
    class LeaderBoardWindow : MonoBehaviour
    {
        private static GameObject contents;
        private static GameObject scrollView;
        private static Vector2 originalSize;
        public static LeaderBoardWindow instance;
        private static List<Actor> filteredActors = new List<Actor>();
        private static bool UIReadyToLoad = false;
        private static bool loading = false;
        private static GameObject progressBar;

        public static void init()
        {
            contents = WindowManager.windowContents["leaderBoardWindow"];
            instance = new GameObject("LeaderBoardWindowInstance").AddComponent<LeaderBoardWindow>();
            scrollView = GameObject.Find($"/Canvas Container Main/Canvas - Windows/windows/leaderBoardWindow/Background/Scroll View");
            originalSize = contents.GetComponent<RectTransform>().sizeDelta;
            progressBar = NewUI.createProgressBar(scrollView, new Vector3(0, 0, 0));
            progressBar.SetActive(false);
            Button filterButton = NewUI.createBGWindowButton(
                scrollView, 
                50, 
                "iconSettings", 
                "FiltersBGButton", 
                "Filters", 
                "Add Filters To The LeaderBoard",
                FilterWindow.openWindow
            );
        }

        public void startFilterCoroutine()
        {
            StartCoroutine(this.loadFilteredActors());
        }

        public IEnumerator loadFilteredActors()
        {
            while (loading)
            {
                yield return new WaitForSeconds(0.1f);
            }
            contents.GetComponent<RectTransform>().sizeDelta = originalSize;
            foreach(Transform child in contents.transform)
            {
                Destroy(child.gameObject);
            }
            loading = true;
            UIReadyToLoad = false;
            StartCoroutine(this.getFilteredActors(getSortNames(), getActorList()));
            while (!UIReadyToLoad)
            {
                yield return new WaitForSeconds(0.05f);
            }
            contents.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 150 * (filteredActors.Count/2));
            int index = 0;
            int gapY = -70;
            foreach(Actor pActor in filteredActors)
            {
                GameObject textRank = NewUI.addText((index+1).ToString() + ".", contents, 20, new Vector3(50, (gapY*index), 0)).gameObject;
                NewUI.createActorUI(pActor, textRank, new Vector3(40, 0, 0));
                GameObject infoHolder = NewUI.createSubWindow(textRank, new Vector3(120, 0, 0), new Vector2(250, 100), new Vector2(250, 150));
                ActorData pData = (ActorData)Reflection.GetField(typeof(ActorBase), pActor, "data");
                BaseStats pStats = (BaseStats)Reflection.GetField(typeof(BaseSimObject), pActor, "stats");
                Text infoText = NewUI.addText(
$@"Name: {pActor.getName()}
Age: {pActor.getAge().ToString()}
Level: {pData.level.ToString()}
DMG: {pStats[S.damage].ToString()}
Kills: {pData.kills.ToString()}",
                    infoHolder,
                    15,
                    new Vector3(0, 50, 0),
                    new Vector2(100, 100)
                );
                infoText.alignment = TextAnchor.MiddleLeft;
                string talentID = CustomizableRPG.getTalent(pActor);
                if (!string.IsNullOrEmpty(talentID))
                {
                    infoText.text += $"\nTalent: {talentID}";
                }
                index++;
                yield return new WaitForSeconds(0.01f);
            }
            loading = false;
            progressBar.SetActive(false);
            yield break;
        }

        private IEnumerator getFilteredActors(List<string> sortNames, List<Actor> actorList)
        {
            filteredActors.Clear();
            if (sortNames.Count <= 0 || actorList.Count <= 0)
            {
                UIReadyToLoad = true;
                yield break;
            }
            progressBar.SetActive(true);
            int listCount = actorList.Count;
            int maxCount = listCount;
            if (listCount >= 100)
            {
                maxCount = 100;
            }
            for(int i = 0; i < listCount; i++)
            {
                if (filteredActors.Count >= 100)
                {
                    break;
                }
                Actor actor1 = null;
                float num = 0f;
                foreach(Actor actor2 in actorList)
                {
                    if (actor2.asset.animal || !actor2.asset.unit || filteredActors.Contains(actor2))
                    {
                        continue;
                    }
                    ActorData data2 = (ActorData)Reflection.GetField(typeof(ActorBase), actor2, "data");
                    BaseStats stats2 = (BaseStats)Reflection.GetField(typeof(BaseSimObject), actor2, "stats");
                    float num2 = 0f;
                    foreach(string sortName in sortNames)
                    {
                        switch(sortName)
                        {
                            case "SortLevel":
                                num2 += data2.level;
                                break;
                            case "SortKill":
                                num2 += data2.kills;
                                break;
                            case "nothing":
                                num2 += 0;
                                break;
                            case "SortAge":
                                num2 += actor2.getAge();
                                break;
                            case "SortDmg":
                                num2 += stats2[S.damage];
                                break;
                            case "SortTalent":
                                string talent = CustomizableRPG.getTalent(actor2);
                                num2 += Main.savedSettings.crpgTraits[talent].leaderBoardPriority;
                                break;
                        }
                    }
                    if(actor1 != null && num2 == num)
                    {
                        ActorData data1 = (ActorData)Reflection.GetField(typeof(ActorBase), actor1, "data");
                        BaseStats stats1 = (BaseStats)Reflection.GetField(typeof(BaseSimObject), actor1, "stats");
                        float attribute1 = data1.kills + stats1[S.damage] + data1.level;
                        float attribute2 = data2.kills + stats2[S.damage] + data2.level;
                        if (attribute2 > attribute1)
                        {
                            actor1 = actor2;
                        }
                    }
                    else if (actor1 == null || num2 > num)
                    {
                        num = num2;
                        actor1 = actor2;
                    }
                }
                if (actor1 != null)
                {
                    filteredActors.Add(actor1);
                    setProgressBar(filteredActors.Count, maxCount, "/" + maxCount.ToString(), false);
                    yield return new WaitForSeconds(0.01f);
                }
            }
            UIReadyToLoad = true;
            yield break;
        }

        public static void openWindow()
        {
            instance.startFilterCoroutine();
            Windows.ShowWindow("leaderBoardWindow");
        }

        private static List<string> getSortNames()
        {
            List<string> sortNames = new List<string>();
            foreach(KeyValuePair<string, bool> kv in Main.savedSettings.boolOptions)
            {
                if (!kv.Key.Contains("Sort"))
                {
                    continue;
                }
                if (kv.Value)
                {
                    sortNames.Add(kv.Key);
                }
            }
            if (sortNames.Count <= 0)
            {
                sortNames.Add("nothing");
            }
            return sortNames;
        }

        private static List<Actor> getActorList()
        {
            List<Actor> actorList = new List<Actor>();
            List<string> traitCheckList = new List<string>();
            foreach(KeyValuePair<string, bool> kv in Main.savedSettings.boolOptions)
            {
                if (!kv.Key.Contains("Filter"))
                {
                    continue;
                }
                if (kv.Value)
                {
                    string filterID = kv.Key.Remove(kv.Key.IndexOf("Filter"));
                    if (AssetManager.raceLibrary.dict.ContainsKey(filterID))
                    {
                        Race race = AssetManager.raceLibrary.get(filterID);
                        ActorContainer actorContainer = (ActorContainer)Reflection.GetField(typeof(Race), race, "units");
                        actorList.AddRange(actorContainer.getSimpleList());
                    }
                    else if (AssetManager.traits.dict.ContainsKey(filterID))
                    {
                        ActorTrait trait = AssetManager.traits.get(filterID);
                        traitCheckList.Add(filterID);
                    }
                }
            }
            List<Actor> filteredActorList = new List<Actor>();
            if (traitCheckList.Count > 0)
            {
                foreach(Actor pActor in actorList)
                {
                    foreach(string traitID in traitCheckList)
                    {
                        if (pActor.hasTrait(traitID))
                        {
                            filteredActorList.Add(pActor);
                            break;
                        }
                    }
                }
                actorList = filteredActorList;
            }
            
            if (int.Parse(Main.savedSettings.multipleInputOptions["Level Filter"]["Min Level"].value) >= 0 && 
            int.Parse(Main.savedSettings.multipleInputOptions["Level Filter"]["Max Level"].value) >= 0 )
            {
                filteredActorList.Clear();
                foreach(Actor pActor in actorList)
                {
                    if (pActor.data.level < int.Parse(Main.savedSettings.multipleInputOptions["Level Filter"]["Min Level"].value))
                    {
                        continue;
                    }
                    if (pActor.data.level > int.Parse(Main.savedSettings.multipleInputOptions["Level Filter"]["Max Level"].value))
                    {
                        continue;
                    }
                    filteredActorList.Add(pActor);
                }
                actorList = filteredActorList;
            }
            return actorList;
        }

        private static void setProgressBar(float pVal, float pMax, string pEnding, bool pReset = true, bool pFloat = false, bool pUpdateText = true, bool pWithoutTween = false)
        {
            StatBar statBar = progressBar.GetComponent<StatBar>();
            statBar.setBar(pVal, pMax, pEnding, pReset, pFloat, pUpdateText, pWithoutTween);
        }
    }
}