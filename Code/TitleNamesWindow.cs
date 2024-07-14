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
    class TitleNamesWindow : MonoBehaviour{

        public static void init(){
            var scrollView = GameObject.Find($"/Canvas Container Main/Canvas - Windows/windows/titleNamesWindow/Background/Scroll View");
            NameInput nameField = NewUI.createInputOption(
                "NameInputHolder",
                "Name Condition",
                "Type in a name that would serve as a condition",
                -30,
                WindowManager.windowContents["titleNamesWindow"],
                ""
            );

            Button addNameButton = NewUI.createBGWindowButton(
                scrollView,
                50,
                "iconGoldCrown",
                "AddNameButton",
                "Create Title",
                "Add A New Title With Conditions And Results",
                () => submit(nameField.inputField.text)
            );
        }

        public static void openWindow(){
            Windows.ShowWindow("titleNamesWindow");
        }

        private static void submit(string value){
            if (string.IsNullOrEmpty(value)){
                Windows.ShowWindow("conditionsWindow");
                return;
            }
            ConditionsWindow.addConditionText(ConditionType.namej, value);
            ConditionsWindow.addTitleConditions(ConditionType.namej, null, null, null, value);
            Windows.ShowWindow("conditionsWindow");
        }
    }
}