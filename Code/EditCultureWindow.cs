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
    class EditCultureWindow : MonoBehaviour
    {
        private static GameObject contents;
        private static GameObject scrollView;
        private static Vector2 originalSize;

        public static void init()
        {
            contents = WindowManager.windowContents["editCultureWindow"];
            scrollView = GameObject.Find($"/Canvas Container Main/Canvas - Windows/windows/editCultureWindow/Background/Scroll View");
            originalSize = contents.GetComponent<RectTransform>().sizeDelta;
        }

        public static void openWindow()
        {
            loadCultures();
            Windows.ShowWindow("editCultureWindow");
        }

        private static void loadCultures()
        {
            foreach(Transform child in contents.transform)
            {
                Destroy(child.gameObject);
            }
            RectTransform contentsRect = contents.GetComponent<RectTransform>();
            contentsRect.sizeDelta = ((World.world.cultures.list.Count-4) > 0) ? originalSize + new Vector2(0, (World.world.cultures.list.Count-4)*50) : originalSize;

            int index = 0;
            foreach(Culture culture in World.world.cultures.list)
            {
                GameObject banner = NewUI.createCultureBanner(contents, culture, new Vector3(130, -40+(index*-40), 0));
                banner.GetComponent<Button>().onClick.AddListener(() => CultureTechWindow.openWindow(culture));
                index++;
            }
        }
    }
}