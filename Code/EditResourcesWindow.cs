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
    class EditResourcesWindow : MonoBehaviour
    {
        private static GameObject contents;
        private static GameObject scrollView;
        public static City currentCity;

        public static void init()
        {
            contents = WindowManager.windowContents["editResourcesWindow"];
            scrollView = GameObject.Find($"Canvas Container Main/Canvas - Windows/windows/editResourcesWindow/Background/Scroll View");
            GridLayoutGroup layoutGroup = contents.AddComponent<GridLayoutGroup>();
            layoutGroup.cellSize = new Vector2(60, 50);
            layoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            layoutGroup.constraintCount = 3;
            layoutGroup.childAlignment = TextAnchor.UpperCenter;
            loadResourceStats();
            NewUI.createBGWindowButton(
                GameObject.Find($"Canvas Container Main/Canvas - Windows/windows/village"), 
                5, 
                "iconTalents", 
                "openResourceWindow_dej", 
                "Edit Resources", 
                "Click Here To Edit City Resources!",
                openWindow
            );
        }

        public static void openWindow()
        {
            Windows.ShowWindow("editResourcesWindow");
        }

        private static void loadResourceStats()
        {
            contents.GetComponent<RectTransform>().sizeDelta += new Vector2(0, (AssetManager.resources.list.Count/12)*200);
            foreach(ResourceAsset resource in AssetManager.resources.list)
            {
                createResourceInput(contents.transform, resource.id, new Vector3(0, 0, 0), "0");
            }
        }

        private static void createResourceInput(Transform parent, string resourceName, Vector3 pos, string textValue)
        {
            GameObject inputHolder = new GameObject("levelInput");
            inputHolder.transform.SetParent(parent);

            Text infoText = NewUI.addText(resourceName, inputHolder, 10, new Vector3(0, 45, 0));
            RectTransform infoTextRect = infoText.gameObject.GetComponent<RectTransform>();
            infoTextRect.sizeDelta = new Vector2(infoTextRect.sizeDelta.x, 80);

            GameObject inputRef = GameObjects.FindEvenInactive("NameInputElement");

            GameObject inputField = Instantiate(inputRef, inputHolder.transform);
            NameInput nameInputComp = inputField.GetComponent<NameInput>();
            nameInputComp.setText(textValue);
            RectTransform inputRect = inputField.GetComponent<RectTransform>();
            inputRect.localPosition = new Vector3(0, -25, 0);
            inputRect.sizeDelta += new Vector2(25, 10);

            nameInputComp.inputField.characterValidation = InputField.CharacterValidation.Integer;
            nameInputComp.inputField.onValueChanged.AddListener(delegate{
                string pValue = NewUI.checkStatInput(nameInputComp);
                currentCity.data.storage.set(resourceName, int.Parse(pValue));
                nameInputComp.setText(pValue);
            });
            RectTransform inputHolderRect = inputHolder.AddComponent<RectTransform>();
            inputHolderRect.localPosition = pos;
        }
    }
}