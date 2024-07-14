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
    class MoodsWindow : MonoBehaviour
    {
        private static GameObject contents;
        private static GameObject scrollView;
        private static Vector2 originalSize;
        private static Actor currentActor;

        public static void init()
        {
            contents = WindowManager.windowContents["moodsWindow"];
            scrollView = GameObject.Find($"/Canvas Container Main/Canvas - Windows/windows/moodsWindow/Background/Scroll View");
            originalSize = contents.GetComponent<RectTransform>().sizeDelta;
            GridLayoutGroup layoutGroup = contents.AddComponent<GridLayoutGroup>();
            layoutGroup.cellSize = new Vector2(30, 30);
            layoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            layoutGroup.constraintCount = 4;
            layoutGroup.childAlignment = TextAnchor.UpperCenter;
            layoutGroup.spacing = new Vector2(15, 5);

            GameObject inspectWindow = ScrollWindow.allWindows["inspect_unit"].gameObject;
            Button moodSelectButton = inspectWindow.gameObject.GetComponent<WindowCreatureInfo>().moodBG.gameObject.AddComponent<Button>();
            moodSelectButton.onClick.AddListener(openWindow);
            loadMoodButtons();
        }

        private static void loadMoodButtons()
        {
            contents.GetComponent<RectTransform>().sizeDelta = new Vector2(0, (AssetManager.moods.list.Count/16)*200);
            foreach(MoodAsset mood in AssetManager.moods.list)
            {
                // if (building.id.Contains("!"))
                // {
                //     continue;
                // }
                Sprite icon = Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.iconArrowBack.png");
                if (!string.IsNullOrEmpty(mood.icon))
                {
                    icon = mood.getSprite();
                }
                PowerButtons.CreateButton(
                    $"{mood.id}_dej", 
                    icon, 
                    mood.id, 
                    $"Select {mood.id}", 
                    new Vector2(0, 0), 
                    ButtonType.Click, 
                    contents.transform, 
                    () => setMood(mood.id)
                );
            }
        }

        public static void openWindow()
        {
            Windows.ShowWindow("moodsWindow");
        }

        private static void setMood(string moodID)
        {
            Config.selectedUnit.changeMood(moodID);
        }
    }
}