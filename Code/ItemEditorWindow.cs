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
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CollectionMod
{
    class ItemEditorWindow : MonoBehaviour
    {
        private static GameObject contents;
        private static GameObject scrollView;
        private static Vector2 originalSize;
        private static Culture currentCulture;
        private static int buttonID = 0;
        public static Dictionary<string, ItemOption> itemAssets = new Dictionary<string, ItemOption>();
        public static Dictionary<string, NameInput> itemNames = new Dictionary<string, NameInput>();
        public static Dictionary<string, List<ItemAsset>> itemModifiers = new Dictionary<string, List<ItemAsset>>();

        public static void init()
        {
            contents = WindowManager.windowContents["itemEditorWindow"];
            scrollView = GameObject.Find($"/Canvas Container Main/Canvas - Windows/windows/itemEditorWindow/Background/Scroll View");
            originalSize = contents.GetComponent<RectTransform>().sizeDelta;
            VerticalLayoutGroup layoutGroup = contents.AddComponent<VerticalLayoutGroup>();
            layoutGroup.childControlHeight = false;
            layoutGroup.childControlWidth = false;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.childScaleHeight = true;
            layoutGroup.childScaleWidth = true;
            layoutGroup.childAlignment = TextAnchor.UpperCenter;
            layoutGroup.spacing = 90;

            Button loadOutOneButton = NewUI.createBGWindowButton(
                scrollView,
                90,
                "load_icon",
                "itemLoadOutOneButton",
                "Insert Load Out One!",
                "Load In The Saved Items",
                () => loadSavedItems("Load Out 1")
            );
            Button saveLoadOutOneButton = NewUI.createBGWindowButton(
                scrollView,
                60,
                "save_icon",
                "itemSaveLoadOutOneButton",
                "Save Load Out One!",
                "Save These Selected Items So That You Can Open Them Again Later",
                () => Main.modifySavedItems("Load Out 1")
            );

            Button loadOutTwoButton = NewUI.createBGWindowButton(
                scrollView,
                10,
                "load_icon",
                "itemLoadOutTwoButton",
                "Insert Load Out Two!",
                "Load In The Saved Items",
                () => loadSavedItems("Load Out 2")
            );
            Button saveLoadOutTwoButton = NewUI.createBGWindowButton(
                scrollView,
                -20,
                "save_icon",
                "itemSaveLoadOutTwoButton",
                "Save Load Out One!",
                "Save These Selected Items So That You Can Open Them Again Later",
                () => Main.modifySavedItems("Load Out 2")
            );

            Button loadOutThreeButton = NewUI.createBGWindowButton(
                scrollView,
                -70,
                "load_icon",
                "itemLoadOutThreeButton",
                "Insert Load Out Three!",
                "Load In The Saved Items",
                () => loadSavedItems("Load Out 3")
            );
            Button saveLoadOutThreButton = NewUI.createBGWindowButton(
                scrollView,
                -100,
                "save_icon",
                "itemSaveLoadOutThreeButton",
                "Save Load Out One!",
                "Save These Selected Items So That You Can Open Them Again Later",
                () => Main.modifySavedItems("Load Out 3")
            );

            loadItems();
        }

        private static void loadItems()
        {
            foreach(EquipmentType type in Enum.GetValues(typeof(EquipmentType)))
            {
                addNewItem(type);
            }
        }

        public static void openWindow()
        {
            Windows.ShowWindow("itemEditorWindow");
        }

        private static void addNewItem(EquipmentType type)
        {
            int currentButtonID = 0;
            currentButtonID += buttonID;

            itemModifiers.Add(currentButtonID.ToString(), new List<ItemAsset>());
            itemAssets.Add(currentButtonID.ToString(), new ItemOption());

            contents.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 150);
            GameObject itemHolder = new GameObject("ItemHolder");
            Image itemHolderImg = itemHolder.AddComponent<Image>();
            itemHolderImg.color = new Color(0f, 0f, 0f, 0.01f);
            itemHolder.transform.SetParent(contents.transform);
            NameInput nameInput = NewUI.createInputOption(
                $"itemNameOption_{currentButtonID.ToString()}",
                $"Name Your {type.ToString()}",
                $"Write Down A Name For Your Chosen {type.ToString()}",
                0,
                itemHolder,
                ""
            );
            nameInput.transform.parent.GetComponent<RectTransform>().localPosition = new Vector3(0, -50, 0);
            itemNames.Add(currentButtonID.ToString(), nameInput);

            PowerButton itemTypeButton = PowerButtons.CreateButton(
                $"Item_type_dej_{currentButtonID.ToString()}", 
                Sprites.LoadSprite($"{Mod.Info.Path}/icon.png"), 
                "Item Type", 
                "Choose The Equipment Type!", 
                new Vector2(-100, -160), 
                ButtonType.Click, 
                itemHolder.transform, 
                () => ItemTypeWindow.openWindow(currentButtonID)
            );
            itemTypeButton.gameObject.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);
            itemTypeButton.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);

            PowerButton itemModButton = PowerButtons.CreateButton(
                $"Item_modifiers_dej_{currentButtonID.ToString()}", 
                Sprites.LoadSprite($"{Mod.Info.Path}/icon.png"), 
                "Item Modifiers", 
                "Choose The Modifiers For Your Equipment!", 
                new Vector2(0, -160), 
                ButtonType.Click, 
                itemHolder.transform, 
                () => ItemModWindow.openWindow(currentButtonID)
            );
            itemModButton.gameObject.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);
            itemModButton.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);

            PowerButton itemActiveButton = PowerButtons.CreateButton(
                $"Item_active_dej_{currentButtonID.ToString()}", 
                Sprites.LoadSprite($"{Mod.Info.Path}/icon.png"), 
                "Activate Item", 
                "Toggle Whether Item Will Be Dropped Or Not!", 
                new Vector2(100, -160), 
                ButtonType.Toggle, 
                itemHolder.transform, 
                () => toggleItemActive(currentButtonID.ToString())
            );
            itemActiveButton.gameObject.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);
            itemActiveButton.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);
            buttonID++;
        }

        private static void toggleItemActive(string id)
        {
            if (itemAssets.ContainsKey(id))
            {
                itemAssets[id].active = PowerButtons.GetToggleValue($"Item_active_dej_{id.ToString()}");
            }
        }

        private static void loadSavedItems(string key)
        {
            if (!Main.savedSettings.savedItems.ContainsKey(key))
            {
                return;
            }
            itemAssets = Main.savedSettings.savedItems[key].itemAssets;
            itemModifiers = Main.savedSettings.savedItems[key].itemModifiers;
            foreach(KeyValuePair<string, NameInput> kv in itemNames)
            {
                kv.Value.setText(Main.savedSettings.savedItems[key].itemNames[kv.Key]);
            }
            foreach(KeyValuePair<string, ItemOption> kv in itemAssets)
            {
                if (kv.Value.asset == null)
                {
                    continue;
                }
                GameObject icon = PowerButtons.CustomButtons[$"Item_type_dej_{kv.Key}"].gameObject.transform.GetChild(0).gameObject;
                string icon_path = $"ui/Icons/items/icon_{kv.Value.asset.id}";
                if (kv.Value.material != null && kv.Value.material != "base")
                {
                    icon_path = $"ui/Icons/items/icon_{kv.Value.asset.id}_{kv.Value.material}";
                }
                icon.GetComponent<Image>().sprite = Resources.Load<Sprite>(icon_path);
            }
        }
    }

    [Serializable]
    public class ItemOption
    {
        public ItemAsset asset;
        public bool active;
        public string material;
        public int id;
    }
}