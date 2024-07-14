using System;
using System.Text;
using System.Threading.Tasks;
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
    class TabManager : MonoBehaviour
    {
        public static void init()
        {
            NewUI.createTab("Button Tab_CollectionMod", "Tab_CollectionMod", "CollectionMod", "This mod is a collection of features that came from my past mods", -150);
            loadButtons();
        }

        private static void loadButtons()
        {
            PowersTab collectionTab = getPowersTab("CollectionMod");
            int index = 0;
            int xPos = 72;
            int yPos = 18;
            int gap = 35;
            PowerButtons.CreateButton(
                "warrior_dej", 
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.warrior_icon2.png"), 
                "Warrior", 
                "Change Unit's Job To Warrior", 
                new Vector2(xPos + (index*gap), yPos), 
                ButtonType.GodPower, 
                collectionTab.transform, 
                null
            );
            index++;

            PowerButtons.CreateButton(
                "citizen_dej", 
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.civilian_icon2.png"), 
                "Citizen", 
                "Change Unit's Job To Citizen", 
                new Vector2(xPos + (index*gap), yPos), 
                ButtonType.GodPower, 
                collectionTab.transform, 
                null
            );
            index++;

            PowerButtons.CreateButton(
                "king_dej", 
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.king_icon2.png"), 
                "King", 
                "Change Unit's Job To King", 
                new Vector2(xPos + (index*gap), yPos), 
                ButtonType.GodPower, 
                collectionTab.transform, 
                null
            );
            index++;

            PowerButtons.CreateButton(
                "city_convert_dej", 
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.icon_ConvertCity.png"), 
                "Convert City", 
                "Change A City's Fealty To Another Kingdom", 
                new Vector2(xPos + (index*gap), yPos), 
                ButtonType.GodPower, 
                collectionTab.transform, 
                null
            );
            index++;

            PowerButtons.CreateButton(
                "leader_board_dej", 
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.iconLeaderBoardCivs2.png"), 
                "Leader Board", 
                "View The Significant Units Within Your World", 
                new Vector2(xPos + (index*gap), yPos), 
                ButtonType.Click, 
                collectionTab.transform, 
                LeaderBoardWindow.openWindow
            );
            index += 2;

            PowerButtons.CreateButton(
                "level_up_dej", 
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.iconLevels.png"), 
                "Level Up", 
                "Increases Unit's Level By Level Rate", 
                new Vector2(xPos + (index*gap), yPos), 
                ButtonType.GodPower, 
                collectionTab.transform, 
                null
            );
            createLevelInput(collectionTab.transform, new Vector3(xPos + 90 + ((index-2)*gap), 0, 0), Main.savedSettings.inputOptions["LevelRate"].value);
            index++;

            PowerButtons.CreateButton(
                "specific_level_up_dej", 
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.iconLevels.png"), 
                "Specific Level Up", 
                "Changes Unit's Level To Level Rate And Not Add Upon It", 
                new Vector2(xPos + (index*gap), yPos), 
                ButtonType.GodPower, 
                collectionTab.transform, 
                null
            );
            index += 2;

            PowerButtons.CreateButton(
                "heroic_titles_dej", 
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.icon_HeroicTitle.png"), 
                "Heroic Titles", 
                "Modify And Add Titles To Give To Units!", 
                new Vector2(xPos + (index*gap), yPos), 
                ButtonType.Click, 
                collectionTab.transform, 
                TitlesWindow.openWindow
            );
            index++;

            PowerButtons.CreateButton(
                "alliance_dej", 
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.icon_AllianceWhisper.png"), 
                "Whisper Of Alliance", 
                "Create An Alliance Between Two Kingdoms", 
                new Vector2(xPos + (index*gap), yPos), 
                ButtonType.GodPower, 
                collectionTab.transform, 
                null
            );
            index++;

            PowerButtons.CreateButton(
                "edit_cultures_dej", 
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.culture_background.png"), 
                "Edit Cultures", 
                "Add Or Remove Tech From Cultures", 
                new Vector2(xPos + (index*gap), yPos), 
                ButtonType.Click, 
                collectionTab.transform, 
                EditCultureWindow.openWindow
            );
            index = 0;
            yPos -= 36;

            PowerButtons.CreateButton(
                "debug_dej", 
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.iconDebug.png"), 
                "Debug", 
                "Show The Debug Window", 
                new Vector2(xPos + (index*gap), yPos), 
                ButtonType.Click, 
                collectionTab.transform, 
                () => Windows.ShowWindow("debug")
            );
            index++;

            PowerButtons.CreateButton(
                "holy_history_dej", 
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.iconWorldLaws.png"), 
                "Holy History Book (WIP)", 
                "View The History Of Your Favorite Culture", 
                new Vector2(xPos + (index*gap), yPos), 
                ButtonType.Click, 
                collectionTab.transform, 
                null
            );
            index++;

            PowerButtons.CreateButton(
                "expand_zone_dej", 
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.expand_zone_icon.png"), 
                "Expansion Beam", 
                "Expand Zones Of A City", 
                new Vector2(xPos + (index*gap), yPos), 
                ButtonType.GodPower, 
                collectionTab.transform, 
                null
            );
            index++;

            PowerButtons.CreateButton(
                "remove_zone_dej", 
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.remove_zone_icon.png"), 
                "Removal Beam", 
                "Remove Zones Of A City", 
                new Vector2(xPos + (index*gap), yPos), 
                ButtonType.GodPower, 
                collectionTab.transform, 
                null
            );
            index++;

            PowerButtons.CreateButton(
                "stat_limiter_dej", 
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.icon_imperial_thinking.png"), 
                "Unit Stat Limiter", 
                "Set Up Your Own Limits To Stats For Units", 
                new Vector2(xPos + (index*gap), yPos), 
                ButtonType.Click, 
                collectionTab.transform, 
                StatLimiterWindow.openWindow
            );
            index+=5;

            PowerButtons.CreateButton(
                "imperial_thinking_dej", 
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.icon_imperial_thinking.png"), 
                "Imperial Thinking", 
                "Village Of Different Races Can Be Taken Over Instead Of Killed", 
                new Vector2(xPos + (index*gap), yPos), 
                ButtonType.Toggle, 
                collectionTab.transform, 
                NewActions.turnOnImperialThinking
            );
            if (Main.savedSettings.boolOptions["imperialThinkingOption"])
            {
                PowerButtons.ToggleButton("imperial_thinking_dej");
                NewActions.turnOnImperialThinking();
            }
            index++;

            PowerButtons.CreateButton(
                "buildings_select_dej", 
                Resources.Load<Sprite>($"buildings/{SB.tree}/main_0"), 
                "Select Building", 
                "Select A Building To Be Dropped", 
                new Vector2(xPos + (index*gap), yPos), 
                ButtonType.Click, 
                collectionTab.transform, 
                BuildingsWindow.openWindow
            );
            index++;

            PowerButtons.CreateButton(
                "building_drop_dej", 
                Resources.Load<Sprite>($"buildings/{SB.tree}/main_0"), 
                "Drop Building", 
                "Drop Selected Building", 
                new Vector2(xPos + (index*gap), yPos), 
                ButtonType.GodPower, 
                collectionTab.transform, 
                null
            );
            index++;

            PowerButtons.CreateButton(
                "expand_culture_zone_dej", 
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.culture_zone_icon 1.png"), 
                "Culture Expansion Beam", 
                "Expand Zones Of A Culture", 
                new Vector2(xPos + (index*gap), yPos), 
                ButtonType.GodPower, 
                collectionTab.transform, 
                null
            );
            index++;

            PowerButtons.CreateButton(
                "remove_culture_zone_dej", 
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.culture_zone_icon2.png"), 
                "Culture Removal Beam", 
                "Remove Zones Of A Culture", 
                new Vector2(xPos + (index*gap), yPos), 
                ButtonType.GodPower, 
                collectionTab.transform, 
                null
            );
            index++;

            PowerButtons.CreateButton(
                "spawn_city_dej", 
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.settle_icon.png"), 
                "Force Settle", 
                "Create A New City On A Unit", 
                new Vector2(xPos + (index*gap), yPos), 
                ButtonType.GodPower, 
                collectionTab.transform, 
                null
            );
            index++;

            PowerButtons.CreateButton(
                "Item_editor_dej", 
                Sprites.LoadSprite($"{Mod.Info.Path}/icon.png"), 
                "Item Editor", 
                "Create New Items To Give To Units!", 
                new Vector2(xPos + (index*gap), yPos), 
                ButtonType.Click, 
                collectionTab.transform, 
                ItemEditorWindow.openWindow
            );
            index++;

            PowerButtons.CreateButton(
                "Item_drop_dej", 
                Sprites.LoadSprite($"{Mod.Info.Path}/icon.png"), 
                "Item Drop", 
                "Give Active Items To Units!", 
                new Vector2(xPos + (index*gap), yPos), 
                ButtonType.GodPower, 
                collectionTab.transform, 
                null
            );
            index = 12;
            yPos += 36;

            PowerButtons.CreateButton(
                "force_citizen_dej", 
                Sprites.LoadSprite($"{Mod.Info.Path}/icon.png"), 
                "Force Citizenship", 
                "Add A Unit To The City They Are Standing On", 
                new Vector2(xPos + (index*gap), yPos), 
                ButtonType.GodPower, 
                collectionTab.transform, 
                null
            );
            index++;

            PowerButtons.CreateButton(
                "force_capital_dej", 
                Sprites.LoadSprite($"{Mod.Info.Path}/icon.png"), 
                "Force Capital", 
                "Make City The Capital Of Their Kingdom", 
                new Vector2(xPos + (index*gap), yPos), 
                ButtonType.GodPower, 
                collectionTab.transform, 
                null
            );
            index++;

            PowerButtons.CreateButton(
                "naval_warfare_dej", 
                Sprites.LoadSprite($"{Mod.Info.Path}/icon.png"), 
                "Naval Warfare", 
                "Enable Naval Warfare", 
                new Vector2(xPos + (index*gap), yPos), 
                ButtonType.Toggle, 
                collectionTab.transform, 
                BoatManager.toggleNavalWarfare
            );
            if (Main.savedSettings.boolOptions["navalWarfareOption"])
            {
                PowerButtons.ToggleButton("naval_warfare_dej");
                BoatManager.toggleNavalWarfare();
            }
            index++;

            PowerButtons.CreateButton(
                "boat_spawn_dej", 
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.iconNavalWarfare.png"), 
                "Battle Boat", 
                "Spawn A Boat Used In Naval Warfare", 
                new Vector2(xPos + (index*gap), yPos), 
                ButtonType.GodPower, 
                collectionTab.transform, 
                null
            );
            index++;

            PowerButtons.CreateButton(
                "mod_settings_dej", 
                Sprites.LoadSprite($"{Mod.Info.Path}/icon.png"), 
                "Mod Settings", 
                "Customize Different Options In The Settings", 
                new Vector2(xPos + (index*gap), yPos), 
                ButtonType.Click, 
                collectionTab.transform, 
                ModSettingsWindow.openWindow
            );
            index += 2;

            PowerButtons.CreateButton(
                "crpg_toggle_dej", 
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.iconCRPGButton.png"), 
                "Customizable RPG", 
                "Toggle This To Turn On CRPG Features (Restart Game After Turning ON/OFF)", 
                new Vector2(xPos + (index*gap), yPos), 
                ButtonType.Toggle, 
                collectionTab.transform, 
                CustomizableRPG.turnOnCRPG
            );
            if (Main.savedSettings.boolOptions["CRPGOption"])
            {
                PowerButtons.ToggleButton("crpg_toggle_dej");
                CustomizableRPG.turnOnCRPG();
            }
            index++;

            PowerButtons.CreateButton(
                "crpg_settings_dej", 
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.iconSettings.png"), 
                "CRPG Settings", 
                "Customize Different Options For CustomizableRPG", 
                new Vector2(xPos + (index*gap), yPos), 
                ButtonType.Click, 
                collectionTab.transform, 
                CRPGSettingsWindow.openWindow
            );
            index++;

            PowerButtons.CreateButton(
                "crpg_talent_stats_dej", 
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.iconTalentsButton.png"), 
                "CRPG Talent Stats", 
                "View Number Of Talents In World", 
                new Vector2(xPos + (index*gap), yPos), 
                ButtonType.Click, 
                collectionTab.transform, 
                CRPGTalentStatsWindow.openWindow
            );
            index = 18;
            yPos -= 36;

            PowerButtons.CreateButton(
                "crpg_level_stats_dej", 
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.iconLevelsButton.png"), 
                "CRPG Level Stats", 
                "View Number Of Levels In World", 
                new Vector2(xPos + (index*gap), yPos), 
                ButtonType.Click, 
                collectionTab.transform, 
                CRPGLevelStatsWindow.openWindow
            );
            index++;

            PowerButtons.CreateButton(
                "crpg_titles_dej", 
                Sprites.LoadSprite($"{Mod.Info.Path}/icon.png"), 
                "CRPG Titles", 
                "Modify The Level Titles Of Units", 
                new Vector2(xPos + (index*gap), yPos), 
                ButtonType.Click, 
                collectionTab.transform, 
                CRPGTitlesWindow.openWindow
            );
            index++;

            PowerButtons.CreateButton(
                "crpg_attributes_dej", 
                Sprites.LoadSprite($"{Mod.Info.Path}/icon.png"), 
                "CRPG Attribute Traits", 
                "Modify The Settings Of Attribute Traits", 
                new Vector2(xPos + (index*gap), yPos), 
                ButtonType.Click, 
                collectionTab.transform, 
                CRPGAttributesWindow.openWindow
            );
            index += 2;

            PowerButtons.CreateButton(
                "family_addition_dej", 
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.family_icon.png"), 
                "Create A Family Unit", 
                "Drop This On Top Of A Unit To Give Them A Family", 
                new Vector2(xPos + (index*gap), yPos), 
                ButtonType.Click, 
                collectionTab.transform, 
                null
            );
            yPos += 36;

            PowerButtons.CreateButton(
                "familyTreeMod_dej", 
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.family_icon.png"), 
                "Family Tree Mod", 
                "Enable This For Family Tree Features", 
                new Vector2(xPos + (index*gap), yPos), 
                ButtonType.Toggle, 
                collectionTab.transform, 
                null
            );
            index += 2;

            PowerButtons.CreateButton(
                "creature_select_dej", 
                Resources.Load<Sprite>($"ui/Icons/iconHumans"), 
                "Select Creature", 
                "Select A Creature To Be Dropped", 
                new Vector2(xPos + (index*gap), yPos), 
                ButtonType.Click, 
                collectionTab.transform, 
                CreaturesWindow.openWindow
            );
            index++;

            PowerButtons.CreateButton(
                "creature_drop_dej", 
                Resources.Load<Sprite>($"ui/Icons/iconHumans"), 
                "Drop Creature", 
                "Spawn The Selected Creature", 
                new Vector2(xPos + (index*gap), yPos), 
                ButtonType.GodPower, 
                collectionTab.transform, 
                null
            );
            index++;

            PowerButtons.CreateButton(
                "age_select_dej", 
                Resources.Load<Sprite>(EraLibrary.hope.path_icon),
                "Select Age", 
                "Select Any Age Or Era", 
                new Vector2(xPos + (index*gap), yPos), 
                ButtonType.Click, 
                collectionTab.transform, 
                AgesWindow.openWindow
            );
            index++;

            PowerButtons.CreateButton(
                "age_toggle_dej", 
                Resources.Load<Sprite>(EraLibrary.hope.path_icon),
                "Toggle Ages", 
                "Enable Or Disable Any Age", 
                new Vector2(xPos + (index*gap), yPos), 
                ButtonType.Click, 
                collectionTab.transform, 
                ToggleAgesWindow.openWindow
            );
            index = 24;
            yPos -= 36;

            PowerButtons.CreateButton(
                "magnet_dej", 
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.iconMagnet.png"),
                "Filtered Magnet", 
                "Use Filtered Magnet", 
                new Vector2(xPos + (index*gap), yPos), 
                ButtonType.GodPower, 
                collectionTab.transform, 
                null
            );
            index++;

            PowerButtons.CreateButton(
                "magnet_settings_dej", 
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.iconMagnet.png"),
                "Magnet Settings", 
                "Use Filtered Magnet", 
                new Vector2(xPos + (index*gap), yPos), 
                ButtonType.Click, 
                collectionTab.transform, 
                MagnetSettingsWindow.openWindow
            );
        }

        private static PowersTab getPowersTab(string id)
		{
			GameObject gameObject = GameObjects.FindEvenInactive("Tab_" + id);
			return gameObject.GetComponent<PowersTab>();
		}

        private static void createLevelInput(Transform parent, Vector3 pos, string textValue)
        {
            GameObject inputHolder = new GameObject("levelInput");
            inputHolder.transform.SetParent(parent);

            Text infoText = NewUI.addText("Level Rate:", inputHolder, 10, new Vector3(0, 45, 0));
            RectTransform infoTextRect = infoText.gameObject.GetComponent<RectTransform>();
            infoTextRect.sizeDelta = new Vector2(infoTextRect.sizeDelta.x, 80);

            GameObject inputRef = GameObjects.FindEvenInactive("NameInputElement");

            GameObject inputField = Instantiate(inputRef, inputHolder.transform);
            NameInput nameInputComp = inputField.GetComponent<NameInput>();
            nameInputComp.setText(textValue);
            RectTransform inputRect = inputField.GetComponent<RectTransform>();
            inputRect.localPosition = new Vector3(0, -25, 0);
            inputRect.sizeDelta += new Vector2(25, 10);

            nameInputComp.inputField.characterValidation = InputField.CharacterValidation.Integer;
            nameInputComp.inputField.onValueChanged.AddListener(delegate{
                string pValue = NewUI.checkStatInput(nameInputComp);
                Main.modifyInputOption("LevelRate", pValue, true);
                nameInputComp.setText(pValue);
            });

            TipButton inputTip = infoText.gameObject.AddComponent<TipButton>();
            inputTip.textOnClick = "dej_mod_creator";
            inputTip.CallMethod("Awake");
            inputTip.CallMethod("Start");
            RectTransform inputHolderRect = inputHolder.AddComponent<RectTransform>();
            inputHolderRect.localPosition = pos;
        }
    }
}