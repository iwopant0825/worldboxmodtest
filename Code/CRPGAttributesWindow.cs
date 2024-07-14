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
    class CRPGAttributesWindow : MonoBehaviour
    {
        private static GameObject contents;
        private static GameObject scrollView;
        private static Vector2 originalSize;
        private static Dictionary<string, int> levelStats = new Dictionary<string, int>();

        public static void openWindow()
        {
            Windows.ShowWindow("crpgAttributesWindow");
        }

        public static void init()
        {
            contents = WindowManager.windowContents["crpgAttributesWindow"];
            scrollView = GameObject.Find($"Canvas Container Main/Canvas - Windows/windows/crpgAttributesWindow/Background/Scroll View");
            originalSize = contents.GetComponent<RectTransform>().sizeDelta;
            VerticalLayoutGroup layoutGroup = contents.AddComponent<VerticalLayoutGroup>();
            layoutGroup.childControlHeight = false;
            layoutGroup.childControlWidth = false;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.childScaleHeight = true;
            layoutGroup.childScaleWidth = true;
            layoutGroup.childAlignment = TextAnchor.UpperCenter;
            layoutGroup.spacing = 25;
            loadAttributeSettings();
        }

        private static void loadAttributeSettings()
        {
            contents.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 6000);
            foreach(KeyValuePair<string, AttributeTrait> kv in Main.savedSettings.crpgAttributes)
            {
                Dictionary<string, InputOption> attributeFields = new Dictionary<string, InputOption>();
                foreach(FieldInfo field in typeof(AttributeTrait).GetFields())
                {
                    if (field.Name == "attackAction")
                    {
                        continue;
                    }
                    attributeFields.Add(field.Name, new InputOption{active = true, value = field.GetValue(kv.Value).ToString()});
                }

                Dictionary<string, NameInput> inputs = NewUI.createMultipleInputOption($"{kv.Key} Title Holder", 
                    $"{kv.Key} Settings", 
                    "Modify The Values", 
                    0, 
                    contents, 
                    attributeFields
                );

                foreach(KeyValuePair<string, NameInput> input in inputs)
                {
                    FieldInfo fieldInfo = typeof(AttributeTrait).GetField(input.Key);
                    if (fieldInfo.FieldType == typeof(float))
                    {
                        input.Value.inputField.characterValidation = InputField.CharacterValidation.Decimal;
                        input.Value.inputField.onValueChanged.AddListener(delegate{
                            string pValue = NewUI.checkStatFloatInput(input.Value);
                            Main.modifyCRPGAttribute(kv.Key, fieldInfo, pValue);
                            input.Value.setText(pValue);
                        });
                    }
                    else if (fieldInfo.FieldType == typeof(int))
                    {
                        input.Value.inputField.characterValidation = InputField.CharacterValidation.Integer;
                        input.Value.inputField.onValueChanged.AddListener(delegate{
                            string pValue = NewUI.checkStatInput(input.Value);
                            Main.modifyCRPGAttribute(kv.Key, fieldInfo, pValue);
                            input.Value.setText(pValue);
                        });
                    }
                    else if (fieldInfo.FieldType == typeof(string))
                    {
                        input.Value.inputField.characterValidation = InputField.CharacterValidation.None;
                        input.Value.inputField.onValueChanged.AddListener(delegate{
                            Main.modifyCRPGAttribute(kv.Key, fieldInfo, input.Value.inputField.text);
                            input.Value.setText(input.Value.inputField.text);
                        });
                    }
                }
            }
        }
    }
}