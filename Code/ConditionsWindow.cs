using System;
using System.Collections;
using System.Collections.Generic;
using NCMS;
using NCMS.Utils;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using ReflectionUtility;

namespace CollectionMod{
    class ConditionsWindow{
        private static GameObject traitObj;
        private static GameObject boolObj;
        private static GameObject statObj;
        private static GameObject nameObj;
        private static Title currentTitle = new Title();

        public static void init(){
            WindowManager.windowContents["conditionsWindow"].GetComponent<RectTransform>().sizeDelta += new Vector2(0, 50);
            createConditionButtons();
            traitObj = createConditionText(
                "Traits:",
                new Vector3(50, -40, 0)
            );
            boolObj = createConditionText(
                "Bools:",
                new Vector3(100, -40, 0)
            );
            statObj = createConditionText(
                "Stats:",
                new Vector3(150, -40, 0)
            );
            nameObj = createConditionText(
                "Names:",
                new Vector3(200, -40, 0)
            );
        }

        private static GameObject createConditionText(string text, Vector3 pos){
            GameObject condObj = NewUI.addText(
                text, 
                WindowManager.windowContents["conditionsWindow"],
                7,
                pos
            ).gameObject;
            GameObject adjustText = NewUI.addText(
                "",
                condObj,
                5,
                new Vector3(0, 0, 0)
            ).gameObject;
            adjustText.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 50);
            return condObj;
        }

        private static void createConditionButtons(){
            var scrollView = GameObject.Find($"/Canvas Container Main/Canvas - Windows/windows/conditionsWindow/Background/Scroll View");
            Button resultsButton = NewUI.createBGWindowButton(
                scrollView,
                50,
                "iconGoldCrown",
                "resultsButton",
                "Next!",
                "Create Results",
                submitCurrentTitle
            );

            Button resetButton = NewUI.createBGWindowButton(
                scrollView,
                10,
                "delete_ui",
                "resetButton",
                "Reset Conditions!",
                "Remove All Set Conditions",
                resetTitleConditions
            );

            NewUI.createTextButtonWSize(
                "addTraitCondition",
                "Add Trait Condition",
                new Vector2(80, -20),
                new Color(1f, 0.25f, 0f, 1f),
                WindowManager.windowContents["conditionsWindow"].transform,
                TitleTraitsWindow.openWindow,
                new Vector2(0, 15)
            );

            NewUI.createTextButtonWSize(
                "addBoolCondition",
                "Add Bool Condition",
                new Vector2(80, -60),
                new Color(1f, 0.25f, 0f, 1f),
                WindowManager.windowContents["conditionsWindow"].transform,
                TitleBoolsWindow.openWindow,
                new Vector2(0, 15)
            );

            NewUI.createTextButtonWSize(
                "addStatCondition",
                "Add Stat Condition",
                new Vector2(180, -20),
                new Color(1f, 0.25f, 0f, 1f),
                WindowManager.windowContents["conditionsWindow"].transform,
                TitleStatsWindow.openWindow,
                new Vector2(0, 15)
            );

            NewUI.createTextButtonWSize(
                "addNameCondition",
                "Add Name Condition",
                new Vector2(180, -60),
                new Color(1f, 0.25f, 0f, 1f),
                WindowManager.windowContents["conditionsWindow"].transform,
                TitleNamesWindow.openWindow,
                new Vector2(0, 15)
            );
        }

        public static void openWindow(){
            Windows.ShowWindow("conditionsWindow");
        }

        public static void addConditionText(ConditionType type, string text){
            Text textComp = null;
            switch(type){
                case ConditionType.traitj:
                    textComp = traitObj.transform.GetChild(0).GetComponent<Text>();
                    break;
                case ConditionType.boolj:
                    textComp = boolObj.transform.GetChild(0).GetComponent<Text>();
                    break;
                case ConditionType.statj:
                    textComp = statObj.transform.GetChild(0).GetComponent<Text>();
                    break;
                case ConditionType.namej:
                    textComp = nameObj.transform.GetChild(0).GetComponent<Text>();
                    break;
            }
            if (textComp == null){
                return;
            }
            textComp.text += $"\n{text}";
        }

        public static void resetConditionText(ConditionType type){
            Text textComp = null;
            switch(type){
                case ConditionType.traitj:
                    textComp = traitObj.transform.GetChild(0).GetComponent<Text>();
                    break;
                case ConditionType.boolj:
                    textComp = boolObj.transform.GetChild(0).GetComponent<Text>();
                    break;
                case ConditionType.statj:
                    textComp = statObj.transform.GetChild(0).GetComponent<Text>();
                    break;
                case ConditionType.namej:
                    textComp = nameObj.transform.GetChild(0).GetComponent<Text>();
                    break;
            }
            if (textComp == null){
                return;
            }
            textComp.text = "";
        }

        public static void addTitleConditions(ConditionType type, List<string> traitType = null, Dictionary<string, bool> boolType = null, 
        Dictionary<string, int> statType = null, string nameType = null){
            switch(type){
                case ConditionType.traitj:
                    currentTitle.titleTraits = traitType;
                    break;
                case ConditionType.boolj:
                    currentTitle.titleBools = boolType;
                    break;
                case ConditionType.statj:
                    currentTitle.titleStats = statType;
                    break;
                case ConditionType.namej:
                    if (!currentTitle.titleNames.Contains(nameType)){
                        currentTitle.titleNames.Add(nameType);
                    }
                    break;
            }
        }

        private static void resetCurrentTitle(){
            currentTitle = new Title();
        }

        private static void submitCurrentTitle(){
            ResultsWindow.setCurrentTitle(currentTitle);
            ResultsWindow.openWindow();
            resetTitleConditions();
        }

        private static void resetTitleConditions(){
            resetCurrentTitle();
            foreach(ConditionType type in Enum.GetValues(typeof(ConditionType))){
                resetConditionText(type);
            }
        }
    }
    public enum ConditionType{
        traitj,
        boolj,
        statj,
        namej
    }
}