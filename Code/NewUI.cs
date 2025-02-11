using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Globalization;
using NCMS;
using NCMS.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ReflectionUtility;

namespace CollectionMod
{
    class NewUI : MonoBehaviour
    {
        private static GameObject avatarRef;
        private static GameObject textRef;

        public static void init()
        {
            avatarRef = GameObject.Find(
                $"Canvas Container Main/Canvas - Windows/windows/inspect_unit/Background/Scroll View/Viewport/Content/Part 1/BackgroundAvatar"
            );
        }
        
        public static void createActorUI(Actor actor, GameObject parent, Vector3 pos)
        {
            GameObject GO = Instantiate(avatarRef);
            GO.transform.SetParent(parent.transform);
            var avatarElement = GO.GetComponent<UiUnitAvatarElement>();
            avatarElement.show_banner_clan = true;
            avatarElement.show_banner_kingdom = true;
            avatarElement.show(actor);
            RectTransform GORect = GO.GetComponent<RectTransform>();
            GORect.localPosition = pos;
            GORect.localScale = new Vector3(1, 1, 1);


            Button GOButton = GO.AddComponent<Button>();
            GOButton.OnHover(new UnityAction(() => actorTooltip(actor)));
			GOButton.OnHoverOut(new UnityAction(Tooltip.hideTooltip));
            GOButton.onClick.AddListener(() => showActor(actor));
            GO.AddComponent<GraphicRaycaster>();
        }

        private static void actorTooltip(Actor actor)
        {
            if (actor == null)
            {
                return;
            }
            string text = "actor";
            if (actor.isKing())
            {
                text = "actor_king";
            }
            else if (actor.isCityLeader())
            {
                text = "actor_leader";
            }
            Tooltip.show(actor, text, new TooltipData
            {
                actor = actor,
            });
            return;
        }

        public static void showActor(Actor pActor)
        {
            Config.selectedUnit = pActor;
            ScrollWindow.showWindow("inspect_unit");
        }

        public static Button createBGWindowButton(GameObject parent, int posY, string iconName, string buttonName, string buttonTitle, 
        string buttonDesc, UnityAction call)
        {
            PowerButton button = PowerButtons.CreateButton(
                buttonName,
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.{iconName}.png"),
                buttonTitle,
                buttonDesc,
                new Vector2(118, posY),
                ButtonType.Click,
                parent.transform,
                call
            );

            Image buttonBG = button.gameObject.GetComponent<Image>();
            buttonBG.sprite = Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.UI.backgroundTabButton.png");
            Button buttonButton = button.gameObject.GetComponent<Button>();
            buttonBG.rectTransform.localScale = Vector3.one;

            return buttonButton;
        }

        public static Text addText(string textString, GameObject parent, int sizeFont, Vector3 pos, Vector2 addSize = default(Vector2))
        {
            textRef = GameObject.Find($"/Canvas Container Main/Canvas - Windows/windows/leaderBoardWindow/Background/Title");
            GameObject textGo = Instantiate(textRef, parent.transform);
            textGo.SetActive(true);

            var textComp = textGo.GetComponent<Text>();
            textComp.fontSize = sizeFont;
            textComp.resizeTextMaxSize = sizeFont;
            var textRect = textGo.GetComponent<RectTransform>();
            textRect.position = new Vector3(0,0,0);
            textRect.localPosition = pos + new Vector3(0, -50, 0);
            textRect.sizeDelta = new Vector2(100, 100) + addSize;
            textGo.AddComponent<GraphicRaycaster>();
            textComp.text = textString;
        
            return textComp;
        }

        public static void loadTraitButton(string pID, Vector2 pos, GameObject parent)
        {
            WindowCreatureInfo info = GameObjects.FindEvenInactive("inspect_unit").GetComponent<WindowCreatureInfo>();
            TraitButton traitButton = Instantiate<TraitButton>(info.prefabTrait, parent.transform);
            Reflection.CallMethod(traitButton, "Awake");
            Reflection.CallMethod(traitButton, "load", pID);
            RectTransform component = traitButton.GetComponent<RectTransform>();
            component.localPosition = pos;
        }

