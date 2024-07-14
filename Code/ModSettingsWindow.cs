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
    class ModSettingsWindow : MonoBehaviour
    {
        private static GameObject contents;
        private static GameObject scrollView;
        private static Dictionary<string, int> savedRaceCityLimit = new Dictionary<string, int>();

        public static void init()
        {
            contents = WindowManager.windowContents["modSettingsWindow"];
            scrollView = GameObject.Find($"Canvas Container Main/Canvas - Windows/windows/modSettingsWindow/Background/Scroll View");
            VerticalLayoutGroup layoutGroup = contents.AddComponent<VerticalLayoutGroup>();
            layoutGroup.childControlHeight = false;
            layoutGroup.childControlWidth = false;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.childScaleHeight = true;
            layoutGroup.childScaleWidth = true;
            layoutGroup.childAlignment = TextAnchor.UpperCenter;
            layoutGroup.spacing = 40;

            foreach(Race pRace in AssetManager.raceLibrary.list)
            {
                savedRaceCityLimit.Add(pRace.id, pRace.civ_baseCities);
            }
            loadSettingOptions();
        }

        private static void loadSettingOptions()
        {
            loadBoolOptions();
            loadInputOptions();
            loadMultipleInputOptions();
        }

        public static void openWindow()
        {
            Windows.ShowWindow("modSettingsWindow");
        }

        private static void loadBoolOptions()
        {
            contents.GetComponent<RectTransform>().sizeDelta += new Vector2(0, ((Main.savedSettings.boolOptions.Count-8)/3)*180);
            foreach(KeyValuePair<string, bool> kv in Main.savedSettings.boolOptions)
            {
                if (!kv.Key.Contains("Option") || kv.Key == "navalWarfareOption" || kv.Key == "imperialThinkingOption" || kv.Key == "CRPGOption")
                {
                    continue;
                }
                UnityAction call = null;
                switch (kv.Key)
                {
                    case "IgnoreWarriorLimitOption":
                        call = NewJobs.turnOnIgnoreWarriorLimit;
                        break;
                    // case "KingdomStartWith1CityLimitOption":
                    //     call = delegate{
                    //         if (Main.savedSettings.boolOptions["KingdomStartWith1CityLimitOption"])
                    //         {

                    //         }
                    //         foreach(Race pRace in AssetManager.raceLibrary)
                    //         {
                    //             pRace.civ_baseCities = 1;
                    //         }
                    //     };
                    //     break;
                }

                PowerButtons.CreateButton(
                    $"{kv.Key}_setting", 
                    Sprites.LoadSprite($"{Mod.Info.Path}/icon.png"), 
                    $"Toggle {kv.Key}", 
                    "Toggle This Option In The Settings", 
                    new Vector2(0, 0), 
                    ButtonType.Toggle, 
                    contents.transform, 
                    delegate{
                        Main.modifyBoolOption(kv.Key, PowerButtons.GetToggleValue($"{kv.Key}_setting"), call);
                    }
                );
                if (kv.Value)
                {
                    PowerButtons.ToggleButton($"{kv.Key}_setting");
                    if (call != null)
                    {
                        call.Invoke();
                    }
                }
            }
        }

        private static void loadInputOptions()
        {
            contents.GetComponent<RectTransform>().sizeDelta += new Vector2(0, ((Main.savedSettings.inputOptions.Count-5)/3)*250);
            foreach(KeyValuePair<string, InputOption> kv in Main.savedSettings.inputOptions)
            {
                if (!kv.Key.Contains("Option") || kv.Key == "LevelRate")
                {
                    continue;
                }

                UnityAction call = null;
                switch(kv.Key)
                {
                    case "KingdomBaseCityLimitOption":
                        call = delegate{
                            if (Main.savedSettings.inputOptions["KingdomBaseCityLimitOption"].active)
                            {
                                foreach(Race pRace in AssetManager.raceLibrary.list)
                                {
                                    pRace.civ_baseCities = int.Parse(Main.savedSettings.inputOptions["KingdomBaseCityLimitOption"].value);
                                }
                            }
                            else
                            {
                                foreach(Race pRace in AssetManager.raceLibrary.list)
                                {
                                    pRace.civ_baseCities = savedRaceCityLimit[pRace.id];
                                }
                            }
                        };
                        break;
                    case "WorldLawElectionsOption":
                        call = delegate{
                            GetNextJobID newKingdomJobID = null;
                            GetNextJobID newCityJobID = null;
                            if (Main.savedSettings.inputOptions["WorldLawElectionsOption"].active)
                            {
                                newKingdomJobID = new GetNextJobID(NewJobs.getNextKingdomJobDej);
                                newCityJobID = new GetNextJobID(NewJobs.getNextCityJobDej);
                                Debug.Log("active");
                            }
                            else
                            {
                                newKingdomJobID = new GetNextJobID(NewJobs.getNextKingdomJob);
                                newCityJobID = new GetNextJobID(NewJobs.getNextCityJob);
                                Debug.Log("Inactive");
                            }
                            foreach(Kingdom kingdom in World.world.kingdoms.list_civs)
                            {
                                kingdom.ai.nextJobDelegate = newKingdomJobID;
                            }
                            foreach(City city in World.world.cities.list)
                            {
                                city.ai.nextJobDelegate = newCityJobID;
                            }
                        };
                        break;
                }
                if (call != null)
                {
                    call.Invoke();
                }

                NameInput input = NewUI.createInputOption(
                    $"{kv.Key}_setting", 
                    kv.Key, 
                    "Modify The Value Of This Setting", 
                    0, 
                    contents, 
                    kv.Value.value
                );
                input.inputField.characterValidation = InputField.CharacterValidation.Integer;
                input.inputField.onValueChanged.AddListener(delegate{
                    string pValue = NewUI.checkStatInput(input);
                    Main.modifyInputOption(kv.Key, pValue, PowerButtons.GetToggleValue($"{kv.Key}Button"), call);
                    input.setText(pValue);
                });

                PowerButton activeButton = PowerButtons.CreateButton(
                    $"{kv.Key}Button", 
                    Sprites.LoadSprite($"{Mod.Info.Path}/icon.png"), 
                    "Activate Setting", 
                    "", 
                    new Vector2(200, 0), 
                    ButtonType.Toggle, 
                    input.transform.parent.transform, 
                    delegate{
                        string pValue = NewUI.checkStatInput(input);
                        Main.modifyInputOption(kv.Key, pValue, PowerButtons.GetToggleValue($"{kv.Key}Button"), call);
                        input.setText(pValue);
                    }
                );
                if (kv.Value.active)
                {
                    PowerButtons.ToggleButton($"{kv.Key}Button");
                }
                activeButton.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(64, 64);
            }
        }

        private static void loadMultipleInputOptions()
        {
            contents.GetComponent<RectTransform>().sizeDelta += new Vector2(0, ((Main.savedSettings.multipleInputOptions.Count-2)/3)*300);
            foreach(KeyValuePair<string, Dictionary<string, InputOption>> kv in Main.savedSettings.multipleInputOptions)
            {
                if (!kv.Key.Contains("Option"))
                {
                    continue;
                }
                Dictionary<string, NameInput> inputs = NewUI.createMultipleInputOption(
                    $"{kv.Key}_multiSetting", 
                    kv.Key, 
                    "Modify These Options", 
                    0, 
                    contents, 
                    kv.Value
                );

                foreach(KeyValuePair<string, NameInput> input in inputs)
                {
                    input.Value.inputField.characterValidation = InputField.CharacterValidation.Decimal;
                    input.Value.inputField.onValueChanged.AddListener(delegate{
                        string pValue = NewUI.checkStatFloatInput(input.Value);
                        Main.modifyMultipleInputOption(kv.Key, input.Key, pValue);
                        input.Value.setText(pValue);
                        Main.updateDirtyStats();
                    });
                }
            }
        }
    }
}