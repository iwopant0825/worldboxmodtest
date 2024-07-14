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
    class AgesWindow : MonoBehaviour
    {
        private static GameObject contents;
        private static GameObject scrollView;
        private static Vector2 originalSize;

        public static void init()
        {
            contents = WindowManager.windowContents["agesWindow"];
            scrollView = GameObject.Find($"/Canvas Container Main/Canvas - Windows/windows/agesWindow/Background/Scroll View");
            originalSize = contents.GetComponent<RectTransform>().sizeDelta;
            GridLayoutGroup layoutGroup = contents.AddComponent<GridLayoutGroup>();
            layoutGroup.cellSize = new Vector2(30, 30);
            layoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            layoutGroup.constraintCount = 4;
            layoutGroup.childAlignment = TextAnchor.UpperCenter;
            layoutGroup.spacing = new Vector2(15, 5);
            loadAgeButtons();
        }

        private static void loadAgeButtons()
        {
            contents.GetComponent<RectTransform>().sizeDelta = new Vector2(0, (AssetManager.era_library.list.Count/16)*200);
            foreach(EraAsset era in AssetManager.era_library.list)
            {
                // if (building.id.Contains("!"))
                // {
                //     continue;
                // }
                Sprite icon = Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.iconArrowBack.png");
                if (!string.IsNullOrEmpty(era.path_icon))
                {
                    icon = Resources.Load<Sprite>(era.path_icon);
                }
                PowerButtons.CreateButton(
                    $"{era.id}_dej", 
                    icon, 
                    era.id, 
                    $"Select {era.id}", 
                    new Vector2(0, 0), 
                    ButtonType.Click, 
                    contents.transform, 
                    () => setAge(era.id, icon)
                );
            }
        }

        public static void openWindow()
        {
            Windows.ShowWindow("agesWindow");
        }

        private static void setAge(string eraID, Sprite eraIcon)
        {
            World.world.eraManager.setEra(eraID);
            GameObject icon = PowerButtons.CustomButtons["age_select_dej"].gameObject.transform.GetChild(0).gameObject;
            icon.GetComponent<Image>().sprite = eraIcon;
        }
    }
}