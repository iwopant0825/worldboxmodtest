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
    class BuildingsWindow : MonoBehaviour
    {
        private static GameObject contents;
        private static GameObject scrollView;
        private static Vector2 originalSize;
        public static string currentBuilding;

        public static void init()
        {
            currentBuilding = SB.tree;
            contents = WindowManager.windowContents["buildingsWindow"];
            scrollView = GameObject.Find($"/Canvas Container Main/Canvas - Windows/windows/buildingsWindow/Background/Scroll View");
            originalSize = contents.GetComponent<RectTransform>().sizeDelta;
            GridLayoutGroup layoutGroup = contents.AddComponent<GridLayoutGroup>();
            layoutGroup.cellSize = new Vector2(30, 30);
            layoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            layoutGroup.constraintCount = 4;
            layoutGroup.childAlignment = TextAnchor.UpperCenter;
            layoutGroup.spacing = new Vector2(15, 5);
            loadBuildingButtons();
        }

        private static void loadBuildingButtons()
        {
            contents.GetComponent<RectTransform>().sizeDelta = new Vector2(0, (AssetManager.buildings.list.Count/16)*200);
            foreach(BuildingAsset building in AssetManager.buildings.list)
            {
                if (building.id.Contains("!"))
                {
                    continue;
                }
                PowerButtons.CreateButton(
                    $"{building.id}_dej", 
                    Resources.Load<Sprite>($"buildings/{building.id}/main_0"), 
                    building.id, 
                    $"Select {building.id}", 
                    new Vector2(0, 0), 
                    ButtonType.Click, 
                    contents.transform, 
                    () => setBuildingPower(building.id)
                );
            }
        }

        public static void openWindow()
        {
            Windows.ShowWindow("buildingsWindow");
        }

        private static void setBuildingPower(string buildingID)
        {
            currentBuilding = buildingID;
            NewActions.selectBuildingPower("building_drop_dej");
            GameObject icon = PowerButtons.CustomButtons["buildings_select_dej"].gameObject.transform.GetChild(0).gameObject;
            icon.GetComponent<Image>().sprite = Resources.Load<Sprite>($"buildings/{buildingID}/main_0");
            GameObject icon2 = PowerButtons.CustomButtons["building_drop_dej"].gameObject.transform.GetChild(0).gameObject;
            icon2.GetComponent<Image>().sprite = Resources.Load<Sprite>($"buildings/{buildingID}/main_0");
        }
    }
}