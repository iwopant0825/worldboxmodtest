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
    class StatLimiterWindow : MonoBehaviour
    {
        private static GameObject contents;
        private static GameObject scrollView;
        private static Vector2 originalSize;
        private static Culture currentCulture;
        private static List<string> currentStats;

        public static void init()
        {
            currentStats = new List<string>{
                S.health,
                S.damage,
                "level",
                S.armor,
                S.attack_speed
            };
            contents = WindowManager.windowContents["statLimiterWindow"];
            scrollView = GameObject.Find($"/Canvas Container Main/Canvas - Windows/windows/statLimiterWindow/Background/Scroll View");
            originalSize = contents.GetComponent<RectTransform>().sizeDelta;
            VerticalLayoutGroup layoutGroup = contents.AddComponent<VerticalLayoutGroup>();
            layoutGroup.childControlHeight = false;
            layoutGroup.childControlWidth = false;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.childScaleHeight = true;
            layoutGroup.childScaleWidth = true;
            layoutGroup.childAlignment = TextAnchor.UpperCenter;
            layoutGroup.spacing = 50;

            NewUI.createBGWindowButton(
                scrollView,
                50,
                "iconTalents",
                "submitLimits",
                "Submit",
                "Add Changes To World",
                Main.updateDirtyStats
            );
            loadStatInputs();
        }

        private static void loadStatInputs()
        {
            contents.GetComponent<RectTransform>().sizeDelta += new Vector2(0, (currentStats.Count/2)*300);
            foreach(string stats in currentStats)
            {
                NameInput input = NewUI.createInputOption(
                    "statLimitHolder",
                    stats,
                    $"Modify the limits of {stats}:",
                    0,
                    contents,
                    Main.savedSettings.inputOptions[$"{stats}Limit"].value
                );
                input.inputField.characterValidation = InputField.CharacterValidation.Integer;
                input.inputField.onValueChanged.AddListener(delegate{
                    string pValue = NewUI.checkStatInput(input);
                    Main.modifyInputOption($"{stats}Limit", pValue, PowerButtons.GetToggleValue($"{stats}LimitButton"));
                    modifyStatLimits(stats);
                    input.setText(pValue);
                });
                PowerButton activeButton = PowerButtons.CreateButton(
                    $"{stats}LimitButton", 
                    Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.icon_imperial_thinking.png"), 
                    "Activate", 
                    "Turn On The Limiter For This Stat", 
                    new Vector2(200, 0), 
                    ButtonType.Toggle, 
                    input.transform.parent.transform, 
                    delegate{
                        string pValue = NewUI.checkStatInput(input);
                        Main.modifyInputOption($"{stats}Limit", pValue, PowerButtons.GetToggleValue($"{stats}LimitButton"));
                        modifyStatLimits(stats);
                        input.setText(pValue);
                    }
                );
                if (Main.savedSettings.inputOptions[$"{stats}Limit"].active)
                {
                    PowerButtons.ToggleButton($"{stats}LimitButton");
                }
                activeButton.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(64, 64);

            }
        }

        public static void openWindow()
        {
            Windows.ShowWindow("statLimiterWindow");
        }

        private static void modifyStatLimits(string stats)
        {
            BaseStatAsset statAsset = AssetManager.base_stats_library.get(stats);
            if (Main.savedSettings.inputOptions[$"{stats}Limit"].active && stats != "level")
            {
                statAsset.normalize_max = int.Parse(Main.savedSettings.inputOptions[$"{stats}Limit"].value);
            }
            else if (!Main.savedSettings.inputOptions[$"{stats}Limit"].active && stats != "level")
            {
                switch (stats)
                {
                    case "attack_speed":
                        statAsset.normalize_max = 300;
                        break;
                    case "armor":
                        statAsset.normalize_max = 99;
                        break;
                    default:
                        statAsset.normalize_max = int.MaxValue;
                        break;
                }
            }
        }
    }
}