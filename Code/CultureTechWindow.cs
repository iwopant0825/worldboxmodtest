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
    class CultureTechWindow : MonoBehaviour
    {
        private static GameObject contents;
        private static GameObject scrollView;
        private static Vector2 originalSize;
        private static Culture currentCulture;

        public static void init()
        {
            contents = WindowManager.windowContents["cultureTechWindow"];
            scrollView = GameObject.Find($"/Canvas Container Main/Canvas - Windows/windows/cultureTechWindow/Background/Scroll View");
            originalSize = contents.GetComponent<RectTransform>().sizeDelta;
            loadCultureTech();
        }

        public static void openWindow(Culture culture)
        {
            currentCulture = culture;
            checkTechToggle();
            Windows.ShowWindow("cultureTechWindow");
        }

        private static void loadCultureTech()
        {
            foreach(Transform child in contents.transform)
            {
                Destroy(child.gameObject);
            }
            contents.GetComponent<RectTransform>().sizeDelta = new Vector2(0, (AssetManager.culture_tech.list.Count/16)*originalSize.y) + originalSize;

            int index = 0;
            int indexY = 0;
            foreach(CultureTechAsset tech in AssetManager.culture_tech.list)
            {
                PowerButtons.CreateButton(
                    $"{tech.id}_dej", 
                    Resources.Load<Sprite>($"ui/Icons/{tech.path_icon}"), 
                    tech.id, 
                    tech.id, 
                    new Vector2(60 + (index*35), -40 + (indexY*-40)), 
                    ButtonType.Toggle, 
                    contents.transform, 
                    () => onTechClick(tech.id)
                );
                index++;
                if (index > 4)
                {
                    index = 0;
                    indexY++;
                }
            }
        }

        private static void onTechClick(string techID)
        {
            if (PowerButtons.GetToggleValue($"{techID}_dej"))
            {
                currentCulture.addFinishedTech(techID);
                return;
            }
            removeTech(techID);
        }

        private static void removeTech(string pTech)
        {
            currentCulture.data.list_tech_ids.Remove(pTech);
            currentCulture.setDirty();
        }

        private static void checkTechToggle()
        {
            foreach(CultureTechAsset techAsset in AssetManager.culture_tech.list)
            {
                if (PowerButtons.GetToggleValue($"{techAsset.id}_dej"))
                {
                    PowerButtons.ToggleButton($"{techAsset.id}_dej");
                }
            }
            foreach(string tech in currentCulture.data.list_tech_ids)
            {
                PowerButtons.ToggleButton($"{tech}_dej");
            }
        }
    }
}