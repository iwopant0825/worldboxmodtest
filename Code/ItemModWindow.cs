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
    class ItemModWindow : MonoBehaviour
    {
        private static GameObject contents;
        private static GameObject scrollView;
        private static Vector2 originalSize;
        public static int currentButtonID;

        public static void init()
        {
            contents = WindowManager.windowContents["itemModWindow"];
            scrollView = GameObject.Find($"/Canvas Container Main/Canvas - Windows/windows/itemModWindow/Background/Scroll View");
            originalSize = contents.GetComponent<RectTransform>().sizeDelta;
            Button submitButton = NewUI.createBGWindowButton(
                scrollView,
                50,
                "refresh_icon",
                "itemModClearButton",
                "Clear!",
                "Refresh The Selected Modifiers For The Item",
                clearToggleButtons
            );
            loadItemMods();
        }

        private static void loadItemMods()
        {
            foreach(Transform child in contents.transform)
            {
                Destroy(child.gameObject);
            }
            contents.GetComponent<RectTransform>().sizeDelta = new Vector2(0, (AssetManager.items_modifiers.list.Count/16)*originalSize.y) + originalSize;

            int index = 0;
            int indexY = 0;
            foreach(ItemAsset mod in AssetManager.items_modifiers.list)
            {
                if (PowerButtons.CustomButtons.ContainsKey($"{mod.id}_modifier_dej"))
                {
                    PowerButtons.CustomButtons.Remove($"{mod.id}_modifier_dej");
                }
                Sprite iconSprite = Sprites.LoadSprite($"{Mod.Info.Path}/icon.png");
                if(mod.mod_type != "mastery")
                {
                    iconSprite = Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.ItemIcons.{mod.id}.png");
                }
                PowerButtons.CreateButton(
                    $"{mod.id}_modifier_dej", 
                    iconSprite, 
                    mod.id, 
                    mod.id, 
                    new Vector2(60 + (index*35), -40 + (indexY*-40)), 
                    ButtonType.Toggle, 
                    contents.transform, 
                    () => onModClick(mod)
                );
                increaseIndex(ref index, ref indexY);
            }
        }

        private static void increaseIndex(ref int index, ref int indexY)
        {
            index++;
            if (index > 4)
            {
                index = 0;
                indexY++;
            }
        }

        public static void openWindow(int buttonID)
        {
            currentButtonID = buttonID;
            checkToggleButtons();
            Windows.ShowWindow("itemModWindow");
        }

        private static void onModClick(ItemAsset mod)
        {
            if (!PowerButtons.GetToggleValue($"{mod.id}_modifier_dej") && ItemEditorWindow.itemModifiers.ContainsKey(currentButtonID.ToString()))
            {
                ItemEditorWindow.itemModifiers[currentButtonID.ToString()].Remove(mod);
            }
            else if (ItemEditorWindow.itemModifiers.ContainsKey(currentButtonID.ToString()))
            {
                ItemEditorWindow.itemModifiers[currentButtonID.ToString()].Add(mod);
            }
            else
            {
                ItemEditorWindow.itemModifiers.Add(currentButtonID.ToString(), new List<ItemAsset>{mod});
            }
        }

        private static void checkToggleButtons()
        {
            foreach(KeyValuePair<string, PowerButton> kv in PowerButtons.CustomButtons)
            {
                if (!kv.Key.Contains("_modifier_dej"))
                {
                    continue;
                }
                string itemID = kv.Key.Remove(kv.Key.IndexOf("_modifier_dej"));
                ItemAsset asset = AssetManager.items_modifiers.get(itemID);
                if (!PowerButtons.GetToggleValue(kv.Key) && !ItemEditorWindow.itemModifiers[currentButtonID.ToString()].Contains(asset))
                {
                    continue;
                }
                if (PowerButtons.GetToggleValue(kv.Key) && ItemEditorWindow.itemModifiers[currentButtonID.ToString()].Contains(asset))
                {
                    continue;
                }
                PowerButtons.ToggleButton(kv.Key);
            }
        }

        private static void clearToggleButtons()
        {
            foreach(KeyValuePair<string, PowerButton> kv in PowerButtons.CustomButtons)
            {
                if (!kv.Key.Contains("_modifier_dej"))
                {
                    continue;
                }
                if (!PowerButtons.GetToggleValue(kv.Key))
                {
                    continue;
                }
                string itemID = kv.Key.Remove(kv.Key.IndexOf("_modifier_dej"));
                ItemAsset asset = AssetManager.items_modifiers.get(itemID);
                ItemEditorWindow.itemModifiers[currentButtonID.ToString()].Remove(asset);
                PowerButtons.ToggleButton(kv.Key);
            }
        }
    }
}