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
    class CRPGTitlesWindow : MonoBehaviour
    {
        private static GameObject contents;
        private static GameObject scrollView;
        private static Vector2 originalSize;
        private static Dictionary<string, int> levelStats = new Dictionary<string, int>();

        public static void openWindow()
        {
            Windows.ShowWindow("crpgTitlesWindow");
        }

        public static void init()
        {
            contents = WindowManager.windowContents["crpgTitlesWindow"];
            scrollView = GameObject.Find($"Canvas Container Main/Canvas - Windows/windows/crpgTitlesWindow/Background/Scroll View");
            originalSize = contents.GetComponent<RectTransform>().sizeDelta;
            VerticalLayoutGroup layoutGroup = contents.AddComponent<VerticalLayoutGroup>();
            layoutGroup.childControlHeight = false;
            layoutGroup.childControlWidth = false;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.childScaleHeight = true;
            layoutGroup.childScaleWidth = true;
            layoutGroup.childAlignment = TextAnchor.UpperCenter;
            layoutGroup.spacing = 100;
            loadTitleSettings();
        }

        private static void loadTitleSettings()
        {
            contents.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 2000);
            foreach(KeyValuePair<string, Dictionary<string, string>> kv in Main.savedSettings.CRPGTitles)
            {
                Dictionary<string, NameInput> inputs = NewUI.createMultipleInputOption($"{kv.Key} Title Holder", 
                    $"{kv.Key}-Digit Titles", 
                    "Modify The Titles", 
                    0, contents, 
                    kv.Value.ToDictionary(x => x.Key, y => new InputOption{active = true, value = y.Value})
                );
                GameObject inputParent = inputs.First().Value.gameObject.transform.parent.gameObject;
                inputParent.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 400);
                foreach(Transform child in inputParent.transform)
                {
                    child.localPosition += new Vector3(0, 190, 0);
                }

                foreach(KeyValuePair<string, NameInput> input in inputs)
                {
                    input.Value.inputField.onValueChanged.AddListener(delegate{
                        Main.modifyCRPGTitleOption(kv.Key, input.Key, input.Value.inputField.text);
                    });
                }
            }
        }
    }
}