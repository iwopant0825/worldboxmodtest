using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NCMS;
using NCMS.Utils;
using UnityEngine;
using UnityEngine.UI;
using ReflectionUtility;

namespace CollectionMod{
    class TitlesWindow : MonoBehaviour{
        
        private static Vector2 originalSize;

        public static void init(){
            originalSize = WindowManager.windowContents["titlesWindow"].GetComponent<RectTransform>().sizeDelta;
            var scrollView = GameObject.Find($"/Canvas Container Main/Canvas - Windows/windows/titlesWindow/Background/Scroll View");
            Button addTitleButton = NewUI.createBGWindowButton(
                scrollView,
                50,
                "iconGoldCrown",
                "AddTitleButton",
                "Create Title",
                "Add A New Title With Conditions And Results",
                ConditionsWindow.openWindow
            );
            // addTitleButton.onClick.AddListener(ConditionsWindow.openWindow);
        }

        public static void openWindow(){
            loadTitles();
            Windows.ShowWindow("titlesWindow");
        }

        private static void loadTitles(){
            foreach(Transform child in WindowManager.windowContents["titlesWindow"].transform){
                Destroy(child.gameObject);
            }
            WindowManager.windowContents["titlesWindow"].GetComponent<RectTransform>().sizeDelta += new Vector2(0, 80*(Main.savedSettings.titles.Count/2));
            int yPos = 0;
            foreach(Title title in Main.savedSettings.titles){
                Vector2 pos = new Vector2(130, -30+(yPos*-80));
                GameObject textButton = PowerButtons.CreateTextButton(
                    $"{title.name}_title",
                    title.name,
                    pos,
                    new Color(0, 0, 0, 1),
                    WindowManager.windowContents["titlesWindow"].transform,
                    TitlesWindow.openWindow
                ).gameObject;
                addTitleInfo(textButton, title);
                createDeleteTitleButton(WindowManager.windowContents["titlesWindow"], pos, title);
                createUpButton(WindowManager.windowContents["titlesWindow"], pos, title);
                createPopCount(WindowManager.windowContents["titlesWindow"], pos, title);
                yPos++;
            }
            Main.saveSettings();
        }

        private static void createDeleteTitleButton(GameObject parent, Vector2 pos, Title pTitle){
            GameObject deleteObj = new GameObject("deleteButton");
            deleteObj.transform.SetParent(parent.transform);
            Image deleteImg = deleteObj.AddComponent<Image>();
            deleteImg.sprite = Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.UI.delete_ui.png");
            Button deleteButton = deleteObj.AddComponent<Button>();
            deleteButton.onClick.AddListener(
                () => removeTitle(pTitle)
            );
            deleteObj.GetComponent<RectTransform>().localPosition = new Vector3(210, pos.y, 0);
        }

        private static void removeTitle(Title title){
            Main.savedSettings.titles.Remove(title);
            openWindow();
        }

        private static void addTitleInfo(GameObject parent, Title pTitle){
            GameObject infoHolder = NewUI.createSubWindow(parent, new Vector3(0, -35, 0), new Vector2(500, 100), new Vector2(500, 200));
            infoHolder.GetComponent<RectTransform>().localPosition = new Vector3(0, -50, 0);

            Text traitConditionsText = NewUI.addText("Trait Conditions:", infoHolder, 10, new Vector3(-175, -150, 0), new Vector2(0, 400));
            Text boolConditionsText = NewUI.addText("Bool Conditions:", infoHolder, 10, new Vector3(-85, -150, 0), new Vector2(0, 400));
            Text statConditionsText = NewUI.addText("Stat Conditions:", infoHolder, 10, new Vector3(5, -150, 0), new Vector2(0, 400));
            Text nameConditionsText = NewUI.addText("Name Conditions:", infoHolder, 10, new Vector3(85, -150, 0), new Vector2(0, 400));

            foreach(FieldInfo field in typeof(Title).GetFields()){
                if (field.FieldType == typeof(Dictionary<string, bool>)){
                    foreach(KeyValuePair<string, bool> kv in (Dictionary<string, bool>)field.GetValue(pTitle)){
                        string newKey = kv.Key;
                        if (MapBox.instance.cities.get(kv.Key) != null){
                            newKey = MapBox.instance.cities.get(kv.Key).data.name;
                        }
                        boolConditionsText.text += $"\n{newKey} : {kv.Value.ToString()}";
                    }
                }
                else if (field.FieldType == typeof(Dictionary<string, int>)){
                    foreach(KeyValuePair<string, int> kv in (Dictionary<string, int>)field.GetValue(pTitle)){
                        statConditionsText.text += $"\n{kv.Key} : {kv.Value.ToString()}";
                    }
                }
                else if (field.FieldType == typeof(List<string>) && field.Name == "titleTraits"){
                    foreach(string key in (List<string>)field.GetValue(pTitle)){
                        traitConditionsText.text += $"\n{key}";
                    }
                }
                else if (field.FieldType == typeof(List<string>) && field.Name == "titleNames"){
                    foreach(string key in (List<string>)field.GetValue(pTitle)){
                        nameConditionsText.text += $"\n{key}";
                    }
                }
            }
            traitConditionsText.gameObject.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 80);
            traitConditionsText.alignment = TextAnchor.UpperLeft;
            boolConditionsText.gameObject.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 80);
            boolConditionsText.alignment = TextAnchor.UpperLeft;
            statConditionsText.gameObject.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 80);
            statConditionsText.alignment = TextAnchor.UpperLeft;
            nameConditionsText.gameObject.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 80);
            nameConditionsText.alignment = TextAnchor.UpperLeft;

            Text resultsText = NewUI.addText("Results:", infoHolder, 10, new Vector3(180, -150, 0), new Vector2(0, 400));
            foreach(KeyValuePair<string, string> kv in pTitle.titleResults){
                resultsText.text += $"\n{kv.Key} : {kv.Value}";
            }
            resultsText.gameObject.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 80);
            resultsText.alignment = TextAnchor.UpperLeft;
        }

        private static void createUpButton(GameObject parent, Vector2 pos, Title pTitle){
            GameObject upObj = new GameObject("upButton");
            upObj.transform.SetParent(parent.transform);
            Image upImg = upObj.AddComponent<Image>();
            upImg.sprite = Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.UI.iconTalents.png");
            Button upButton = upObj.AddComponent<Button>();
            upButton.onClick.AddListener(
                () => reloadTitlePriority(pTitle)
            );
            upObj.GetComponent<RectTransform>().localPosition = new Vector3(60, pos.y, 0);
        }

        private static void reloadTitlePriority(Title pTitle){
            HeroicTitles.upTitlePriority(pTitle);
            loadTitles();
        }

        private static void createPopCount(GameObject parent, Vector2 pos, Title pTitle){
            GameObject popObj = new GameObject("PopTitleCount");
            popObj.transform.SetParent(parent.transform);
            popObj.AddComponent<Image>().sprite = Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.iconPopulationSmall.png");
            RectTransform popRect = popObj.GetComponent<RectTransform>();
            popRect.localPosition = new Vector3(80, pos.y+5, 0);
            popRect.sizeDelta /= 2;

            NewUI.addText(pTitle.previousNumber.ToString(), popObj, 25, new Vector3(0, 0, 0));
        }
    }
}
