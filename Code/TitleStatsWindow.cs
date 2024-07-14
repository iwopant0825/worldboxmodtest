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
    class TitleStatsWindow : MonoBehaviour{
        private static Dictionary<string, NameInput> inputFields = new Dictionary<string, NameInput>();

        public static void init(){
            var scrollView = GameObject.Find($"/Canvas Container Main/Canvas - Windows/windows/titleStatsWindow/Background/Scroll View");
            Button submitButton = NewUI.createBGWindowButton(
                scrollView,
                50,
                "iconGoldCrown",
                "statSubmitButton",
                "Submit!",
                "Add The Stat Conditions",
                submit
            );

            initOptions();
        }

        public static void openWindow(){
            Windows.ShowWindow("titleStatsWindow");
        }

        private static void initOptions(){
            createStatInputOption(
                "healthInput", 
                "Health Stat Condition", 
                "Set how much health is needed to clear the conditions", 
                -40, 
                WindowManager.windowContents["titleStatsWindow"], 
                "0"
            );

            createStatInputOption(
                "killInput", 
                "Kills Stat Condition", 
                "Set how much kills is needed to clear the conditions", 
                -90, 
                WindowManager.windowContents["titleStatsWindow"], 
                "0"
            );

            createStatInputOption(
                "levelInput", 
                "Levels Stat Condition", 
                "Set how much levels is needed to clear the conditions", 
                -140, 
                WindowManager.windowContents["titleStatsWindow"], 
                "0"
            );

            createStatInputOption(
                "ageInput", 
                "Age Stat Condition", 
                "Set how old a unit need to be to clear the conditions", 
                -190, 
                WindowManager.windowContents["titleStatsWindow"], 
                "0"
            );
        }

        private static void createStatInputOption(string objName, string title, string desc, int posY, GameObject parent, string textValue = "-1"){
            NameInput pInput = NewUI.createInputOption(
                objName, 
                title, 
                desc, 
                posY, 
                parent, 
                textValue
            );
            pInput.inputField.characterValidation = InputField.CharacterValidation.Integer;
            pInput.inputField.onValueChanged.AddListener(delegate{
                checkStatInput(pInput);
            });
            inputFields.Add(objName, pInput);
        }

        private static void checkStatInput(NameInput pInput){
            int num = -1;
            if (!int.TryParse(pInput.inputField.text, out num)){
                pInput.setText("0");
                return;
            }
            if (num > 9999){
                pInput.setText("9999");
                return;
            }
            if (num < 0){
                pInput.setText("0");
                return;
            }
        }

        private static void submit(){
            Dictionary<string, int> statConditions = new Dictionary<string, int>();
            foreach(KeyValuePair<string, NameInput> kv in inputFields){
                int i = kv.Key.IndexOf("Input");
                statConditions.Add(kv.Key.Remove(i), int.Parse(kv.Value.inputField.text));
                ConditionsWindow.addConditionText(ConditionType.statj, $"{kv.Key.Remove(i)} : {kv.Value.inputField.text}");
                kv.Value.setText("0");
            }

            ConditionsWindow.addTitleConditions(ConditionType.statj, null, null, statConditions);
            ConditionsWindow.openWindow();
        }
    }
}