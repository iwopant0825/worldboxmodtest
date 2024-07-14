using System;
using System.Collections;
using System.Collections.Generic;
using NCMS;
using NCMS.Utils;
using UnityEngine;
using ReflectionUtility;

namespace CollectionMod
{
    class WindowManager
    {
        public static Dictionary<string, GameObject> windowContents = new Dictionary<string, GameObject>();
        public static Dictionary<string, ScrollWindow> createdWindows = new Dictionary<string, ScrollWindow>();

        public static void init()
        {
            newWindow("leaderBoardWindow", "Leader Board");
            LeaderBoardWindow.init();
            newWindow("filterWindow", "Filters");
            FilterWindow.init();
            newWindow("titlesWindow", "Titles");
            TitlesWindow.init();
            newWindow("conditionsWindow", "Conditions");
            ConditionsWindow.init();
            newWindow("resultsWindow", "Results");
            ResultsWindow.init();
            newWindow("titleTraitsWindow", "Select Traits");
            TitleTraitsWindow.init();
            newWindow("titleStatsWindow", "Select Stats");
            TitleStatsWindow.init();
            newWindow("titleBoolsWindow", "Select Bools");
            TitleBoolsWindow.init();
            newWindow("titleNamesWindow", "Select Names");
            TitleNamesWindow.init();
            newWindow("resultTraitsWindow", "Select Traits");
            ResultTraitsWindow.init();
            newWindow("editCultureWindow", "Edit Cultures");
            EditCultureWindow.init();
            newWindow("cultureTechWindow", "Culture Technologies");
            CultureTechWindow.init();
            newWindow("statLimiterWindow", "Limit Stats");
            StatLimiterWindow.init();
            newWindow("buildingsWindow", "Select Building");
            BuildingsWindow.init();
            newWindow("itemEditorWindow", "Item Editor");
            ItemEditorWindow.init();
            newWindow("itemTypeWindow", "Item Types");
            ItemTypeWindow.init();
            newWindow("itemModWindow", "Item Modifiers");
            ItemModWindow.init();
            newWindow("editResourcesWindow", "Edit Resources");
            EditResourcesWindow.init();
            newWindow("modSettingsWindow", "Mod Settings");
            ModSettingsWindow.init();
            newWindow("crpgSettingsWindow", "CRPG Settings");
            CRPGSettingsWindow.init();
            newWindow("crpgTalentStatsWindow", "CRPG Talent Stats");
            CRPGTalentStatsWindow.init();
            newWindow("crpgLevelStatsWindow", "CRPG Talent Stats");
            CRPGLevelStatsWindow.init();
            newWindow("crpgTitlesWindow", "CRPG Titles");
            CRPGTitlesWindow.init();
            newWindow("crpgAttributesWindow", "CRPG Attributes");
            CRPGAttributesWindow.init();
            newWindow("creaturesWindow", "Select Creature");
            CreaturesWindow.init();
            newWindow("agesWindow", "Select Age");
            AgesWindow.init();
            newWindow("moodsWindow", "Select Mood");
            MoodsWindow.init();
            newWindow("toggleAgesWindow", "Toggle Age");
            ToggleAgesWindow.init();
            newWindow("magnetSettingsWindow", "Magnet Settings");
            MagnetSettingsWindow.init();
        }

        private static void newWindow(string id, string title)
        {
            ScrollWindow window;
            GameObject content;
            window = Windows.CreateNewWindow(id, title);
            createdWindows.Add(id, window);

            GameObject scrollView = GameObject.Find($"/Canvas Container Main/Canvas - Windows/windows/{window.name}/Background/Scroll View");
            scrollView.gameObject.SetActive(true);

            content = GameObject.Find($"/Canvas Container Main/Canvas - Windows/windows/{window.name}/Background/Scroll View/Viewport/Content");
            if (content != null)
            {
                windowContents.Add(id, content);
            }
        }

        public static void updateScrollRect(GameObject content, int count, int size)
        {
            var scrollRect = content.GetComponent<RectTransform>();
            scrollRect.sizeDelta = new Vector2(0, count*size);
        }
    }
}