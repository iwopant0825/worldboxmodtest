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
    class FilterWindow : MonoBehaviour
    {
        private static GameObject contents;
        private static GameObject scrollView;

        public static void init()
        {
            contents = WindowManager.windowContents["filterWindow"];
            scrollView = GameObject.Find($"/Canvas Container Main/Canvas - Windows/windows/filterWindow/Background/Scroll View");
            GridLayoutGroup layoutGroup = contents.AddComponent<GridLayoutGroup>();
            layoutGroup.cellSize = new Vector2(30, 35);
            layoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            layoutGroup.constraintCount = 4;
            layoutGroup.childAlignment = TextAnchor.UpperCenter;
            layoutGroup.spacing = new Vector2(15, 5);
            loadFilters();
            Button filterButton = NewUI.createBGWindowButton(
                scrollView, 
                50, 
                "iconTalents", 
                "submitFiltersButton", 
                "Submit", 
                "Click Here To Submit Changes",
                resetLeaderBoard
            );
        }

        private static void loadFilters()
        {
            contents.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 210);
            foreach(KeyValuePair<string, bool> kv in Main.savedSettings.boolOptions)
            {
                if (!kv.Key.Contains("Filter") && !kv.Key.Contains("Sort"))
                {
                    continue;
                }
                PowerButtons.CreateButton(
                    kv.Key, 
                    Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.{kv.Key}.png"), 
                    kv.Key, 
                    "Select This Filter", 
                    new Vector2(0, 0), 
                    ButtonType.Toggle, 
                    contents.transform, 
                    () => toggleFilter(kv.Key)
                );
                if (kv.Value)
                {
                    PowerButtons.ToggleButton(kv.Key);
                }
            }
            Dictionary<string, NameInput> inputs = NewUI.createMultipleInputOption(
                "Level Filter_multiSetting", 
                "Level Filter", 
                "Filter Out Units With Their Levels (Keep them at -1 to disable)", 
                -300, 
                contents, 
                Main.savedSettings.multipleInputOptions["Level Filter"]
            );
            LayoutElement layout = inputs.First().Value.gameObject.transform.parent.gameObject.AddComponent<LayoutElement>();
            layout.ignoreLayout = true;
            foreach(KeyValuePair<string, NameInput> input in inputs)
            {
                input.Value.inputField.characterValidation = InputField.CharacterValidation.Integer;
                input.Value.inputField.onValueChanged.AddListener(delegate{
                    string pValue = NewUI.checkStatInput(input.Value);
                    Main.modifyMultipleInputOption("Level Filter", input.Key, pValue);
                    input.Value.setText(pValue);
                });
            }
        }

        private static void toggleFilter(string key)
        {
            Main.modifyBoolOption(key, PowerButtons.GetToggleValue(key));
        }

        private static void resetLeaderBoard()
        {
            LeaderBoardWindow.instance.startFilterCoroutine();
        }

        public static void openWindow()
        {
            Windows.ShowWindow("filterWindow");
        }
    }
}