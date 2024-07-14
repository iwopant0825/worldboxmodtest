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
    class MagnetSettingsWindow : MonoBehaviour
    {
        private static GameObject contents;
        private static GameObject scrollView;
        private static Vector2 originalSize;

        public static void init()
        {
            contents = WindowManager.windowContents["magnetSettingsWindow"];
            scrollView = GameObject.Find($"/Canvas Container Main/Canvas - Windows/windows/magnetSettingsWindow/Background/Scroll View");
            originalSize = contents.GetComponent<RectTransform>().sizeDelta;
            loadContent();
        }

        public static void openWindow()
        {
            Windows.ShowWindow("magnetSettingsWindow");
        }

        private static void loadContent()
        {
            Dictionary<string, NameInput> inputs = NewUI.createMultipleInputOption(
                "Magnet Level Filter_multiSetting", 
                "Magnet Level Filter", 
                "Filter Out Units From The Magnet With Their Levels (Keep them at -1 to disable)", 
                -60, 
                contents, 
                Main.savedSettings.multipleInputOptions["Magnet Level Filter"]
            );
            foreach(KeyValuePair<string, NameInput> input in inputs)
            {
                input.Value.inputField.characterValidation = InputField.CharacterValidation.Integer;
                input.Value.inputField.onValueChanged.AddListener(delegate{
                    string pValue = NewUI.checkStatInput(input.Value);
                    Main.modifyMultipleInputOption("Magnet Level Filter", input.Key, pValue);
                    input.Value.setText(pValue);
                });
            }
        }
    }
}