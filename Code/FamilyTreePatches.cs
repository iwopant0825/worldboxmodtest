using ReflectionUtility;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using NCMS;
using NCMS.Utils;
using Newtonsoft.Json;
using ai;
using ai.behaviours;
using HarmonyLib;

namespace CollectionMod
{

    class FamilyTreePatches : MonoBehaviour
    {
        public static void init()
        {
          // commented out due to apparently compatibility issue with HeraldicBox, he should be the one figuring it out because this code came first but I temporarily commented the patch out anyways since it's unused
          /*
            Patches.harmony.Patch(
                AccessTools.Method(typeof(CityBehProduceUnit), "produceNewCitizen"),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(FamilyTreePatches), "produceNewCitizen_Prefix"))
            );
          */
        }

        public static bool produceNewCitizen_Prefix(Building pBuilding, City pCity, CityBehProduceUnit __instance, ref bool __result)
		{
			Actor actor = __instance._possibleParents.Pop<Actor>();
			if (actor == null)
			{
                __result = false;
				return false;
			}
			if (!Toolbox.randomChance(actor.stats[S.fertility]))
			{
                __result = false;
				return false;
			}
			Actor actor2 = null;
			if (__instance._possibleParents.Count > 0)
			{
				actor2 = __instance._possibleParents.Pop<Actor>();
			}
			ResourceAsset foodItem = pCity.getFoodItem(null);
			pCity.eatFoodItem(foodItem.id);
			pCity.status.housingFree--;
			pCity.data.born++;
			if (pCity.kingdom != null)
			{
				pCity.kingdom.data.born++;
			}
			ActorAsset asset = actor.asset;
			ActorData actorData = new ActorData();
			actorData.created_time = BehaviourActionBase<City>.world.getCreationTime();
			actorData.cityID = pCity.data.id;
			actorData.id = BehaviourActionBase<City>.world.mapStats.getNextId("unit");
			actorData.asset_id = asset.id;
			ActorBase.generateCivUnit(actor.asset, actorData, actor.race);
			actorData.generateTraits(asset, actor.race);
			// actorData.inheritTraits(actor.data.traits);
            inheritTraitsAndAttributes(actorData, actor);
			actorData.hunger = asset.maxHunger / 2;
			actor.data.makeChild(BehaviourActionBase<City>.world.getCurWorldTime());
			if (actor2 != null)
			{
				// actorData.inheritTraits(actor2.data.traits);
				inheritTraitsAndAttributes(actorData, actor2);
				actor2.data.makeChild(BehaviourActionBase<City>.world.getCurWorldTime());
			}
			Clan clan = CityBehProduceUnit.checkGreatClan(actor, actor2);
			actorData.skin = ActorTool.getBabyColor(actor, actor2);
			actorData.skin_set = actor.data.skin_set;
			Culture babyCulture = CityBehProduceUnit.getBabyCulture(actor, actor2);
			if (babyCulture != null)
			{
				actorData.culture = babyCulture.data.id;
				actorData.level = babyCulture.getBornLevel();
			}
			//Set Up Family Tree
			// if ()
			if (clan != null)
			{
				Actor actor3 = pCity.spawnPopPoint(actorData, actor.currentTile);
				clan.addUnit(actor3);
			}
			else
			{
				pCity.addPopPoint(actorData);
			}
            __result = true;
			return false;
		}

        private static void inheritTraitsAndAttributes(ActorData actorData, Actor pActor)
        {
			foreach(string checkTrait in actorData.traits)
			{
				if (Main.savedSettings.crpgAttributes.ContainsKey(checkTrait))
				{
					actorData.inheritTraits(pActor.data.traits);
					return;
				}
			}
            for (int i = 0; i < pActor.data.traits.Count; i++)
            {
                string text = pActor.data.traits[i];
                ActorTrait actorTrait = AssetManager.traits.get(text);
                if (actorTrait != null && actorTrait.inherit != 0f)
                {
                    float num = Toolbox.randomFloat(0f, 100f);
                    if (Main.savedSettings.crpgAttributes.ContainsKey(text))
                    {
                        string newAttributeTrait = null;
                        int curAttributeNum = int.Parse(text[text.Length-1].ToString());
                        string newText = text.Remove(text.Length-1);
                        if (num <= 3.25f && curAttributeNum >= 6){
                            newAttributeTrait = newText + "6";
                        }
                        else if (num <= 6.5f && curAttributeNum >= 5){
                            newAttributeTrait = newText + "5";
                        }
                        else if (num <= 13f && curAttributeNum >= 4){
                            newAttributeTrait = newText + "4";
                        }
                        else if (num <= 25f && curAttributeNum >= 3){
                            newAttributeTrait = newText + "3";
                        }
                        else if (num <= 50f && curAttributeNum >= 2){
                            newAttributeTrait = newText + "2";
                        }
                        else if (curAttributeNum >= 1){
                            newAttributeTrait = newText + "1";
                        }
                        actorData.addTrait(newAttributeTrait);
                    }
                    else if (actorTrait.inherit >= num && !actorData.traits.Contains(actorTrait.id) && !actorData.haveOppositeTrait(actorTrait))
                    {
                        actorData.addTrait(actorTrait.id);
                    }
                }
            }
        }
    }
}
