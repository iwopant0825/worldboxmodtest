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
    class CreaturesWindow : MonoBehaviour
    {
        private static GameObject contents;
        private static GameObject scrollView;
        private static Vector2 originalSize;
        public static string currentCreature;

        public static void init()
        {
            currentCreature = SA.unit_human;
            contents = WindowManager.windowContents["creaturesWindow"];
            scrollView = GameObject.Find($"/Canvas Container Main/Canvas - Windows/windows/creaturesWindow/Background/Scroll View");
            originalSize = contents.GetComponent<RectTransform>().sizeDelta;
            GridLayoutGroup layoutGroup = contents.AddComponent<GridLayoutGroup>();
            layoutGroup.cellSize = new Vector2(30, 30);
            layoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            layoutGroup.constraintCount = 4;
            layoutGroup.childAlignment = TextAnchor.UpperCenter;
            layoutGroup.spacing = new Vector2(15, 5);
            loadCreatureButtons();
        }

        private static void loadCreatureButtons()
        {
            contents.GetComponent<RectTransform>().sizeDelta = new Vector2(0, (AssetManager.actor_library.list.Count/16)*200);
            foreach(ActorAsset actor in AssetManager.actor_library.list)
            {
                // if (building.id.Contains("!"))
                // {
                //     continue;
                // }
                Sprite icon = Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.iconArrowBack.png");
                if (!string.IsNullOrEmpty(actor.texture_path))
                {
                    icon = Resources.Load<Sprite>($"actors/{actor.texture_path}");
                }
                PowerButtons.CreateButton(
                    $"{actor.id}_dej", 
                    icon, 
                    actor.id, 
                    $"Select {actor.id}", 
                    new Vector2(0, 0), 
                    ButtonType.Click, 
                    contents.transform, 
                    () => setCreaturePower(actor.id, icon)
                );
            }
        }

        public static void openWindow()
        {
            Windows.ShowWindow("creaturesWindow");
        }

        private static void setCreaturePower(string creatureID, Sprite actorIcon)
        {
            currentCreature = creatureID;
            GodPower creatureDropPower = AssetManager.powers.get("creature_drop_dej");
            creatureDropPower.actor_asset_id = creatureID;
            GameObject icon = PowerButtons.CustomButtons["creature_drop_dej"].gameObject.transform.GetChild(0).gameObject;
            icon.GetComponent<Image>().sprite = actorIcon;
            GameObject icon2 = PowerButtons.CustomButtons["creature_select_dej"].gameObject.transform.GetChild(0).gameObject;
            icon2.GetComponent<Image>().sprite = actorIcon;
        }
    }
}