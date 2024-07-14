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
    class CRPGSettingsWindow : MonoBehaviour
    {
        private static GameObject contents;
        private static GameObject scrollView;
        private static Vector2 originalSize;

        public static void openWindow()
        {
            loadCRPGSettingOptions();
            Windows.ShowWindow("crpgSettingsWindow");
        }

        public static void init()
        {
            contents = WindowManager.windowContents["crpgSettingsWindow"];
            scrollView = GameObject.Find($"Canvas Container Main/Canvas - Windows/windows/crpgSettingsWindow/Background/Scroll View");
            originalSize = contents.GetComponent<RectTransform>().sizeDelta;
            VerticalLayoutGroup layoutGroup = contents.AddComponent<VerticalLayoutGroup>();
            layoutGroup.childControlHeight = false;
            layoutGroup.childControlWidth = false;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.childScaleHeight = true;
            layoutGroup.childScaleWidth = true;
            layoutGroup.childAlignment = TextAnchor.UpperCenter;
            layoutGroup.spacing = 15;
        }

        private static void loadCRPGSettingOptions()
        {
            loadCRPGTraits();
        }

        private static void loadCRPGTraits()
        {
            contents.GetComponent<RectTransform>().sizeDelta = new Vector2(0, (Main.savedSettings.crpgTraits.Count)*400) + originalSize;
            foreach(Transform child in contents.transform)
            {
                Destroy(child.gameObject);
            }
            foreach(KeyValuePair<string, CRPGTrait> kv in Main.savedSettings.crpgTraits)
            {
                createCRPGInput(contents, kv.Value.expGain.ToString(), kv.Key, typeof(CRPGTrait).GetField("expGain"));
                createCRPGInput(contents, kv.Value.expGainHit.ToString(), kv.Key, typeof(CRPGTrait).GetField("expGainHit"));
                createCRPGInput(contents, kv.Value.expGainHurt.ToString(), kv.Key, typeof(CRPGTrait).GetField("expGainHurt"));
                createCRPGInput(contents, kv.Value.birthRate.ToString(), kv.Key, typeof(CRPGTrait).GetField("birthRate"));
                createCRPGInput(contents, kv.Value.expGainKill.ToString(), kv.Key, typeof(CRPGTrait).GetField("expGainKill"));
                createCRPGInput(contents, kv.Value.EXPRequirement.ToString(), kv.Key, typeof(CRPGTrait).GetField("EXPRequirement"));
                createCRPGInput(contents, kv.Value.talentLevelCap.ToString(), kv.Key, typeof(CRPGTrait).GetField("talentLevelCap"));
            }
        }

        private static void createCRPGInput(GameObject parent, string textValue, string traitID, FieldInfo field)
        {
            GameObject inputRef = NCMS.Utils.GameObjects.FindEvenInactive("NameInputElement");

            GameObject inputField = Instantiate(inputRef, parent.transform);
            NameInput nameInputComp = inputField.GetComponent<NameInput>();
            nameInputComp.setText(textValue);
            RectTransform inputRect = inputField.GetComponent<RectTransform>();
            // inputRect.localPosition -= new Vector3(40,0,0);
            inputRect.sizeDelta = new Vector2(100, 25);

            GameObject inputChild = inputField.transform.Find("InputField").gameObject;
            RectTransform inputChildRect = inputChild.GetComponent<RectTransform>();
            // inputChildRect.sizeDelta *= 2;
            Text inputChildText = inputChild.GetComponent<Text>();
            inputChildText.resizeTextMaxSize = 15;
            
            if (field.FieldType == typeof(int))
            {
                nameInputComp.inputField.characterValidation = InputField.CharacterValidation.Integer;
                nameInputComp.inputField.onValueChanged.AddListener(delegate{
                    string pValue = NewUI.checkStatInput(nameInputComp);
                    Main.modifyCRPGTrait(traitID, field, pValue);
                    nameInputComp.setText(pValue);
                });
            }
            else if (field.FieldType == typeof(float))
            {
                nameInputComp.inputField.characterValidation = InputField.CharacterValidation.Decimal;
                nameInputComp.inputField.onValueChanged.AddListener(delegate{
                    string pValue = NewUI.checkStatFloatInput(nameInputComp);
                    Main.modifyCRPGTrait(traitID, field, pValue);
                    nameInputComp.setText(pValue);
                    if (field.Name == "birthRate")
                    {
                        AssetManager.traits.get(traitID).birth = Main.savedSettings.crpgTraits[traitID].birthRate;
                    }
                });
            }
            NewUI.addText($"{traitID} :", inputField, 8, new Vector3(-70, 50, 50), default(Vector2));
            int fontSize = 8;
            if (field.Name.Length > 12)
            {
                fontSize = fontSize/2;
            }
            NewUI.addText($"{field.Name}", inputField, fontSize, new Vector3(70, 50, 50), default(Vector2));
            inputField.SetActive(true);
        }
    }
}