        public static Dictionary<string, NameInput> createMultipleInputOption(string objName, string title, string desc, int posY, GameObject parent, Dictionary<string, InputOption> textValue)
        {
            GameObject statHolder = new GameObject("OptionHolder");
            statHolder.transform.SetParent(parent.transform);
            Image statImage = statHolder.AddComponent<Image>();
            statImage.sprite = Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.UI.windowInnerSlicedBIG.png");
            RectTransform statHolderRect = statHolder.GetComponent<RectTransform>();
            statHolderRect.localPosition = new Vector3(130, posY, 0);
            statHolderRect.sizeDelta = new Vector2(400, 400);

            Text statText = addText(title, statHolder, 20, new Vector3(0, 210, 0), new Vector2(100, 0));

            Text descText = addText(desc, statHolder, 20, new Vector3(0, 160, 0), new Vector2(300, 0));

            GameObject inputRef = NCMS.Utils.GameObjects.FindEvenInactive("NameInputElement");

            Dictionary<string, NameInput> nameInputList = new Dictionary<string, NameInput>();
            int valueY = 0;
            foreach(KeyValuePair<string, InputOption> kv in textValue)
            {
                GameObject inputField = Instantiate(inputRef, statHolder.transform);
                NameInput nameInputComp = inputField.GetComponent<NameInput>();
                nameInputComp.setText(kv.Value.value);
                RectTransform inputRect = inputField.GetComponent<RectTransform>();
                inputRect.localPosition = new Vector3(0,60+(valueY*-60),0);
                inputRect.sizeDelta += new Vector2(120, 40);

                GameObject inputChild = inputField.transform.Find("InputField").gameObject;
                RectTransform inputChildRect = inputChild.GetComponent<RectTransform>();
                inputChildRect.sizeDelta *= 2;
                Text inputChildText = inputChild.GetComponent<Text>();
                inputChildText.resizeTextMaxSize = 20;
                nameInputList.Add(kv.Key, nameInputComp);
                valueY++;

                addText(kv.Key, inputField, 15, new Vector3(-150, 50, 0), new Vector2(100, 0));
            }
            return nameInputList;
        }

        public static NameInput createInputOption(string objName, string title, string desc, int posY, GameObject parent, string textValue = "-1")
        {
            GameObject statHolder = new GameObject("OptionHolder");
            statHolder.transform.SetParent(parent.transform);
            Image statImage = statHolder.AddComponent<Image>();
            statImage.sprite = Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.UI.windowInnerSliced.png");
            RectTransform statHolderRect = statHolder.GetComponent<RectTransform>();
            statHolderRect.localPosition = new Vector3(130, posY, 0);
            statHolderRect.sizeDelta = new Vector2(400, 150);

            Text statText = addText(title, statHolder, 20, new Vector3(0, 110, 0), new Vector2(100, 0));
            RectTransform statTextRect = statText.gameObject.GetComponent<RectTransform>();
            statTextRect.sizeDelta = new Vector2(statTextRect.sizeDelta.x+50, 80);

            Text descText = addText(desc, statHolder, 20, new Vector3(0, 60, 0), new Vector2(300, 0));
            RectTransform descTextRect = descText.gameObject.GetComponent<RectTransform>();
            descTextRect.sizeDelta = new Vector2(descTextRect.sizeDelta.x, 80);

            GameObject inputRef = NCMS.Utils.GameObjects.FindEvenInactive("NameInputElement");

            GameObject inputField = Instantiate(inputRef, statHolder.transform);
            NameInput nameInputComp = inputField.GetComponent<NameInput>();
            nameInputComp.setText(textValue);
            RectTransform inputRect = inputField.GetComponent<RectTransform>();
            inputRect.localPosition = new Vector3(0,-40,0);
            inputRect.sizeDelta += new Vector2(120, 40);

            GameObject inputChild = inputField.transform.Find("InputField").gameObject;
            RectTransform inputChildRect = inputChild.GetComponent<RectTransform>();
            inputChildRect.sizeDelta *= 2;
            Text inputChildText = inputChild.GetComponent<Text>();
            inputChildText.resizeTextMaxSize = 20;
            return nameInputComp;
        }

