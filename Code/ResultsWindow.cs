using System;
using System.Collections;
using System.Collections.Generic;
using NCMS;
using NCMS.Utils;
using UnityEngine;
using UnityEngine.UI;
using ReflectionUtility;

namespace CollectionMod{
    class ResultsWindow{

        private static Title currentTitle;
        private static List<NameInput> inputFields = new List<NameInput>();

        public static void init(){
            WindowManager.windowContents["resultsWindow"].GetComponent<RectTransform>().sizeDelta += new Vector2(0, 100);
            var scrollView = GameObject.Find($"/Canvas Container Main/Canvas - Windows/windows/resultsWindow/Background/Scroll View");

            NameInput titleNameField = NewUI.createInputOption(
                "TitleNameInputHolder",
                "Name Of Title",
                "Make a name for the title you want to create",
                -30,
                WindowManager.windowContents["resultsWindow"],
                ""
            );

            Button doneButton = NewUI.createBGWindowButton(
                scrollView,
                50,
                "iconGoldCrown",
                "doneButton",
                "Done!",
                "Click This To Create The Title",
                () => setTitleName(titleNameField.inputField.text)
            );

            PowerButton favoriteButton = PowerButtons.CreateButton(
                "favoriteResultButton", 
                SpriteTextureLoader.getSprite("ui/Icons/iconFavoriteStar"), 
                "Favorite The Unit", 
                "Toggle this to make it so that a unit will automatically be favorited when conditions are cleared!", 
                new Vector2(130, -70), 
                ButtonType.Toggle, 
                WindowManager.windowContents["resultsWindow"].transform, 
                null
            );
            doneButton.onClick.AddListener(
                () => addTitleResults("Favorite", PowerButtons.GetToggleValue("favoriteResultButton").ToString())
            );
            favoriteButton.gameObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

            NameInput prefixField = NewUI.createInputOption(
                "PrefixInputHolder",
                "Name Prefix",
                "Add a prefix to the unit's name",
                -120,
                WindowManager.windowContents["resultsWindow"],
                ""
            );
            doneButton.onClick.AddListener(
                () => addTitleResults("Prefix", prefixField.inputField.text)
            );

            NameInput suffixField = NewUI.createInputOption(
                "SuffixInputHolder",
                "Name Suffix",
                "Add a suffix to the unit's name",
                -170,
                WindowManager.windowContents["resultsWindow"],
                ""
            );
            doneButton.onClick.AddListener(
                () => addTitleResults("Suffix", suffixField.inputField.text)
            );

            PowerButton nameNumButton = PowerButtons.CreateButton(
                "nameNumResultButton", 
                SpriteTextureLoader.getSprite("ui/Icons/iconFavoriteStar"), 
                "Numbered Names", 
                "Toggle this to make it so that a unit will get a prefixed number on their name!", 
                new Vector2(130, -210), 
                ButtonType.Toggle, 
                WindowManager.windowContents["resultsWindow"].transform, 
                null
            );
            doneButton.onClick.AddListener(
                () => addTitleResults("NameNumber", PowerButtons.GetToggleValue("nameNumResultButton").ToString())
            );
            nameNumButton.gameObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

            PowerButton resultTraitsButton = PowerButtons.CreateButton(
                "resultTraitsButton", 
                SpriteTextureLoader.getSprite("ui/Icons/iconFavoriteStar"), 
                "Trait Results", 
                "Add traits that will be given to the unit that passed the conditions!", 
                new Vector2(130, -250), 
                ButtonType.Click, 
                WindowManager.windowContents["resultsWindow"].transform, 
                ResultTraitsWindow.openWindow
            );

            doneButton.onClick.AddListener(submitNewTitle);

            inputFields.Add(titleNameField);
            inputFields.Add(prefixField);
            inputFields.Add(suffixField);
        }

        public static void openWindow(){
            Windows.ShowWindow("resultsWindow");
        }

        public static void setCurrentTitle(Title title){
            currentTitle = title;
        }

        public static void addTitleResults(string resultName, string resultValue){
            currentTitle.titleResults.Add(resultName, resultValue);
        }

        private static void resetCurrentTitle(){
            currentTitle = new Title();
        }

        private static void submitNewTitle(){
            Main.savedSettings.titles.Add(currentTitle);
            resetCurrentTitle();
            if (PowerButtons.GetToggleValue("favoriteResultButton")){
                PowerButtons.ToggleButton("favoriteResultButton");
            }
            if (PowerButtons.GetToggleValue("nameNumResultButton")){
                PowerButtons.ToggleButton("nameNumResultButton");
            }
            foreach(NameInput input in inputFields){
                input.setText("");
            }
            TitlesWindow.openWindow();
        }

        private static void setTitleName(string text){
            currentTitle.name = text;
        }
    }
}