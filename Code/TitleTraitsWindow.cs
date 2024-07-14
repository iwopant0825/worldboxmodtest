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
    class TitleTraitsWindow : MonoBehaviour{
        
        private static Vector2 originalSize;

        public static void init(){
            var scrollView = GameObject.Find($"/Canvas Container Main/Canvas - Windows/windows/titleTraitsWindow/Background/Scroll View");
            Button submitButton = NewUI.createBGWindowButton(
                scrollView,
                50,
                "iconGoldCrown",
                "traitSubmitButton",
                "Submit!",
                "Add The Trait Conditions",
                submit
            );
            originalSize = WindowManager.windowContents["titleTraitsWindow"].GetComponent<RectTransform>().sizeDelta;
        }

        private static void createTraitButtons(){
            WindowManager.windowContents["titleTraitsWindow"].GetComponent<RectTransform>().sizeDelta = originalSize;
            WindowManager.windowContents["titleTraitsWindow"].GetComponent<RectTransform>().sizeDelta *= (int)(AssetManager.traits.list.Count/20)+1;
            int x = 65;
            int y = -20;
            foreach(ActorTrait pTrait in AssetManager.traits.list){
                string newID = $"{pTrait.id}_TraitCondition";
                if (PowerButtons.CustomButtons.ContainsKey(newID)){
                    PowerButtons.CustomButtons.Remove(newID);
                    PowerButtons.ToggleValues.Remove(newID);
                }
                string name = pTrait.id;
                string desc = "";
                Main.getLocalization($"trait_{pTrait.id}", ref name, ref desc, "_info");
                Sprite traitIcon = null;
                if (string.IsNullOrEmpty(pTrait.path_icon)){
                    traitIcon = SpriteTextureLoader.getSprite("ui/Icons/iconFavoriteStar");
                }else{
                    traitIcon = SpriteTextureLoader.getSprite(pTrait.path_icon);
                }
                PowerButton traitButton = PowerButtons.CreateButton(
                    newID, 
                    traitIcon, 
                    name, 
                    desc, 
                    new Vector2(x, y), 
                    ButtonType.Toggle, 
                    WindowManager.windowContents["titleTraitsWindow"].transform, 
                    null
                );
                traitButton.gameObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                x += 40;
                if (x >= 220){
                    y -= 40;
                    x = 65;
                }
            }
        }

        public static void openWindow(){   
            createTraitButtons();
            Windows.ShowWindow("titleTraitsWindow");
        }

        private static void submit(){
            ConditionsWindow.resetConditionText(ConditionType.traitj);
            List<string> toggledTraits = checkButtonToggles();
            foreach(string toggleTrait in toggledTraits){
                ConditionsWindow.addConditionText(ConditionType.traitj, LocalizedTextManager.getText($"trait_{toggleTrait}"));
            }
            ConditionsWindow.addTitleConditions(ConditionType.traitj, toggledTraits);
            Windows.ShowWindow("conditionsWindow");
        }

        private static List<string> checkButtonToggles(){
            List<string> toggledTraits = new List<string>();
            foreach(KeyValuePair<string, bool> kv in PowerButtons.ToggleValues){
                if (kv.Key.Contains("_TraitCondition") && kv.Value){
                    int i = kv.Key.IndexOf("_TraitCondition");
                    toggledTraits.Add(kv.Key.Remove(i));
                }
            }
            foreach(string trait in toggledTraits){
                PowerButtons.ToggleButton($"{trait}_TraitCondition");
            }
            return toggledTraits;
        }
    }
}