        public static string checkStatInput(NameInput pInput = null, string pText = null){
            string text = pText;
            if (pInput != null)
            {
                text = pInput.inputField.text;
            }
            int num = -1;
            if (!int.TryParse(text, out num)){
                return "0";
            }
            if (num > 999999999){
                return "999999999";
            }
            if (num < -999999999){
                return "-999999999";
            }
            return text;
        }

        public static string checkStatFloatInput(NameInput pInput = null, string pText = null){
            string text = pText;
            if (pInput != null)
            {
                text = pInput.inputField.text;
            }
            float num = -1f;
            if (!float.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out num)){
                return "0";
            }
            if (num > 999999999f){
                return "999999999";
            }
            if (num < -999999999f){
                return "-999999999";
            }
            return text;
        }

        public static void createTextButtonWSize(string name, string title, Vector2 pos, Color color, Transform parent, UnityAction callback, Vector2 size)
        {
            Button textButton = PowerButtons.CreateTextButton(
                name,
                title,
                pos,
                color,
                parent,
                callback
            );
            if (title.Length > 7)
            {
                textButton.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta += new Vector2(0, 10);
            }
            textButton.gameObject.GetComponent<RectTransform>().sizeDelta += size;
        }

        public static void createTab(string buttonID, string tabID, string name, string desc, int xPos)
        {
            GameObject OtherTabButton = GameObjects.FindEvenInactive("Button_Other");
            if (OtherTabButton != null)
            {
                Localization.AddOrSet(buttonID, name);
                Localization.AddOrSet($"{buttonID} Description", desc);
                Localization.AddOrSet("dej_mod_creator",  "Made By: Dej#0594");
                Localization.AddOrSet(tabID, name);


                GameObject newTabButton = GameObject.Instantiate(OtherTabButton);
                newTabButton.transform.SetParent(OtherTabButton.transform.parent);
                Button buttonComponent = newTabButton.GetComponent<Button>();
                TipButton tipButton = buttonComponent.gameObject.GetComponent<TipButton>();
                tipButton.textOnClick = buttonID;
                tipButton.textOnClickDescription = $"{buttonID} Description";
                tipButton.text_description_2 = "dej_mod_creator";



                newTabButton.transform.localPosition = new Vector3(xPos, 49.62f);
                newTabButton.transform.localScale = new Vector3(1f, 1f);
                newTabButton.name = buttonID;

                Sprite spriteForTab = Sprites.LoadSprite($"{Mod.Info.Path}/icon.png");
                newTabButton.transform.Find("Icon").GetComponent<Image>().sprite = spriteForTab;


                GameObject OtherTab = GameObjects.FindEvenInactive("Tab_Other");
                foreach (Transform child in OtherTab.transform)
                {
                    child.gameObject.SetActive(false);
                }

                GameObject additionalPowersTab = GameObject.Instantiate(OtherTab);

                foreach (Transform child in additionalPowersTab.transform)
                {
                    if (child.gameObject.name == "tabBackButton" || child.gameObject.name == "-space")
                    {
                        child.gameObject.SetActive(true);
                        continue;
                    }

                    GameObject.Destroy(child.gameObject);
                }

                foreach (Transform child in OtherTab.transform)
                {
                    child.gameObject.SetActive(true);
                }


                additionalPowersTab.transform.SetParent(OtherTab.transform.parent);
                PowersTab powersTabComponent = additionalPowersTab.GetComponent<PowersTab>();
                powersTabComponent.powerButton = buttonComponent;
                powersTabComponent.powerButtons.Clear();


                additionalPowersTab.name = tabID;
                powersTabComponent.powerButton.onClick = new Button.ButtonClickedEvent();
                powersTabComponent.powerButton.onClick.AddListener(() => tabOnClick(tabID));
                Reflection.SetField<GameObject>(powersTabComponent, "parentObj", OtherTab.transform.parent.parent.gameObject);

                additionalPowersTab.SetActive(true);
                powersTabComponent.powerButton.gameObject.SetActive(true);
            }
        }

