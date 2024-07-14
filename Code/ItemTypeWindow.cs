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
    class ItemTypeWindow : MonoBehaviour
    {
        private static GameObject contents;
        private static GameObject scrollView;
        private static Vector2 originalSize;
        public static int currentButtonID;
        private static List<string> wrongItems = new List<string>{
            "base",
            "claws",
            "hands",
            "fire_hands",
            "jaws",
            "bite",
            "rocks",
            "snowball"
        };

        public static void init()
        {
            contents = WindowManager.windowContents["itemTypeWindow"];
            scrollView = GameObject.Find($"/Canvas Container Main/Canvas - Windows/windows/itemTypeWindow/Background/Scroll View");
            originalSize = contents.GetComponent<RectTransform>().sizeDelta;
            GridLayoutGroup layoutGroup = contents.AddComponent<GridLayoutGroup>();
            layoutGroup.cellSize = new Vector2(30, 30);
            layoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            layoutGroup.constraintCount = 4;
            layoutGroup.childAlignment = TextAnchor.UpperCenter;
            layoutGroup.spacing = new Vector2(15, 5);
        }

        private static void loadItemTypes()
        {
            foreach(Transform child in contents.transform)
            {
                Destroy(child.gameObject);
            }
            contents.GetComponent<RectTransform>().sizeDelta = new Vector2(0, (AssetManager.items.list.Count/16)*originalSize.y) + originalSize;
            contents.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

            foreach(ItemAsset item in AssetManager.items.list)
            {
                if (item.id[0] == '_' || item.equipmentType != (EquipmentType)currentButtonID || wrongItems.Contains(item.id) )
                {
                    continue;
                }
                if (item.materials.Count <= 0)
                {
                    if (PowerButtons.CustomButtons.ContainsKey($"{item.id}_dej"))
                    {
                        PowerButtons.CustomButtons.Remove($"{item.id}_dej");
                    }
                    PowerButtons.CreateButton(
                        $"{item.id}_dej", 
                        Resources.Load<Sprite>($"ui/Icons/items/icon_{item.id}"), 
                        item.id, 
                        item.id, 
                        new Vector2(0, 0), 
                        ButtonType.Click, 
                        contents.transform, 
                        () => onItemClick(item)
                    );
                    continue;
                }
                foreach(string material in item.materials)
                {
                    if (PowerButtons.CustomButtons.ContainsKey($"{item.id}_dej_{material}"))
                    {
                        PowerButtons.CustomButtons.Remove($"{item.id}_dej_{material}");
                    }
                    Sprite pSprite = null;
                    if (material != "base")
                    {
                        pSprite = Resources.Load<Sprite>($"ui/Icons/items/icon_{item.id}_{material}");
                    }
                    else
                    {
                        pSprite = Resources.Load<Sprite>($"ui/Icons/items/icon_{item.id}");
                    }
                    PowerButtons.CreateButton(
                        $"{item.id}_dej_{material}", 
                        pSprite, 
                        $"{item.id}_{material}", 
                        $"{item.id}_{material}", 
                        new Vector2(0, 0), 
                        ButtonType.Click, 
                        contents.transform, 
                        () => onItemClick(item, material)
                    );
                }
            }
        }

        public static void openWindow(int buttonID)
        {
            currentButtonID = buttonID;
            loadItemTypes();
            Windows.ShowWindow("itemTypeWindow");
        }

        private static void onItemClick(ItemAsset asset, string material = null)
        {
            ItemEditorWindow.itemAssets[currentButtonID.ToString()] = new ItemOption{
                asset = asset,
                active = PowerButtons.GetToggleValue($"Item_active_dej_{currentButtonID.ToString()}"),
                material = material,
                id = currentButtonID
            };
            GameObject icon = PowerButtons.CustomButtons[$"Item_type_dej_{currentButtonID.ToString()}"].gameObject.transform.GetChild(0).gameObject;
            string icon_path = $"ui/Icons/items/icon_{asset.id}";
            if (material != null)
            {
                icon_path = $"ui/Icons/items/icon_{asset.id}_{material}";
            }
            icon.GetComponent<Image>().sprite = Resources.Load<Sprite>(icon_path);
            Windows.ShowWindow("itemEditorWindow");
            return;
        }
    }
}