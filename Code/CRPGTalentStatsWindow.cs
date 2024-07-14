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
    class CRPGTalentStatsWindow : MonoBehaviour
    {
        private static GameObject contents;
        private static GameObject scrollView;
        private static Vector2 originalSize;
        private static Dictionary<string, int> talentStats = new Dictionary<string, int>();

        public static void openWindow()
        {
            loadStats();
            Windows.ShowWindow("crpgTalentStatsWindow");
        }

        private static void openKingdomWindow(List<Actor> units)
        {
            loadStats(units);
            Windows.ShowWindow("crpgTalentStatsWindow");
        }

        public static void init()
        {
            contents = WindowManager.windowContents["crpgTalentStatsWindow"];
            scrollView = GameObject.Find($"Canvas Container Main/Canvas - Windows/windows/crpgTalentStatsWindow/Background/Scroll View");
            originalSize = contents.GetComponent<RectTransform>().sizeDelta;
            VerticalLayoutGroup layoutGroup = contents.AddComponent<VerticalLayoutGroup>();
            layoutGroup.childControlHeight = false;
            layoutGroup.childControlWidth = false;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.childScaleHeight = true;
            layoutGroup.childScaleWidth = true;
            layoutGroup.childAlignment = TextAnchor.UpperCenter;
            layoutGroup.spacing = -70;

            ScrollWindow kingdomWindow = Windows.GetWindow("kingdom");
            Button kingdomTalentsButton = NewUI.createBGWindowButton(
                kingdomWindow.gameObject,
                50,
                "iconTalentsButton",
                "kingdomTalentsButton",
                "View Current Talents Within The Kingdom!",
                "Look At An Overview Of What Talent Every Unit In The Kingdom Has!",
                () => openKingdomWindow(kingdomWindow.gameObject.GetComponent<KingdomWindow>().kingdom.units.getSimpleList())
            );
        }

        private static void loadStats(List<Actor> units = null)
        {
            talentStats.Clear();
            contents.GetComponent<RectTransform>().sizeDelta = new Vector2(0, (Main.savedSettings.crpgTraits.Count/5)*150) + originalSize;
            foreach(string key in Main.savedSettings.crpgTraits.Keys)
            {
                talentStats.Add(key, 0);
            }

            if (units != null)
            {
                foreach(Actor pActor in units)
                {
                    string talentID = CustomizableRPG.getTalent(pActor);
                    if (string.IsNullOrEmpty(talentID))
                    {
                        continue;
                    }
                    talentStats[talentID]++;
                }
            }
            else
            {
                foreach(Actor pActor in World.world.units)
                {
                    string talentID = CustomizableRPG.getTalent(pActor);
                    if (string.IsNullOrEmpty(talentID))
                    {
                        continue;
                    }
                    talentStats[talentID]++;
                }
            }

            foreach(Transform child in contents.transform)
            {
                Destroy(child.gameObject);
            }

            foreach(KeyValuePair<string, int> kv in talentStats)
            {
                NewUI.addText($"{kv.Key} Rank: {kv.Value}", contents, 15, new Vector3(0, 0, 0), new Vector2(100, 0));
            }
        }
    }
}