        public static void tabOnClick(string tabID)
        {
            GameObject AdditionalTab = GameObjects.FindEvenInactive(tabID);
            PowersTab AdditionalPowersTab = AdditionalTab.GetComponent<PowersTab>();

            AdditionalPowersTab.showTab(AdditionalPowersTab.powerButton);
        }

        public static GameObject createSubWindow(GameObject parent, Vector3 pos, Vector2 size, Vector2 infoSize)
        {
            GameObject parentScrollHolder = new GameObject("scrollHolder");
            parentScrollHolder.transform.SetParent(parent.transform);
            Image scrollImg = parentScrollHolder.AddComponent<Image>();
            scrollImg.sprite = Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.UI.windowInnerSliced.png");
            RectTransform scrollHolderRect = parentScrollHolder.GetComponent<RectTransform>();
            scrollHolderRect.localPosition = pos;
            scrollHolderRect.sizeDelta = size;

            GameObject infoHolder = new GameObject("titleInfoHolder");
            infoHolder.transform.SetParent(parentScrollHolder.transform);
            infoHolder.AddComponent<Image>().color = new Color(0, 0, 0, 0.01f);
            RectTransform infoRect = infoHolder.GetComponent<RectTransform>();
            infoRect.sizeDelta = infoSize;
            ScrollRect scroll = parentScrollHolder.AddComponent<ScrollRect>();
            scroll.scrollSensitivity = 13f;
            scroll.viewport = parentScrollHolder.GetComponent<RectTransform>();
            scroll.content = infoHolder.GetComponent<RectTransform>();
            parentScrollHolder.AddComponent<Mask>();
            infoRect.localPosition = new Vector3(25, 0, 0);

            return infoHolder;
        }

        public static GameObject createProgressBar(GameObject parent, Vector3 pos)
        {
            GameObject researchBar = GameObjects.FindEvenInactive("HealthBar");
            GameObject progressBar = Instantiate(researchBar, parent.transform);
            progressBar.name = "ProgressBar";
            progressBar.SetActive(true);

            RectTransform progressRect = progressBar.GetComponent<RectTransform>();
            progressRect.localPosition = pos;

            StatBar statBar = progressBar.GetComponent<StatBar>();
            statBar.CallMethod("restartBar");

            TipButton tipButton = progressBar.GetComponent<TipButton>();
            tipButton.textOnClick = "Progress Bar";

            GameObject icon = progressBar.transform.Find("Icon").gameObject;
            icon.SetActive(false);

            return progressBar;
        }

        public static GameObject createCultureBanner(GameObject parent, Culture culture, Vector3 pos)
        {
            GameObject cultureHolder = new GameObject("cultureHolder");
            cultureHolder.transform.SetParent(parent.transform);
            Image cultureHolderImg = cultureHolder.AddComponent<Image>();
            cultureHolderImg.sprite = Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.culture_background.png");

            GameObject partIcon = new GameObject("partIcon");
            partIcon.transform.SetParent(cultureHolder.transform);
            Image partIconImg = partIcon.AddComponent<Image>();
            partIcon.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            GameObject partIconDecoration = new GameObject("partIconDecoration");
            partIconDecoration.transform.SetParent(cultureHolder.transform);
            Image partIconDecorationImg = partIconDecoration.AddComponent<Image>();
            partIconDecoration.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

            cultureHolder.AddComponent<Button>();
            cultureHolder.AddComponent<TipButton>().type = "culture";
            BannerLoaderCulture loader = cultureHolder.AddComponent<BannerLoaderCulture>();
            loader.partIcon = partIconImg;
            loader.partIconDecoration = partIconDecorationImg;
            loader.load(culture);
            cultureHolder.GetComponent<RectTransform>().localPosition = pos;
            
            return cultureHolder;
        }
    }
}