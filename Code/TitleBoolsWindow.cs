using System;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using NCMS;
using NCMS.Utils;
using UnityEngine;
using UnityEngine.UI;
using ReflectionUtility;

namespace CollectionMod{
    class TitleBoolsWindow : MonoBehaviour{

        private static Vector2 originalSize;

        public static void init(){
            originalSize = WindowManager.windowContents["titleBoolsWindow"].GetComponent<RectTransform>().sizeDelta;
            var scrollView = GameObject.Find($"/Canvas Container Main/Canvas - Windows/windows/titleBoolsWindow/Background/Scroll View");
            Button submitButton = NewUI.createBGWindowButton(
                scrollView,
                50,
                "iconGoldCrown",
                "boolSubmitButton",
                "Submit!",
                "Add The Trait Conditions",
                submit
            );
        }
        
        private static void addActorStatsButtons(){
            WindowManager.windowContents["titleBoolsWindow"].GetComponent<RectTransform>().sizeDelta = originalSize;
            int x = 65;
            int y = -20;
            foreach(ActorAsset aStats in AssetManager.actor_library.list){
                if (!aStats.unit || aStats.baby){
                    continue;
                }
                string newID = $"{aStats.id}_BoolCondition";
                if (PowerButtons.CustomButtons.ContainsKey(newID)){
                    PowerButtons.CustomButtons.Remove(newID);
                    PowerButtons.ToggleValues.Remove(newID);
                }
                string name = aStats.nameLocale;
                string desc = "";
                Main.getLocalization(aStats.nameLocale, ref name,  ref desc, " Description");
                PowerButton aStatsButton = PowerButtons.CreateButton(
                    newID, 
                    (Sprite)Resources.Load("ui/Icons/" + aStats.icon, typeof(Sprite)), 
                    name, 
                    desc, 
                    new Vector2(x, y), 
                    ButtonType.Toggle, 
                    WindowManager.windowContents["titleBoolsWindow"].transform, 
                    null
                );
                aStatsButton.gameObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                x += 40;
                if (x >= 220){
                    y -= 40;
                    x = 65;
                }
            }
            WindowManager.windowContents["titleBoolsWindow"].GetComponent<RectTransform>().sizeDelta += new Vector2(0, 200*(MapBox.instance.cities.list.Count/16));
            foreach(City pCity in MapBox.instance.cities.list){
                string newID = $"{pCity.data.id}_BoolCondition";
                if (PowerButtons.CustomButtons.ContainsKey(newID)){
                    PowerButtons.CustomButtons.Remove(newID);
                    PowerButtons.ToggleValues.Remove(newID);
                }
                Kingdom pKingdom = (Kingdom)Reflection.GetField(typeof(City), pCity, "kingdom");
                Race pRace = (Race)Reflection.GetField(typeof(Kingdom), pKingdom, "race");
                BannerContainer bannerContainer = BannerGenerator.dict[pRace.banner_id];
                Sprite backgroundBanner = bannerContainer.backrounds[pKingdom.data.banner_background_id];
                string name = pCity.data.name;
                string desc = pCity.data.race;
                PowerButton pCityButton = PowerButtons.CreateButton(
                    newID, 
                    backgroundBanner, 
                    name, 
                    desc, 
                    new Vector2(x, y), 
                    ButtonType.Toggle, 
                    WindowManager.windowContents["titleBoolsWindow"].transform, 
                    null
                );
                pCityButton.gameObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                Image bannerBG = pCityButton.gameObject.transform.Find("Icon").GetComponent<Image>();
                ColorAsset pKingdomColor = (ColorAsset)Reflection.GetField(typeof(Kingdom), pKingdom, "kingdomColor");
                bannerBG.color = pKingdomColor.getColorBorderOut_capture();

                GameObject bannerIcon = new GameObject("BannerIcon");
                bannerIcon.transform.SetParent(pCityButton.gameObject.transform);
                Image bannerIconImg = bannerIcon.AddComponent<Image>();
                bannerIconImg.sprite = bannerContainer.icons[pKingdom.data.banner_icon_id];
                bannerIconImg.color = pKingdomColor.getColorBorderInsideAlpha();
                RectTransform bannerIconRect = bannerIcon.GetComponent<RectTransform>();
                bannerIconRect.localPosition = new Vector3(0, 0, 0);
                bannerIconRect.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                x += 40;
                if (x >= 220){
                    y -= 40;
                    x = 65;
                }
            }
        }

        public static void openWindow(){
            foreach(Transform child in WindowManager.windowContents["titleBoolsWindow"].transform){
                Destroy(child.gameObject);
            }
            addActorStatsButtons();
            Windows.ShowWindow("titleBoolsWindow");
        }

        private static void submit(){
            ConditionsWindow.resetConditionText(ConditionType.boolj);
            Dictionary<string, bool> toggledBools = checkButtonToggles();
            foreach(KeyValuePair<string, bool> kv in toggledBools){
                string newKey = kv.Key;
                if (MapBox.instance.cities.get(kv.Key) != null){
                    newKey = MapBox.instance.cities.get(kv.Key).data.name;
                }
                ConditionsWindow.addConditionText(ConditionType.boolj, $"{newKey} : {kv.Value}");
            }
            ConditionsWindow.addTitleConditions(ConditionType.boolj, null, toggledBools);
            Windows.ShowWindow("conditionsWindow");
        }

        private static Dictionary<string, bool> checkButtonToggles(){
            Dictionary<string, bool> toggledBools = new Dictionary<string, bool>();
            foreach(KeyValuePair<string, bool> kv in PowerButtons.ToggleValues){
                if (kv.Key.Contains("_BoolCondition") && kv.Value){
                    int i = kv.Key.IndexOf("_BoolCondition");
                    toggledBools.Add(kv.Key.Remove(i), kv.Value);
                }
            }
            foreach (KeyValuePair<string, bool> kv in toggledBools){
                PowerButtons.ToggleButton($"{kv.Key}_BoolCondition");
            }
            return toggledBools;
        }
    }
}