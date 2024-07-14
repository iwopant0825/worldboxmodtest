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
    class CRPGLevelStatsWindow : MonoBehaviour
    {
        private static GameObject contents;
        private static GameObject scrollView;
        private static Vector2 originalSize;
        private static Dictionary<string, int> levelStats = new Dictionary<string, int>();

        public static void openWindow()
        {
            loadStats();
            Windows.ShowWindow("crpgLevelStatsWindow");
        }

        private static void openKingdomWindow(List<Actor> units)
        {
            loadStats(units);
            Windows.ShowWindow("crpgLevelStatsWindow");
        }

        public static void init()
        {
            contents = WindowManager.windowContents["crpgLevelStatsWindow"];
            scrollView = GameObject.Find($"Canvas Container Main/Canvas - Windows/windows/crpgLevelStatsWindow/Background/Scroll View");
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
            Button kingdomLevelsButton = NewUI.createBGWindowButton(
                kingdomWindow.gameObject,
                10,
                "iconLevelsButton",
                "kingdomLevelsButton",
                "View Current Levels Within The Kingdom!",
                "Look At An Overview Of What Levels Every Unit In The Kingdom Has!",
                () => openKingdomWindow(kingdomWindow.gameObject.GetComponent<KingdomWindow>().kingdom.units.getSimpleList())
            );
        }

        private static void loadStats(List<Actor> units = null)
        {
            levelStats.Clear();

            if (units == null) units = World.world.units.getSimpleList();

            List<Actor> flawedActors = new List<Actor>();
            foreach (Actor pActor in units.Where(pActor => pActor != null)) {
                if (pActor.data != null) {
                    string pLevel = (Mathf.FloorToInt(pActor.data.level/10) * 10).ToString();
                    if (!levelStats.ContainsKey(pLevel))
                    {
                        levelStats.Add(pLevel, 0);
                    }
                    levelStats[pLevel]++;
                } else {
                    flawedActors.Add(pActor);
                }
            }

            foreach (Actor actor in flawedActors) {
                units.Remove(actor);
                World.world.units._container._simpleList.Remove(actor);
            }

            contents.GetComponent<RectTransform>().sizeDelta = new Vector2(0, (levelStats.Count/5)*150) + originalSize;

            foreach(Transform child in contents.transform)
            {
                Destroy(child.gameObject);
            }

            Dictionary<string, int> tempLevelStats = (from ele in levelStats orderby int.Parse(ele.Key) ascending select ele).ToDictionary(key => key.Key, value => value.Value);

            foreach(KeyValuePair<string, int> kv in tempLevelStats)
            {
                NewUI.addText($"Level {kv.Key}-{int.Parse(kv.Key) + 9} : {kv.Value} Units", contents, 15, new Vector3(0, 0, 0), new Vector2(120, 0));
            }
        }
    }
}